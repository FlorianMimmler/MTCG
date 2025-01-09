using MTCG.Auth;
using MTCG.BusinessLayer.Model.Achievements;
using MTCG.BusinessLayer.Model.User;

namespace MTCG.BusinessLayer.Interface
{
    public interface IUser
    {
        int Id { get; set; }
        Credentials Credentials { get; set; }
        AuthToken Token { get; set; }
        bool Admin { get; set; }
        int Coins { get; set; }
        Stats Stats { get; set; }
        ICardWrapper Deck { get; set; }
        ICardWrapper Stack { get; set; }
        List<int>? AchievementIds { get; set; }
        List<Achievement> NewAchievements { get; set; }

        Task<int> BuyPackage();
        Task<bool> CheckAndUnlockAchievements();
        Task<List<Achievement>> GetAchievements();
        Task<bool> Edit(string? username, string? password);
        Task<bool> SaveStats();
        Task<bool> SelectDeck(int[] selection);
        ICard GetRandomCardFromDeck();
        ICardWrapper GetDeck();
        string GetName();
        bool IsPasswordEqual(string password);
    }
}
