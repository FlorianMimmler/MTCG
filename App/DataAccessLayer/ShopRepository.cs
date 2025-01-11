using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model;
using MTCG.BusinessLayer.Model.Achievements;
using MTCG.BusinessLayer.Model.Shop;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DataAccessLayer
{
    internal class ShopRepository : IRepository<IShopItem>
    {

        private static ShopRepository? _instance;

        public static ShopRepository Instance
        {
            get => _instance ??= new ShopRepository();
            set => _instance = value;
        }

        public Task<int> Add(IShopItem entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> Delete(IShopItem entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IShopItem>?> GetAll()
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return null;
            }

            command.CommandText = """
                                  SELECT s.id, s.name, s.type, s.description, s.price
                                  FROM "ShopItem" s
                                  """;

            await using var reader = await command.ExecuteReaderAsync();

            var shopItems = new List<IShopItem>();
            while (await reader.ReadAsync())
            {
                if(ShopItem.StringTypeToEnum(reader.GetString("type")) == ShopItemType.MysteryPack) {
                    shopItems.Add(new MysteryPack(reader.GetInt32("id"), reader.GetString("name"), reader.GetString("description"), reader.GetInt32("price")));
                }
                
            }
            await command.Connection.CloseAsync();
            return shopItems;
        }

        public Task<IShopItem?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(IShopItem entity)
        {
            throw new NotImplementedException();
        }
    }
}
