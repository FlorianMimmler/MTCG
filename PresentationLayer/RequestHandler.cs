using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using MTCG.Auth;
using MTCG.BusinessLayer.Model.Card;

namespace MTCG.PresentationLayer
{
    internal class RequestHandler
    {
        public Tuple<HttpStatusCode, string> HandleRequest(string request, string httpMethod, string body, string requestAuthToken)
        {
            Console.WriteLine($"Handle Request {request} {httpMethod}");

            if (httpMethod == "POST")
            {
                if (request.StartsWith("/sessions"))
                {
                    var requestData = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

                    if (!requestData.TryGetValue("Username", out var username) ||
                        !requestData.TryGetValue("Password", out var password))
                        return Tuple.Create(HttpStatusCode.Unauthorized, "Invalid username/password provided");

                    var authToken = AuthenticationController.Instance.Login(new Credentials(username.ToString(), password.ToString()));

                    return authToken.Valid ? Tuple.Create(HttpStatusCode.Created, $"{{ \"Token\": {authToken.Value} }}") : Tuple.Create(HttpStatusCode.Unauthorized, "Invalid username/password provided");
                }

                if (request.StartsWith("/users"))
                {
                    var requestData = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

                    if (!requestData.TryGetValue("Username", out var username) ||
                        !requestData.TryGetValue("Password", out var password))
                        return Tuple.Create(HttpStatusCode.BadRequest, "Wrong arguments");

                    var success = AuthenticationController.Instance.Signup(new Credentials(username.ToString(), password.ToString()));

                    return success ? Tuple.Create(HttpStatusCode.OK, "User successfully created") : Tuple.Create(HttpStatusCode.Conflict, " User already exists");

                }

                if (request.StartsWith("/transactions/packages"))
                {
                    if (!AuthenticationController.Instance.IsAuthorized(requestAuthToken))
                    {
                        return Tuple.Create(HttpStatusCode.Unauthorized, "Not authorized");
                    }

                    var user = AuthenticationController.Instance.GetUserByToken(requestAuthToken);

                    var success = user.BuyPackage();

                    return success ? Tuple.Create(HttpStatusCode.OK, "A package has been successfully bought") : Tuple.Create(HttpStatusCode.Forbidden, "Not enough money");

                }
            }

            if (httpMethod == "GET")
            {

                if (request == "/cards")
                {
                    if (!AuthenticationController.Instance.IsAuthorized(requestAuthToken))
                    {
                        return Tuple.Create(HttpStatusCode.Unauthorized, "Not authorized");
                    }

                    var user = AuthenticationController.Instance.GetUserByToken(requestAuthToken);

                    var usersCardDtos = user.Stack.Cards.Select(card => new CardDTO()
                    {
                        name = card.Name,
                        damage = card.Damage
                    });

                    var jsonResult = JsonSerializer.Serialize(usersCardDtos);

                    return Tuple.Create(HttpStatusCode.OK, jsonResult);
                }
            }
            
            return Tuple.Create(HttpStatusCode.NotFound, "No Enpoint  found");
        }
    }
}
