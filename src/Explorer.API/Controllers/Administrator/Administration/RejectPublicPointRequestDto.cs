using System.ComponentModel.DataAnnotations;

namespace Explorer.Tours.API.Controllers.Administrator.Administration
{
    public class RejectPublicPointRequestDto
    {
        [Required]
        public string Comment { get; set; } = string.Empty;
    }
}
