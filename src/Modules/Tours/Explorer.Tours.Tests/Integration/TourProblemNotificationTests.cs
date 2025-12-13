using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration
{
    [Collection("Sequential")]
    public class TourProblemNotificationTests : BaseToursIntegrationTest
    {
        public TourProblemNotificationTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void AddAuthorReply_creates_notification()
        {
            int problemId;

            // ===== FIRST SCOPE: Create problem (tourist) =====
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateController(scope);

                var dto = new TourProblemDto
                {
                    TourId = -1,
                    Category = "Equipment",
                    Priority = "Medium",
                    Description = "Problem for notification test",
                    TimeReported = DateTime.UtcNow,
                    IsSolved = false
                };

                var created = ((ObjectResult)controller.Create(dto).Result)?.Value as TourProblemDto;
                created.ShouldNotBeNull();

                problemId = created.Id;
                problemId.ShouldBeGreaterThan(0);
            }

            // ===== SECOND SCOPE: count notifications BEFORE + call AddAuthorReply =====
            int beforeCount;
            using (var scope = Factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
                var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

                beforeCount = dbContext.Notifications.Count();

                // turist šalje poruku (kod tebe AddAuthorReply koristi i turist i autor/admin)
                service.AddAuthorReply(problemId, -1, "Hello from tourist");
            }

            // ===== THIRD SCOPE: count notifications AFTER =====
            using (var scope = Factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
                var afterCount = dbContext.Notifications.Count();

                // očekujemo da se kreirala makar 1 notifikacija
                afterCount.ShouldBeGreaterThan(beforeCount);
            }
        }

        [Fact]
        public void MarkAsUnresolved_creates_notification()
        {
            int problemId;

            // ===== FIRST SCOPE: Create problem =====
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateController(scope);

                var dto = new TourProblemDto
                {
                    TourId = -1,
                    Category = "Safety",
                    Priority = "High",
                    Description = "Initial problem",
                    TimeReported = DateTime.UtcNow,
                    IsSolved = false
                };

                var created = ((ObjectResult)controller.Create(dto).Result)?.Value as TourProblemDto;
                created.ShouldNotBeNull();

                problemId = created.Id;
                problemId.ShouldBeGreaterThan(0);
            }

            // ===== SECOND SCOPE: count BEFORE + call MarkAsUnresolved =====
            int beforeCount;
            using (var scope = Factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
                var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

                beforeCount = dbContext.Notifications.Count();

                service.MarkAsUnresolved(problemId, -1, "Not resolved yet");
            }

            // ===== THIRD SCOPE: count AFTER =====
            using (var scope = Factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
                var afterCount = dbContext.Notifications.Count();

                afterCount.ShouldBeGreaterThan(beforeCount);
            }
        }

        [Fact]
        public void MarkAsUnresolved_fails_empty_message_and_does_not_create_notification()
        {
            int problemId;

            // ===== FIRST SCOPE: Create problem =====
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateController(scope);

                var dto = new TourProblemDto
                {
                    TourId = -1,
                    Category = "Other",
                    Priority = "Low",
                    Description = "Test problem",
                    TimeReported = DateTime.UtcNow,
                    IsSolved = false
                };

                var created = ((ObjectResult)controller.Create(dto).Result)?.Value as TourProblemDto;
                created.ShouldNotBeNull();

                problemId = created.Id;
                problemId.ShouldBeGreaterThan(0);
            }

            // ===== SECOND SCOPE: count BEFORE + assert throws =====
            int beforeCount;
            using (var scope = Factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
                var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

                beforeCount = dbContext.Notifications.Count();

                Should.Throw<ArgumentException>(() =>
                    service.MarkAsUnresolved(problemId, -1, "")
                );
            }

            // ===== THIRD SCOPE: count AFTER (should be same) =====
            using (var scope = Factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
                var afterCount = dbContext.Notifications.Count();

                afterCount.ShouldBe(beforeCount);
            }
        }

        private static TourProblemController CreateController(IServiceScope scope)
        {
            return new TourProblemController(
                scope.ServiceProvider.GetRequiredService<ITourProblemService>())
            {
                ControllerContext = BuildContext("-1") // Ulogovan turist = -1
            };
        }
    }
}
