using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.UTests.Entity
{
    public class T_NoDefaultConstructorEntity : T_PrivateEntity
    {
        public T_NoDefaultConstructorEntity(string fString) : base(fString)
        {
        }
    }
}