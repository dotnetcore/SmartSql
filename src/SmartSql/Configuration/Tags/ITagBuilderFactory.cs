using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public interface ITagBuilderFactory
    {
        void Register(String nodeName, ITagBuilder tagBuilder);

        ITagBuilder Get(String nodeName);
    }
}
