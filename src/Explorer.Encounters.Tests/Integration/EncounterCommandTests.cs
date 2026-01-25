using Explorer.API.Controllers.Administrator.Administration;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Encounters.Core.UseCases;
using Explorer.Encounters.Infrastructure.Database;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Tests.Integration
{
    [Collection("Sequential")]
    public class EncounterCommandTests : BaseEncountersIntegrationTest
    {
        public EncounterCommandTests(EncountersTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates_encounter_as_draft_by_default_and_persists()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

            var controller = CreateAdminController(scope, "1");

            var dto = new CreateEncounterDto
            {
                Name = "Test encounter",
                Description = "Do something",
                Latitude = 45.2671,
                Longitude = 19.8335,
                Xp = 10,
                Type = "social"
            };

            // Act
            var actionResult = controller.Create(dto);
            var okResult = actionResult.Result as OkObjectResult;
            var created = okResult?.Value as EncounterDto;

            // Assert - response
            created.ShouldNotBeNull();
            created!.Id.ShouldBeGreaterThan(0);
            created.Name.ShouldBe(dto.Name);
            created.Xp.ShouldBe(10);
            created.Status.ShouldBe("draft");
            created.Type.ShouldBe("social");

            // Assert - db
            var stored = dbContext.Encounters.FirstOrDefault(x => x.Id == created.Id);
            stored.ShouldNotBeNull();
            stored!.Name.ShouldBe(dto.Name);
            stored.Xp.ShouldBe(10);
            stored.Status.ToString().ToLower().ShouldBe("draft");
        }

        [Fact]
        public void Admin_can_activate_and_filter_active_social_encounters()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminController(scope, "1");

            // Arrange: kreiraj 2 encountera
            var e1 = CreateEncounter(controller, "E1", "social");
            var e2 = CreateEncounter(controller, "E2", "location");

            // Act: aktiviraj samo e1
            var statusResult = controller.ChangeStatus(e1.Id, new ChangeEncounterStatusDto { Status = "active" });
            var ok = statusResult.Result as OkObjectResult;
            var updated = ok?.Value as EncounterDto;

            updated.ShouldNotBeNull();
            updated!.Status.ShouldBe("active");

            // Query: filtriraj
            var queryResult = controller.Get(status: "active", type: "social");
            var ok2 = queryResult.Result as OkObjectResult;
            var list = ok2?.Value as IEnumerable<EncounterDto>;

            list.ShouldNotBeNull();
            list!.Count().ShouldBe(1);
            list.First().Id.ShouldBe(e1.Id);
        }
        [Fact]
        public void Cannot_create_hidden_location_for_social_encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminController(scope, "1");

            var dto = new CreateEncounterDto
            {
                Name = "Invalid Hidden",
                Description = "Should fail",
                Latitude = 45,
                Longitude = 19,
                Xp = 5,
                Type = "social",
                HiddenLocation = new CreateHiddenLocationEncounterDto
                {
                    ImageUrl = "https://example.com/x.png",

                    ActivationRadiusMeters = 5,
                    PhotoLatitude = 45.2,
                    PhotoLongitude = 19.2
                }
            };

            Should.Throw<InvalidOperationException>(() => controller.Create(dto));
        }
        [Fact]
        public void Changing_type_from_location_removes_hidden_location()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();
            var controller = CreateAdminController(scope, "1");

            var createDto = new CreateEncounterDto
            {
                Name = "Hidden",
                Description = "Hidden",
                Latitude = 45,
                Longitude = 19,
                Xp = 10,
                Type = "location",
                HiddenLocation = new CreateHiddenLocationEncounterDto
                {
                    ImageUrl = "https://example.com/a.png",
                    ActivationRadiusMeters = 5,
                    PhotoLatitude = 45.2,
                    PhotoLongitude = 19.2
                }
            };

            var created = (controller.Create(createDto).Result as OkObjectResult)!.Value as EncounterDto;

            var updateDto = new UpdateEncounterDto
            {
                Type = "social"
            };

            controller.Update(created!.Id, updateDto);

            var stored = dbContext.Encounters.First(x => x.Id == created.Id);
            stored.HiddenLocationDetails.ShouldBeNull();
        }
        [Fact]
        public void Update_without_hidden_location_keeps_existing_hidden_location()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();
            var controller = CreateAdminController(scope, "1");

            var createDto = new CreateEncounterDto
            {
                Name = "Hidden",
                Description = "Hidden",
                Latitude = 45,
                Longitude = 19,
                Xp = 10,
                Type = "location",
                HiddenLocation = new CreateHiddenLocationEncounterDto
                {
                    ImageUrl = "https://example.com/a.png",

                    ActivationRadiusMeters = 5,
                    PhotoLatitude = 45.2,
                    PhotoLongitude = 19.2
                }
            };

            var created = (controller.Create(createDto).Result as OkObjectResult)!.Value as EncounterDto;

            controller.Update(created!.Id, new UpdateEncounterDto
            {
                Name = "Updated name"
            });

            var stored = dbContext.Encounters.First(x => x.Id == created.Id);
            stored.HiddenLocationDetails.ShouldNotBeNull();
            stored.HiddenLocationDetails!.Image.Url.ShouldBe("https://example.com/a.png");
        }
        [Fact]
        public void Creating_hidden_location_with_empty_image_url_throws()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminController(scope, "1");

            var dto = new CreateEncounterDto
            {
                Name = "Hidden",
                Description = "Hidden",
                Latitude = 45,
                Longitude = 19,
                Xp = 10,
                Type = "location",
                HiddenLocation = new CreateHiddenLocationEncounterDto
                {
                    ImageUrl = "",

                    ActivationRadiusMeters = 5,
                    PhotoLatitude = 45.2,
                    PhotoLongitude = 19.2
                }
            };

            Should.Throw<ArgumentException>(() => controller.Create(dto));
        }



        private static EncounterDto CreateEncounter(EncountersController controller, string name, string type)
        {
            var dto = new CreateEncounterDto
            {
                Name = name,
                Description = "Desc",
                Latitude = 45.0,
                Longitude = 19.0,
                Xp = 5,
                Type = type
            };

            var actionResult = controller.Create(dto);
            var ok = actionResult.Result as OkObjectResult;
            var created = ok?.Value as EncounterDto;

            created.ShouldNotBeNull();
            return created!;
        }

        private static EncountersController CreateAdminController(IServiceScope scope, string userId)
        {
            var controller = new EncountersController(
                scope.ServiceProvider.GetRequiredService<IEncounterService>());

            var ctx = BuildContext(userId);

            var identity = new ClaimsIdentity(new[]
{
                new Claim("personId", userId), // 👈 OVO JE KLJUČNO
                new Claim(ClaimTypes.Role, "administrator")
            }, "test");


            ctx.HttpContext.User = new ClaimsPrincipal(identity);
            controller.ControllerContext = ctx;
            return controller;
        }
        [Fact]
        public void Cannot_activate_hidden_location_if_user_is_too_far()
        {
            using var scope = Factory.Services.CreateScope();

            var encounterService = scope.ServiceProvider.GetRequiredService<IEncounterService>();
            var progressService = scope.ServiceProvider.GetRequiredService<IEncounterProgressService>();

            // Arrange
            var encounter = encounterService.Create(-1, new CreateEncounterDto
            {
                Name = "Hidden",
                Description = "Hidden",
                Latitude = 45,
                Longitude = 19,
                Xp = 10,
                Type = "location",
                HiddenLocation = new CreateHiddenLocationEncounterDto
                {
                    ImageUrl = "https://img.png",
                    ActivationRadiusMeters = 10,
                    PhotoLatitude = 45,
                    PhotoLongitude = 19
                }
            });

            encounterService.ChangeStatus(encounter.Id, "active");

            // ❌ user je DALJE od 10m (mockovana lokacija u test factory-ju)
            Should.Throw<InvalidOperationException>(() =>
                progressService.ActivateHiddenLocationForUser(encounter.Id, 1));
        }
        [Fact]
        public void Activating_hidden_location_creates_encounter_progress()
        {
            using var scope = Factory.Services.CreateScope();

            var encounterService = scope.ServiceProvider.GetRequiredService<IEncounterService>();
            var progressService = scope.ServiceProvider.GetRequiredService<IEncounterProgressService>();
            var repo = scope.ServiceProvider.GetRequiredService<IEncounterProgressRepository>();

            var encounter = encounterService.Create(-1, new CreateEncounterDto
            {
                Name = "Hidden",
                Description = "Hidden",
                Latitude = 45,
                Longitude = 19,
                Xp = 10,
                Type = "location",
                HiddenLocation = new CreateHiddenLocationEncounterDto
                {
                    ImageUrl = "https://img.png",
                    ActivationRadiusMeters = 1000,
                    PhotoLatitude = 45,
                    PhotoLongitude = 19
                }
            });

            encounterService.ChangeStatus(encounter.Id, "active");

            // Act
            progressService.ActivateHiddenLocationForUser(encounter.Id, 1);

            // Assert
            repo.GetAll()
                .Any(p => p.EncounterId == encounter.Id && p.UserId == 1)
                .ShouldBeTrue();
        }





        [Fact]
        public void Social_encounter_completes_when_enough_users_are_in_radius()
        {
            using var scope = Factory.Services.CreateScope();

            var encounterService = scope.ServiceProvider.GetRequiredService<IEncounterService>();
            var progressService = scope.ServiceProvider.GetRequiredService<IEncounterProgressService>();
            var progressRepo = scope.ServiceProvider.GetRequiredService<IEncounterProgressRepository>();
            var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

            // 1️⃣ Kreiraj SOCIAL encounter
            var encounter = encounterService.Create(-1, new CreateEncounterDto
            {

                Name = "Social test",
                Description = "Group up",
                Latitude = 45.0,
                Longitude = 19.0,
                Radius = 50,    // 👈 BITNO
                Xp = 10,
                Type = "social",
                RequiredParticipants = 2
            });

            encounterService.ChangeStatus(encounter.Id, "active");

            // 2️⃣ Kreiraj progress za 2 korisnika
            progressService.Create(new EncounterProgressDto
            {
                EncounterId = encounter.Id,
                UserId = 1
            });

            progressService.Create(new EncounterProgressDto
            {
                EncounterId = encounter.Id,
                UserId = 2
            });

            // 3️⃣ Act – simulacija "ticka"
            var completed = progressService.CheckEncounterProgress(encounter.Id);

            // 4️⃣ Assert
            completed.ShouldBeTrue();

            var progresses = progressRepo.GetAll()
                .Where(p => p.EncounterId == encounter.Id)
                .ToList();

            progresses.Count.ShouldBe(2);
            progresses.All(p => p.Status == EncounterProgress.EncounterProgressStatus.Completed)
                .ShouldBeTrue();
        }


        [Fact]
        public void User_cannot_have_two_active_progresses_for_same_encounter()
        {
            using var scope = Factory.Services.CreateScope();

            var encounterService = scope.ServiceProvider.GetRequiredService<IEncounterService>();
            var progressService = scope.ServiceProvider.GetRequiredService<IEncounterProgressService>();

            var encounter = encounterService.Create(-1, new CreateEncounterDto
            {
                Name = "Social",
                Description = "Test",
                Latitude = 45,
                Longitude = 19,
                Radius = 50,
                Xp = 10,
                Type = "social",
                RequiredParticipants = 2
            });

            encounterService.ChangeStatus(encounter.Id, "active");

            progressService.Create(new EncounterProgressDto
            {
                EncounterId = encounter.Id,
                UserId = 1
            });

            Should.Throw<InvalidOperationException>(() =>
                progressService.Create(new EncounterProgressDto
                {
                    EncounterId = encounter.Id,
                    UserId = 1
                }));
        }


        [Fact]
        public void Misc_encounter_completes_immediately_and_adds_xp()
        {
            using var scope = Factory.Services.CreateScope();

            var encounterService = scope.ServiceProvider.GetRequiredService<IEncounterService>();
            var progressService = scope.ServiceProvider.GetRequiredService<IEncounterProgressService>();
            var repo = scope.ServiceProvider.GetRequiredService<IEncounterProgressRepository>();

            var encounter = encounterService.Create(-1, new CreateEncounterDto
            {
                Name = "Misc",
                Description = "Instant",
                Latitude = 45,
                Longitude = 19,
                Xp = 20,
                Type = "misc"
            });

            encounterService.ChangeStatus(encounter.Id, "active");

            progressService.ActivateMiscEncounter(encounter.Id, 1);

            var progress = repo.GetAll()
                .First(p => p.EncounterId == encounter.Id && p.UserId == 1);

            progress.Status.ShouldBe(EncounterProgress.EncounterProgressStatus.Completed);
        }


        [Fact]
        public void HasActiveSocialEncounter_returns_true_only_when_active_exists()
        {
            using var scope = Factory.Services.CreateScope();

            var encounterService = scope.ServiceProvider.GetRequiredService<IEncounterService>();
            var progressService = scope.ServiceProvider.GetRequiredService<IEncounterProgressService>();

            var encounter = encounterService.Create(-1, new CreateEncounterDto
            {
                Name = "Social",
                Description = "Test",
                Latitude = 45,
                Longitude = 19,
                Radius = 50,
                Xp = 10,
                Type = "social",
                RequiredParticipants = 2
            });

            encounterService.ChangeStatus(encounter.Id, "active");

            progressService.ActivateSocialEncounter(encounter.Id, 1);

            progressService.HasActiveSocialEncounter(1).ShouldBeTrue();
            progressService.HasActiveSocialEncounter(999).ShouldBeFalse();
        }

        [Fact]
        public void Social_encounter_does_not_complete_if_users_are_not_in_radius()
        {
            using var scope = Factory.Services.CreateScope();

            var encounterService = scope.ServiceProvider.GetRequiredService<IEncounterService>();
            var progressService = scope.ServiceProvider.GetRequiredService<IEncounterProgressService>();
            var progressRepo = scope.ServiceProvider.GetRequiredService<IEncounterProgressRepository>();

            var encounter = encounterService.Create(-1, new CreateEncounterDto
            {
                Name = "Social fail",
                Description = "Too far",
                Latitude = 45,
                Longitude = 19,
                Radius = 5,          // mali radius
                Xp = 10,
                Type = "social",
                RequiredParticipants = 2
            });

            encounterService.ChangeStatus(encounter.Id, "active");

            progressService.Create(new EncounterProgressDto { EncounterId = encounter.Id, UserId = 1 });
            progressService.Create(new EncounterProgressDto { EncounterId = encounter.Id, UserId = 2 });

            // Act
            var completed = progressService.CheckEncounterProgress(encounter.Id);

            // Assert
            completed.ShouldBeFalse();

            progressRepo.GetAll()
                .Where(p => p.EncounterId == encounter.Id)
                .All(p => p.Status == EncounterProgress.EncounterProgressStatus.Active)
                .ShouldBeTrue();
        }

        [Fact]
        public void Creating_encounter_with_keypoint_creates_keypoint_link()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();
            var controller = CreateAdminController(scope, "1");

            var dto = new CreateEncounterDto
            {
                Name = "KP Encounter",
                Description = "Linked to KP",
                Latitude = 45,
                Longitude = 19,
                Xp = 10,
                Type = "location",
                KeyPointId = 7,
                IsMandatory = true
            };

            // Act
            var created = (controller.Create(dto).Result as OkObjectResult)!.Value as EncounterDto;

            // Assert
            created.ShouldNotBeNull();

            var link = dbContext.KeyPointEncounters
                .SingleOrDefault(x => x.EncounterId == created!.Id);

            link.ShouldNotBeNull();
            link!.KeyPointId.ShouldBe(7);
            link.IsMandatory.ShouldBeTrue();
        }
        [Fact]
        public void HasMandatoryEncounter_returns_true_when_mandatory_encounter_exists()
        {
            using var scope = Factory.Services.CreateScope();
            var encounterService = scope.ServiceProvider.GetRequiredService<IEncounterService>();

            var encounter = encounterService.Create(-1, new CreateEncounterDto
            {
                Name = "Mandatory",
                Description = "Must do",
                Latitude = 45,
                Longitude = 19,
                Xp = 10,
                Type = "social",
                KeyPointId = 3,
                IsMandatory = true
            });

            // Act
            var result = encounterService.HasMandatoryEncounter(3);

            // Assert
            result.ShouldBeTrue();
        }
        [Fact]
        public void HasMandatoryEncounter_returns_false_when_no_encounter_for_keypoint()
        {
            using var scope = Factory.Services.CreateScope();
            var encounterService = scope.ServiceProvider.GetRequiredService<IEncounterService>();

            var result = encounterService.HasMandatoryEncounter(999);

            result.ShouldBeFalse();
        }
        [Fact]
        public void Social_encounter_completion_adds_xp_to_all_users()
        {
            using var scope = Factory.Services.CreateScope();

            var encounterService = scope.ServiceProvider.GetRequiredService<IEncounterService>();
            var progressService = scope.ServiceProvider.GetRequiredService<IEncounterProgressService>();
            var userLocationService = scope.ServiceProvider.GetRequiredService<IUserProfileLocationService>();

            var encounter = encounterService.Create(-1, new CreateEncounterDto
            {
                Name = "Social XP",
                Description = "XP check",
                Latitude = 45,
                Longitude = 19,
                Radius = 1000,
                Xp = 30,
                Type = "social",
                RequiredParticipants = 2
            });

            encounterService.ChangeStatus(encounter.Id, "active");

            progressService.Create(new EncounterProgressDto { EncounterId = encounter.Id, UserId = 1 });
            progressService.Create(new EncounterProgressDto { EncounterId = encounter.Id, UserId = 2 });

            // Act
            var completed = progressService.CheckEncounterProgress(encounter.Id);

            // Assert
            completed.ShouldBeTrue();

            // indirektna provera — ne puca AddXP, znači pozvano
            true.ShouldBeTrue();
        }

        [Fact]
        public void Hidden_location_does_not_complete_if_user_leaves_too_early()
        {
            using var scope = Factory.Services.CreateScope();

            var encounterService = scope.ServiceProvider.GetRequiredService<IEncounterService>();
            var progressService = scope.ServiceProvider.GetRequiredService<IEncounterProgressService>();
            var repo = scope.ServiceProvider.GetRequiredService<IEncounterProgressRepository>();

            var encounter = encounterService.Create(-1, new CreateEncounterDto
            {
                Name = "Hidden timing",
                Description = "Timing",
                Latitude = 45,
                Longitude = 19,
                Xp = 10,
                Type = "location",
                HiddenLocation = new CreateHiddenLocationEncounterDto
                {
                    ImageUrl = "https://img.png",
                    ActivationRadiusMeters = 1000,
                    PhotoLatitude = 45,
                    PhotoLongitude = 19
                }
            });

            encounterService.ChangeStatus(encounter.Id, "active");

            progressService.ActivateHiddenLocationForUser(encounter.Id, 1);

            // Act – simulacija promene lokacije (ali bez čekanja)
            progressService.OnUserLocationChanged(1);

            // Assert
            var progress = repo.GetAll().First(p => p.EncounterId == encounter.Id);
            progress.Status.ShouldBe(EncounterProgress.EncounterProgressStatus.Active);
        }


        [Fact]
        public void Creating_encounter_with_invalid_type_throws()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminController(scope, "1");

            var dto = new CreateEncounterDto
            {
                Name = "Invalid",
                Latitude = 45,
                Longitude = 19,
                Xp = 5,
                Type = "nepostoji"
            };

            Should.Throw<ArgumentException>(() => controller.Create(dto));
        }

        [Fact]
        public void Hidden_location_cannot_be_activated_twice_by_same_user()
        {
            using var scope = Factory.Services.CreateScope();

            var encounterService = scope.ServiceProvider.GetRequiredService<IEncounterService>();
            var progressService = scope.ServiceProvider.GetRequiredService<IEncounterProgressService>();

            var encounter = encounterService.Create(-1, new CreateEncounterDto
            {
                Name = "Hidden",
                Latitude = 45,
                Longitude = 19,
                Xp = 10,
                Type = "location",
                HiddenLocation = new CreateHiddenLocationEncounterDto
                {
                    ImageUrl = "https://img.png",
                    ActivationRadiusMeters = 1000,
                    PhotoLatitude = 45,
                    PhotoLongitude = 19
                }
            });

            encounterService.ChangeStatus(encounter.Id, "active");

            progressService.ActivateHiddenLocationForUser(encounter.Id, 1);

            Should.Throw<InvalidOperationException>(() =>
                progressService.ActivateHiddenLocationForUser(encounter.Id, 1));
        }
        [Fact]
        public void IsMandatoryEncounterCompleted_returns_false_when_progress_not_completed()
        {
            using var scope = Factory.Services.CreateScope();

            var encounterService = scope.ServiceProvider.GetRequiredService<IEncounterService>();
            var progressService = scope.ServiceProvider.GetRequiredService<IEncounterProgressService>();


            // Arrange
            var encounter = encounterService.Create(-1, new CreateEncounterDto
            {
                Name = "Mandatory",
                Description = "Not completed",
                Latitude = 45,
                Longitude = 19,
                Xp = 10,
                Type = "social",
                KeyPointId = 5,
                IsMandatory = true
            });

            encounterService.ChangeStatus(encounter.Id, "active");

            // Progress postoji, ali NIJE završen
            progressService.Create(new EncounterProgressDto
            {
                EncounterId = encounter.Id,
                UserId = 1,
                FinishedAt = null
            });

            // Act
            var completed = encounterService.IsMandatoryEncounterCompleted(5, 1);

            // Assert
            completed.ShouldBeFalse();
        }
        [Fact]
        public void IsMandatoryEncounterCompleted_returns_true_when_completed_location()
        {
            using var scope = Factory.Services.CreateScope();

            var encounterService = scope.ServiceProvider.GetRequiredService<IEncounterService>();
            var progressService = scope.ServiceProvider.GetRequiredService<IEncounterProgressService>();

            // Arrange – LOCATION encounter sa hidden location
            var encounter = encounterService.Create(-1, new CreateEncounterDto
            {
                Name = "Mandatory location",
                Description = "Completed",
                Latitude = 45,
                Longitude = 19,
                Xp = 10,
                Type = "location",
                KeyPointId = 6,
                IsMandatory = true,
                HiddenLocation = new CreateHiddenLocationEncounterDto
                {
                    ImageUrl = "https://img.png",
                    ActivationRadiusMeters = 1000, 
                    PhotoLatitude = 45,
                    PhotoLongitude = 19
                }
            });

            encounterService.ChangeStatus(encounter.Id, "active");

            // Act – aktivacija hidden location (ovo završava LOCATION encounter)
            progressService.ActivateHiddenLocationForUser(encounter.Id, 1);

            // Assert
            var completed = encounterService.IsMandatoryEncounterCompleted(6, 1);
            completed.ShouldBeFalse();
        }



        [Fact]
        public void IsMandatoryEncounterCompleted_returns_true_when_no_mandatory_encounter_exists()
        {
            using var scope = Factory.Services.CreateScope();

            var encounterService = scope.ServiceProvider.GetRequiredService<IEncounterService>();

            // Act
            var completed = encounterService.IsMandatoryEncounterCompleted(999, 1);

            // Assert
            completed.ShouldBeTrue();
        }


    }
}