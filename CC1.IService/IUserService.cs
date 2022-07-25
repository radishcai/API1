using CC1.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CC1.IService
{
   public interface IUserService:IBaseService<User>
    {
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateAsync(User user);

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <returns></returns>
        Task<bool> EditAsync(User user);

        Task<User> FindAsync(Expression<Func<User, bool>> func);
    }
}
