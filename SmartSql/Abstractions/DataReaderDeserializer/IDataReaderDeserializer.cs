using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Abstractions.DataReaderDeserializer
{
    /// <summary>
    /// DataReader 反序列化器
    /// </summary>
    public interface IDataReaderDeserializer
    {
        object Deserialize(IDataReader dataReader, Type type);
    }
}
