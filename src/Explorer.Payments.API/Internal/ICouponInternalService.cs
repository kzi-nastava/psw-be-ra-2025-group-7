using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Internal;

public interface ICouponInternalService
{
    CouponDto CreateUniversalCouponForAuthor(long authorId, int discountPercentage);
    CouponDto CreateUniversalCoupon(int discountPercentage);
}
