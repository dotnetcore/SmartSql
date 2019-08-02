using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SmartSql.DML
{
    public class Insert : IDML
    {
        public StatementType StatementType => StatementType.Insert;
        public string Operation => "INSERT INTO";
        public string Table { get; set; }
        public string ParameterPrefix { get; set; }

        /// <summary>
        /// Column : ParameterMapping
        /// </summary>
        public Dictionary<String, String> ColumnParameterMapping { get; set; }

        #region Static
        
        public static Insert Parse(string sql)
        {
            
            throw new NotImplementedException();
        }

        #endregion

        public void Add(string column, string parameter)
        {
            ColumnParameterMapping.Add(column, parameter);
        }

        public bool Remove(string column)
        {
            return ColumnParameterMapping.Remove(column);
        }

        public override string ToString()
        {
            var columns = String.Join(",", ColumnParameterMapping.Keys.ToArray());
            var parameters = String.Join(",",
                ColumnParameterMapping.Values.Select(p => $"{ParameterPrefix}{p}").ToArray());
            return $"{Operation} {Table} ({columns}) Values ({parameters});";
        }
    }
}