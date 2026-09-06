using MiniTicketSystem.Application.DTOs;
using MiniTicketSystem.Application.Interfaces;
using MiniTicketSystem.Application.Models;
using MiniTicketSystem.Application.Services;
using MiniTicketSystem.Domain.Entities;
using MiniTicketSystem.Domain.Enum;
using Moq;
using Shouldly;

namespace MiniTicketSystem.UnitTests
{
    public class TicketCommandServiceTests
    {
        private Mock<ITicketRepository> _repositoryMock;
        private TicketCommandService _service;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<ITicketRepository>();
            _service = new TicketCommandService(_repositoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _repositoryMock = null;
            _service = null;
        }

        [Test]
        public async Task CreateTicketAsync_ValidDto_ReturnsMappedDtoWithOpenStatus()
        {
            // Arrange
            var dto = new TicketInsertDto("New Ticket", "New Description");

            // Act
            var result = await _service.CreateTicketAsync(dto);

            // Assert
            result.Title.ShouldBe(dto.Title);
            result.Description.ShouldBe(dto.Description);
            result.Status.ShouldBe(TicketStatus.Open);
            result.Id.ShouldNotBe(Guid.Empty);
            result.UpdatedAt.ShouldBeNull();
        }

        [Test]
        public async Task CreateTicketAsync_ValidDto_CallsRepositoryAddAsyncOnce()
        {
            // Arrange
            var dto = new TicketInsertDto("New Ticket", "New Description");

            // Act
            await _service.CreateTicketAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.AddAsync(It.Is<Ticket>(t =>
                t.Title == dto.Title &&
                t.Description == dto.Description &&
                t.Status == TicketStatus.Open)), Times.Once);
        }

        [Test]
        public void CreateTicketAsync_RepositoryThrows_PropagatesException()
        {
            // Arrange
            var dto = new TicketInsertDto("New Ticket", "New Description");
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Ticket>())).ThrowsAsync(new InvalidOperationException("repository failure"));

            // Act & Assert
            Should.ThrowAsync<InvalidOperationException>(() => _service.CreateTicketAsync(dto));
        }

        [Test]
        public async Task CreateTicketAsync_ValidDto_LeavesUpdatedAtNull()
        {
            // Arrange
            var dto = new TicketInsertDto("New Ticket", "New Description");
            Ticket? addedTicket = null;
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Ticket>())).Callback<Ticket>(t => addedTicket = t);

            // Act
            await _service.CreateTicketAsync(dto);

            // Assert
            addedTicket.ShouldNotBeNull();
            addedTicket!.UpdatedAt.ShouldBeNull();
        }

        [Test]
        public async Task UpdateTicketAsync_ExistingTicket_ReturnsUpdatedMappedDto()
        {
            // Arrange
            var ticket = new Ticket { Id = Guid.NewGuid(), Title = "Old Title", Description = "Old Description", Status = TicketStatus.Open };
            var dto = new TicketUpdateDto("New Title", ticket.Id, "New Description", TicketStatus.InProgress);
            _repositoryMock.Setup(r => r.GetByIdAsync(ticket.Id)).ReturnsAsync(ticket);

            // Act
            var result = await _service.UpdateTicketAsync(dto);

            // Assert
            result.ShouldNotBeNull();
            result!.Id.ShouldBe(ticket.Id);
            result.Title.ShouldBe(dto.Title);
            result.Description.ShouldBe(dto.Description);
            result.Status.ShouldBe(dto.Status);
            result.UpdatedAt.ShouldNotBeNull();
        }

        [Test]
        public async Task UpdateTicketAsync_NonExistingTicket_ReturnsNull()
        {
            // Arrange
            var dto = new TicketUpdateDto("Title", Guid.NewGuid(), "Description", TicketStatus.Open);
            _repositoryMock.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync((Ticket?)null);

            // Act
            var result = await _service.UpdateTicketAsync(dto);

            // Assert
            result.ShouldBeNull();
        }

        [Test]
        public async Task UpdateTicketAsync_NonExistingTicket_DoesNotCallRepositoryUpdateAsync()
        {
            // Arrange
            var dto = new TicketUpdateDto("Title", Guid.NewGuid(), "Description", TicketStatus.Open);
            _repositoryMock.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync((Ticket?)null);

            // Act
            await _service.UpdateTicketAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
        }

        [Test]
        public async Task UpdateTicketAsync_StatusChangedToClosed_SetsDateClosed()
        {
            // Arrange
            var ticket = new Ticket { Id = Guid.NewGuid(), Title = "Title", Description = "Description", Status = TicketStatus.Open, DateClosed = default };
            var dto = new TicketUpdateDto("Title", ticket.Id, "Description", TicketStatus.Closed);
            _repositoryMock.Setup(r => r.GetByIdAsync(ticket.Id)).ReturnsAsync(ticket);

            // Act
            await _service.UpdateTicketAsync(dto);

            // Assert
            ticket.DateClosed.ShouldNotBe(default);
        }

        [Test]
        public async Task UpdateTicketAsync_StatusChangedFromClosedToOpen_ClearsDateClosed()
        {
            // Arrange
            var ticket = new Ticket { Id = Guid.NewGuid(), Title = "Title", Description = "Description", Status = TicketStatus.Closed, DateClosed = DateTime.UtcNow };
            var dto = new TicketUpdateDto("Title", ticket.Id, "Description", TicketStatus.Open);
            _repositoryMock.Setup(r => r.GetByIdAsync(ticket.Id)).ReturnsAsync(ticket);

            // Act
            await _service.UpdateTicketAsync(dto);

            // Assert
            ticket.DateClosed.ShouldBe(default);
        }

        [Test]
        public async Task UpdateTicketAsync_StatusUnchangedClosed_DoesNotResetDateClosed()
        {
            // Arrange
            var originalDateClosed = DateTime.UtcNow.AddDays(-1);
            var ticket = new Ticket { Id = Guid.NewGuid(), Title = "Title", Description = "Description", Status = TicketStatus.Closed, DateClosed = originalDateClosed };
            var dto = new TicketUpdateDto("Title", ticket.Id, "Description", TicketStatus.Closed);
            _repositoryMock.Setup(r => r.GetByIdAsync(ticket.Id)).ReturnsAsync(ticket);

            // Act
            await _service.UpdateTicketAsync(dto);

            // Assert
            ticket.DateClosed.ShouldBe(originalDateClosed);
        }

        [Test]
        public async Task UpdateTicketAsync_ValidCall_CallsRepositoryUpdateAsyncOnce()
        {
            // Arrange
            var ticket = new Ticket { Id = Guid.NewGuid(), Title = "Title", Description = "Description", Status = TicketStatus.Open };
            var dto = new TicketUpdateDto("New Title", ticket.Id, "New Description", TicketStatus.InProgress);
            _repositoryMock.Setup(r => r.GetByIdAsync(ticket.Id)).ReturnsAsync(ticket);

            // Act
            await _service.UpdateTicketAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.UpdateAsync(ticket), Times.Once);
        }

        [Test]
        public async Task UpdateTicketAsync_ValidCall_SetsUpdatedAt()
        {
            // Arrange
            var ticket = new Ticket { Id = Guid.NewGuid(), Title = "Title", Description = "Description", Status = TicketStatus.Open, UpdatedAt = null };
            var dto = new TicketUpdateDto("New Title", ticket.Id, "New Description", TicketStatus.InProgress);
            _repositoryMock.Setup(r => r.GetByIdAsync(ticket.Id)).ReturnsAsync(ticket);

            // Act
            var result = await _service.UpdateTicketAsync(dto);

            // Assert
            ticket.UpdatedAt.ShouldNotBeNull();
            result!.UpdatedAt.ShouldBe(ticket.UpdatedAt);
        }

        [Test]
        public void UpdateTicketAsync_RepositoryGetByIdThrows_PropagatesException()
        {
            // Arrange
            var dto = new TicketUpdateDto("Title", Guid.NewGuid(), "Description", TicketStatus.Open);
            _repositoryMock.Setup(r => r.GetByIdAsync(dto.Id)).ThrowsAsync(new InvalidOperationException("repository failure"));

            // Act & Assert
            Should.ThrowAsync<InvalidOperationException>(() => _service.UpdateTicketAsync(dto));
        }
    }
}
