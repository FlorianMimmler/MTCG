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
    public class TradeRepository : ITradeRepository
    {

        private static ITradeRepository? _instance;

        public static ITradeRepository Instance
        {
            get => _instance ??= new TradeRepository();
            set => _instance = value;
        } 

        private TradeRepository()
        {
        }
        public async Task<int> Add(TradingDeal entity)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return -1;
            }

            command.CommandText = "INSERT INTO \"Trade\" (\"offeredCard\", requirements, \"userID\") VALUES (@offeredCard, @requirements, @userID) RETURNING id";

            ConnectionController.AddParameterWithValue(command, "offeredCard", DbType.Int32, entity.OfferedCard.Id);
            ConnectionController.AddParameterWithValue(command, "requirements", DbType.String, JsonSerializer.Serialize(entity.Requirements));
            ConnectionController.AddParameterWithValue(command, "userID", DbType.Int32, entity.OfferingUserId);

            try
            {
                return (int)(await command.ExecuteScalarAsync() ?? -1);
            }
            catch (Exception)
            {
                // Any other exceptions
                return -1;
            }
            finally
            {
                await command.Connection.CloseAsync(); // Ensure connection is closed
            }


}

        public async Task<int> Delete(TradingDeal entity)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return -1;
            }
            try { 
                command.CommandText = "DELETE FROM\"Trade\" WHERE id = @id";
                ConnectionController.AddParameterWithValue(command, "id", DbType.Int32, entity.Id ?? -1);

                return await command.ExecuteNonQueryAsync();
            }
            finally
            {
                await command.Connection.CloseAsync(); // Ensure connection is closed
            }

        }

        public async Task<IEnumerable<TradingDeal>?> GetAll()
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return null;
            }

            command.CommandText = "SELECT *, \"Trade\".id as tradeid, c.id as cardid FROM \"Trade\" left join \"Card\" as c on \"Trade\".\"offeredCard\" = c.id";

            try
            {
                await using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync()) return null;

                var tradingDeals = new List<TradingDeal>();
                do
                {

                    ICard offeredCard = reader.GetInt32("cardType") == 1 ?
                        new MonsterCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), (MonsterType)reader.GetInt32("monsterType"), reader.GetInt32("cardid"), reader.GetString("name"))
                        : new SpellCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), reader.GetInt32("cardid"), reader.GetString("name"));

                    tradingDeals.Add(new TradingDeal(offeredCard, JsonSerializer.Deserialize<TradingDealRequirements>(reader.GetString("requirements")),
                        reader.GetInt32("userID"), reader.GetInt32("tradeid")));


                } while (await reader.ReadAsync());

                return tradingDeals;
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

        public async Task<IEnumerable<TradingDeal>?> GetByUserId(int userId)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return null;
            }

            command.CommandText = "SELECT *, \"Trade\".id as tradeid, c.id as cardid FROM \"Trade\" left join \"Card\" as c on \"Trade\".\"offeredCard\" = c.id WHERE \"Trade\".\"userID\" = @userID";
            ConnectionController.AddParameterWithValue(command, "userID", DbType.Int32, userId);

            try
            {
                await using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync()) return null;

                var tradingDeals = new List<TradingDeal>();
                do
                {

                    ICard offeredCard = reader.GetInt32("cardType") == 1 ?
                        new MonsterCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), (MonsterType)reader.GetInt32("monsterType"), reader.GetInt32("cardid"), reader.GetString("name"))
                        : new SpellCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), reader.GetInt32("cardid"), reader.GetString("name"));

                    tradingDeals.Add(new TradingDeal(offeredCard, JsonSerializer.Deserialize<TradingDealRequirements>(reader.GetString("requirements")),
                        reader.GetInt32("userID"), reader.GetInt32("tradeid")));


                } while (await reader.ReadAsync());

                return tradingDeals;
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

        public async Task<TradingDeal?> GetById(int id)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return null;
            }

            command.CommandText = "SELECT *, \"Trade\".id as tradeid, c.id as cardid FROM \"Trade\" left join \"Card\" as c on \"Trade\".\"offeredCard\" = c.id WHERE \"Trade\".\"id\" = @id";
            ConnectionController.AddParameterWithValue(command, "id", DbType.Int32, id);

            try
            {
                await using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync()) return null;

                ICard offeredCard = reader.GetInt32("cardType") == 1 ?
                    new MonsterCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), (MonsterType)reader.GetInt32("monsterType"), reader.GetInt32("cardid"), reader.GetString("name"))
                    : new SpellCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), reader.GetInt32("cardid"), reader.GetString("name"));

                return new TradingDeal(offeredCard, JsonSerializer.Deserialize<TradingDealRequirements>(reader.GetString("requirements")),
                    reader.GetInt32("userID"), reader.GetInt32("tradeid"));

            }
            catch (Exception )
            {
                // Any other exceptions
                return null;
            }
            finally
            {
                await command.Connection.CloseAsync(); // Ensure connection is closed
            }
        }

        public Task<bool> Update(TradingDeal entity)
        {
            throw new NotImplementedException();
        }
    }
}
