using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface IClubService
    {
        ClubDto Create(ClubDto club);
        ClubDto Update(ClubDto club);
        void Delete(long Id);
        List<ClubDto> GetAll();

    }
}
