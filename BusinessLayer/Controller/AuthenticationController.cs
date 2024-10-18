using MTCG.DataAccessLayer;
using System.Collections.Generic;
using System.Linq;

namespace MTCG.Auth
{
    internal class AuthenticationController
    {

        private static AuthenticationController _instance;
        public static AuthenticationController Instance => _instance ??= new AuthenticationController();

        private AuthenticationController()
        {
            this._users = [];
        }

        /* ONLY FOR USAGE WITHOUT DB */

        private List<User> _users;

        public List<User> GetUsers()
        {
            return _users;
        }

        /* _____ */


        public async Task<AuthToken> Login(Credentials creds)
        {
            if (creds == null)
            {
                return new AuthToken();
            }

            var dbUser = await UserRepository.Instance.GetByUsername(creds.Username);

            if (dbUser == null)
            {
                return new AuthToken();
            }

            creds.HashPasswordWithSalt(dbUser.Credentials.Salt);

            if (!dbUser.IsPasswordEqual(creds.Password)) return new AuthToken();
            {
                var authToken = new AuthToken(true);
                //_users.Find(u => u.GetName() == creds.Username).Token = authToken;
                _ = await UserTokenRepository.Instance.Add(new UserToken(dbUser.Id, authToken));
                return authToken;
            }

        }

        public async Task<int> Signup(Credentials creds)
        {
            var user = this._users.FirstOrDefault(u => u.GetName() == creds.Username);

            if (user != null)
            {
                return -1;
            }

            var newUser = new User(creds);
            this._users.Add(newUser);

            newUser.Stats.Id = await StatsRepository.Instance.Add(newUser.Stats);
            return await UserRepository.Instance.Add(newUser);
        }

        public bool Logout(string authToken)
        {
            var user = this._users.FirstOrDefault(u => u.Token.Value == authToken);

            if (user == null)
            {
                return false;
            }

            user.Token.Reset();

            return user.Token.Valid == false;
        }

        public bool IsAuthorized(string authToken)
        {
            var user = this._users.FirstOrDefault(u => u.Token.Value == authToken);

            return user != null;

        }

        public User GetUserByToken(string authToken)
        {
            var user = this._users.FirstOrDefault(u => u.Token.Value == authToken);

            return user;
        }

        public bool UserExists(string username)
        {
            return this._users.FirstOrDefault(u => u.GetName() == username) != null;
        }

        public User GetUserByName(string username)
        {
            return this._users.FirstOrDefault(u => u.GetName() == username);
        }

    }
}
