using System.Data;
using System.Data.Common;
using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model.Card;
using Npgsql;

namespace MTCG.DataAccessLayer
{
    internal class CardRepository : IRepository<ICard>

    {

    private static CardRepository? _instance;

    public static CardRepository Instance => _instance ??= new CardRepository();

    private CardRepository()
    {
    }

        public Task<int> Add(ICard entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddMultiple(List<ICard> entities, int userID)
        {

            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null)
            {
                return false;
            }

            command.CommandText = "INSERT INTO \"Card\" (\"userID\", \"cardType\", \"name\", \"damage\", \"elementType\", \"monsterType\") VALUES ";
            var index = 0;

            foreach (var card in entities)
            {
                // Add the values placeholders, appending with commas
                if (index > 0)
                {
                    command.CommandText += ", "; // Add a comma to separate multiple rows
                }

                // Create unique parameter names for each card
                command.CommandText += $"(@userID{index}, @cardType{index}, @name{index}, @damage{index}, @elementType{index}, @monsterType{index})";

                // Add the parameters with unique names for each card
                ConnectionController.AddParameterWithValue(command, $"userID{index}", DbType.Int32, userID);
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

        public Task<bool> Update(ICard entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateUserId(ICard entity, int newUserId)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null)
            {
                return false;
            }

            command.CommandText = "UPDATE \"Card\" SET \"userID\" = @userID WHERE id = @id";

            ConnectionController.AddParameterWithValue(command, "userID", DbType.Int32, newUserId);
            ConnectionController.AddParameterWithValue(command, "id", DbType.Int32, entity.Id);

            try
            {
                return await command.ExecuteNonQueryAsync() == 1;
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

        public Task<int> Delete(ICard entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ICard>?> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<List<ICard>?> GetByUser(int userId)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null)
            {
                return null;
            }

            command.CommandText = "SELECT * FROM \"Card\" WHERE \"userID\" = @userID";

            ConnectionController.AddParameterWithValue(command, "userID", DbType.Int32, userId);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var cards = new List<ICard>();
                do
                {
                    
                    cards.Add(reader.GetInt32("cardType") == 1 ? 
                        new MonsterCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), (MonsterType)reader.GetInt32("monsterType"), reader.GetInt32("id"), reader.GetString("name"))
                        : new SpellCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), reader.GetInt32("id"), reader.GetString("name"))
                    );

                } while (await reader.ReadAsync());

                return cards;
            }

            return null;
        }

        public async Task<List<ICard>?> GetDeckByUser(int userId)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null)
            {
                return null;
            }

            command.CommandText = "SELECT * FROM \"Card\" WHERE \"userID\" = @userID AND \"isDeck\"";

            ConnectionController.AddParameterWithValue(command, "userID", DbType.Int32, userId);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var cards = new List<ICard>();
                do
                {

                    cards.Add(reader.GetInt32("cardType") == 1 ?
                        new MonsterCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), (MonsterType)reader.GetInt32("monsterType"), reader.GetInt32("id"), reader.GetString("name"))
                        : new SpellCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"), reader.GetInt32("id"), reader.GetString("name"))
                    );

                } while (await reader.ReadAsync());

                return cards;
            }

            return null;
        }

        public async Task<ICard?> GetById(int id)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null)
            {
                return null;
            }

            command.CommandText = "SELECT * FROM \"Card\" WHERE \"id\" = @cardID";

            ConnectionController.AddParameterWithValue(command, "cardID", DbType.Int32, id);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader.GetInt32("cardType") == 1
                    ? new MonsterCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"),
                        (MonsterType)reader.GetInt32("monsterType"), reader.GetInt32("id"), reader.GetString("name"))
                    : new SpellCard(reader.GetInt32("damage"), (ElementType)reader.GetInt32("elementType"),
                        reader.GetInt32("id"), reader.GetString("name"));
            }

            return null;

        }

        public async Task<int> ClearDeckFromUser(int userId)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null)
            {
                return -1;
            }

            command.CommandText = "UPDATE \"Card\" SET \"isDeck\" = false WHERE \"userID\" = @useriD";
            ConnectionController.AddParameterWithValue(command, "userID", DbType.Int32, userId);

            return (int)(await command.ExecuteNonQueryAsync());
        }

        public async Task<int> SetDeckByCards(int[] cards, int userID)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null)
            {
                return -1;
            }

            command.CommandText = "UPDATE \"Card\" SET \"isDeck\" = true WHERE \"userID\" = @userID and \"id\" IN (@card1, @card2, @card3, @card4)";
            ConnectionController.AddParameterWithValue(command, "card1", DbType.Int32, cards[0]);
            ConnectionController.AddParameterWithValue(command, "card2", DbType.Int32, cards[1]);
            ConnectionController.AddParameterWithValue(command, "card3", DbType.Int32, cards[2]);
            ConnectionController.AddParameterWithValue(command, "card4", DbType.Int32, cards[3]);
            ConnectionController.AddParameterWithValue(command, "userID", DbType.Int32, userID);

            return (int)(await command.ExecuteNonQueryAsync());
        }
    }
}
