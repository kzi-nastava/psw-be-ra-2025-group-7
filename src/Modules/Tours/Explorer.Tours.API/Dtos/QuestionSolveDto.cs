using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class QuestionSolveDto
    {
        public long QuestionId { get; set; }
        public List<long> SelectedOptionIds { get; set; }
    }
}
