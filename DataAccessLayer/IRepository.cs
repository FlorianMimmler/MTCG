using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DataAccessLayer
{
    internal interface IRepository<T>
    {

        public Task<int> Add(T entity);

        public void Update(T entity);

        public void Delete(T entity);

        public IEnumerable<T> GetAll();

        public T GetById(int id);

    }
}
