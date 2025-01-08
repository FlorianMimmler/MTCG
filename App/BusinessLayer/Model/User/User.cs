using MTCG.Auth;
using MTCG.BusinessLayer.Controller;
using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model;
using MTCG.BusinessLayer.Model.Achievements;
using MTCG.BusinessLayer.Model.User;
using MTCG.DataAccessLayer;

namespace MTCG
{
    public class User
    {
        public int Id { get; set; }
        public Credentials Credentials { get; set; }
        public AuthToken Token { get; set; }
        public bool Admin { get; set; } = true;

        public int Coins { get; set; } = 20;

        public Stats Stats = new Stats();

        public ICardWrapper Deck { get; set; } = new Deck();

        public ICardWrapper Stack { get; set; } = new Stack();

        public List<int>? AchievementIds { get; set; }
        public List<Achievement> NewAchievements { get; set; } = [];

        public User() {}

        public User(Credentials creds)
        {
            Credentials = creds;
            _ = LoadAchievements();

        }

        private async Task<int> LoadAchievements()
        {
            AchievementIds = (await AchievementRepository.Instance.GetAchievementsByUser(this.Id)).Select(achievement => achievement.Id)
                .ToList();
            return AchievementIds.Count;
        }

        public async Task<List<Achievement>> GetAchievements()
        {
            AchievementIds ??= (await AchievementRepository.Instance.GetAchievementsByUser(this.Id))
                    .Select(achievement => achievement.Id)
                    .ToList();

            return AchievementController.Instance.GetAchievements()
                .Where(achievement => AchievementIds.Contains(achievement.Id)).ToList();
        }

        public async Task<int> BuyPackage()
        {
            if (Coins < Package.Price) return 2;

            if (!(await UserRepository.Instance.UpdateCoins(this.Coins - Package.Price, this.Id)))
            {
                return 0;
            }
            var newCards = CardController.Instance.GetCards(Package.MaxCards);
            Stack.AddCards(newCards);
            var result = await CardRepository.Instance.AddMultiple(newCards, Id);

            return result ? 1 : 0;

        }

        public void PrintStack()
        {
            Console.WriteLine("User's stack:");
            this.Stack.PrintCards();
        }

        public void PrintDeck()
        {
            Console.WriteLine("User's deck:");
            this.Deck.PrintCards();
        }

        public async Task<bool> SelectDeck(int[] selection)
        {
            var oldDeck = await CardRepository.Instance.GetDeckByUser(this.Id);

            var oldDeckSelection = oldDeck?.Select(card => card.Id).ToArray();

            _ = await CardRepository.Instance.ClearDeckFromUser(Id);

            var result = await CardRepository.Instance.SetDeckByCards(selection, this.Id);

            if (result != 4)
            {
                _ = await CardRepository.Instance.ClearDeckFromUser(Id);
                _ = oldDeckSelection != null ? await CardRepository.Instance.SetDeckByCards(oldDeckSelection, this.Id) : 1;
                return false;
            }

            return true;

        }

        public async Task<bool> Edit(string? username, string? password)
        {
            if (username != null)
            {
                var dbUser = await UserRepository.Instance.GetByUsername(username);
                if (dbUser != null)
                {
                    return false;
                }

                this.Credentials.Username = username;
            }

            if (password != null)
            {
                this.Credentials.SetPasswordAndHash(password);
            }

            return await UserRepository.Instance.Update(this);
        }

        public async Task<bool> SaveStats()
        {
            return await StatsRepository.Instance.Update(this.Stats);
        }

        public async Task<bool> CheckAndUnlockAchievements()
        {
            if (AchievementIds == null)
            {
                var count = await LoadAchievements();
                Console.WriteLine("unlocked achievments player " + this.Id + ": " + count);
            }
            var achievementList = AchievementController.Instance.GetAchievements() ?? [];
            var success = true;
            foreach (var achievement in achievementList.Where(achievement => !AchievementIds.Contains(achievement.Id)))
            {
                Console.WriteLine(achievement.Id + " is open");
                if (AchievementTypes.Wins == achievement.Type && this.Stats.Wins >= achievement.Value ||
                    AchievementTypes.Elo == achievement.Type && this.Stats.Elo.EloScore >= achievement.Value)
                {
                    Console.WriteLine(achievement.Id + " gets added");
                    success = await AddAchievement(achievement);
                }
            }
            return success;
        }

        private async Task<bool> AddAchievement(Achievement achievement)
        {
            NewAchievements.Add(achievement);
            Console.WriteLine("New Achievements: " + NewAchievements.Count);
            if (await UserAchievementRepository.Instance.Add(new UserAchievement(this.Id, achievement)) >= 0)
            {
                return await ApplyAchievementRewards(achievement);
            }

            return false;
        }

        private async Task<bool> ApplyAchievementRewards(Achievement achievement)
        {
            switch (achievement.RewardType)
            {
                case AchievementTypes.Coins:
                    this.Coins += achievement.RewardValue;
                    return await UserRepository.Instance.UpdateCoins(this.Coins, this.Id);
                case AchievementTypes.Elo:
                    this.Stats.Elo.EloScore += achievement.RewardValue;
                    return await StatsRepository.Instance.Update(this.Stats);
                default:
                    return true;
            }
        }

        public ICard GetRandomCardFromDeck()
        {
            return this.Deck.GetRandomCard();
        }

        public ICardWrapper GetDeck()
        {
            return this.Deck;
        }

        public string GetName()
        {
            return this.Credentials.Username;
        }

        public bool IsPasswordEqual(string password)
        {
            return this.Credentials.Password == password;
        }
    }
}
