namespace SmartSql.Configuration.Tags
{
    public class IsNotProperty : Tag
    {
        public override bool IsCondition(AbstractRequestContext context)
        {
            return !context.Parameters.ContainsKey(Property);
        }
    }
}