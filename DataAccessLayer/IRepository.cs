
namespace MTCG.DataAccessLayer
{
    internal interface IRepository<T>
    {

        public Task<int> Add(T entity);

        public Task<bool> Update(T entity);

        public Task<int> Delete(T entity);

        public Task<IEnumerable<T>?> GetAll();

        public Task<T?> GetById(int id);

    }
}
