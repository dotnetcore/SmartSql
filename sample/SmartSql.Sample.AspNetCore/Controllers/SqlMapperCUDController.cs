using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartSql.DbSession;
using SmartSql.Test.Entities;

namespace SmartSql.Sample.AspNetCore.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class SqlMapperCUDController : ControllerBase
    {
        private readonly ISqlMapper _sqlMapper;

        public SqlMapperCUDController(ISqlMapper sqlMapper)
        {
            _sqlMapper = sqlMapper;
        }
        [HttpGet]
        public AllPrimitive GetById(long id)
        {
            return _sqlMapper.GetById<AllPrimitive, long>(id);
        }
        [HttpPost]
        public long Insert(AllPrimitive entity)
        {
            return _sqlMapper.Insert<AllPrimitive, long>(entity);
        }
        [HttpPost]
        public int DeleteById(long id)
        {
            return _sqlMapper.DeleteById<AllPrimitive, long>(id);
        }
        [HttpPost]
        public int Update(AllPrimitive entity)
        {
            return _sqlMapper.Update<AllPrimitive>(entity);
        }
        [HttpPost]
        public int DeleteMany(long[] ids)
        {
            return _sqlMapper.DeleteMany<AllPrimitive, long>(ids);
        }
    }
}