using System;

namespace CC1.IService
{
    public interface IBaseService<TEntity>  where TEntity : class, new()
    {
    }
}
