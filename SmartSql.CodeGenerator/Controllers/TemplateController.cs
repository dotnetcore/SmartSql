using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartSql.CodeGenerator.Models;
using SmartSql;
using SmartSql.Abstractions;
using SmartSql.DataAccess;
namespace SmartSql.CodeGenerator.Controllers
{
    public class TemplateController : Controller
    {

        public async Task<IActionResult> Tables()
        {
            var sqlMapper = MapperContainer.Instance.GetSqlMapper();
            sqlMapper.BeginTransaction();
            var tables = await sqlMapper.QueryAsync<Table>(new RequestContext
            {
                Scope = "DataBase-SqlServer",
                SqlId = "GetTables",
                Request = new { }
            });
            tables = await sqlMapper.QueryAsync<Table>(new RequestContext
            {
                Scope = "DataBase-SqlServer",
                SqlId = "GetTables",
                Request = new { }
            });
            tables = await sqlMapper.QueryAsync<Table>(new RequestContext
            {
                Scope = "DataBase-SqlServer",
                SqlId = "GetTables",
                Request = new { }
            });
            sqlMapper.RollbackTransaction();
            return Json(tables);
        }

        public IActionResult GetColumnsByTableId(long TableId)
        {
            var sqlMapper = MapperContainer.Instance.GetSqlMapper();
            var columns = sqlMapper.Query<Column>(new RequestContext
            {
                Scope = "DataBase-SqlServer",
                SqlId = "GetColumnsByTableId",
                Request = new { TableId = TableId }
            });

            return Json(columns);
        }
    }
}