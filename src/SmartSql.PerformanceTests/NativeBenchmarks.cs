using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.PerformanceTests
{
    public class NativeBenchmarks : BenchmarkBase
    {
        public override long Insert()
        {
            throw new NotImplementedException();
        }
        [Benchmark(Description = "Query_GetValue_DbNull")]
        public IEnumerable<T_Entity> Parser_GetValue()
        {
            var list = new List<T_Entity>();
            WrapParser((dataReader) =>
            {
                if (dataReader.Read())
                {
                    do
                    {
                        T_Entity obj = DataReader2Entity_GetValue(dataReader);
                        list.Add(obj);
                    } while (dataReader.Read());
                }
                dataReader.Dispose();
            });
            return list;
        }

        private static T_Entity DataReader2Entity(System.Data.IDataReader dataReader)
        {
            T_Entity obj = new T_Entity
            {
                FLong = dataReader.GetInt64(0),
                FString = dataReader.GetString(1),
                FDecimal = dataReader.GetDecimal(2),
                FNullDecimal = dataReader.IsDBNull(3) ? default(decimal?) : dataReader.GetDecimal(3),
                FBool = dataReader.GetBoolean(4),
                FNullBool = dataReader.IsDBNull(5) ? default(bool?) : dataReader.GetBoolean(5),
                Status = (EntityStatus)Enum.ToObject(_entityStatusType, dataReader.GetInt16(7)),
                NullStatus = dataReader.IsDBNull(8) ? default(EntityStatus?) : (EntityStatus)Enum.ToObject(_entityStatusType, dataReader.GetInt16(8)),
                CreationTime = dataReader.GetDateTime(9),
                LastUpdateTime = dataReader.IsDBNull(10) ? default(DateTime?) : dataReader.GetDateTime(10),
            };
            return obj;
        }
        private static readonly Type _entityStatusType = typeof(EntityStatus);
        private static T_Entity DataReader2Entity_GetValue(System.Data.IDataReader dataReader)
        {
            var FNullDecimal = dataReader.GetValue(3);
            var FNullBool = dataReader.GetValue(5);
            var NullStatus = dataReader.GetValue(8);
            var LastUpdateTime = dataReader.GetValue(10);
            T_Entity obj = new T_Entity
            {
                FLong = dataReader.GetInt64(0),
                FString = dataReader.GetString(1),
                FDecimal = dataReader.GetDecimal(2),
                FNullDecimal = FNullDecimal == DBNull.Value ? default(decimal?) : (decimal)FNullDecimal,
                FBool = dataReader.GetBoolean(4),
                FNullBool = FNullBool == DBNull.Value ? default(bool?) : (bool)FNullBool,
                Status = (EntityStatus)Enum.ToObject(_entityStatusType, dataReader.GetInt16(7)),
                NullStatus = NullStatus == DBNull.Value ? default(EntityStatus?) : (EntityStatus)Enum.ToObject(_entityStatusType, NullStatus),
                CreationTime = dataReader.GetDateTime(9),
                LastUpdateTime = LastUpdateTime == DBNull.Value ? default(DateTime?) : (DateTime)LastUpdateTime,
            };
            return obj;
        }

        [Benchmark(Description = "Query_IsDBNull_GetValue")]
        public override IEnumerable<T_Entity> Query()
        {
            var list = new List<T_Entity>();
            WrapParser((dataReader) =>
            {
                if (dataReader.Read())
                {
                    do
                    {
                        T_Entity obj = DataReader2Entity(dataReader);
                        list.Add(obj);
                    } while (dataReader.Read());
                }
                dataReader.Dispose();
            });
            return list;
        }

        public override int Update()
        {
            throw new NotImplementedException();
        }

    }
}
