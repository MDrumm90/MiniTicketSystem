using MiniTicketSystem.Application.Interfaces;
using MiniTicketSystem.Application.Models;
using MiniTicketSystem.Application.Services;
using MiniTicketSystem.Domain.Entities;
using MiniTicketSystem.Domain.Enum;
using Moq;
using Shouldly;

namespace MiniTicketSystem.UnitTests
{
    public class TicketQueryServiceTests
    {
        private Mock<ITicketRepository> _repositoryMock;
        private TicketQueryService _service;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<ITicketRepository>();
            _service = new TicketQueryService(_repositoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _repositoryMock = null;
            _service = null;
        }

        [Test]
        public async Task GetAllTicketsAsync_MultipleTickets_ReturnsAllMappedDtos()
        {
            // Arrange
            var tickets = new List<Ticket>
            {
                new Ticket { Id = Guid.NewGuid(), Title = "Ticket 1", Description = "Description 1", Status = TicketStatus.Open },
                new Ticket { Id = Guid.NewGuid(), Title = "Ticket 2", Description = "Description 2", Status = TicketStatus.Closed }
            };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(tickets);

            // Act
            var result = (await _service.GetAllTicketsAsync()).ToList();

            // Assert
            result.Count.ShouldBe(2);
            for (int i = 0; i < tickets.Count; i++)
            {
                result[i].Id.ShouldBe(tickets[i].Id);
                result[i].Title.ShouldBe(tickets[i].Title);
                result[i].Description.ShouldBe(tickets[i].Description);
                result[i].Status.ShouldBe(tickets[i].Status);
            }
        }

        [Test]
        public async Task GetAllTicketsAsync_EmptyRepository_ReturnsEmptyCollection()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Ticket>());

            // Act
            var result = await _service.GetAllTicketsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Test]
        public async Task GetAllTicketsAsync_ValidCall_CallsRepositoryGetAllAsyncOnce()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Ticket>());

            // Act
            await _service.GetAllTicketsAsync();

            // Assert
            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Test]
        public void GetAllTicketsAsync_RepositoryThrows_PropagatesException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new InvalidOperationException("repository failure"));

            // Act & Assert
            Should.ThrowAsync<InvalidOperationException>(() => _service.GetAllTicketsAsync());
        }

        [Test]
        public async Task GetPagedTicketsAsync_ValidParameters_ReturnsMappedPagedResult()
        {
            // Arrange
            var tickets = new List<Ticket>
            {
                new Ticket { Id = Guid.NewGuid(), Title = "Ticket 1", Description = "Description 1", Status = TicketStatus.Open },
                new Ticket { Id = Guid.NewGuid(), Title = "Ticket 2", Description = "Description 2", Status = TicketStatus.Closed }
            };
            var ticketPage = new TicketPage(tickets, TotalCount: 25);
            _repositoryMock.Setup(r => r.GetPageAsync(TicketStatus.Open, "foo", 2, 10)).ReturnsAsync(ticketPage);

            // Act
            var result = await _service.GetPagedTicketsAsync(TicketStatus.Open, "foo", 2, 10);

            // Assert
            result.Items.Count.ShouldBe(2);
            result.Items[0].Id.ShouldBe(tickets[0].Id);
            result.Items[1].Id.ShouldBe(tickets[1].Id);
            result.Page.ShouldBe(2);
            result.PageSize.ShouldBe(10);
            result.TotalCount.ShouldBe(25);
            result.TotalPages.ShouldBe(3); // ceiling(25 / 10)
        }

        [Test]
        public async Task GetPagedTicketsAsync_EmptyResult_ReturnsEmptyItemsAndZeroTotalPages()
        {
            // Arrange
            var ticketPage = new TicketPage(new List<Ticket>(), TotalCount: 0);
            _repositoryMock.Setup(r => r.GetPageAsync(null, null, 1, 10)).ReturnsAsync(ticketPage);

            // Act
            var result = await _service.GetPagedTicketsAsync(null, null, 1, 10);

            // Assert
            result.Items.ShouldBeEmpty();
            result.TotalCount.ShouldBe(0);
            result.TotalPages.ShouldBe(0);
        }

        [Test]
        public async Task GetPagedTicketsAsync_PageLessThanOne_ClampsPageToOneBeforeCallingRepository()
        {
            // Arrange
            var ticketPage = new TicketPage(new List<Ticket>(), TotalCount: 0);
            _repositoryMock.Setup(r => r.GetPageAsync(null, null, 1, 10)).ReturnsAsync(ticketPage);

            // Act
            var result = await _service.GetPagedTicketsAsync(null, null, 0, 10);

            // Assert
            _repositoryMock.Verify(r => r.GetPageAsync(null, null, 1, 10), Times.Once);
            result.Page.ShouldBe(1);
        }

        [Test]
        public async Task GetPagedTicketsAsync_PageSizeGreaterThan100_ClampsPageSizeTo100()
        {
            // Arrange
            var ticketPage = new TicketPage(new List<Ticket>(), TotalCount: 0);
            _repositoryMock.Setup(r => r.GetPageAsync(null, null, 1, 100)).ReturnsAsync(ticketPage);

            // Act
            var result = await _service.GetPagedTicketsAsync(null, null, 1, 500);

            // Assert
            _repositoryMock.Verify(r => r.GetPageAsync(null, null, 1, 100), Times.Once);
            result.PageSize.ShouldBe(100);
        }

        [Test]
        public async Task GetPagedTicketsAsync_PageSizeLessThanOne_ClampsPageSizeToOne()
        {
            // Arrange
            var ticketPage = new TicketPage(new List<Ticket>(), TotalCount: 0);
            _repositoryMock.Setup(r => r.GetPageAsync(null, null, 1, 1)).ReturnsAsync(ticketPage);

            // Act
            var result = await _service.GetPagedTicketsAsync(null, null, 1, 0);

            // Assert
            _repositoryMock.Verify(r => r.GetPageAsync(null, null, 1, 1), Times.Once);
            result.PageSize.ShouldBe(1);
        }

        [Test]
        public async Task GetPagedTicketsAsync_SearchWithSurroundingWhitespace_TrimsSearchBeforeCallingRepository()
        {
            // Arrange
            var ticketPage = new TicketPage(new List<Ticket>(), TotalCount: 0);
            _repositoryMock.Setup(r => r.GetPageAsync(null, "foo", 1, 10)).ReturnsAsync(ticketPage);

            // Act
            await _service.GetPagedTicketsAsync(null, "  foo  ", 1, 10);

            // Assert
            _repositoryMock.Verify(r => r.GetPageAsync(null, "foo", 1, 10), Times.Once);
        }

        [Test]
        public async Task GetPagedTicketsAsync_NullSearch_PassesNullToRepository()
        {
            // Arrange
            var ticketPage = new TicketPage(new List<Ticket>(), TotalCount: 0);
            _repositoryMock.Setup(r => r.GetPageAsync(null, null, 1, 10)).ReturnsAsync(ticketPage);

            // Act
            await _service.GetPagedTicketsAsync(null, null, 1, 10);

            // Assert
            _repositoryMock.Verify(r => r.GetPageAsync(null, null, 1, 10), Times.Once);
        }

        [Test]
        public void GetPagedTicketsAsync_RepositoryThrows_PropagatesException()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetPageAsync(It.IsAny<TicketStatus?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new InvalidOperationException("repository failure"));

            // Act & Assert
            Should.ThrowAsync<InvalidOperationException>(() => _service.GetPagedTicketsAsync(null, null, 1, 10));
        }
    }
}
