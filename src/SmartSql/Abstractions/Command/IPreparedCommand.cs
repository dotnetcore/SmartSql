using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.Abstractions.Command
{
    public class OnPreparedEventArgs : EventArgs
    {
        public RequestContext RequestContext { get; set; }
        public IDbConnectionSession DbSession { get; set; }
        public IDbCommand DbCommand { get; set; }
    }
    public delegate void OnPreparedHandler(object sender, OnPreparedEventArgs eventArgs);

    public interface IPreparedCommand
    {
        event OnPreparedHandler OnPrepared;
        IDbCommand Prepare(IDbConnectionSession dbSession, RequestContext context);
    }
}
