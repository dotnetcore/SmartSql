using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartSql.CodeGenerator.Models;

namespace SmartSql.CodeGenerator.Controllers
{
    public class TemplateController : Controller
    {
        public IActionResult Entity()
        {

            var table = new Table
            {
                Id = 1,
                Type = Table.TableType.Table,
                Name = "T_Member",
                Description = "会员",
                Author = new Author
                {
                    Name = "Ahoo Wang"
                },
                Database = new Database
                {
                    Namespace = "Ahoo.GoodJob"
                },
                Columns = new List<Column> {
                     new Column {
                          Id=1,
                            Name="Id",
                             Description="编号",
                              Type="int",
                               IsIdentity=true,
                                IsNullable=false,
                                 IsPrimaryKey=true
                     },
                                          new Column {
                          Id=2,
                            Name="UserName",
                             Description="用户名",
                              Type="string",
                               IsIdentity=false,
                                IsNullable=false,
                                 IsPrimaryKey=false
                     },
                }
            };
            return View(table);
        }
    }
}