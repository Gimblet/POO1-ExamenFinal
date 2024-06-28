using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Web.WebPages;

namespace WebApplication1.Controllers
{
    public class AlmacenController : Controller
    {
        private static string Cadena = ConfigurationManager.ConnectionStrings["Cadena"].ConnectionString;
        SqlConnection cnx = new SqlConnection(Cadena);

        #region Metodos

        IEnumerable<Almacen> listarAlmacenes()
        {
            List<Almacen> almacenes = new List<Almacen>();
            SqlCommand cmd = new SqlCommand("SP_ListarAlmacen", cnx);
            cmd.CommandType = CommandType.StoredProcedure;
            cnx.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Almacen almacen = new Almacen();
                {
                    almacen.COD_ALM = dr.GetInt32(0);
                    almacen.NOM_ALM = dr.GetString(1);
                    almacen.UBI_ALM = dr.GetString(2);
                };
                almacenes.Add(almacen);
            };
            cnx.Close();
            return almacenes;
        }

        private string AgregarAlmacen(Almacen almacen)
        {
            string mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("SP_AgregarAlmacen", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codigo", almacen.COD_ALM);
                cmd.Parameters.AddWithValue("@nombre", almacen.NOM_ALM);
                cmd.Parameters.AddWithValue("@ubicacion", almacen.UBI_ALM);
                cnx.Open();
                int value = cmd.ExecuteNonQuery();
                mensaje = $"Se Agrego {value} almacen/es";
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }
            finally
            {
                cnx.Close();
            }
            return mensaje;
        }

        private string ActualizarAlmacen(Almacen almacen)
        {
            string mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("SP_ActualizarAlmacen", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codigo", almacen.COD_ALM);
                cmd.Parameters.AddWithValue("@nombre", almacen.NOM_ALM);
                cmd.Parameters.AddWithValue("@ubicacion", almacen.UBI_ALM);
                cnx.Open();
                int value = cmd.ExecuteNonQuery();
                mensaje = $"Se modificaron los atributos de {value} almacen/es";
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }
            finally
            {
                cnx.Close();
            }
            return mensaje;
        }

        private string EliminarAlmacen(int codigo)
        {
            string mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("SP_EliminarAlmacen", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codigo", codigo);
                cnx.Open();
                int value = cmd.ExecuteNonQuery();
                mensaje = $"Se eliminaron {value} almacen/es";
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                if (mensaje.Contains("FK_TO_ALMACEN"))
                {
                    mensaje = "Error >> No se puede eliminar el almacen porque actualmente almacena productos";
                }
                else
                {
                    mensaje = "Error >> Ocurrió un problema";
                }
            }
            finally
            {
                cnx.Close();
            }
            return mensaje;
        }

        private Almacen obtenerAlmacen(int ID)
        {
            Almacen almacen = null;
            try
            {
                SqlCommand cmd = new SqlCommand("SP_BuscarAlmacen", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codigo", ID);
                cnx.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    almacen = new Almacen();
                    almacen.COD_ALM = dr.GetInt32(0);
                    almacen.NOM_ALM = dr.GetString(1);
                    almacen.UBI_ALM = dr.GetString(2);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cnx.Close();
            }
            return almacen;
        }

        int GenerarCodigo()
        {
            SqlCommand cmd = new SqlCommand("SP_GenerarCodigoAlmacen", cnx);
            cmd.CommandType = CommandType.StoredProcedure;
            cnx.Open();
            int codigo = Convert.ToInt32(cmd.ExecuteScalar());
            cnx.Close();
            return codigo;
        }

        private Almacen Buscar(int id)
        {
            Almacen reg;
            reg = obtenerAlmacen(id);
            return reg;
        }

        #endregion

        public ActionResult Index(string mensaje)
        {
            if (!mensaje.IsEmpty() && mensaje.Contains("Error"))
            {
                ViewBag.error = mensaje;
                return View(listarAlmacenes());
            }
            else
            {
                ViewBag.mensaje = mensaje;
            }
            return View(listarAlmacenes());
        }
        public ActionResult Create()
        {
            Almacen almacen = new Almacen();
            {
                almacen.COD_ALM = GenerarCodigo();
            };
            return View(almacen);
        }

        [HttpPost]
        public ActionResult Create(Almacen almacen)
        {
            string mensaje = string.Empty;
            if (ModelState.IsValid)
            {
                mensaje = AgregarAlmacen(almacen);
                return RedirectToAction("Index", new { mensaje = mensaje });
            }
            else
            {
                mensaje = "Error";
                return RedirectToAction("Create", new { error = mensaje });
            }
        }

        public ActionResult Edit(int id)
        {
            return View(Buscar(id));
        }

        [HttpPost]
        public ActionResult Edit(Almacen almacen)
        {
            string mensaje = string.Empty;
            if (ModelState.IsValid)
            {
                mensaje = ActualizarAlmacen(almacen);
                return RedirectToAction("Index", new { mensaje = mensaje });
            }
            else
            {
                mensaje = "Error";
                return RedirectToAction("Edit", new { mensaje = mensaje });
            }
        }

        public ActionResult Delete(int id)
        {
            string mensaje = EliminarAlmacen(id);
            return RedirectToAction("Index", new { mensaje = mensaje });
        }

        public ActionResult Details(Almacen almacen)
        {
            return View(almacen);
        }

        [HttpPost]
        public ActionResult BuscarXCodigo(string ID)
        {
            int codigo = Convert.ToInt32(ID);
            Almacen almacen = obtenerAlmacen(codigo);
            if (almacen == null)
            {
                string mensaje = "Error >> Almacen no encontrado";
                return RedirectToAction("Index", new { mensaje = mensaje });
            }
            return View("Details", almacen);
        }
    }
}