using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Maps
{
    public class MapFactory
    {
        public static ResultMap LoadResultMap(XmlElement xmlNode, SmartSqlMapConfig sqlMapConfig)
        {
            ResultMap resultMap = new ResultMap
            {
                Id = xmlNode.Attributes["Id"].Value,
                Results = new List<Result> { }
            };
            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                var result = new Result
                {
                    Property = childNode.Attributes["Property"].Value,
                    Column= (childNode.Attributes["Column"] ?? childNode.Attributes["Property"]).Value,
                    TypeHandler = childNode.Attributes["TypeHandler"].Value
                };
                result.Handler = sqlMapConfig.TypeHandlers.First(th => th.Name == result.TypeHandler).Handler;
                resultMap.Results.Add(result);
            }
            return resultMap;
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
                parameter.Handler = sqlMapConfig.TypeHandlers.First(th => th.Name == parameter.TypeHandler).Handler;
                parameterMap.Parameters.Add(parameter);
            }

            return parameterMap;
        }
    }
}
