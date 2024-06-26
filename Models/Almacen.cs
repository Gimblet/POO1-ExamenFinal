using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Almacen
    {
        [Required]
        [Display(Name = "Codigo Almacen", Order = 0)]
        public int COD_ALM { get; set; }

        [Required]
        [Display(Name = "Nombre", Order = 1)]
        public string NOM_ALM { get; set; }

        [Required]
        [Display(Name = "Ubicacion", Order = 2)]
        public string UBI_ALM { get; set; }
    }
}