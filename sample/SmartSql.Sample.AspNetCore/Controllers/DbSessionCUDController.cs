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
    public class DbSessionCUDController : ControllerBase
    {
        private readonly IDbSessionFactory _dbSessionFactory;

        public DbSessionCUDController(IDbSessionFactory dbSessionFactory)
        {
            _dbSessionFactory = dbSessionFactory;
        }
        [HttpGet]
        public AllPrimitive GetById(long id)
        {
            using (var dbSession = _dbSessionFactory.Open())
            {
                return dbSession.GetById<AllPrimitive, long>(id);
            }
        }
        [HttpPost]
        public long Insert(AllPrimitive entity)
        {
            using (var dbSession = _dbSessionFactory.Open())
            {
                return dbSession.Insert<AllPrimitive, long>(entity);
            }
        }
        [HttpPost]
        public int DeleteById(long id)
        {
            using (var dbSession = _dbSessionFactory.Open())
            {
                return dbSession.DeleteById<AllPrimitive, long>(id);
            }
        }
        [HttpPost]
        public int Update(AllPrimitive entity)
        {
            using (var dbSession = _dbSessionFactory.Open())
            {
                return dbSession.Update<AllPrimitive>(entity);
            }
        }
        [HttpPost]
        public int DeleteMany(long[] ids)
        {
            using (var dbSession = _dbSessionFactory.Open())
            {
                return dbSession.DeleteMany<AllPrimitive, long>(ids);
            }
        }
    }
}