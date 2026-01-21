using Explorer.Payments.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Public
{
    public interface ISaleService
    {
        List<SaleDto> GetAll();
        List<SaleDto> GetAllActive();
        SaleDto Create(SaleDto sale);
        SaleDto Activate(long id);
        void Archive(long id);
        SaleDto Update(SaleDto sale);
    }
}
