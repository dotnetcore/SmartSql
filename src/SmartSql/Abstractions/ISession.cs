using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions
{
    public interface ISession
    {
        /// <summary>
        /// 开启会话
        /// </summary>
        /// <returns></returns>
        IDbConnectionSession BeginSession();
        /// <summary>
        /// 开启会话
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IDbConnectionSession BeginSession(RequestContext context);
        /// <summary>
        /// 结束会话
        /// </summary>
        void EndSession();
    }
}
