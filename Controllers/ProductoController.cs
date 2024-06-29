using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.WebPages;

namespace WebApplication1.Controllers
{
    public class ProductoController : Controller
    {
        private static string Cadena = ConfigurationManager.ConnectionStrings["Cadena"].ConnectionString;
        SqlConnection cnx = new SqlConnection(Cadena);

        #region Metodos

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

        private string AgregarProducto(Producto producto)
        {
            string mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("SP_AgregarProducto", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codigo", producto.COD_PRO);
                cmd.Parameters.AddWithValue("@nombre", producto.NOM_PRO);
                cmd.Parameters.AddWithValue("@unidad", producto.UME_PRO);
                cnx.Open();
                int value = cmd.ExecuteNonQuery();
                mensaje = $"Se Agrego {value} producto/s";
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

        private string ActualizarProducto(Producto producto)
        {
            string mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("SP_ActualizarProducto", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codigo", producto.COD_PRO);
                cmd.Parameters.AddWithValue("@nombre", producto.NOM_PRO);
                cmd.Parameters.AddWithValue("@unidad", producto.UME_PRO);
                cnx.Open();
                int value = cmd.ExecuteNonQuery();
                mensaje = $"Se modificaron los atributos de {value} producto/s";
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

        private string EliminarProducto(int codigo)
        {
            string mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("SP_EliminarProducto", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codigo", codigo);
                cnx.Open();
                int value = cmd.ExecuteNonQuery();
                mensaje = $"Se eliminaron {value} producto/s";
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                if (mensaje.Contains("FK_TO_PRODUCTO"))
                {
                    mensaje = "Error >> No se puede eliminar el producto porque actualmente se encuentra almacenado";
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

        private Producto obtenerProducto(int ID)
        {
            Producto producto = null;
            try
            {
                SqlCommand cmd = new SqlCommand("SP_BuscarProducto", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codigo", ID);
                cnx.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    producto = new Producto();
                    producto.COD_PRO = dr.GetInt32(0);
                    producto.NOM_PRO = dr.GetString(1);
                    producto.UME_PRO = dr.GetString(2);
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
            return producto;
        }

        int GenerarCodigo()
        {
            SqlCommand cmd = new SqlCommand("SP_GenerarCodigoProducto", cnx);
            cmd.CommandType = CommandType.StoredProcedure;
            cnx.Open();
            int codigo = Convert.ToInt32(cmd.ExecuteScalar());
            cnx.Close();
            return codigo;
        }

        private Producto Buscar(int id)
        {
            Producto reg;
            reg = obtenerProducto(id);
            return reg;
        }

        #endregion

        public ActionResult Index(string mensaje)
        {
            if(!mensaje.IsEmpty() && mensaje.Contains("Error"))
            {
                ViewBag.error = mensaje;
                return View(listarProductos());
            }
            else
            {
                ViewBag.mensaje = mensaje;
            }
            return View(listarProductos());
        }

        public ActionResult Create()
        {
            Producto producto = new Producto();
            {
                producto.COD_PRO = GenerarCodigo();
            };
            return View(producto);
        }

        [HttpPost]
        public ActionResult Create(Producto producto)
        {
            string mensaje = string.Empty;
            if (ModelState.IsValid)
            {
                mensaje = AgregarProducto(producto);
                return RedirectToAction("Index", new { mensaje = mensaje});
            }
            else
            {
                mensaje = "Error";
                return RedirectToAction("Create", new { error = mensaje});
            }
        }

        public ActionResult Edit(int id)
        {
            return View(Buscar(id));
        }

        [HttpPost]
        public ActionResult Edit(Producto producto)
        {
            string mensaje = string.Empty;
            if (ModelState.IsValid)
            {
                mensaje = ActualizarProducto(producto);
                return RedirectToAction("Index", new { mensaje = mensaje});
            }
            else
            {
                mensaje = "Error";
                return RedirectToAction("Edit", new { mensaje = mensaje});
            }
        }

        public ActionResult Delete(int id)
        {
            string mensaje = EliminarProducto(id);
            return RedirectToAction("Index", new { mensaje = mensaje});
        }

        public ActionResult Details(Producto producto)
        {
            return View(producto);
        }

        [HttpPost]
        public ActionResult BuscarXCodigo(string ID)
        {
            int codigo = -1;
            try
            {
                codigo = Convert.ToInt32(ID);
            }
            catch (Exception ex)
            {
                string mensaje = "Error >> El codigo debe ser un número entero";
                return RedirectToAction("Index", new { mensaje = mensaje });
            }
            Producto producto = obtenerProducto(codigo);
            if (producto == null)
            {
                string mensaje = "Error >> Producto no encontrado";
                return RedirectToAction("Index", new { mensaje = mensaje });
            }
            return View("Details", producto);
        }
    }
}