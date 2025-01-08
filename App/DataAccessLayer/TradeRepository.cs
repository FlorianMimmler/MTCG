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
    internal class TradeRepository : IRepository<TradingDeal>
    {

        private static TradeRepository? _instance;

        public static TradeRepository Instance => _instance ??= new TradeRepository();

        private TradeRepository()
        {
        }
        public async Task<int> Add(TradingDeal entity)
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

            command.CommandText = "INSERT INTO \"Trade\" (\"offeredCard\", requirements, \"userID\") VALUES (@offeredCard, @requirements, @userID) RETURNING id";

            ConnectionController.AddParameterWithValue(command, "offeredCard", DbType.Int32, entity.OfferedCard.Id);
            ConnectionController.AddParameterWithValue(command, "requirements", DbType.String, JsonSerializer.Serialize(entity.Requirements));
            ConnectionController.AddParameterWithValue(command, "userID", DbType.Int32, entity.OfferingUserId);

            Console.WriteLine("Save to DB");
            try
            {
                return (int)(await command.ExecuteScalarAsync() ?? -1);
            }
            catch (NpgsqlException ex)
            {
                // Specific Npgsql exception handling
                Console.WriteLine($"PostgreSQL error: {ex.Message}");
            }
            catch (DbException ex)
            {
                // General database exception
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Any other exceptions
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }

            return -1;
        }

        public Task<int> Delete(TradingDeal entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TradingDeal>?> GetAll()
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

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
            catch (NpgsqlException ex)
            {
                // Specific Npgsql exception handling
                Console.WriteLine($"PostgreSQL error: {ex.Message}");
                return null;
            }
            catch (DbException ex)
            {
                // General database exception
                Console.WriteLine($"Database error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Any other exceptions
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return null;
            }

            

        }

        public async Task<IEnumerable<TradingDeal>?> GetByUserId(int userId)
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

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
            catch (NpgsqlException ex)
            {
                // Specific Npgsql exception handling
                Console.WriteLine($"PostgreSQL error: {ex.Message}");
                return null;
            }
            catch (DbException ex)
            {
                // General database exception
                Console.WriteLine($"Database error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Any other exceptions
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return null;
            }
        }

        public async Task<TradingDeal?> GetById(int id)
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

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
            catch (NpgsqlException ex)
            {
                // Specific Npgsql exception handling
                Console.WriteLine($"PostgreSQL error: {ex.Message}");
                return null;
            }
            catch (DbException ex)
            {
                // General database exception
                Console.WriteLine($"Database error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Any other exceptions
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return null;
            }
        }

        public Task<bool> Update(TradingDeal entity)
        {
            throw new NotImplementedException();
        }
    }
}
