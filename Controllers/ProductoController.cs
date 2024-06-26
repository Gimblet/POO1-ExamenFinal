using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

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
            } catch (Exception ex)
            {
                mensaje = ex.Message;
            }
            finally
            {
                cnx.Close();
            }
            return mensaje;
        }

        private Producto obtenerProducto(int codigo)
        {
            Producto producto = null;
            SqlCommand cmd = new SqlCommand("SP_BuscarProducto", cnx);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codigo", codigo);
            cnx.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                producto.COD_PRO = dr.GetInt32(0);
                producto.NOM_PRO = dr.GetString(1);
                producto.UME_PRO = dr.GetString(2);
            };
            cnx.Close();
            dr.Close();
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

        public ActionResult Index()
        {
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
            if (ModelState.IsValid)
            {
                ViewBag.mensaje = AgregarProducto(producto);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.mensaje = "Error";
                return View("Create");
            }
        }
    }
}