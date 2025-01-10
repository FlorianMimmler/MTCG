using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model.Card;
using MTCG.BusinessLayer.Model.Trading;
using Npgsql;
using NpgsqlTypes;

namespace MTCG.DataAccessLayer
{
    internal class TradeRequestRepository : IRepository<TradingDealRequest>
    {

        private static TradeRequestRepository? _instance;

        public static TradeRequestRepository Instance => _instance ??= new TradeRequestRepository();

        private TradeRequestRepository()
        {
        }
        public async Task<int> Add(TradingDealRequest entity)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null)
            {
                return -1;
            }

            command.CommandText = "INSERT INTO \"TradeRequest\" (\"offeredCard\", \"tradeID\", \"requestUserID\") VALUES (@offeredCard, @tradeID, @requestUserID) RETURNING id";
 
            ConnectionController.AddParameterWithValue(command, "offeredCard", DbType.Int32, entity.OfferedCard.Id);
            ConnectionController.AddParameterWithValue(command, "tradeID", DbType.Int32, entity.TradeId);
            ConnectionController.AddParameterWithValue(command, "requestUserID", DbType.Int32, entity.RequestUserId);

            try
            {
                return (int)(await command.ExecuteScalarAsync() ?? -1);
            }
            catch (Exception)
            {
                // Any other exceptions
                return -1;
            }
        }

        public Task<int> Delete(TradingDealRequest entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TradingDealRequest>?> GetAll()
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null)
            {
                return null;
            }

            command.CommandText = "SELECT *, c.id as cardid FROM \"TradeRequest\" left join \"Card\" as c on \"TradeRequest\".\"offeredCard\" = c.id";

            try
            {
                await using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync()) return null;

                var tradingDeals = new List<TradingDealRequest>();
                do
                {
/*
                    ICard offeredCard = reader.GetInt32("cardType") == 1 ?
                        new MonsterCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), (MonsterType)reader.GetInt32("monsterType"), reader.GetInt32("cardid"), reader.GetString("name"))
                        : new SpellCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), reader.GetInt32("cardid"), reader.GetString("name"));

                    tradingDeals.Add(new TradingDealRequest(offeredCard, JsonSerializer.Deserialize<TradingDealRequirements>(reader.GetString("requirements")),
                        reader.GetInt32("userID")));
*/

                } while (await reader.ReadAsync());

                return tradingDeals;
            }
            catch (Exception)
            {
                // Any other exceptions
                return null;
            }
        }

        public async Task<IEnumerable<TradingDealRequest>?> GetByTradeId(int id)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null)
            {
                return null;
            }

            command.CommandText = "SELECT *, \"TradeRequest\".id as traderequestid, c.id as cardid FROM \"TradeRequest\" left join \"Card\" as c on \"TradeRequest\".\"offeredCard\" = c.id WHERE \"TradeRequest\".\"tradeID\" = @tradeID";
            ConnectionController.AddParameterWithValue(command, "tradeID", DbType.Int32, id);


            try
            {
                await using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync()) return null;

                var tradingDealRequests = new List<TradingDealRequest>();
                do
                {
                    
                    ICard offeredCard = reader.GetInt32("cardType") == 1 ?
                        new MonsterCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), (MonsterType)reader.GetInt32("monsterType"), reader.GetInt32("cardid"), reader.GetString("name"))
                        : new SpellCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), reader.GetInt32("cardid"), reader.GetString("name"));

                    tradingDealRequests.Add(new TradingDealRequest(id,reader.GetInt32("userID"), offeredCard, reader.GetInt32("traderequestid")));
                    

                } while (await reader.ReadAsync());

                return tradingDealRequests;
            }
            catch (Exception)
            {
                // Any other exceptions
                return null;
            }
        }

        public async Task<TradingDealRequest?> GetById(int id)
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

            command.CommandText = "SELECT *, \"TradeRequest\".id as traderequestid, c.id as cardid FROM \"TradeRequest\" left join \"Card\" as c on \"TradeRequest\".\"offeredCard\" = c.id WHERE \"TradeRequest\".\"id\" = @id";
            ConnectionController.AddParameterWithValue(command, "id", DbType.Int32, id);

            try
            {
                await using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync()) return null;


                ICard offeredCard = reader.GetInt32("cardType") == 1 ?
                    new MonsterCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), (MonsterType)reader.GetInt32("monsterType"), reader.GetInt32("cardid"), reader.GetString("name"))
                    : new SpellCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), reader.GetInt32("cardid"), reader.GetString("name"));

                return new TradingDealRequest(reader.GetInt32("tradeID"), reader.GetInt32("userID"), offeredCard, reader.GetInt32("traderequestid"));

            }
            catch (Exception)
            {
                // Any other exceptions
                return null;
            }
        }

        public Task<bool> Update(TradingDealRequest entity)
        {
            throw new NotImplementedException();
        }
    }
}
