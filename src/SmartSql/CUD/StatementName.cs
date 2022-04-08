
namespace SmartSql.CUD
{

    public sealed class CUDStatementName
    {
        public const string Insert = "Insert";
        public const string Update = "Update";
        public const string DeleteAll = "DeleteById";
        public const string DeleteById = "DeleteMany";
        public const string DeleteMany = "DeleteAll";

        public static readonly string[] DefaultFlushOnExecutes = new string[] { Insert, Update, DeleteAll, DeleteById, DeleteMany };

    }
    
}
