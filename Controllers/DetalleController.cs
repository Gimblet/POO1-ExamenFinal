using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Data;

namespace WebApplication1.Controllers
{
    public class DetalleController : Controller
    {
        private static string Cadena = ConfigurationManager.ConnectionStrings["Cadena"].ConnectionString;
        SqlConnection cnx = new SqlConnection(Cadena);

        #region Metodos

        private IEnumerable<Detalle> listarDetalle()
        {
            List<Detalle> lista = new List<Detalle>();
            SqlCommand cmd = new SqlCommand("SP_ListadoGeneral", cnx);
            cmd.CommandType = CommandType.StoredProcedure;
            cnx.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Detalle detalle = new Detalle();
                {
                    detalle.COD_ALM = dr.GetInt32(0);
                    detalle.COD_PRO = dr.GetInt32(1);
                    detalle.IDE_DET = dr.GetInt32(2);
                    detalle.NOM_PRO = dr.GetString(3);
                    detalle.UME_PRO = dr.GetString(4);
                    detalle.CAN_PRO = dr.GetInt32(5);
                    detalle.NOM_ALM = dr.GetString(6);
                    detalle.UBI_ALM = dr.GetString(7);
                    detalle.TIP_DET = dr.GetString(8);
                };
                lista.Add(detalle);
            };
            cnx.Close();
            dr.Close();
            return lista;
        }
        #endregion

        public ActionResult Index()
        {
            return View(listarDetalle());
        }
    }
}