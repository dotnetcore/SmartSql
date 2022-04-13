using SmartSql.DyRepository.Annotations;
using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Test.Repositories
{
    public interface IColumnAnnotationRepository
    {
        ISqlMapper SqlMapper { get; }

        [Statement(Sql = "Select T.* From t_column_annotation_entity T where T.id=?id limit 1")]
        ColumnAnnotationEntity GetEntity(long id);

        [Statement(Sql =
            "INSERT INTO t_column_annotation_entity(name,extend_data)VALUES(?Name,?Data);Select Last_Insert_Id();")]
        int Insert(ColumnAnnotationEntity entity);

        [Statement(Id = "InsertWithParam",Sql =
            "INSERT INTO t_column_annotation_entity(name,extend_data)VALUES(?Name,?Data);Select Last_Insert_Id();")]
        int Insert([Param("Name")] string name,
            [Param("Data", TypeHandler = "Json")] ColumnAnnotationEntity.ExtendData data);
    }
}