using MTCG.BusinessLayer.Model.User;
using MTCG.DataAccessLayer;

namespace MTCG.Auth
{
    public class AuthenticationController
    {

        private static AuthenticationController? _instance;
        public static AuthenticationController Instance
        {
            get => _instance ??= new AuthenticationController();
            set => _instance = value;
        } 

        private AuthenticationController()
        {
        }



        public async Task<AuthToken?> Login(Credentials creds)
        {

            var dbUser = await UserRepository.Instance.GetByUsername(creds.Username);

            if (dbUser == null)
            {
                return new AuthToken();
            }

            if (dbUser.GetName() == "__connection__error__")
            {
                return null;
            }

            creds.HashPasswordWithSalt(dbUser.Credentials.Salt);

            if (!dbUser.IsPasswordEqual(creds.Password)) return new AuthToken();
            {
                var authToken = new AuthToken(true);
                var authTokenId = await UserTokenRepository.Instance.Add(new UserToken(dbUser.Id, authToken));
                if (authTokenId < 0)
                {
                    authToken.Valid = false;
                }
                return authToken;
            }

        }

        public async Task<int> Signup(Credentials creds)
        {
            var dbUser = await UserRepository.Instance.GetByUsername(creds.Username);

            if (dbUser != null)
            {
                return -1;
            }

            var newUser = new User(creds);

            newUser.Stats.Id = await StatsRepository.Instance.Add(newUser.Stats);

            if (newUser.Stats.Id < 0) return -1;

            return await UserRepository.Instance.Add(newUser);
        }

        public async Task<bool> Logout(string authToken)
        {
            var authTokenObj = new AuthToken() { Valid = true, Value = authToken };
            var result = await UserTokenRepository.Instance.Delete(new UserToken(-1, authTokenObj));

            return result == 1;
        }

        public async Task<bool> IsAuthorized(string authToken)
        {
            var result = await UserTokenRepository.Instance.GetByAuthToken(authToken);

            return result >= 0;

        }

    }
}
