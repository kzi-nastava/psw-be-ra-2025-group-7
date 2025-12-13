using Explorer.API.Controllers.Administrator;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Integration.Administration
{
    [Collection("Sequential")] // testovi se izvršavaju redom
    public class TourProblemControllerTests : BaseToursIntegrationTest
    {
        public TourProblemControllerTests(ToursTestFactory factory) : base(factory)
        {
            SeedTestData();
        }

        private void SeedTestData()
        {
            using var scope = Factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Obrisi postojece
            context.TourProblems.RemoveRange(context.TourProblems);
            context.SaveChanges();

            // Seed testnih problema
            context.TourProblems.Add(new TourProblem(
                tourId: -1,
                touristId: -11,
                category: ProblemCategory.Safety,
                priority: ProblemPriority.High,
                description: "Test problem 1",
                isSolved: false
            ));
            context.TourProblems.Add(new TourProblem(
                tourId: -2,
                touristId: -12,
                category: ProblemCategory.Equipment,
                priority: ProblemPriority.Medium,
                description: "Test problem 2",
                isSolved: true
            ));
            context.SaveChanges();
        }

        private AdminTourProblemController CreateController(IServiceScope scope)
        {
            return new AdminTourProblemController(
                scope.ServiceProvider.GetRequiredService<ITourProblemService>(),
                scope.ServiceProvider.GetRequiredService<Explorer.Notifications.API.Public.INotificationService>(),
                scope.ServiceProvider.GetRequiredService<Explorer.Tours.API.Public.Administration.ITourService>()
            );
        }

        [Fact]
        public void Can_get_all_problems()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var result = ((ObjectResult)controller.GetAll().Result).Value as List<TourProblemDto>;

            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
        }

        [Fact]
        public void Can_set_resolve_due()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var repo = scope.ServiceProvider.GetRequiredService<ITourProblemRepository>();

            var problem = repo.GetAll().First(r => !r.IsSolved);
            var resolveDate = DateTime.UtcNow.AddDays(3);

            var dto = new ResolveDueDto { ResolveDue = resolveDate.ToString("yyyy-MM-dd") };
            var result = ((ObjectResult)controller.SetResolveDue(problem.Id, dto).Result).Value as TourProblemDto;

            result.ShouldNotBeNull();
            result.ResolveDue.ShouldNotBeNull();
            result.ResolveDue.HasValue.ShouldBeTrue();  // Proveravamo da li je postavljen
            result.ResolveDue.Value.Date.ShouldBe(resolveDate.Date);
        }

        [Fact]
        public void Can_mark_problem_as_solved()
        {
            using var scope = Factory.Services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ITourProblemRepository>();

            var problem = repo.GetAll().First(r => !r.IsSolved);
            problem.IsSolved.ShouldBeFalse();

            problem.IsSolved = true;
            repo.Update(problem);

            var updated = repo.Get(problem.Id);
            updated.IsSolved.ShouldBeTrue();
        }

        [Fact]
        public void Old_and_unsolved_problem_should_be_highlighted()
        {
            using var scope = Factory.Services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ITourProblemRepository>();

            var problem = repo.GetAll().First(r => !r.IsSolved);
            problem.TimeReported = DateTime.UtcNow.AddDays(-6);
            repo.Update(problem);

            var nowProblem = repo.Get(problem.Id);
            nowProblem.IsSolved.ShouldBeFalse();
            (DateTime.UtcNow - nowProblem.TimeReported).TotalDays.ShouldBeGreaterThan(5);
        }

        [Fact]
        public void Can_set_penalty_on_problem()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var repo = scope.ServiceProvider.GetRequiredService<ITourProblemRepository>();

            // Uzimamo prvi problem koji nije rešen
            var problem = repo.GetAll().First(r => !r.IsSolved);
            problem.IsSolved.ShouldBeFalse();
            problem.Status.ShouldNotBe(ProblemStatus.Unresolved);

            // Pozivamo novu funkciju iz kontrolera
            var result = controller.SetPenalty(problem.Id).Result; // pretpostavljamo da je async ili ObjectResult
            var dto = ((ObjectResult)result).Value as TourProblemDto;

            // Proveravamo rezultat
            dto.ShouldNotBeNull();
            dto.IsSolved.ShouldBeTrue();
            dto.Status.ShouldBe(ProblemStatus.Unresolved.ToString());

            // Proveravamo i u bazi
            var updated = repo.Get(problem.Id);
            updated.IsSolved.ShouldBeTrue();
            updated.Status.ShouldBe(ProblemStatus.Unresolved);
        }

    }
}
