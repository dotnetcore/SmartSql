using SmartSql.DataConnector.Configuration;

namespace SmartSql.DataConnector
{
    public interface ITask
    {
        Task Task { get;  }
        void Start();
        void Stop();
    }
}