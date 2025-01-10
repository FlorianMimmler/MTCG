using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model.RequestObjects;
using MTCG.BusinessLayer.Model.Trading;
using MTCG.BusinessLayer.Model.User;
using MTCG.DataAccessLayer;
using MTCG.PresentationLayer;

namespace MTCG.BusinessLayer.Controller
{
    internal class TradingController
    {
        private static TradingController? _instance;
        public static TradingController Instance
        {
            get => _instance ??= new TradingController();
            set => _instance = value;
        } 

        private TradingController()
        {
        }

        public async Task<HttpResponse> CreateTrade(TradeRequest tradeRequest, User user)
        {
            var offeredCard = await CardRepository.Instance.GetByIdAndUserId(tradeRequest.OfferedCardId, user.Id);
            if (offeredCard == null)
            {
                return new HttpResponse()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ResponseText = "Invalid input data"
                };
            }
            

            var newTradingDeal = new TradingDeal(offeredCard, tradeRequest.Requirements, user.Id);

            var result = await TradeRepository.Instance.Add(newTradingDeal);

            if (result >= 0)
            {
                return new HttpResponse() { StatusCode = HttpStatusCode.OK, ResponseText = "Trade Created" };
            }

            return new HttpResponse()
            { StatusCode = HttpStatusCode.InternalServerError, ResponseText = "Internal Server Error" };
        }

        public async Task<HttpResponse> CreateTradeRquest(TradeRequestRequest tradeRequest, User user)
        {
            var offeredCard = await CardRepository.Instance.GetByIdAndUserId(tradeRequest.OfferedCardId, user.Id);
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

        public TradeRequest? ValidateTradeRequest(string rawTradeRequest)
        {
            var tradeRequest = JsonSerializer.Deserialize<TradeRequest>(rawTradeRequest);

            if (tradeRequest == null || tradeRequest.OfferedCardId < 0)
            {
                return null;
            }

            return tradeRequest;
        }

        public TradeRequestRequest? ValidateTradeRequestRequest(string rawTradeRequestRequest)
        {
            var tradeRequest = JsonSerializer.Deserialize<TradeRequestRequest>(rawTradeRequestRequest);

            if (tradeRequest == null || tradeRequest.OfferedCardId < 0 || tradeRequest.TradeId < 0)
            {
                return null;
            }

            return tradeRequest;
        }

        public async Task<IEnumerable<TradingDeal>?> GetTrades()
        {
            return await TradeRepository.Instance.GetAll();
        }

        public async Task<IEnumerable<TradingDeal>?> GetTradesByUser(User requestedUser)
        {
            return await TradeRepository.Instance.GetByUserId(requestedUser.Id);
        }

        public async Task<(IEnumerable<TradingDealRequest>?, bool)> GetTradeRequestsForTrade(int tradeId, User user)
        {
            var trade = await TradeRepository.Instance.GetById(tradeId);

            if (trade == null || (trade.OfferingUserId != user.Id && !user.Admin))
            {
                return (null, false);
            }

            return (await TradeRequestRepository.Instance.GetByTradeId(tradeId), true);
        }

        public async Task<HttpResponse> AcceptTradeRequest(int tradeRequestId, User callingUser)
        {
            var tradeRequest = await TradeRequestRepository.Instance.GetById(tradeRequestId);
            if (tradeRequest == null)
            {
                return new HttpResponse()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ResponseText = "Bad Request"
                };
            }

            var tradeOffer = await TradeRepository.Instance.GetById(tradeRequest.TradeId);
            if (tradeOffer == null)
            {
                return new HttpResponse()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ResponseText = "Internal Server Error"
                };
            }

            if (tradeOffer.OfferingUserId != callingUser.Id && !callingUser.Admin)
            {
                return new HttpResponse()
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    ResponseText = "Not authorized"
                };
            }

            if (await CardRepository.Instance.UpdateUserId(tradeRequest.OfferedCard,
                    tradeOffer.OfferingUserId))
            {
                if (await CardRepository.Instance.UpdateUserId(tradeOffer.OfferedCard,
                        tradeRequest.RequestUserId))
                {
                    _ = RemoveTrade(tradeOffer);
                    return new HttpResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ResponseText = "Trade executed"
                    };
                }
                else
                {
                    _ = await CardRepository.Instance.UpdateUserId(tradeRequest.OfferedCard,
                        tradeRequest.RequestUserId);
                }
            }

            return new HttpResponse()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                ResponseText = "Internal Server Error"
            };
        }

        private async Task<bool> RemoveTrade(TradingDeal trade)
        {
            return await TradeRepository.Instance.Delete(trade) == 1;
        }
    }
}
