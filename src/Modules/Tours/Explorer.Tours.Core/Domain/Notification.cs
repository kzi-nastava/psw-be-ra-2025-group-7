using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class Notification : Entity
    {
        public long UserId { get; private set; }      // primaoc notifikacije
        public string Title { get; private set; }    // npr. "Nova poruka na prijavljenom problemu"
        public string Preview { get; private set; }  // prvih 20-30 karaktera poruke
        public int ProblemId { get; private set; }   // kojim problemom je vezana
        public DateTime CreatedAt { get; private set; }
        public bool IsRead { get; private set; }

        private Notification() { } // za EF

        public Notification(long userId, string title, string preview, int problemId, DateTime createdAt)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.");
            if (string.IsNullOrWhiteSpace(preview)) throw new ArgumentException("Preview is required.");

            UserId = userId;
            Title = title.Trim();
            Preview = preview.Trim();
            ProblemId = problemId;
            CreatedAt = createdAt;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}
