namespace SmartSql.AutoConverter
{
    public class NoneAutoConverter : IAutoConverter
    {
        public static IAutoConverter INSTANCE = new NoneAutoConverter();
        public string Name => "None";

        public string Convert(string input)
        {
            return input;
        }
    }
}