using MTCG.BusinessLayer.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DataAccessLayer
{
    public interface IUserRepository : IRepository<User>
    {

        public Task<User?> GetByUsername(string username);

        public Task<User?> GetUserDataByUsername(string username);

        public Task<User?> GetByAuthToken(string authToken);

        public Task<bool> UpdateCoins(int newCoinsCount, int userId);

    }
}
