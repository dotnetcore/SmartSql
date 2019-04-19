using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Deserializer
{
    [Collection("GlobalSmartSql")]
    public class ValueTypeDeserializerTest 
    {
        protected ISqlMapper SqlMapper { get; }

        public ValueTypeDeserializerTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }
        private const int ONE = 1;
        private const string SELECT_ONE = "Select 1;";
        #region Int

        [Fact]
        public void Int_To_Int()
        {
            var result = SqlMapper.ExecuteScalar<int>(new RequestContext { RealSql = SELECT_ONE });
            Assert.Equal(ONE, result);
        }
        [Fact]
        public void Int_To_NullInt()
        {
            var result = SqlMapper.ExecuteScalar<int?>(new RequestContext { RealSql = SELECT_ONE });
            Assert.Equal(ONE, result);
        }
        #endregion
        #region Enum
        [Fact]
        public void Int_To_Enum()
        {
            var result = SqlMapper.ExecuteScalar<NumericalEnum>(new RequestContext { RealSql = SELECT_ONE });
            Assert.Equal(NumericalEnum.One, result);
        }
        [Fact]
        public void Int_To_NullEnum()
        {
            var result = SqlMapper.ExecuteScalar<NumericalEnum?>(new RequestContext { RealSql = SELECT_ONE });
            Assert.Equal(NumericalEnum.One, result);
        }
        #endregion
        #region DateTime
        private const string DATE_TIME = "2019-08-08 08:08";
        private readonly static string SELECT_DATE_TIME = $"Select Convert(datetime,'{DATE_TIME}')";
        private readonly static DateTime TEST_DATE_TIME = DateTime.Parse(DATE_TIME);

        [Fact]
        public void DateTime_To_DateTime()
        {
            var result = SqlMapper.ExecuteScalar<DateTime>(new RequestContext { RealSql = SELECT_DATE_TIME });
            Assert.Equal(TEST_DATE_TIME, result);
        }
        [Fact]
        public void DateTime_To_NullDateTime()
        {
            var result = SqlMapper.ExecuteScalar<DateTime?>(new RequestContext { RealSql = SELECT_DATE_TIME });
            Assert.Equal(TEST_DATE_TIME, result);
        }
        #endregion
        #region Timespan

        private readonly static string SELECT_TIMESPAN = $"Select {TEST_DATE_TIME.Ticks}";
        private readonly static TimeSpan TEST_TIMESPAN = new TimeSpan(TEST_DATE_TIME.Ticks);

        //[Fact]
        //public void TimeSpan_To_TimeSpan()
        //{
        //    var result = SqlMapper.ExecuteScalar<TimeSpan>(new RequestContext { RealSql = SELECT_TIMESPAN });
        //    Assert.Equal(TEST_TIMESPAN, result);
        //}
        //[Fact]
        //public void TimeSpan_To_NullTimeSpan()
        //{
        //    var result = SqlMapper.ExecuteScalar<TimeSpan?>(new RequestContext { RealSql = SELECT_TIMESPAN });
        //    Assert.Equal(TEST_TIMESPAN, result);
        //}
        #endregion
        #region Guid
        private const string GUID = "96061D08-C029-4A36-AB40-FDBFA546EC82";
        private readonly static string SELECT_GUID = $"Select Convert(uniqueidentifier,'{GUID}')";
        private readonly static Guid TEST_GUID = new Guid(GUID);

        [Fact]
        public void Guid_To_Guid()
        {
            var result = SqlMapper.ExecuteScalar<Guid>(new RequestContext { RealSql = SELECT_GUID });
            Assert.Equal(TEST_GUID, result);
        }
        [Fact]
        public void Guid_To_NullGuid()
        {
            var result = SqlMapper.ExecuteScalar<Guid?>(new RequestContext { RealSql = SELECT_GUID });
            Assert.Equal(TEST_GUID, result);
        }
        #endregion
        #region String
        private const string STRING = "SmartSql";
        private readonly static string SELECT_STRING = $"Select '{STRING}';";

        [Fact]
        public void String_To_String()
        {
            var result = SqlMapper.ExecuteScalar<string>(new RequestContext { RealSql = SELECT_STRING });
            Assert.Equal(STRING, result);
        }
        #endregion
        #region Null
        private readonly static int DEFAULT_INT = default(int);
        private readonly static String SELECT_NULL = "Select Null;";

        [Fact]
        public void Null_To_Int()
        {
            var result = SqlMapper.ExecuteScalar<int>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Equal(DEFAULT_INT, result);
        }
        [Fact]
        public void Null_To_NullInt()
        {
            var result = SqlMapper.ExecuteScalar<int?>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Null(result);
        }

        [Fact]
        public void Null_To_Guid()
        {
            var result = SqlMapper.ExecuteScalar<Guid>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Equal(default(Guid), result);
        }
        [Fact]
        public void Null_To_NullGuid()
        {
            var result = SqlMapper.ExecuteScalar<Guid?>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Null(result);
        }

        [Fact]
        public void Null_To_DateTime()
        {
            var result = SqlMapper.ExecuteScalar<DateTime>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Equal(default(DateTime), result);
        }
        [Fact]
        public void Null_To_NullDateTime()
        {
            var result = SqlMapper.ExecuteScalar<DateTime?>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Null(result);
        }

        [Fact]
        public void Null_To_TimeSpan()
        {
            var result = SqlMapper.ExecuteScalar<TimeSpan>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Equal(default(TimeSpan), result);
        }
        [Fact]
        public void Null_To_NullTimeSpan()
        {
            var result = SqlMapper.ExecuteScalar<TimeSpan?>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Null(result);
        }

        [Fact]
        public void Null_To_Enum()
        {
            var result = SqlMapper.ExecuteScalar<NumericalEnum>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Equal(default(NumericalEnum), result);
        }
        [Fact]
        public void Null_To_NullEnum()
        {
            var result = SqlMapper.ExecuteScalar<NumericalEnum?>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Null(result);
        }
        [Fact]
        public void Null_To_String()
        {
            var result = SqlMapper.ExecuteScalar<String>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Null(result);
        }
        #endregion
    }
}
