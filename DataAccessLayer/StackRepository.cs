using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MTCG.Auth;
using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model.User;
using Npgsql;
using static System.Net.Mime.MediaTypeNames;

namespace MTCG.DataAccessLayer
{
    internal class StackRepository : IRepository<ICard>

    {

    private static StackRepository? _instance;

    public static StackRepository Instance => _instance ??= new StackRepository();

    private StackRepository()
    {
    }

        public Task<int> Add(ICard entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddMultiple(List<ICard> entities, int userID)
        {

            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

            command.CommandText = "INSERT INTO \"Card\" (\"userID\", \"cardID\", \"cardType\", \"name\", \"damage\", \"elementType\", \"monsterType\") VALUES ";
            var index = 0;

            foreach (var card in entities)
            {
                // Add the values placeholders, appending with commas
                if (index > 0)
                {
                    command.CommandText += ", "; // Add a comma to separate multiple rows
                }

                // Create unique parameter names for each card
                command.CommandText += $"(@userID{index}, @cardID{index}, @cardType{index}, @name{index}, @damage{index}, @elementType{index}, @monsterType{index})";

                // Add the parameters with unique names for each card
                ConnectionController.AddParameterWithValue(command, $"userID{index}", DbType.Int32, userID);
                ConnectionController.AddParameterWithValue(command, $"cardID{index}", DbType.String, card.Id);
                ConnectionController.AddParameterWithValue(command, $"cardType{index}", DbType.Int32, card is MonsterCard ? 1 : 2); // Assuming card type is 1 for Monster, 2 for Spell
                ConnectionController.AddParameterWithValue(command, $"name{index}", DbType.String, card.Name);
                ConnectionController.AddParameterWithValue(command, $"damage{index}", DbType.Int32, card.Damage);
                ConnectionController.AddParameterWithValue(command, $"elementType{index}", DbType.Int32, (int) card.ElementType);
                ConnectionController.AddParameterWithValue(command, $"monsterType{index}", DbType.Int32, card is MonsterCard monsterCard ? (int) monsterCard.MonsterType : (object)DBNull.Value);

                index++;
            }

            try
            {
                var result = await command.ExecuteNonQueryAsync();
                return result == index;
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

            return false;


        }

        public void Update(ICard entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> Delete(ICard entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICard> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<List<ICard>?> GetByUser(int userId)
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

            command.CommandText = "SELECT * FROM \"Card\" WHERE \"userID\" = @userID";

            ConnectionController.AddParameterWithValue(command, "userID", DbType.Int32, userId);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var cards = new List<ICard>();
                do
                {
                    
                    cards.Add(reader.GetInt32("cardType") == 1 ? 
                        new MonsterCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), (MonsterType)reader.GetInt32("monsterType"), reader.GetString("cardID"), reader.GetString("name"))
                        : new SpellCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), reader.GetString("cardID"), reader.GetString("name"))
                    );

                } while (await reader.ReadAsync());

                return cards;
            }

            return null;
        }

        public async Task<List<ICard>?> GetDeckByUser(int userId)
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

            command.CommandText = "SELECT * FROM \"Card\" WHERE \"userID\" = @userID AND \"isDeck\"";

            ConnectionController.AddParameterWithValue(command, "userID", DbType.Int32, userId);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var cards = new List<ICard>();
                do
                {

                    cards.Add(reader.GetInt32("cardType") == 1 ?
                        new MonsterCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), (MonsterType)reader.GetInt32("monsterType"), reader.GetString("cardID"), reader.GetString("name"))
                        : new SpellCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), reader.GetString("cardID"), reader.GetString("name"))
                    );

                } while (await reader.ReadAsync());

                return cards;
            }

            return null;
        }

        public ICard GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> ClearDeckFromUser(int userId)
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

            command.CommandText = "UPDATE \"Card\" SET \"isDeck\" = false WHERE \"userID\" = @useriD";
            ConnectionController.AddParameterWithValue(command, "userID", DbType.Int32, userId);

            return (int)(await command.ExecuteNonQueryAsync());
        }

        public async Task<int> SetDeckByCards(string[] cards)
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

            command.CommandText = "UPDATE \"Card\" SET \"isDeck\" = true WHERE \"cardID\" IN (@card1, @card2, @card3, @card4)";
            ConnectionController.AddParameterWithValue(command, "card1", DbType.String, cards[0]);
            ConnectionController.AddParameterWithValue(command, "card2", DbType.String, cards[1]);
            ConnectionController.AddParameterWithValue(command, "card3", DbType.String, cards[2]);
            ConnectionController.AddParameterWithValue(command, "card4", DbType.String, cards[3]);

            return (int)(await command.ExecuteNonQueryAsync());
        }
    }
}
