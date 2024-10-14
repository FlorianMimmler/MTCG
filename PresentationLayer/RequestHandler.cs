using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MTCG.Auth;
using MTCG.BusinessLayer.Model.Card;
using MTCG.BusinessLayer.Model.User;

namespace MTCG.PresentationLayer
{
    internal class RequestHandler
    {
        public HttpResponse HandleRequest(string request, string httpMethod, string body, string requestAuthToken)
        {
            Console.WriteLine($"Handle Request {request} {httpMethod}");

            if (httpMethod == "POST")
            {
                if (request.StartsWith("/sessions"))
                {
                    var requestData = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

                    if (!requestData.TryGetValue("Username", out var username) ||
                        !requestData.TryGetValue("Password", out var password))
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Invalid username/password provided"
                        };

                    var authToken = AuthenticationController.Instance.Login(new Credentials(username.ToString(), password.ToString()));

                    return authToken.Valid
                        ? new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Created,
                            ResponseText = $"{{ \"Token\": {authToken.Value} }}"
                        }
                        : new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Invalid username/password provided"
                        };
                }

                if (request.StartsWith("/users"))
                {
                    var requestData = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

                    if (!requestData.TryGetValue("Username", out var username) ||
                        !requestData.TryGetValue("Password", out var password))
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ResponseText = "Wrong arguments"
                        };

                    var success = AuthenticationController.Instance.Signup(new Credentials(username.ToString(), password.ToString()));

                    return success ?
                        new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.OK,
                            ResponseText = "User successfully created"
                        }:
                        new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Conflict,
                            ResponseText = "User already exists"
                        };

                }

                if (request.StartsWith("/transactions/packages"))
                {
                    if (!AuthenticationController.Instance.IsAuthorized(requestAuthToken))
                    {
                        return 
                            new HttpResponse()
                            {
                                StatusCode = HttpStatusCode.Unauthorized, 
                                ResponseText = "Not authorized"
                            };
                    }

                    var user = AuthenticationController.Instance.GetUserByToken(requestAuthToken);

                    var success = user.BuyPackage();

                    return success ? new HttpResponse()
                    {
                        StatusCode = HttpStatusCode.OK, 
                        ResponseText = "A package has been successfully bought"
                    } : new HttpResponse()
                    {
                        StatusCode = HttpStatusCode.Forbidden, 
                        ResponseText = "Not enough money"
                    };

                }
            }

            if (httpMethod == "GET")
            {

                if (request.StartsWith("/users/"))
                {
                    if (!AuthenticationController.Instance.IsAuthorized(requestAuthToken))
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized, 
                            ResponseText = "Not authorized"
                        };
                    }

                    User user = null;
                    if ((user = AuthenticationController.Instance.GetUserByToken(requestAuthToken)) == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized, 
                            ResponseText = "Not authorized"
                        };
                    }

                    var username = request.Substring(request.LastIndexOf("/", StringComparison.Ordinal)+1);

                    if (!AuthenticationController.Instance.UserExists(username))
                    {
                        return user.Admin ? 
                            new HttpResponse()
                            {
                                StatusCode = HttpStatusCode.NotFound, 
                                ResponseText = "User not found"
                            } : new HttpResponse()
                            {
                                StatusCode = HttpStatusCode.Unauthorized, 
                                ResponseText = "Not authorized"
                            };
                    }

                    if (username != user.GetName() && !user.Admin) return 
                        new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized, 
                            ResponseText = "Not authorized"
                        };

                    var resultUser = AuthenticationController.Instance.GetUserByName(username);

                    var result = new UserDTO()
                    {
                        Username = resultUser.GetName(),
                        Coins = resultUser.Coins,
                        CardCount = resultUser.Stack.Cards.Count,
                        EloPoints = resultUser.Elo.EloScore,
                        EloName = resultUser.Elo.GetEloName()
                    };

                    return 
                        new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.OK,
                            ResponseText = JsonSerializer.Serialize(result)
                        };

                }

                if (request == "/cards")
                {
                    if (!AuthenticationController.Instance.IsAuthorized(requestAuthToken))
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized, 
                            ResponseText = "Not authorized"
                        };
                    }

                    var user = AuthenticationController.Instance.GetUserByToken(requestAuthToken);

                    var usersCardDtos = user.Stack.Cards.Select(card => new CardDTO()
                    {
                        name = card.Name,
                        damage = card.Damage
                    });

                    var jsonResult = JsonSerializer.Serialize(usersCardDtos);

                    return new HttpResponse()
                    {
                        StatusCode = HttpStatusCode.OK, 
                        ResponseText = jsonResult
                    };
                }

                if (request == "/deck")
                {
                    if (!AuthenticationController.Instance.IsAuthorized(requestAuthToken))
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    var user = AuthenticationController.Instance.GetUserByToken(requestAuthToken);

                    if (user.Deck.IsEmpty())
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.NoContent,
                            ResponseText = "Deck is empty"
                        };
                    }

                    var usersCardDtos = user.Deck.Cards.Select(card => new CardDTO()
                    {
                        name = card.Name,
                        damage = card.Damage
                    });

                    var jsonResult = JsonSerializer.Serialize(usersCardDtos);

                    return new HttpResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ResponseText = jsonResult
                    };
                }
            }

            if (httpMethod == "DELETE")
            {
                if (request.StartsWith("/sessions"))
                {
                    if (!AuthenticationController.Instance.IsAuthorized(requestAuthToken))
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized, 
                            ResponseText = "Not authorized"
                        };
                    }

                    var result = AuthenticationController.Instance.Logout(requestAuthToken);

                    return result
                        ? new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.OK, 
                            ResponseText = "Successfully logged out"
                        }
                        : new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError, 
                            ResponseText = "Some Error occured"
                        };
                }
            }
            
            return new HttpResponse()
            {
                StatusCode = HttpStatusCode.NotFound, 
                ResponseText = "No Endpoint found"
            };
        }
    }
}
