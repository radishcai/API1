using CC1.IRepository;
using CC1.IService;
using System;

namespace CC1.Service
{
    public class BaseService<T>: IBaseService<T> where T:class,new()
    {
        //从子类构造函数取值
        protected  IBaseRepository<T> _iBaseRepository;
        //public BaseService(IBaseRepository<T> iBaseRepository)
        //{
           // _iBaseRepository = iBaseRepository;
       //}
    }
}
