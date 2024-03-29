﻿using System;
using System.Collections.Generic;
using System.Data;
using SmartSql.TypeHandlers;

namespace SmartSql.Configuration
{
    public class ParameterMap
    {
        public string Id { get; set; }
        /// <summary>
        ///  Key : Property
        /// </summary>
        public IDictionary<String, Parameter> Parameters { get; set; }
        public Parameter GetParameter(string columnName)
        {
            if (Parameters.TryGetValue(columnName, out var parameter))
            {
                return parameter;
            }
            return null;
        }
    }
    public class Parameter
    {
        public string Property { get; set; }

        public DbType? DbType { get; set; }

        public ITypeHandler Handler { get; set; }
    }
}
