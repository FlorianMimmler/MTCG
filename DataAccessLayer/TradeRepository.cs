using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MTCG.BusinessLayer.Model.Trading;

namespace MTCG.DataAccessLayer
{
    internal class TradeRepository : IRepository<TradingDeal>
    {
        public async Task<int> Add(TradingDeal entity)
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

            command.CommandText = "INSERT INTO \"Trade\" (\"offeredCard\", requirements, \"userID\") VALUES (@offeredCard, @requirements, @userID) RETURNING id";

            ConnectionController.AddParameterWithValue(command, "offeredCard", DbType.Int32, entity.OfferedCard.Id);
            ConnectionController.AddParameterWithValue(command, "requirements", DbType.String, JsonSerializer.Serialize(entity.Requirements));
            ConnectionController.AddParameterWithValue(command, "userID", DbType.Int32, entity.OfferingUserId);

            return (int)(await command.ExecuteScalarAsync() ?? -1);
        }

        public Task<int> Delete(TradingDeal entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TradingDeal>?> GetAll()
        {
            throw new NotImplementedException();
        }

        public TradingDeal GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(TradingDeal entity)
        {
            throw new NotImplementedException();
        }
    }
}
