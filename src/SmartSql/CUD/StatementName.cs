
namespace SmartSql.CUD
{

    public sealed class CUDStatementName
    {
        public const string Insert = "Insert";
        public const string InsertReturnId = "InsertReturnId";
        public const string Update = "Update";
        public const string DeleteAll = "DeleteAll";
        public const string DeleteById = "DeleteById";
        public const string DeleteMany = "DeleteMany";
        public const string GetById = "GetById";

        public static readonly string[] DefaultFlushOnExecutes = new string[] { Insert, Update, DeleteAll, DeleteById, DeleteMany, InsertReturnId };

        public static readonly string[] DefaultCUDStatements = new string[] { Insert, Update, DeleteAll, DeleteById, DeleteMany, InsertReturnId };

    }
    
}
