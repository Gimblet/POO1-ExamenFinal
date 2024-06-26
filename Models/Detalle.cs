using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Detalle
    {
        [Required]
        [Display(Name = "Codigo del Detalle", Order = 0)]
        public int IDE_DET { get; set; }

        [Required]
        [Display(Name = "Codigo del Almacen", Order = 1)]
        public int COD_ALM { get; set; }

        [Required]
        [Display(Name = "Codigo del Producto", Order = 2)]
        public int COD_PRO { get; set; }

        [Required]
        [Display(Name = "Cantidad", Order = 3)]
        public int CAN_PRO { get; set; }

        [Required]
        [Display(Name = "Tipo de Detalle", Order = 4)]
        public string TIP_DET { get; set; }

        [Display(Name = "Nombre del Producto")]
        public string NOM_PRO { get; set; }

        [Display(Name = "Ubicacion")]
        public string UBI_ALM { get; set; }

        [Display(Name = "Almacen")]
        public string NOM_ALM { get; set; }

        [Display(Name = "Unidad de Medida")]
        public string UME_PRO { get; set; }
    }
}