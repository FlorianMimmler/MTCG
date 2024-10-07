using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using MTCG.Auth;

namespace MTCG.PresentationLayer
{
    internal class RequestHandler
    {
        public Tuple<HttpStatusCode, string> HandleRequest(string request, string httpMethod, string body, string requestAuthToken)
        {
            Console.WriteLine($"Handle Request {request} {httpMethod}");

            // Unterscheidung nach HTTP-Methode
            if (httpMethod == "POST" && request.StartsWith("/sessions"))
            {
                var requestData = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

                if (!requestData.TryGetValue("Username", out var username) ||
                    !requestData.TryGetValue("Password", out var password))
                    return Tuple.Create(HttpStatusCode.Unauthorized, "Invalid username/password provided");


                Console.WriteLine($"Username: {username}");
                Console.WriteLine($"Password: {password}");

                var authToken = AuthenticationController.Instance.Login(new Credentials(username.ToString(), password.ToString()));

                return authToken.Valid ? Tuple.Create(HttpStatusCode.Created, $"{{ \"Token\": {authToken.Value} }}") : Tuple.Create(HttpStatusCode.Unauthorized, "Invalid username/password provided");
            }

            if (httpMethod == "POST" && request.StartsWith("/users"))
            {
                var requestData = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

                if (!requestData.TryGetValue("Username", out var username) ||
                    !requestData.TryGetValue("Password", out var password))
                    return Tuple.Create(HttpStatusCode.BadRequest, "Wrong arguments");


                Console.WriteLine($"Username: {username}");
                Console.WriteLine($"Password: {password}");

                var success = AuthenticationController.Instance.Signup(new Credentials(username.ToString(), password.ToString()));

                return success ? Tuple.Create(HttpStatusCode.OK, "User successfully created") : Tuple.Create(HttpStatusCode.Conflict, " User already exists");

            }

            if (httpMethod == "POST" && request.StartsWith("/transactions/packages"))
            {
                if (!IsAuthorized(requestAuthToken))
                {
                    return Tuple.Create(HttpStatusCode.Unauthorized, "Not authorized");
                }

                var user = AuthenticationController.Instance.GetUserByToken(requestAuthToken);

                var success = user.BuyPackage();

                return success ? Tuple.Create(HttpStatusCode.OK, "A package has been successfully bought") : Tuple.Create(HttpStatusCode.Forbidden, "Not enough money");

            }

            if (httpMethod == "GET" && request == "/user")
            {
                var user = new User("player1");
                var result = "<html><body>";
                result += $"<p>username: {user.GetName()}, coins: {user.Coins}</p>";
                result += "</body></html>";
                return Tuple.Create(HttpStatusCode.OK, result);
            }

            if (httpMethod == "GET" && request == "/cards")
            {
                if (!IsAuthorized(requestAuthToken))
                {
                    return Tuple.Create(HttpStatusCode.Unauthorized, "Not authorized");
                }

                var user = AuthenticationController.Instance.GetUserByToken(requestAuthToken);

                var result = user.Stack.Cards;

                //return Tuple.Create(HttpStatusCode.OK, result);
            }

            return Tuple.Create(HttpStatusCode.NotFound, "No Enpoint  found");
        }

        private bool IsAuthorized(string authToken)
        {
            return AuthenticationController.Instance.IsAuthorized(authToken);
        }

        private string? ExtractQueryParam(string request, string param)
        {
            var uri = new Uri("http://localhost:8080" + request);
            var query = HttpUtility.ParseQueryString(uri.Query);
            return query.Get(param);
        }
    }
}
