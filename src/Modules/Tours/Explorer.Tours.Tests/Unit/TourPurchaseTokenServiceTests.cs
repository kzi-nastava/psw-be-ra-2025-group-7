using System;
using System.Collections.Generic;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.UseCases.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Payments.API.Internal;
using Explorer.Stakeholders.API.Public;
using AutoMapper;
using Explorer.Tours.Core.Mappers;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit
{
    public class TourPurchaseTokenServiceTests
    {
        private readonly IMapper _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ToursProfile>()).CreateMapper();

        private static Tour CreatePublishedTour(long id = 1, decimal price = 100)
        {
            var tour = new Tour(1, "Name", "Description", TourDifficulty.Medium, new List<string>{"t"});
            var kp1 = new KeyPoint(45.0, 19.0, "Start", "Desc", "Secret");
            var kp2 = new KeyPoint(45.1, 19.1, "End", "Desc", "Secret");
            tour.AddKeyPoint(kp1);
            tour.AddKeyPoint(kp2);
            tour.AddDuration(new TourDuration(TravelType.Walk, 60));
            tour.Publish();
            tour.SetPrice(price);
            // set Id via reflection
            var idProp = typeof(Tour).BaseType?.GetProperty("Id");
            idProp?.SetValue(tour, id);
            return tour;
        }

        [Fact]
        public void Create_Succeeds_When_Wallet_Has_Enough()
        {
            var tour = CreatePublishedTour(3, 150);
            var repo = new FakeTokenRepo();
            var tourRepo = new FakeTourRepo(tour);
            var wallet = new FakeWalletInternalService(200);
            var payment = new FakePaymentInternalService(true);
            var notification = new FakeNotificationService();

            var svc = new TourPurchaseTokenService(repo, tourRepo, _mapper, wallet, payment, notification);

            var dto = svc.Create(10, 3);

            dto.ShouldNotBeNull();
            dto.UserId.ShouldBe(10);
            dto.TourId.ShouldBe(3);
        }

        [Fact]
        public void Create_Throws_When_Insufficient_Funds()
        {
            var tour = CreatePublishedTour(4, 500);
            var repo = new FakeTokenRepo();
            var tourRepo = new FakeTourRepo(tour);
            var wallet = new FakeWalletInternalService(100);
            var payment = new FakePaymentInternalService(false);
            var notification = new FakeNotificationService();

            var svc = new TourPurchaseTokenService(repo, tourRepo, _mapper, wallet, payment, notification);

            Should.Throw<InvalidOperationException>(() => svc.Create(11, 4)).Message.ShouldContain("Insufficient funds");
        }

        [Fact]
        public void Create_Throws_When_Already_Purchased()
        {
            var tour = CreatePublishedTour(5, 50);
            var repo = new FakeTokenRepo { HasPurchased = true };
            var tourRepo = new FakeTourRepo(tour);
            var wallet = new FakeWalletInternalService(1000);
            var payment = new FakePaymentInternalService(true);
            var notification = new FakeNotificationService();

            var svc = new TourPurchaseTokenService(repo, tourRepo, _mapper, wallet, payment, notification);

            Should.Throw<InvalidOperationException>(() => svc.Create(12, 5)).Message.ShouldContain("already purchased");
        }

        // Fake implementations
        class FakeTokenRepo : ITourPurchaseTokenRepository
        {
            public bool HasPurchased { get; set; } = false;
            public bool HasPurchasedCalled { get; private set; } = false;
            public bool HasPurchasedArgUser { get; private set; }
            public bool HasPurchasedArgTour { get; private set; }
            public bool HasUserPurchasedTour(long userId, long tourId)
            {
                HasPurchasedCalled = true;
                HasPurchasedArgUser = userId > 0;
                HasPurchasedArgTour = tourId > 0;
                return HasPurchased;
            }

            public TourPurchaseToken Create(TourPurchaseToken token)
            {
                // set id via reflection
                var idProp = typeof(TourPurchaseToken).GetProperty("Id");
                idProp?.SetValue(token, 99L);
                return token;
            }

            public TourPurchaseToken? GetByUserAndTour(long userId, long tourId) => null;
            public PagedResult<TourPurchaseToken> GetPagedByUser(int page, int pageSize, long userId) => new PagedResult<TourPurchaseToken>(new List<TourPurchaseToken>(), 0);
        }

        class FakeTourRepo : ITourRepository
        {
            private readonly Tour _tour;
            public FakeTourRepo(Tour t) => _tour = t;
            public PagedResult<Tour> GetPagedByAuthor(int page, int pageSize, long authorId) => new PagedResult<Tour>(new List<Tour>{_tour},1);
            public PagedResult<Tour> GetPublishedTours(int page, int pageSize) => new PagedResult<Tour>(new List<Tour>{_tour},1);
            public Tour Get(long id) => _tour;
            public Tour Create(Tour entity) => entity;
            public Tour Update(Tour entity) => entity;
            public void Delete(long id) { }
            public List<Tour> GetAll() => new List<Tour>{_tour};
            public IEnumerable<Tour> GetPublishedWithKeyPoints() => new List<Tour>{_tour};
        }

        class FakeWalletInternalService : Explorer.Payments.API.Internal.IWalletInternalService
        {
            private readonly decimal _balance;
            public FakeWalletInternalService(decimal balance) { _balance = balance; }
            public void CreateWallet(long userId) { }
            public decimal GetBalance(long userId) => _balance;
        }

        class FakePaymentInternalService : Explorer.Payments.API.Internal.IPaymentInternalService
        {
            private readonly bool _result;
            public FakePaymentInternalService(bool result) { _result = result; }
            public bool ChargeWallet(long userId, long? tourId, long? bundleId, decimal amount, string description) => _result;
        }

        class FakeNotificationService : Explorer.Stakeholders.API.Public.INotificationService
        {
            public void CreateFollowerMessageNotifications(Explorer.Stakeholders.API.Dtos.FollowerMessageDto message, List<long> followerIds) { }
            public void CreateClubMessageNotifications(Explorer.Stakeholders.API.Dtos.ClubMessageDto message, List<long> memberIds) { }
            public void DeleteClubMessageNotifications(long clubMessageId) { }
            public void DeleteFollowerMessageNotifications(long followerMessageId) { }
            public int GetUnreadCount(long userId) => 0;
            public List<Explorer.Stakeholders.API.Dtos.NotificationDto> GetForUser(long userId, bool onlyUnread = false) => new List<Explorer.Stakeholders.API.Dtos.NotificationDto>();
            public void MarkAllAsRead(long userId) { }
            public void MarkAsRead(long notificationId, long userId) { }
            public void Delete(long notificationId, long userId) { }
        }
    }
}
