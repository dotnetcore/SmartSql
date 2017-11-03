using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
namespace SmartSql.Tests.Tags
{
    public class Include_Test : TestBase
    {
        [Fact]
        public void QueryVEnquiryByPage_Test()
        {
            SqlMapper.Query<object>(new RequestContext
            {
                Scope = "Business",
                SqlId = "QueryVEnquiryByPage",
                Request = new
                {
                    EnquiryType = 1,
                    EnterpriseName = "",
                    EnquiryNo = "",
                    ProductModel = "",
                    UserId = 0,
                    CurrentUserId = 0,
                    IsManager = true,
                    DepartmentId = 0,
                    DepartmentCode = "string",
                    PageIndex = 1,
                    PageSize = 10,
                    Header = new
                    {
                        Channel = "string",
                        Operator = 0,
                        IP = "string"
                    }
                }
            });
        }

    }
}
