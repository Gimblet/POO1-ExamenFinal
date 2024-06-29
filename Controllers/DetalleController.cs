using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Data;
using System.Web.WebPages;

namespace WebApplication1.Controllers
{
    public class DetalleController : Controller
    {
        private static string Cadena = ConfigurationManager.ConnectionStrings["Cadena"].ConnectionString;
        SqlConnection cnx = new SqlConnection(Cadena);

        #region Metodos

        private IEnumerable<Detalle> listarDetalles()
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
                    detalle.IDE_DET = dr.GetInt32(0);
                    detalle.NOM_PRO = dr.GetString(1);
                    detalle.UME_PRO = dr.GetString(2);
                    detalle.CAN_PRO = dr.GetInt32(3);
                    detalle.NOM_ALM = dr.GetString(4);
                    detalle.UBI_ALM = dr.GetString(5);
                    detalle.TIP_DET = dr.GetString(6);
                };
                lista.Add(detalle);
            };
            cnx.Close();
            dr.Close();
            return lista;
        }

        private IEnumerable<Producto> listarProductos()
        {
            List<Producto> lista = new List<Producto>();
            SqlCommand cmd = new SqlCommand("SP_ListarProductos", cnx);
            cmd.CommandType = CommandType.StoredProcedure;
            cnx.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Producto producto = new Producto();
                {
                    producto.COD_PRO = dr.GetInt32(0);
                    producto.NOM_PRO = dr.GetString(1);
                    producto.UME_PRO = dr.GetString(2);
                };
                lista.Add(producto);
            };
            cnx.Close();
            dr.Close();
            return lista;
        }

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

        IEnumerable<string> listarTipos()
        {
            List<string> tipos = new List<string>();
            tipos.Add("Ingreso");
            tipos.Add("Salida");
            return tipos;
        }

        private string AgregarDetalle(Detalle detalle)
        {
            string mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("SP_AgregarDetalle", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codigo", detalle.IDE_DET);
                cmd.Parameters.AddWithValue("@almacen", detalle.COD_ALM);
                cmd.Parameters.AddWithValue("@producto", detalle.COD_PRO);
                cmd.Parameters.AddWithValue("@tipo", detalle.TIP_DET);
                cmd.Parameters.AddWithValue("@cantidad", detalle.CAN_PRO);
                cnx.Open();
                int value = cmd.ExecuteNonQuery();
                mensaje = $"Se Agrego {value} detalle/s";
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                if (mensaje.Contains("CK_Cantidad_PRO"))
                {
                    mensaje = "Error >> La cantidad no puede ser menor o igual a 0";
                }
            }
            finally
            {
                cnx.Close();
            }
            return mensaje;
        }

        private string editarDetalle(Detalle detalle)
        {
            string mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("SP_ActualizarDetalle", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", detalle.IDE_DET);
                cmd.Parameters.AddWithValue("@almacen", detalle.COD_ALM);
                cmd.Parameters.AddWithValue("@producto", detalle.COD_PRO);
                cmd.Parameters.AddWithValue("@cantidad", detalle.CAN_PRO);
                cmd.Parameters.AddWithValue("@tipo", detalle.TIP_DET);
                cnx.Open();
                int value = cmd.ExecuteNonQuery();
                mensaje = $"Se Actualizaron los datos de {value} detalle/s";
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                if (mensaje.Contains("CK_Cantidad_PRO"))
                {
                    mensaje = "Error >> La cantidad no puede ser menor o igual a 0";
                }
            }
            finally
            {
                cnx.Close();
            }
            return mensaje;
        }

        private string eliminarDetalle(int id)
        {
            string mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("SP_EliminarDetalle", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                cnx.Open();
                int value = cmd.ExecuteNonQuery();
                mensaje = $"Se Eliminaron {value} detalle/s";
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

        private Detalle obtenerDetalle(int codigo)
        {
            Detalle detalle = null;
            SqlCommand cmd = new SqlCommand("SP_BuscarDetalle", cnx);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codigo", codigo);
            cnx.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                detalle = new Detalle();
                detalle.IDE_DET = dr.GetInt32(0);
                detalle.COD_ALM = dr.GetInt32(1);
                detalle.COD_PRO = dr.GetInt32(2);
                detalle.CAN_PRO = dr.GetInt32(3);
                detalle.TIP_DET = dr.GetString(4);
            };
            cnx.Close();
            dr.Close();
            return detalle;
        }

        private Detalle obtenerDetalleCompleto(int codigo)
        {
            Detalle detalle = null;
            SqlCommand cmd = new SqlCommand("SP_ListadoGeneralxCodigo", cnx);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codigo", codigo);
            cnx.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                detalle = new Detalle();
                detalle.IDE_DET = dr.GetInt32(0);
                detalle.COD_ALM = dr.GetInt32(1);
                detalle.COD_PRO = dr.GetInt32(2);
                detalle.NOM_PRO = dr.GetString(3);
                detalle.UME_PRO = dr.GetString(4);
                detalle.CAN_PRO = dr.GetInt32(5);
                detalle.NOM_ALM = dr.GetString(6);
                detalle.UBI_ALM = dr.GetString(7);
                detalle.TIP_DET = dr.GetString(8);
            };
            cnx.Close();
            dr.Close();
            return detalle;
        }

        private int GenerarCodigo()
        {
            SqlCommand cmd = new SqlCommand("SP_GenerarCodigoDetalle", cnx);
            cmd.CommandType = CommandType.StoredProcedure;
            cnx.Open();
            int codigo = Convert.ToInt32(cmd.ExecuteScalar());
            cnx.Close();
            return codigo;
        }

        private Detalle Buscar(int id)
        {
            Detalle reg;
            reg = obtenerDetalle(id);
            return reg;
        }

        private string obtenerCantidad(Detalle detalle)
        {
            string mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("SP_ObtenerCantidadActual", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@producto", detalle.COD_PRO);
                cmd.Parameters.AddWithValue("@almacen", detalle.COD_ALM);
                cnx.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    mensaje = "La cantidad actual del producto es : " + rd.GetInt32(0).ToString();
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                if(mensaje.Contains("Data is Null"))
                {
                    mensaje = "Error >> El producto seleccionado no se encuentra almacenado";
                }
            }
            finally
            {
                cnx.Close();
            }
            return mensaje;
        }

        #endregion

        public ActionResult Index(string mensaje)
        {
            if (!mensaje.IsEmpty() && mensaje.Contains("Error"))
            {
                ViewBag.error = mensaje;
                return View(listarDetalles());
            }
            else
            {
                ViewBag.mensaje = mensaje;
            }
            return View(listarDetalles());
        }

        public ActionResult Create()
        {
            ViewBag.Productos = new SelectList(listarProductos(), "COD_PRO", "NOM_PRO");
            ViewBag.Almacenes = new SelectList(listarAlmacenes(), "COD_ALM", "NOM_ALM");
            ViewBag.Tipos = new SelectList(listarTipos());
            Detalle detalle = new Detalle();
            {
                detalle.IDE_DET = GenerarCodigo();
                detalle.CAN_PRO = 1;
            };
            return View(detalle);
        }

        [HttpPost]
        public ActionResult Create(Detalle detalle)
        {
            string mensaje = string.Empty;
            if (ModelState.IsValid)
            {
                mensaje = AgregarDetalle(detalle);
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
            ViewBag.Productos = new SelectList(listarProductos(), "COD_PRO", "NOM_PRO");
            ViewBag.Almacenes = new SelectList(listarAlmacenes(), "COD_ALM", "NOM_ALM");
            ViewBag.Tipos = new SelectList(listarTipos());
            Detalle detalle = Buscar(id);
            return View(detalle);
        }

        [HttpPost]
        public ActionResult Edit(Detalle detalle)
        {
            string mensaje = string.Empty;
            if (ModelState.IsValid)
            {
                mensaje = editarDetalle(detalle);
                return RedirectToAction("Index", new { mensaje = mensaje });
            }
            else
            {
                mensaje = "Error";
                return RedirectToAction("Edit", new { error = mensaje });
            }
        }

        public ActionResult Delete(int id)
        {
            string mensaje = eliminarDetalle(id);
            return RedirectToAction("Index", new { mensaje = mensaje });
        }

        public ActionResult Details(Detalle detalle)
        {
            return View(detalle);
        }

        [HttpPost]
        public ActionResult BuscarXCodigo(string ID)
        {
            int codigo = -1;
            try
            {
                codigo = Convert.ToInt32(ID);
            } catch (Exception ex)
            {
                string mensaje = "Error >> El codigo debe ser un numero entero";
                return RedirectToAction("Index", new { mensaje = mensaje });
            }
            Detalle detalle = obtenerDetalleCompleto(codigo);
            if (detalle == null)
            {
                string mensaje = "Error >> Detalle no encontrado";
                return RedirectToAction("Index", new { mensaje = mensaje });
            }
            return View("Details", detalle);
        }

        public ActionResult Consultas(string mensaje)
        {
            ViewBag.Productos = new SelectList(listarProductos(), "COD_PRO", "NOM_PRO");
            ViewBag.Almacenes = new SelectList(listarAlmacenes(), "COD_ALM", "NOM_ALM");
            if (!mensaje.IsEmpty() && mensaje.Contains("Error"))
            {
                ViewBag.error = mensaje;
                return View();
            }
            ViewBag.mensaje = mensaje;
            return View();
        }

        [HttpPost]
        public ActionResult ActionObtenerCantidad(Detalle detalle)
        {
            string mensaje = string.Empty;
            mensaje = obtenerCantidad(detalle);
            if (!mensaje.IsEmpty())
            {
                ViewBag.mensaje = mensaje;
            }
            return RedirectToAction("Consultas", new { mensaje = mensaje});
        }
    }
}