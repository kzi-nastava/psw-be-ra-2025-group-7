using System.Collections.Generic;

namespace Explorer.Notes.API.Internal
{
    public interface INoteInternalService
    {
        void CreateCouponNote(long touristId, string couponCode, string authorFullName, long? tourId = null);
    }
}
