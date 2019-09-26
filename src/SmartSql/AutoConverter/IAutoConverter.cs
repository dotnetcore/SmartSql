using System;

namespace SmartSql.AutoConverter
{
    public interface IAutoConverter
    {
        String Name { get; }

        String Convert(String input);
    }
}