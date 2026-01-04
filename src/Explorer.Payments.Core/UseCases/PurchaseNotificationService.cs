using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases
{
    public class PurchaseNotificationService : IPurchaseNotificationService
    {
        private readonly IPurchaseNotificationRepository _repository;
        private readonly IMapper _mapper;

        public PurchaseNotificationService(IPurchaseNotificationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public List<PurchaseNotificationDto> GetAll(long touristId)
        {
            var items = _repository.GetAllByTouristId(touristId);
            return _mapper.Map<List<PurchaseNotificationDto>>(items);
        }

        public List<PurchaseNotificationDto> GetUnread(long touristId)
        {
            var items = _repository.GetUnreadByTouristId(touristId);
            return _mapper.Map<List<PurchaseNotificationDto>>(items);
        }

        public void MarkAsRead(long id, long touristId)
        {
            var notification = _repository.Get(id);
            if (notification == null || notification.TouristId != touristId)
            {
                throw new InvalidOperationException("Notification not found.");
            }

            if (!notification.IsRead)
            {
                notification.MarkAsRead();
                _repository.Update(notification);
            }
        }

        public void NotifyPurchaseSuccess(long touristId, IReadOnlyList<string> tourNames)
        {
            var message = BuildMessage(tourNames);
            var notification = new PurchaseNotification(touristId, message);
            _repository.Create(notification);
        }

        private static string BuildMessage(IReadOnlyList<string> tourNames)
        {
            if (tourNames == null || tourNames.Count == 0)
                return "Kupovina je uspešno obavljena! Nova tura je dodata u tvoju kolekciju.";

            if (tourNames.Count == 1)
                return $"Kupovina je uspešno obavljena! Tura \"{tourNames[0]}\" je dodata u tvoju kolekciju.";

            var preview = string.Join(", ", tourNames.Take(3));
            var suffix = tourNames.Count > 3 ? $" (+{tourNames.Count - 3} još)" : "";
            return $"Kupovina je uspešno obavljena! Dodato u tvoju kolekciju: {preview}{suffix}.";
        }
    }
}
