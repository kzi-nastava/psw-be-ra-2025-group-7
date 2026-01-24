using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        public SaleService(ISaleRepository saleRepository, IMapper mapper)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        public List<SaleDto> GetAll()
        {
            var sales = _saleRepository.GetAll();

            foreach (var s in sales)
            {
                Console.WriteLine($"SALE {s.Id} TOURS COUNT = {s.SaleTours.Count}");
            }

            return _mapper.Map<List<SaleDto>>(sales);
        }


        public List<SaleDto> GetAllActive()
        {
            var sales = _saleRepository.GetAllActive(DateTime.UtcNow);
            return _mapper.Map<List<SaleDto>>(sales);
        }

        public SaleDto Create(SaleDto saleDto)
        {

            if (saleDto.TourIds == null || saleDto.TourIds.Count == 0)
                throw new Exception("Sale must contain at least one tour.");

            if (saleDto.DiscountPercentage < 1 || saleDto.DiscountPercentage > 100)
                throw new Exception("Invalid discount percentage.");

            if (saleDto.End < saleDto.Start)
                throw new Exception("End date must be after start date.");

            var sale = new Sale
            {
                AuthorId = saleDto.AuthorId,
                Start = saleDto.Start,
                End = saleDto.End,
                DiscountPercentage = saleDto.DiscountPercentage,
                Status = SaleStatus.Draft,
                SaleTours = saleDto.TourIds
                    .Select(tourId => new SaleTour
                    {
                        TourId = tourId
                    })
                    .ToList()
            };

            var created = _saleRepository.Create(sale);

            return _mapper.Map<SaleDto>(created);
        }

        public SaleDto Activate(long id)
        {
            var sale = _saleRepository.Get(id);
            if (sale == null)
                throw new Exception("Sale not found.");

            if (sale.End > sale.Start.AddDays(14))
                throw new Exception("Sale cannot last more than 14 days.");

            sale.Status = SaleStatus.Active;

            _saleRepository.Update(sale);
            return _mapper.Map<SaleDto>(sale);
        }

        public void Archive(long id)
        {
            var sale = _saleRepository.Get(id);
            if (sale == null) return;

            sale.Status = SaleStatus.Expired;
            _saleRepository.Update(sale);
        }

        public SaleDto Update(SaleDto saleDto)
        {
            var sale = _mapper.Map<Sale>(saleDto);

            Validate(sale);

            _saleRepository.Update(sale);
            return _mapper.Map<SaleDto>(sale);
        }

        // ---------------- VALIDACIJA ----------------
        private void Validate(Sale sale)
        {
            if (sale.SaleTours == null || !sale.SaleTours.Any())
                throw new Exception("Sale must contain at least 1 tour.");

            if (sale.DiscountPercentage < 1 || sale.DiscountPercentage > 100)
                throw new Exception("Invalid discount.");

            if (sale.End < sale.Start)
                throw new Exception("End date must be after start date.");
        }
    }
}
