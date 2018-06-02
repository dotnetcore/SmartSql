using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSql.CodeGenerator.Utils
{
    public interface ITypeConverter
    {
        Type Convert(string type);
    }
}
