using MTCG.BusinessLayer.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DataAccessLayer
{
    internal class StatsRepository : IRepository<Stats>
    {
        private string ConnectionString;

        private StatsRepository _instance;

        public StatsRepository Instance => _instance ??= new StatsRepository();

        private StatsRepository()
        {
            ConnectionString = "Host=localhost;Username=admin;Password=password;Database=MTCG";
        }

        public void Add(Stats entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Stats entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Stats> GetAll()
        {
            throw new NotImplementedException();
        }

        public Stats GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Stats entity)
        {
            throw new NotImplementedException();
        }
    }
}
