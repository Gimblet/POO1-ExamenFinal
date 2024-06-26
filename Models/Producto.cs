using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Producto
    {
        [Required]
        [Display(Name = "ID Producto", Order = 0)]
        public int COD_PRO { get; set; }

        [Required]
        [Display(Name = "Nombre", Order = 1)]
        public string NOM_PRO { get; set; }

        [Required]
        [Display(Name = "Unidad de Medida", Order = 2)]
        public string UME_PRO { get; set; }
    }
}