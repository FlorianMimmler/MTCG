using MTCG.Auth;
using MTCG.BusinessLayer.Controller;
using MTCG.BusinessLayer.Model.Card;
using MTCG.BusinessLayer.Model.User;
using System.Text.Json;
using MTCG.DataAccessLayer;

namespace MTCG.PresentationLayer
{
    internal class RequestHandler
    {
        public async Task<HttpResponse> HandleRequest(string request, string httpMethod, string body, string requestAuthToken)
        {
            Console.WriteLine($"Handle Request {request} {httpMethod}");

            if (httpMethod == "POST")
            {
                if (request.StartsWith("/sessions"))
                {
                    var requestData = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

                    if (requestData == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Invalid username/password provided"
                        };
                    }

                    if (!requestData.TryGetValue("Username", out var username) ||
                        !requestData.TryGetValue("Password", out var password))
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Invalid username/password provided"
                        };
                    }

                    var userCreds = new Credentials() { Username = username.ToString() };
                    userCreds.SetPassword(password.ToString());

                    var authToken = await AuthenticationController.Instance.Login(userCreds);

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

                    var userId = await AuthenticationController.Instance.Signup(new Credentials(username.ToString(), password.ToString()));

                    return userId >= 0 ?
                        new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.OK,
                            ResponseText = "User successfully created"
                        } :
                        new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Conflict,
                            ResponseText = "User already exists"
                        };

                }

                if (request.StartsWith("/transactions/packages"))
                {
                    if (!await AuthenticationController.Instance.IsAuthorized(requestAuthToken))
                    {
                        return
                            new HttpResponse()
                            {
                                StatusCode = HttpStatusCode.Unauthorized,
                                ResponseText = "Not authorized"
                            };
                    }

                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);

                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    var success = await user.BuyPackage();

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
                    if (! await AuthenticationController.Instance.IsAuthorized(requestAuthToken))
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    User? user = null;
                    if ((user = await UserRepository.Instance.GetByAuthToken(requestAuthToken)) == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    var username = request.Substring(request.LastIndexOf("/", StringComparison.Ordinal) + 1);

                    if (username != user.GetName() && !user.Admin) return
                        new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };

                    var resultUser = username == user.GetName() ? user : await UserRepository.Instance.GetUserDataByUsername(username);

                    if (resultUser == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    var result = new UserDTO()
                    {
                        Username = resultUser.GetName(),
                        Coins = resultUser.Coins,
                        CardCount = resultUser.Stack.Cards.Count,
                        EloPoints = resultUser.Stats.Elo.EloScore,
                        EloName = resultUser.Stats.Elo.GetEloName()
                    };

                    return
                        new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.OK,
                            ResponseText = JsonSerializer.Serialize(result)
                        };

                }

                if (request == "/stats")
                {
                    if (!await AuthenticationController.Instance.IsAuthorized(requestAuthToken))
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);

                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    var userStats = new UserStatsDTO()
                    {
                        Username = user.GetName(),
                        Wins = user.Stats.Wins,
                        Losses = user.Stats.Losses,
                        EloPoints = user.Stats.Elo.EloScore,
                        EloName = user.Stats.Elo.GetEloName()
                    };

                    return new HttpResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ResponseText = JsonSerializer.Serialize(userStats)
                    };

                }

                if (request == "/cards")
                {
                    if (!await AuthenticationController.Instance.IsAuthorized(requestAuthToken))
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);

                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    var userCards = await StackRepository.Instance.GetByUser(user.Id);

                    if (userCards == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.NoContent,
                            ResponseText = "No Cards found"
                        };
                    }

                    var usersCardDtos = userCards.Select(card => new CardDTO()
                    {
                        Id = card.Id,
                        Name = card.Name,
                        Damage = card.Damage
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
                    if (!await AuthenticationController.Instance.IsAuthorized(requestAuthToken))
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);
                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    var deck = await StackRepository.Instance.GetDeckByUser(user.Id);

                    if (deck is not { Count: > 0 })
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.NoContent,
                            ResponseText = "Deck is empty"
                        };
                    }

                    var usersCardDtos = deck.Select(card => new CardDTO()
                    {
                        Id = card.Id,
                        Name = card.Name,
                        Damage = card.Damage
                    });

                    var jsonResult = JsonSerializer.Serialize(usersCardDtos);

                    return new HttpResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ResponseText = jsonResult
                    };
                }

                if (request == "/scoreboard")
                {
                    if (!await AuthenticationController.Instance.IsAuthorized(requestAuthToken))
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    var result = await ScoreboardController.Instance.GetScoreboardDTOs();

                    return result != null
                        ? new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.OK,
                            ResponseText = JsonSerializer.Serialize(result)
                        }
                        : new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.NoContent,
                            ResponseText = "Empty Scoreboard"
                        };
                }

            }

            if (httpMethod == "PUT")
            {
                if (request.StartsWith("/deck"))
                {
                    if (!await AuthenticationController.Instance.IsAuthorized(requestAuthToken))
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);
                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    var requestData = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

                    if (requestData == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ResponseText = "Bad Request"
                        };
                    }

                    if (!requestData.TryGetValue("cards", out var cardsObj))
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ResponseText = "Bad Request"
                        };
                    }

                    var cards = JsonSerializer.Deserialize<string[]>(cardsObj.ToString());

                    if (cards is not { Length: 4 })
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ResponseText = "Not enough cards provided"
                        };
                    }

                    if (await user.SelectDeck(cards))
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.OK,
                            ResponseText = "Deck successfully configured"
                        };
                    }

                    return new HttpResponse()
                    {
                        StatusCode = HttpStatusCode.Forbidden,
                        ResponseText = "Card not found"
                    };

                }
            }

            if (httpMethod == "DELETE")
            {
                if (request.StartsWith("/sessions"))
                {
                    if (!await AuthenticationController.Instance.IsAuthorized(requestAuthToken))
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    var result = await AuthenticationController.Instance.Logout(requestAuthToken);

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
