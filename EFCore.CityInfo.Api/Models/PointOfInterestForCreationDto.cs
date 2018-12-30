using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EFCore.CityInfo.Api.Models
{
    public class PointOfInterestForCreationDto
    {
        [Required(ErrorMessage = "This field is Mandatory")]
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(150)]
        public string Description { get; set; }

    }
}
