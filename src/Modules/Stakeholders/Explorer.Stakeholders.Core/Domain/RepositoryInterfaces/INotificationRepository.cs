using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface INotificationRepository
    {
        Notification Create(Notification notification);
        List<Notification> GetForTourist(long touristId);
        Notification Get(long id);       
        void Update(Notification notification);  
    }
}
