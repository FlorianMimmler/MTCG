using System;
using System.Data;
using System.Data.Common;
using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model.Card;
using Npgsql;

namespace MTCG.DataAccessLayer
{
    public class CardRepository : ICardRepository

    {

    private static ICardRepository? _instance;

    public static ICardRepository Instance
        {
            get => _instance ??= new CardRepository();
            set => _instance = value;
        } 

    private CardRepository()
    {
    }

        public Task<int> Add(ICard entity)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Add(ICard entity, int userID)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return -1;
            }

            command.CommandText = "INSERT INTO \"Card\" (\"userID\", \"cardType\", \"name\", \"damage\", \"elementType\", \"monsterType\") " +
                "VALUES (@userID, @cardType, @name, @damage, @elementType, @monsterType) RETURNING id";

            ConnectionController.AddParameterWithValue(command, $"userID", DbType.Int32, userID);
            ConnectionController.AddParameterWithValue(command, $"cardType", DbType.Int32, entity is MonsterCard ? 1 : 2);
            ConnectionController.AddParameterWithValue(command, $"name", DbType.String, entity.Name);
            ConnectionController.AddParameterWithValue(command, $"damage", DbType.Int32, entity.Damage);
            ConnectionController.AddParameterWithValue(command, $"elementType", DbType.Int32, (int)entity.ElementType);
            ConnectionController.AddParameterWithValue(command, $"monsterType", DbType.Int32, entity is MonsterCard monsterCard ? (int)monsterCard.MonsterType : (object)DBNull.Value);

            try
            {
                var result = (await command.ExecuteScalarAsync() ?? -1);
                await command.Connection.CloseAsync();
                return (int)result;
            }
            catch (Exception)
            {
                await command.Connection.CloseAsync();
                return -1;
            }
        }

        public async Task<bool> AddMultiple(List<ICard> entities, int userID)
        {

            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return false;
            }

            command.CommandText = "INSERT INTO \"Card\" (\"userID\", \"cardType\", \"name\", \"damage\", \"elementType\", \"monsterType\") VALUES ";
            var index = 0;

            foreach (var card in entities)
            {
                if (index > 0)
                {
                    command.CommandText += ", ";
                }

                command.CommandText += $"(@userID{index}, @cardType{index}, @name{index}, @damage{index}, @elementType{index}, @monsterType{index})";

                ConnectionController.AddParameterWithValue(command, $"userID{index}", DbType.Int32, userID);
                ConnectionController.AddParameterWithValue(command, $"cardType{index}", DbType.Int32, card is MonsterCard ? 1 : 2);
                ConnectionController.AddParameterWithValue(command, $"name{index}", DbType.String, card.Name);
                ConnectionController.AddParameterWithValue(command, $"damage{index}", DbType.Int32, card.Damage);
                ConnectionController.AddParameterWithValue(command, $"elementType{index}", DbType.Int32, (int) card.ElementType);
                ConnectionController.AddParameterWithValue(command, $"monsterType{index}", DbType.Int32, card is MonsterCard monsterCard ? (int) monsterCard.MonsterType : (object)DBNull.Value);

                index++;
            }

            try
            {
                var result = await command.ExecuteNonQueryAsync();
                await command.Connection.CloseAsync();
                return result == index;
            }
            catch (Exception)
            {
                await command.Connection.CloseAsync();
                return false;
            }
        }

        public Task<bool> Update(ICard entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateUserId(ICard entity, int newUserId)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return false;
            }

            command.CommandText = "UPDATE \"Card\" SET \"userID\" = @userID WHERE id = @id";

            ConnectionController.AddParameterWithValue(command, "userID", DbType.Int32, newUserId);
            ConnectionController.AddParameterWithValue(command, "id", DbType.Int32, entity.Id);

            try
            {
                var result = await command.ExecuteNonQueryAsync() == 1;
                await command.Connection.CloseAsync();
                return result;
            }
            catch (Exception)
            {
                return false;
            }
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

            if (command == null || command.Connection == null)
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

                await command.Connection.CloseAsync();
                return cards;
            }

            await command.Connection.CloseAsync();
            return null;
        }

        public async Task<List<ICard>?> GetDeckByUser(int userId)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
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

                await command.Connection.CloseAsync();
                return cards;
            }

            await command.Connection.CloseAsync();
            return null;
        }

        public Task<ICard?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ICard?> GetByIdAndUserId(int id, int userId)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return null;
            }
            try { 
                command.CommandText = "SELECT * FROM \"Card\" WHERE \"id\" = @cardID AND \"userID\" = @userID";

                ConnectionController.AddParameterWithValue(command, "cardID", DbType.Int32, id);
                ConnectionController.AddParameterWithValue(command, "userID", DbType.Int32, userId);

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
            finally
            {
                await command.Connection.CloseAsync();
            }

        }

        public async Task<int> ClearDeckFromUser(int userId)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return -1;
            }

            try { 
                command.CommandText = "UPDATE \"Card\" SET \"isDeck\" = false WHERE \"userID\" = @useriD";
                ConnectionController.AddParameterWithValue(command, "userID", DbType.Int32, userId);

                return (int)(await command.ExecuteNonQueryAsync());
            }
            finally
            {
                await command.Connection.CloseAsync();
            }
        }

        public async Task<int> SetDeckByCards(int[] cards, int userID)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return -1;
            }

            try { 
                command.CommandText = "UPDATE \"Card\" SET \"isDeck\" = true WHERE \"userID\" = @userID and \"id\" IN (@card1, @card2, @card3, @card4)";
                ConnectionController.AddParameterWithValue(command, "card1", DbType.Int32, cards[0]);
                ConnectionController.AddParameterWithValue(command, "card2", DbType.Int32, cards[1]);
                ConnectionController.AddParameterWithValue(command, "card3", DbType.Int32, cards[2]);
                ConnectionController.AddParameterWithValue(command, "card4", DbType.Int32, cards[3]);
                ConnectionController.AddParameterWithValue(command, "userID", DbType.Int32, userID);

                return (int)(await command.ExecuteNonQueryAsync());
            }
            finally
            {
                await command.Connection.CloseAsync();
            }
        }
    }
}
