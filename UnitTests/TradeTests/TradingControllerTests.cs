using MTCG.BusinessLayer.Controller;
using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model.RequestObjects;
using MTCG.BusinessLayer.Model.Trading;
using MTCG.BusinessLayer.Model.User;
using MTCG.DataAccessLayer;
using MTCG.PresentationLayer;
using NSubstitute;

namespace MTCG_uTests.TradeTests
{
    internal class TradingControllerTests
    {

        private TradingController _controller;
        private ICardRepository _cardRepositoryMock;
        private ITradeRepository _tradeRepositoryMock;
        private ITradeRequestRepository _tradeRequestRepositoryMock;
        private User _user;

        [SetUp]
        public void SetUp()
        {
            _cardRepositoryMock = Substitute.For<ICardRepository>();
            _tradeRepositoryMock = Substitute.For<ITradeRepository>();
            _tradeRequestRepositoryMock = Substitute.For<ITradeRequestRepository>();

            CardRepository.Instance = _cardRepositoryMock;
            TradeRepository.Instance = _tradeRepositoryMock;
            TradeRequestRepository.Instance = _tradeRequestRepositoryMock;

            _controller = TradingController.Instance;
            _user = new User { Id = 1, Admin = false };
        }

        [Test]
        public async Task CreateTrade_ValidRequest_ShouldReturnOk()
        {
            // Arrange
            var tradeRequest = new TradeRequest { OfferedCardId = 1 };
            var card = Substitute.For<ICard>();
            _cardRepositoryMock.GetByIdAndUserId(tradeRequest.OfferedCardId, _user.Id).Returns(card);
            _tradeRepositoryMock.Add(Arg.Any<TradingDeal>()).Returns(Task.FromResult(1));

            // Act
            var response = await _controller.CreateTrade(tradeRequest, _user);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.ResponseText, Is.EqualTo("Trade Created"));
        }

        [Test]
        public async Task CreateTrade_InvalidCard_ShouldReturnBadRequest()
        {
            // Arrange
            var tradeRequest = new TradeRequest { OfferedCardId = 1 };
            _cardRepositoryMock.GetByIdAndUserId(tradeRequest.OfferedCardId, _user.Id).Returns((ICard?)null);

            // Act
            var response = await _controller.CreateTrade(tradeRequest, _user);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.ResponseText, Is.EqualTo("Invalid input data"));
        }

        [Test]
        public async Task CreateTradeRequest_TradeWithSelf_ShouldReturnConflict()
        {
            // Arrange
            var tradeRequest = new TradeRequestRequest { OfferedCardId = 1, TradeId = 2 };
            var card = Substitute.For<ICard>();
            var trade = new TradingDeal(Substitute.For<ICard>(), null, _user.Id);

            _cardRepositoryMock.GetByIdAndUserId(tradeRequest.OfferedCardId, _user.Id).Returns(card);
            _tradeRepositoryMock.GetById(tradeRequest.TradeId).Returns(trade);

            // Act
            var response = await _controller.CreateTradeRquest(tradeRequest, _user);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            Assert.That(response.ResponseText, Is.EqualTo("You can't trade with yourself"));
        }

        [Test]
        public void ValidateTradeRequest_ValidJson_ShouldReturnObject()
        {
            // Arrange
            var json = "{\"OfferedCardId\":1}";

            // Act
            var tradeRequest = _controller.ValidateTradeRequest(json);

            // Assert
            Assert.That(tradeRequest, Is.Not.Null);
            Assert.That(tradeRequest.OfferedCardId, Is.EqualTo(1));
        }

        [Test]
        public void ValidateTradeRequest_InvalidJson_ShouldReturnNull()
        {
            // Arrange
            var json = "{\"InvalidField\":1}";

            // Act
            var tradeRequest = _controller.ValidateTradeRequest(json);

            // Assert
            Assert.That(tradeRequest, Is.Null);
        }

        [Test]
        public async Task AcceptTradeRequest_Unauthorized_ShouldReturnUnauthorized()
        {
            // Arrange
            var tradeRequest = new TradingDealRequest(1, 100, Substitute.For<ICard>());
            var trade = new TradingDeal(Substitute.For<ICard>(), null, 2);

            _tradeRequestRepositoryMock.GetById(Arg.Any<int>()).Returns(tradeRequest);
            _tradeRepositoryMock.GetById(Arg.Any<int>()).Returns(trade);

            // Act
            var response = await _controller.AcceptTradeRequest(1, _user);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(response.ResponseText, Is.EqualTo("Not authorized"));
        }

        [Test]
        public async Task AcceptTradeRequest_TradeExecuted_ShouldReturnOk()
        {
            // Arrange
            var tradeRequest = new TradingDealRequest(1, 3, Substitute.For<ICard>());
            var trade = new TradingDeal(Substitute.For<ICard>(), null, _user.Id); 

            _tradeRequestRepositoryMock.GetById(1).Returns(tradeRequest);
            _tradeRepositoryMock.GetById(tradeRequest.TradeId).Returns(trade);
            _cardRepositoryMock.UpdateUserId(Arg.Any<ICard>(), Arg.Any<int>()).Returns(Task.FromResult(true));

            // Act
            var response = await _controller.AcceptTradeRequest(1, _user);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.ResponseText, Is.EqualTo("Trade executed"));
        }


    }
}
