using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Annotations;


namespace SmartSql.Test.Entities
{
    public class User
    {
        public User()
        {
        }

        public User(long id)
        {
            Id = id;
        }

        public User(long id, string name)
        {
            Id = id;
            UserName = name;
        }

        public User(long id, string name, UserStatus status)
        {
            Id = id;
            UserName = name;
            Status = status;
        }

        public virtual long Id { get; set; }
        public virtual String UserName { get; set; }
        public virtual UserStatus Status { get; set; }
    }

    public enum UserStatus : Int16
    {
        Ok = 1
    }
}