using SmartSql.Configuration;
using System;

namespace SmartSql.TypeHandlers
{
    [Obsolete]
    public interface ITypeHandlerFactory
    {
        #region MappedType
        ITypeHandler Get(string handlerName);
        ITypeHandler Get(Type mappedType);
        void Register(ITypeHandler typeHandler);
        #endregion
        #region Config
        void Register(TypeHandler typeHandler);
        #endregion
    }
}
