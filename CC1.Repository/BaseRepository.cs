using CC1.IRepository;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC1.Repository
{
   public class BaseRepository<T>: SimpleClient<T>, IBaseRepository<T> where T:class,new()
    {
        public BaseRepository(ISqlSugarClient context = null):base(context)
        {
            base.Context = DbScoped.Sugar;
        }
    }
}
