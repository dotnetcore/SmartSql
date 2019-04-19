using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Test.Unit.FlexibleConvert
{
    public class FlexibleTest 
    {
        public const string SQL = @"Select 
Convert(bit,1) As Boolean,
Convert(tinyint,1) As Byte,
Convert(char,1) As Char,
Convert(smallint,1) As Int16,
Convert(int,1) As Int32,
Convert(bigint,1) As Int64,
Convert(real,1) As Single,
Convert(decimal,1) As Decimal,
Convert(varchar,1) As String";
    }
}
