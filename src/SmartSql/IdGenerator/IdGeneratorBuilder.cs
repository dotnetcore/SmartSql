using SmartSql.Reflection;
using SmartSql.Reflection.ObjectFactoryBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.IdGenerator
{
    public class IdGeneratorBuilder : IIdGeneratorBuilder
    {
        public IIdGenerator Build(string typeString, IDictionary<string, object> parameters)
        {
            switch (typeString)
            {
                case nameof(SnowflakeId):
                    {
                        if (parameters == null) { return SnowflakeId.Default; }
                        var idGen = new SnowflakeId();
                        idGen.Initialize(parameters);
                        return idGen;
                    }
                case nameof(CustomSnowflakeId):
                {
                    var idGen = new CustomSnowflakeId();
                    idGen.Initialize(parameters);
                    return idGen;
                }
                case nameof(DbSequence):
                    {
                        var idGen = new DbSequence();
                        idGen.Initialize(parameters);
                        return idGen;
                    }
                default:
                    {
                        var idGenType = TypeUtils.GetType(typeString);
                        var idGen = ObjectFactoryBuilderManager.Expression.GetObjectFactory(idGenType, Type.EmptyTypes)(null) as IIdGenerator;
                        idGen.Initialize(parameters);
                        return idGen;
                    }
            }
        }
    }
}
