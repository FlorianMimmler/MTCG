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
    public class TradeRequestRepository : ITradeRequestRepository
    {

        private static ITradeRequestRepository? _instance;

        public static ITradeRequestRepository Instance
        {
            get => _instance ??= new TradeRequestRepository();
            set => _instance = value;
        } 

        private TradeRequestRepository()
        {
        }
        public async Task<int> Add(TradingDealRequest entity)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
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
                return -1;
            }
            finally
            {
                await command.Connection.CloseAsync();
            }
        }

        public Task<int> Delete(TradingDealRequest entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TradingDealRequest>?> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TradingDealRequest>?> GetByTradeId(int id)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
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
            finally
            {
                await command.Connection.CloseAsync(); // Ensure connection is closed
            }
        }

        public async Task<TradingDealRequest?> GetById(int id)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return null;
            }

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
            finally
            {
                await command.Connection.CloseAsync(); // Ensure connection is closed
            }
        }

        public Task<bool> Update(TradingDealRequest entity)
        {
            throw new NotImplementedException();
        }
    }
}
