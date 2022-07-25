using System;
using System.Collections.Generic;
using System.Text;

namespace CC1.IRepository
{
   public interface IBaseRepository<T> where T : class, new()
    {
    }
}
