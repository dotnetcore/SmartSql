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
            Columns = new List<DbColumn>();
            Rows = new List<DbRow>();
        }
        public string Name { get; set; }
        public IList<DbColumn> Columns { get; set; }
        public IList<DbRow> Rows { get; set; }
        public IEnumerable<String> ColumnNames { get { return Columns.Select(m => m.Name); } }

        public void AddColumn(string columnName)
        {
            Columns.Add(new DbColumn
            {
                Name = columnName
            });
        }
        public void AddColumn(string columnName, Type dataType)
        {
            Columns.Add(new DbColumn
            {
                Name = columnName,
                DataType = dataType
            });
        }
        public void AddColumn(DbColumn column)
        {
            Columns.Add(column);
        }
        public DbRow AddRow()
        {
            var row = new DbRow
            {
                Table = this,
                Cells = new List<DbCell>()
            };
            Rows.Add(row);
            return row;
        }
    }
    public class DbColumn
    {
        public string Name { get; set; }
        public int MaxLength { get; set; }
        public IDictionary<object, object> ExtendedProperties { get; set; }
        public Type DataType { get; set; }
    }

    public class DbRow
    {
        public DbTable Table { get; set; }
        public IList<DbCell> Cells { get; set; }
        public IEnumerable<object> Values { get { return Cells.Select(m => m.Value); } }
        public object this[string columnName]
        {
            get
            {
                return Cells.FirstOrDefault(m => m.Column.Name == columnName)?.Value;
            }
            set
            {
                var cell = Cells.FirstOrDefault(m => m.Column.Name == columnName);
                if (cell == null)
                {
                    var col = Table.Columns.FirstOrDefault(m => m.Name == columnName);
                    Cells.Add(new DbCell
                    {
                        Column = col,
                        Value = value
                    });
                }
                else
                {
                    cell.Value = value;
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
