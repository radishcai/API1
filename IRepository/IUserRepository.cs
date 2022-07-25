using CC1.IRepository;
using CC1.Model;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IRepository
{
    public interface IUserRepository:IBaseRepository<User>
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

        /// <summary>
        /// 自定义查询
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>

        Task<User> FindAsync(Expression<Func<User, bool>> func);
    }
}
