using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Reflection.ObjectFactoryBuilder
{
    public class ObjectFactoryBuilderManager
    {
        public static EmitObjectFactoryBuilder Emit = new EmitObjectFactoryBuilder();
        public static ExpressionObjectFactoryBuilder Expression = new ExpressionObjectFactoryBuilder();
        
    }
}
