
namespace SmartSql.CUD
{

    public sealed class CUDStatementName
    {
        public const string INSERT = "Insert";
        public const string INSERT_RETURN_ID = "InsertReturnId";
        public const string UPDATE = "Update";
        public const string DELETE_ALL = "DeleteAll";
        public const string DELETE_BY_ID = "DeleteById";
        public const string DELETE_MANY = "DeleteMany";
        public const string GET_BY_ID = "GetById";

        public static readonly string[] DefaultFlushOnExecutes = { INSERT, UPDATE, DELETE_ALL, DELETE_BY_ID, DELETE_MANY, INSERT_RETURN_ID };
        
    }
    
}
