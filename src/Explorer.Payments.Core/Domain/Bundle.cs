using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class Bundle : AggregateRoot
    {
        public long AuthorId { get; private set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public BundleStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public List<BundleItem> Items { get; private set; } = new();

        private Bundle() { } // EF

        public Bundle(long authorId, string name, decimal price, IEnumerable<long> tourIds)
        {
            AuthorId = authorId;
            Name = name;
            Price = price;
            Status = BundleStatus.Draft;

            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            SetTours(tourIds);
            Validate();
        }

        public void Update(string name, decimal price, IEnumerable<long> tourIds)
        {
            EnsureDraft();

            Name = name;
            Price = price;

            SetTours(tourIds);

            UpdatedAt = DateTime.UtcNow;
            Validate();
        }

        private void SetTours(IEnumerable<long> tourIds)
        {
            if (tourIds == null) throw new ArgumentNullException(nameof(tourIds));

            var unique = tourIds
                .Distinct()
                .ToList();

            // Dozvoljavamo i negativne i pozitivne ID-eve; samo 0 ne.
            if (unique.Any(id => id == 0))
                throw new ArgumentException("TourIds must not contain 0.");

            // ✅ BITNO: NE ZAMENJUJ LISTU (Items = ...), nego clear + add
            Items.Clear();
            foreach (var id in unique)
            {
                Items.Add(new BundleItem(id));
            }
        }

        private void EnsureDraft()
        {
            if (Status != BundleStatus.Draft)
                throw new InvalidOperationException("Bundle can only be modified while in Draft status.");
        }

        private void Validate()
        {
            // dozvoljavamo negativan authorId (seed), samo 0 ne
            if (AuthorId == 0)
                throw new ArgumentException("AuthorId is required.");

            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Bundle name is required.");

            if (Name.Length > 200)
                throw new ArgumentException("Bundle name cannot exceed 200 characters.");

            if (Price < 0)
                throw new ArgumentException("Bundle price cannot be negative.");

            if (Items == null || Items.Count == 0)
                throw new ArgumentException("Bundle must contain at least one tour.");
        }
    }
}
