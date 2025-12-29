using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class BundleItem : Entity
    {
        public long TourId { get; private set; }

        private BundleItem() { } // EF

        public BundleItem(long tourId)
        {
            if (tourId == 0)
                throw new ArgumentException("TourId must not be 0.");

            TourId = tourId;
        }
    }
}
