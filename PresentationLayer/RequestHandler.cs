using MTCG.Auth;
using MTCG.BusinessLayer.Controller;
using MTCG.BusinessLayer.Model.Card;
using MTCG.BusinessLayer.Model.User;
using System.Text.Json;
using MTCG.DataAccessLayer;
using MTCG.BusinessLayer.Model.RequestObjects;
using MTCG.BusinessLayer.Model.Trading;

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
                        StatusCode = HttpStatusCode.OK,
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
                    var result = battle.StartBattle();

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

                    var tradeRequest = JsonSerializer.Deserialize<TradeRequest>(body);

                    Console.WriteLine(tradeRequest.Requirements.MinDamage);

                    if (tradeRequest == null || tradeRequest.OfferedCardId < 0)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ResponseText = "Invalid input data"
                        };
                    }

                    var offeredCard = await CardRepository.Instance.GetById(tradeRequest.OfferedCardId);
                    if (offeredCard == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ResponseText = "Invalid input data"
                        };
                    }

                    Console.WriteLine(offeredCard.Damage);

                    var newTradingDeal = new TradingDeal(offeredCard, tradeRequest.Requirements, user.Id);

                    Console.WriteLine(newTradingDeal.OfferingUserId);

                    var result = await TradeRepository.Instance.Add(newTradingDeal);

                    if (result >= 0)
                    {
                        return new HttpResponse() { StatusCode = HttpStatusCode.OK, ResponseText = "Trade Created" };
                    }   

                    return new HttpResponse()
                        { StatusCode = HttpStatusCode.InternalServerError, ResponseText = "Internal Server Error" };
                    

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

                    var tradeRequest = JsonSerializer.Deserialize<TradeRequestRequest>(body);

                    if (tradeRequest == null || tradeRequest.OfferedCardId < 0 || tradeRequest.TradeId < 0)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ResponseText = "Invalid input data"
                        };
                    }

                    var offeredCard = await CardRepository.Instance.GetById(tradeRequest.OfferedCardId);
                    if (offeredCard == null)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ResponseText = "Invalid input data"
                        };
                    }

                    var tradeOffer = await TradeRepository.Instance.GetById(tradeRequest.TradeId);

                    if (tradeOffer != null && tradeOffer.OfferingUserId == user.Id)
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Conflict,
                            ResponseText = "You can't trade with yourself"
                        };
                    }

                    var newTradingRequest = new TradingDealRequest(tradeRequest.TradeId, user.Id, offeredCard);

                    var result = await TradeRequestRepository.Instance.Add(newTradingRequest);

                    if (result >= 0)
                    {
                        return new HttpResponse() { StatusCode = HttpStatusCode.OK, ResponseText = "TradeRequest Created" };
                    }

                    return new HttpResponse()
                        { StatusCode = HttpStatusCode.InternalServerError, ResponseText = "Internal Server Error" };


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

                    var result = await TradeRepository.Instance.GetAll();

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

                    var result = await TradeRepository.Instance.GetByUserId(requestedUser.Id);

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

                    if (!int.TryParse(request[(request.IndexOf('/', 2) + 1)..(request.LastIndexOf('/'))],
                            out var tradeId))
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ResponseText = "Invalid input data"
                        };

                    var trade = await TradeRepository.Instance.GetById(tradeId);

                    if (trade == null || (trade.OfferingUserId != user.Id && !user.Admin))
                    {
                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            ResponseText = "Not authorized"
                        };
                    }

                    var result = await TradeRequestRepository.Instance.GetByTradeId(tradeId);

                    return result != null
                        ? new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.OK,
                            ResponseText = JsonSerializer.Serialize(result)
                        }
                        : new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.NoContent,
                            ResponseText = "No TradeRequests Available"
                        };




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

                    if (int.TryParse(request[(request.LastIndexOf('/') + 1)..], out var tradeRequestId))
                    {

                        Console.WriteLine(tradeRequestId);

                        var tradeRequest = await TradeRequestRepository.Instance.GetById(tradeRequestId);
                        if (tradeRequest == null)
                        {
                            return new HttpResponse()
                            {
                                StatusCode = HttpStatusCode.BadRequest,
                                ResponseText = "Bad Request"
                            };
                        }

                        Console.WriteLine(tradeRequest.Id);

                        var tradeOffer = await TradeRepository.Instance.GetById(tradeRequest.TradeId);
                        if (tradeOffer == null)
                        {
                            return new HttpResponse()
                            {
                                StatusCode = HttpStatusCode.InternalServerError,
                                ResponseText = "Internal Server Error"
                            };
                        }
                        Console.WriteLine(tradeOffer.Id);

                        if (tradeOffer.OfferingUserId != callingUser.Id && !callingUser.Admin)
                        {
                            return new HttpResponse()
                            {
                                StatusCode = HttpStatusCode.Unauthorized,
                                ResponseText = "Not authorized"
                            };
                        }

                        Console.WriteLine($"Trade Request offered cardid {tradeRequest.OfferedCard.Id} and should become new UserID {tradeOffer.OfferingUserId}");
                        Console.WriteLine($"Trade Offer offered cardid {tradeOffer.OfferedCard.Id} and should become new UserID {tradeRequest.RequestUserId}");

                        Console.WriteLine("execute trade");

                        if (await CardRepository.Instance.UpdateUserId(tradeRequest.OfferedCard,
                                tradeOffer.OfferingUserId))
                        {
                            if (await CardRepository.Instance.UpdateUserId(tradeOffer.OfferedCard,
                                    tradeRequest.RequestUserId))
                            {
                                return new HttpResponse()
                                {
                                    StatusCode = HttpStatusCode.OK,
                                    ResponseText = "Trade executed"
                                };
                            }
                        }

                        return new HttpResponse()
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            ResponseText = "Internal Server Error"
                        };

                    }

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
