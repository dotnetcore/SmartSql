using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartSql;
using SmartSql.Abstractions;
using SmartSql.DataAccess;
namespace SmartSql.CodeGenerator.Controllers
{
    public class TestsController : Controller
    {
        public IActionResult Transaction()
        {
            ISmartSqlMapper mapper = MapperContainer.GetSqlMapper();
            mapper.BeginTransaction();
            mapper.RollbackTransaction();
            return Json(new { Ok = true });
        }
    }
}