using System;

namespace SmartSql.Test.Entities
{
    [Annotations.Table("t_column_annotation_entity")]
    public class ColumnAnnotationEntity
    {
        [Annotations.Column("id", IsAutoIncrement = true)]
        public long Id { get; set; }

        [Annotations.Column("name")] public String Name { get; set; }

        [Annotations.Column("extend_data", TypeHandler = "Json", Alias = "GlobalSmartSql")]
        public ExtendData Data { get; set; }

        public class ExtendData
        {
            public String Info { get; set; }
        }
    }
}