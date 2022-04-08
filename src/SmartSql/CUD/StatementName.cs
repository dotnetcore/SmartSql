
namespace SmartSql.CUD
{
    enum StatementName
    {
        Insert,
        Update,
        DeleteById,
        DeleteMany,
        DeleteAll
    }

    public sealed class CUDStatementName
    {
        public static string Insert => StatementName.Insert.ToString();
        public static string Update => StatementName.Update.ToString();
        public static string DeleteAll => StatementName.DeleteAll.ToString();
        public static string DeleteById => StatementName.DeleteById.ToString();
        public static string DeleteMany => StatementName.DeleteMany.ToString();
    }
    
}
