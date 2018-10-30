using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartSql
{
    public class DbTable
    {
        public DbTable() : this("")
        {
        }
        public DbTable(string name)
        {
            Name = name;
            Columns = new Dictionary<String, DbColumn>();
            Rows = new List<DbRow>();
        }
        public string Name { get; set; }
        public IDictionary<String, DbColumn> Columns { get; set; }
        public IList<DbRow> Rows { get; set; }
        public IEnumerable<String> ColumnNames { get { return Columns.Keys; } }

        public void AddColumn(string columnName)
        {
            Columns.Add(columnName, new DbColumn
            {
                Name = columnName
            });
        }
        public void AddColumn(string columnName, Type dataType)
        {
            Columns.Add(columnName, new DbColumn
            {
                Name = columnName,
                DataType = dataType
            });
        }
        public void AddColumn(DbColumn column)
        {
            Columns.Add(column.Name, column);
        }
        public DbRow AddRow()
        {
            var row = new DbRow
            {
                Table = this,
                Cells = new Dictionary<String, DbCell>()
            };
            Rows.Add(row);
            return row;
        }
    }
    public class DbColumn
    {
        public string Name { get; set; }
        public Type DataType { get; set; }
        public bool? AutoIncrement { get; set; }
        public bool? AllowDBNull { get; set; }
    }

    public class DbRow
    {
        public DbTable Table { get; set; }
        public IDictionary<String, DbCell> Cells { get; set; }
        public IEnumerable<object> Values { get { return Cells.Select(m => m.Value); } }
        public object this[string columnName]
        {
            get
            {
                if (!Cells.TryGetValue(columnName, out DbCell dbCell))
                {
                    throw new SmartSqlException($"Row Can not find ColumnName:{columnName} Cell!");
                }
                return dbCell.Value;
            }
            set
            {
                if (Cells.TryGetValue(columnName, out DbCell dbCell))
                {
                    dbCell.Value = value;
                }
                else
                {
                    if (!Table.Columns.TryGetValue(columnName, out DbColumn dbColumn))
                    {
                        throw new SmartSqlException($"Table can not find ColumnName:{columnName} Cell!");
                    }
                    Cells.Add(dbColumn.Name, new DbCell
                    {
                        Column = dbColumn,
                        Value = value
                    });
                }
            }
        }
    }

    public class DbCell
    {
        public DbColumn Column { get; set; }
        public object Value { get; set; }
    }
}
