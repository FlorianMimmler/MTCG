using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DataAccessLayer
{
    public interface ICardRepository : IRepository<ICard>
    {
        public Task<bool> AddMultiple(List<ICard> entities, int userID);
        public Task<bool> UpdateUserId(ICard entity, int newUserId);
        public Task<List<ICard>?> GetByUser(int userId);
        public Task<List<ICard>?> GetDeckByUser(int userId);
        public Task<ICard?> GetByIdAndUserId(int id, int userId);
        public Task<int> ClearDeckFromUser(int userId);
        public Task<int> SetDeckByCards(int[] cards, int userID);
    }
}
