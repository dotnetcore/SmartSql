using SmartSql.Abstractions;
using SmartSql.Abstractions.DataReaderDeserializer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.DataReaderDeserializer
{
    public class NestedObjectConverter
    {
        private readonly NestedObjectConvertFactory _nestedObjectConvertFactory;

        public NestedObjectConverter(NestedObjectConvertFactory nestedObjectConvertFactory)
        {
            _nestedObjectConvertFactory = nestedObjectConvertFactory;
        }
        public T ToNested<T>(RequestContext context, IDataReaderWrapper dataReaderWrapper, IDataReaderDeserializer dataReaderDeserializer)
        {
            var convert = _nestedObjectConvertFactory.CreateNestedObjectConvert(context, typeof(T));
            return (T)convert(context, dataReaderWrapper, dataReaderDeserializer);
        }
    }
}
