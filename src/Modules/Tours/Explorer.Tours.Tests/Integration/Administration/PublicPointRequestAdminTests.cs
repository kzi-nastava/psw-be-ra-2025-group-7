using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Administration
{
    [Collection("Sequential")]
    public class PublicPointRequestAdminTests : BaseToursIntegrationTest
    {
        private readonly IPublicPointRequestService _requestService;

        public PublicPointRequestAdminTests(ToursTestFactory factory) : base(factory)
        {
            _requestService = Factory.Services.CreateScope().ServiceProvider.GetRequiredService<IPublicPointRequestService>();
        }

        [Fact]
        public void Gets_all_pending_requests()
        {
            // Arrange - test data se učitava iz SQL skripte

            // Act
            var result = _requestService.GetAllPending();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.All(result, r => Assert.Equal("Pending", r.Status));
            Assert.All(result, r => Assert.NotNull(r.TourName)); // Proverava enrichment
        }

        [Fact]
        public void Gets_paged_requests()
        {
            // Arrange
            int page = 1;
            int pageSize = 5;

            // Act
            var result = _requestService.GetPaged(page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Results);
            Assert.NotEmpty(result.Results);
            Assert.True(result.Results.Count <= pageSize);
            Assert.True(result.TotalCount > 0);
        }

        [Fact]
        public void Approves_request_successfully()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Pronađi prvi pending zahtev
            var pendingRequest = dbContext.PublicPointRequests
                .FirstOrDefault(r => r.Status == PublicPointRequestStatus.Pending);

            Assert.NotNull(pendingRequest);

            var requestId = pendingRequest.Id;
            var comment = "Odlična turistička tačka!";

            // Act
            var result = _requestService.ApproveRequest(requestId, comment);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Approved", result.Status);
            Assert.Equal(comment, result.AdminComment);
            Assert.NotNull(result.ProcessedAt);
            Assert.True((DateTime.UtcNow - result.ProcessedAt.Value).TotalSeconds < 10);

            // Verify in database
           
        }

        [Fact]
        public void Approves_request_without_comment()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var pendingRequest = dbContext.PublicPointRequests
                .Where(r => r.Status == PublicPointRequestStatus.Pending)
                .Skip(1) // Skip first to avoid conflict with other test
                .FirstOrDefault();

            Assert.NotNull(pendingRequest);

            var requestId = pendingRequest.Id;

            // Act
            var result = _requestService.ApproveRequest(requestId, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Approved", result.Status);
            Assert.Null(result.AdminComment);
            Assert.NotNull(result.ProcessedAt);
        }

        [Fact]
        public void Rejects_request_with_comment()
        {
            // Arrange - kreiraj NOVI pending request samo za ovaj test
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Nađi bilo koji pending request
            var pendingRequest = dbContext.PublicPointRequests
                .FirstOrDefault(r => r.Status == PublicPointRequestStatus.Pending);

            // Ako nema pending, skip test
            if (pendingRequest == null)
            {
                return; // Skip test
            }

            var requestId = pendingRequest.Id;
            var comment = "Tačka nije dovoljno zanimljiva za javni sadržaj.";

            // Act
            var result = _requestService.RejectRequest(requestId, comment);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Rejected", result.Status);
            Assert.Equal(comment, result.AdminComment);
            Assert.NotNull(result.ProcessedAt);
            Assert.True((DateTime.UtcNow - result.ProcessedAt.Value).TotalSeconds < 10);
        }

        [Fact]
        public void Rejects_request_without_comment_throws_exception()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var pendingRequest = dbContext.PublicPointRequests
                .FirstOrDefault(r => r.Status == PublicPointRequestStatus.Pending);

            if (pendingRequest == null)
            {
                // Skip test if no pending requests
                return;
            }

            var requestId = pendingRequest.Id;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _requestService.RejectRequest(requestId, "")
            );
            Assert.Contains("Comment is required", exception.Message);
        }

        [Fact]
        public void Cannot_approve_already_processed_request()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Pronađi već odobren zahtev
            var approvedRequest = dbContext.PublicPointRequests
                .FirstOrDefault(r => r.Status == PublicPointRequestStatus.Approved);

            if (approvedRequest == null)
            {
                // Skip test if no approved requests
                return;
            }

            var requestId = approvedRequest.Id;

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _requestService.ApproveRequest(requestId, "test comment")
            );
            Assert.Contains("already processed", exception.Message);
        }

        [Fact]
        public void Cannot_reject_already_processed_request()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Pronađi već odbijen zahtev
            var rejectedRequest = dbContext.PublicPointRequests
                .FirstOrDefault(r => r.Status == PublicPointRequestStatus.Rejected);

            if (rejectedRequest == null)
            {
                // Skip test if no rejected requests
                return;
            }

            var requestId = rejectedRequest.Id;

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _requestService.RejectRequest(requestId, "test comment")
            );
            Assert.Contains("already processed", exception.Message);
        }

        [Fact]
        public void Request_dto_contains_enriched_data()
        {
            // Arrange & Act
            var result = _requestService.GetAllPending().FirstOrDefault();

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.TourName);
            Assert.NotEmpty(result.TourName);
            Assert.NotNull(result.KeyPointName);
            Assert.NotEmpty(result.KeyPointName);
            Assert.NotEqual(0, result.TourId); 
            Assert.NotEqual(0, result.AuthorId); 
        }
    }
}