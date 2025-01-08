using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DataAccessLayer
{
    public interface IUserTokenRepository : IRepository<UserToken>
    {

        public Task<int> GetByAuthToken(string authToken);

    }
}
