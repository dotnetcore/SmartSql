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
        public void IntToInt()
        {
            var result = SqlMapper.ExecuteScalar<int>(new RequestContext { RealSql = SELECT_ONE });
            Assert.Equal(ONE, result);
        }

        [Fact]
        public void IntToNullInt()
        {
            var result = SqlMapper.ExecuteScalar<int?>(new RequestContext { RealSql = SELECT_ONE });
            Assert.Equal(ONE, result);
        }

        #endregion

        #region Enum

        [Fact]
        public void IntToEnum()
        {
            var result = SqlMapper.ExecuteScalar<NumericalEnum>(new RequestContext { RealSql = SELECT_ONE });
            Assert.Equal(NumericalEnum.One, result);
        }

        [Fact]
        public void IntToNullEnum()
        {
            var result = SqlMapper.ExecuteScalar<NumericalEnum?>(new RequestContext { RealSql = SELECT_ONE });
            Assert.Equal(NumericalEnum.One, result);
        }

        #endregion

        #region DateTime

        private const string DATE_TIME = "2019-08-08 08:08";
        private readonly static string SELECT_DATE_TIME = $"Select Convert('{DATE_TIME}',datetime)";
        private readonly static DateTime TEST_DATE_TIME = DateTime.Parse(DATE_TIME);

        [Fact]
        public void DateTimeToDateTime()
        {
            var result = SqlMapper.ExecuteScalar<DateTime>(new RequestContext { RealSql = SELECT_DATE_TIME });
            Assert.Equal(TEST_DATE_TIME, result);
        }

        [Fact]
        public void DateTimeToNullDateTime()
        {
            var result = SqlMapper.ExecuteScalar<DateTime?>(new RequestContext { RealSql = SELECT_DATE_TIME });
            Assert.Equal(TEST_DATE_TIME, result);
        }

        #endregion

        #region Timespan
        private readonly static TimeSpan TEST_TIMESPAN = TimeSpan.Parse("08:08:08");
        private readonly static string SELECT_TIMESPAN = $"Select CONVERT('{TEST_TIMESPAN:g}',time)";

        [Fact]
        public void TimeSpanToTimeSpan()
        {
            var result = SqlMapper.ExecuteScalar<TimeSpan>(new RequestContext { RealSql = SELECT_TIMESPAN });
            Assert.Equal(TEST_TIMESPAN, result);
        }

        [Fact]
        public void TimeSpanToNullTimeSpan()
        {
            var result = SqlMapper.ExecuteScalar<TimeSpan?>(new RequestContext { RealSql = SELECT_TIMESPAN });
            Assert.Equal(TEST_TIMESPAN, result);
        }

        #endregion

        #region Guid

        private const string GUID = "96061D08-C029-4A36-AB40-FDBFA546EC82";
        private readonly static string SELECT_GUID = $"Select '{GUID}'";
        private readonly static Guid TEST_GUID = new Guid(GUID);

        [Fact]
        public void GuidToGuid()
        {
            var result = SqlMapper.ExecuteScalar<Guid>(new RequestContext { RealSql = SELECT_GUID });
            Assert.Equal(TEST_GUID, result);
        }

        [Fact]
        public void GuidToNullGuid()
        {
            var result = SqlMapper.ExecuteScalar<Guid?>(new RequestContext { RealSql = SELECT_GUID });
            Assert.Equal(TEST_GUID, result);
        }

        #endregion

        #region String

        private const string STRING = "SmartSql";
        private readonly static string SELECT_STRING = $"Select '{STRING}';";

        [Fact]
        public void StringToString()
        {
            var result = SqlMapper.ExecuteScalar<string>(new RequestContext { RealSql = SELECT_STRING });
            Assert.Equal(STRING, result);
        }

        #endregion

        #region Null

        private readonly static int DEFAULT_INT = default(int);
        private readonly static String SELECT_NULL = "Select Null;";

        [Fact]
        public void NullToInt()
        {
            var result = SqlMapper.ExecuteScalar<int>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Equal(DEFAULT_INT, result);
        }

        [Fact]
        public void NullToNullInt()
        {
            var result = SqlMapper.ExecuteScalar<int?>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Null(result);
        }

        [Fact]
        public void NullToGuid()
        {
            var result = SqlMapper.ExecuteScalar<Guid>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Equal(default(Guid), result);
        }

        [Fact]
        public void NullToNullGuid()
        {
            var result = SqlMapper.ExecuteScalar<Guid?>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Null(result);
        }

        [Fact]
        public void NullToDateTime()
        {
            var result = SqlMapper.ExecuteScalar<DateTime>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Equal(default(DateTime), result);
        }

        [Fact]
        public void NullToNullDateTime()
        {
            var result = SqlMapper.ExecuteScalar<DateTime?>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Null(result);
        }

        [Fact]
        public void NullToTimeSpan()
        {
            var result = SqlMapper.ExecuteScalar<TimeSpan>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Equal(default(TimeSpan), result);
        }

        [Fact]
        public void NullToNullTimeSpan()
        {
            var result = SqlMapper.ExecuteScalar<TimeSpan?>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Null(result);
        }

        [Fact]
        public void NullToEnum()
        {
            var result = SqlMapper.ExecuteScalar<NumericalEnum>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Equal(default(NumericalEnum), result);
        }

        [Fact]
        public void NullToNullEnum()
        {
            var result = SqlMapper.ExecuteScalar<NumericalEnum?>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Null(result);
        }

        [Fact]
        public void NullToString()
        {
            var result = SqlMapper.ExecuteScalar<String>(new RequestContext { RealSql = SELECT_NULL });
            Assert.Null(result);
        }

        #endregion
    }
}