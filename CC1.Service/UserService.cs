using CC1.IService;
using CC1.Model;
using IRepository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CC1.Service
{
   public class UserService:BaseService<User>, IUserService
    {
        private readonly IUserRepository _UserRepository;
        public UserService(IUserRepository iUserRepository)
        {
            base._iBaseRepository = iUserRepository;
            _UserRepository = iUserRepository;
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> CreateAsync(User user)
        {
            return await _UserRepository.CreateAsync(user);
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> EditAsync(User user)
        {
            return await _UserRepository.EditAsync(user);
        }

        public async Task<User> FindAsync(Expression<Func<User, bool>> func)
        {
            return await _UserRepository.FindAsync(func);
        }

    }
}
