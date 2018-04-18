using System;

namespace SmartSql.DyRespository
{
    public interface IRespositoryBuilder
    {
        Type BuildRespositoryImpl(Type interfaceType);
    }
}