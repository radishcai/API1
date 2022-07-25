using CC1.IRepository;
using CC1.Model;
using IRepository;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CC1.Repository
{
    public class UserRepository: BaseRepository<User>,IUserRepository
    {
        public async Task<bool> CreateAsync(User user)
        {
            return await base.InsertAsync(user);
        }

        public async Task<bool> EditAsync(User user)
        {
            return await base.UpdateAsync(user);
        }

        public async Task<User> FindAsync(Expression<Func<User, bool>> func)
        {
            return await base.GetSingleAsync(func);
        }
    }
}
