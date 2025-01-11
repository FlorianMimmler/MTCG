using MTCG.Auth;
using MTCG.BusinessLayer.Controller;
using MTCG.BusinessLayer.Model.Card;
using MTCG.BusinessLayer.Model.User;
using System.Text.Json;
using MTCG.DataAccessLayer;
using MTCG.BusinessLayer.Model.RequestObjects;
using MTCG.BusinessLayer.Model.Trading;
using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model;

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
                    var loginRequest = JsonSerializer.Deserialize<LoginRequest>(body);

                    if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ResponseText = "Invalid input data"
                        };
                    }

                    var userCreds = new Credentials { Username = loginRequest.Username };
                    userCreds.SetPassword(loginRequest.Password);

                    var authToken = await AuthenticationController.Instance.Login(userCreds);

                    if (authToken == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An error occured"
                        };
                    }

                    return authToken.Valid
                        ? new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Created,
                            ResponseText = JsonSerializer.Serialize(new { Token = authToken.Value })
                        }
                        : new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Invalid username/password provided"
                        };
                }

                if (request.StartsWith("/users"))
                {
                    var signupRequest = JsonSerializer.Deserialize<SignupRequest>(body);

                    if (signupRequest == null || string.IsNullOrEmpty(signupRequest.Username) || string.IsNullOrEmpty(signupRequest.Password))
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ResponseText = "Invalid input data"
                        };
                    }

                    var userId = await AuthenticationController.Instance.Signup(new Credentials(signupRequest.Username, signupRequest.Password));

                    if (userId == -2)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An Error occured"
                        };
                    }

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

                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);

                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    if (user.GetName() == "__connection__error__")
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An error occured"
                        };
                    }

                    var success = await user.BuyPackage();

                    return success == 1 ? new HttpResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ResponseText = "A package has been successfully bought"
                    } : success == 2 ? new HttpResponse()
                    {
                        StatusCode = HttpStatusCode.Forbidden,
                        ResponseText = "Not enough money"
                    } : new HttpResponse()
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        ResponseText = "An Unexpected Error Occured"
                    };

                }

                if (request.StartsWith("/battles"))
                {
                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);
                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    if (user.GetName() == "__connection__error__")
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An error occured"
                        };
                    }

                    var userDeck = await CardRepository.Instance.GetDeckByUser(user.Id);
                    if (userDeck?.Count != 4)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "No Deck selected"
                        };
                    }

                    var result = await BattleLobbyController.Instance.EnterLobby(user);

                    return new HttpResponse()
                    {
                        StatusCode = result.Contains("Error") ? HttpStatusCode.InternalServerError : HttpStatusCode.OK,
                        ResponseText = result
                    };
                }

                if (request.StartsWith("/ai-battle"))
                {
                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);
                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    if (user.GetName() == "__connection__error__")
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An error occured"
                        };
                    }

                    var userDeck = await CardRepository.Instance.GetDeckByUser(user.Id);
                    if (userDeck?.Count != 4)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "No Deck selected"
                        };
                    }

                    user.Deck.SetCards(userDeck);

                    var battle = new BattleController(user);
                    var result = await battle.StartBattle();

                    if (!result)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An Unexpected Error occured"
                        };
                    }

                    var log = battle.GetSerializedBattleLogForPlayer(1);

                    return new HttpResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ResponseText = log
                    };
                }

                if (request == "/trade")
                {
                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);
                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    if (user.GetName() == "__connection__error__")
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An error occured"
                        };
                    }

                    var tradeRequest = TradingController.Instance.ValidateTradeRequest(body);

                    if (tradeRequest == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ResponseText = "Invalid input data"
                        };
                    }

                    return await TradingController.Instance.CreateTrade(tradeRequest, user);
                }

                if (request == "/tradeRequest")
                {
                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);
                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    if (user.GetName() == "__connection__error__")
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An error occured"
                        };
                    }

                    var tradeRequest = TradingController.Instance.ValidateTradeRequestRequest(body);

                    if (tradeRequest == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ResponseText = "Invalid input data"
                        };
                    }

                    return await TradingController.Instance.CreateTradeRquest(tradeRequest, user);
                }

                if (request.StartsWith("/shopitem"))
                {
                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);
                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    if (user.GetName() == "__connection__error__")
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An error occured"
                        };
                    }

                    var rawShopItemId = request[(request.LastIndexOf('/') + 1)..];

                    if (int.TryParse(rawShopItemId, out int shopItemId))
                    {
                        ShopController.Instance.UpdateShop();

                        var result = await ShopController.Instance.BuyItem(shopItemId, user);

                        return new HttpResponse
                        {
                            StatusCode = result.Item1,
                            ResponseText = result.Item2
                        };
                    }

                    return new HttpResponse()
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        ResponseText = "An error occured"
                    };
                }
            }

            if (httpMethod == "GET")
            {

                if (request.StartsWith("/users/"))
                {
                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);
                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    if (user.GetName() == "__connection__error__")
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An error occured"
                        };
                    }

                    var username = request[(request.LastIndexOf('/') + 1)..];

                    if (username != user.GetName() && !user.Admin)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }
                    var resultUser = username == user.GetName() ? user : await UserRepository.Instance.GetUserDataByUsername(username);

                    if (resultUser == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }
                    var userStack = await CardRepository.Instance.GetByUser(resultUser.Id);

                    var result = new UserDTO()
                    {
                        Username = resultUser.GetName(),
                        Coins = resultUser.Coins,
                        CardCount = userStack?.Count ?? 0,
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
                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);

                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    if(user.GetName() == "__connection__error__")
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An error occured"
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
                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);

                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    if (user.GetName() == "__connection__error__")
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An error occured"
                        };
                    }

                    var userCards = await CardRepository.Instance.GetByUser(user.Id);

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

                    return new HttpResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ResponseText = JsonSerializer.Serialize(usersCardDtos)
                    };
                }

                if (request == "/deck")
                {
                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);
                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    if (user.GetName() == "__connection__error__")
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An error occured"
                        };
                    }

                    var deck = await CardRepository.Instance.GetDeckByUser(user.Id);

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

                    return new HttpResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ResponseText = JsonSerializer.Serialize(usersCardDtos)
                    };
                }

                if (request == "/achievements")
                {
                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);
                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    if (user.GetName() == "__connection__error__")
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An error occured"
                        };
                    }

                    var result = await user.GetAchievements();

                    return new HttpResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ResponseText = JsonSerializer.Serialize(result)
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

                if (request == "/trade")
                {
                    if (!await AuthenticationController.Instance.IsAuthorized(requestAuthToken))
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    var result = await TradingController.Instance.GetTrades();

                    return result != null
                        ? new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.OK,
                            ResponseText = JsonSerializer.Serialize(result)
                        }
                        : new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.NoContent,
                            ResponseText = "No Trades Available"
                        };
                }

                if (request.StartsWith("/trade/") && !request.Contains("tradeRequest"))
                {
                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);
                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    if (user.GetName() == "__connection__error__")
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An error occured"
                        };
                    }

                    var username = request[(request.LastIndexOf('/') + 1)..];

                    if (username != user.GetName() && !user.Admin)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    var requestedUser = user.GetName() == username ? user : await UserRepository.Instance.GetByUsername(username);
                    
                    if (requestedUser == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ResponseText = "Invalid input data"
                        };
                    }

                    var result = await TradingController.Instance.GetTradesByUser(requestedUser);

                    return result != null
                        ? new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.OK,
                            ResponseText = JsonSerializer.Serialize(result)
                        }
                        : new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.NoContent,
                            ResponseText = "No Trades Available"
                        };

                }

                if (request.StartsWith("/trade/") && request.Contains("tradeRequest"))
                {
                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);
                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    if (user.GetName() == "__connection__error__")
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An error occured"
                        };
                    }

                    if (!int.TryParse(request[(request.IndexOf('/', 2) + 1)..(request.LastIndexOf('/'))],
                            out var tradeId))
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ResponseText = "Invalid input data"
                        };

                    var result = await TradingController.Instance.GetTradeRequestsForTrade(tradeId, user);

                    if(result.Item2 == false)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    return result.Item1 != null
                        ? new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.OK,
                            ResponseText = JsonSerializer.Serialize(result.Item1)
                        }
                        : new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.NoContent,
                            ResponseText = "No TradeRequests Available"
                        };

                }

                if (request.StartsWith("/shopitem"))
                {
                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);
                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    if (user.GetName() == "__connection__error__")
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An error occured"
                        };
                    }

                    var result = await ShopController.Instance.GetShopItems();

                    if (result == null || result.Count == 0)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.NoContent,
                            ResponseText = "No Shop Items available"
                        };
                    } else
                    {
                        var concreteItems = result.Cast<ShopItem>().ToList();
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.OK,
                            ResponseText = JsonSerializer.Serialize(concreteItems)
                        };
                    }
                }

            }


            if (httpMethod == "PUT")
            {
                if (request.StartsWith("/deck"))
                {
                    var user = await UserRepository.Instance.GetByAuthToken(requestAuthToken);
                    if (user == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    if (user.GetName() == "__connection__error__")
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An error occured"
                        };
                    }

                    var requestData = JsonSerializer.Deserialize<DeckRequest>(body);

                    if (requestData == null || requestData.cards.Length != 4)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ResponseText = "Bad Request"
                        };
                    }

                    if (await user.SelectDeck(requestData.cards))
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

                if(request.StartsWith("/users/"))
                {

                    var callingUser = await UserRepository.Instance.GetByAuthToken(requestAuthToken);
                    if (callingUser == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    if (callingUser.GetName() == "__connection__error__")
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An error occured"
                        };
                    }

                    var username = request[(request.LastIndexOf('/') + 1)..];

                    if (username != callingUser.GetName() && !callingUser.Admin)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    var editUserRequest = JsonSerializer.Deserialize<EditUsersRequest>(body);

                    if (editUserRequest == null || (string.IsNullOrEmpty(editUserRequest.Username) && string.IsNullOrEmpty(editUserRequest.Password)))
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ResponseText = "Invalid input data"
                        };
                    }

                    if (editUserRequest.Username == callingUser.GetName())
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Conflict,
                            ResponseText = "New username must be different"
                        };
                    }

                    var user = await UserRepository.Instance.GetUserDataByUsername(username);

                    var result = user != null && await user.Edit(editUserRequest.Username, editUserRequest.Password);

                    return result
                        ? new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.OK,
                            ResponseText = "User changed"
                        }
                        : new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "Error"
                        };

                }

                if (request.StartsWith("/tradeRequest/"))
                {
                    var callingUser = await UserRepository.Instance.GetByAuthToken(requestAuthToken);
                    if (callingUser == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    if (callingUser.GetName() == "__connection__error__")
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "An error occured"
                        };
                    }

                    if (int.TryParse(request[(request.LastIndexOf('/') + 1)..], out var tradeRequestId))
                    {

                        return await TradingController.Instance.AcceptTradeRequest(tradeRequestId, callingUser);
                    }

                    return new HttpResponse()
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        ResponseText = "Internal Server Error"
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
