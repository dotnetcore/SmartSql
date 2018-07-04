using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Maps
{
    public class MapFactory
    {
        public static ResultMap LoadResultMap(XmlElement xmlNode, SmartSqlMapConfig sqlMapConfig, XmlNamespaceManager xmlNsM)
        {
            ResultMap resultMap = new ResultMap
            {
                Id = xmlNode.Attributes["Id"].Value,
                Results = new List<Result> { }
            };
            LoadCtor(xmlNode, sqlMapConfig, xmlNsM, resultMap);
            LoadResult(xmlNode, sqlMapConfig, xmlNsM, resultMap);
            return resultMap;
        }

        private static void LoadCtor(XmlElement xmlNode, SmartSqlMapConfig sqlMapConfig, XmlNamespaceManager xmlNsM, ResultMap resultMap)
        {
            var ctorNode = xmlNode.SelectSingleNode("//ns:Constructor", xmlNsM);
            if (ctorNode != null)
            {
                var argNodes = ctorNode.SelectNodes("//ns:Arg", xmlNsM);
                if (argNodes.Count > 0)
                {
                    var ctorMap = new Constructor
                    {
                        Args = new List<Arg>()
                    };
                    foreach (XmlNode argNode in argNodes)
                    {
                        var arg = new Arg
                        {
                            Column = argNode.Attributes["Column"].Value,
                            Type = argNode.Attributes["Type"].Value,
                            TypeHandler = argNode.Attributes["TypeHandler"]?.Value
                        };
                        arg.ArgType = ArgTypeConvert(arg.Type);
                        if (!String.IsNullOrEmpty(arg.TypeHandler))
                        {
                            TypeHandler typeHandler = TypeHanderNotNull(sqlMapConfig, arg.TypeHandler);
                            arg.Handler = typeHandler.Handler;
                        }
                        ctorMap.Args.Add(arg);
                    }
                    resultMap.Constructor = ctorMap;
                }
            }
        }

        private static Type ArgTypeConvert(string typeStr)
        {
            switch (typeStr)
            {
                case "Boolean": { return typeof(Boolean); }
                case "Char": { return typeof(Char); }
                case "SByte": { return typeof(SByte); }
                case "Byte": { return typeof(Byte); }
                case "Int16": { return typeof(Int16); }
                case "UInt16": { return typeof(UInt16); }
                case "Int32": { return typeof(Int32); }
                case "UInt32": { return typeof(UInt32); }
                case "Int64": { return typeof(Int64); }
                case "UInt64": { return typeof(UInt64); }
                case "Single": { return typeof(Single); }
                case "Double": { return typeof(Double); }
                case "Decimal": { return typeof(Decimal); }
                case "DateTime": { return typeof(DateTime); }
                case "String": { return typeof(String); }
                case "Guid": { return typeof(Guid); }
                default: { return Type.GetType(typeStr, true); }
            }
        }

        private static void LoadResult(XmlElement xmlNode, SmartSqlMapConfig sqlMapConfig, XmlNamespaceManager xmlNsM, ResultMap resultMap)
        {
            var resultNodes = xmlNode.SelectNodes("//ns:Result", xmlNsM);
            foreach (XmlNode resultNode in resultNodes)
            {
                var result = new Result
                {
                    Property = resultNode.Attributes["Property"].Value,
                    Column = (resultNode.Attributes["Column"] ?? resultNode.Attributes["Property"]).Value,
                    TypeHandler = resultNode.Attributes["TypeHandler"]?.Value
                };
                if (!String.IsNullOrEmpty(result.TypeHandler))
                {
                    TypeHandler typeHandler = TypeHanderNotNull(sqlMapConfig, result.TypeHandler);
                    result.Handler = typeHandler.Handler;
                }
                resultMap.Results.Add(result);
            }
        }

        public static ParameterMap LoadParameterMap(XmlElement xmlNode, SmartSqlMapConfig sqlMapConfig)
        {
            ParameterMap parameterMap = new ParameterMap
            {
                Id = xmlNode.Attributes["Id"].Value,
                Parameters = new List<Parameter> { }
            };

            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                var parameter = new Parameter
                {
                    Property = childNode.Attributes["Property"].Value,
                    Name = (childNode.Attributes["Name"] ?? childNode.Attributes["Property"]).Value,
                    TypeHandler = childNode.Attributes["TypeHandler"].Value
                };

                if (!String.IsNullOrEmpty(parameter.TypeHandler))
                {
                    TypeHandler typeHandler = TypeHanderNotNull(sqlMapConfig, parameter.TypeHandler);
                    parameter.Handler = typeHandler.Handler;
                }
                parameterMap.Parameters.Add(parameter);
            }

            return parameterMap;
        }

        private static TypeHandler TypeHanderNotNull(SmartSqlMapConfig sqlMapConfig, string typeHandlerName)
        {
            var typeHandler = sqlMapConfig.TypeHandlers.FirstOrDefault(th => th.Name == typeHandlerName);
            if (typeHandler == null)
            {
                throw new SmartSqlException($"Can not find TypeHandler.Name:{typeHandlerName}");
            }
            return typeHandler;
        }
    }
}
