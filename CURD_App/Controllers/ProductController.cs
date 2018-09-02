using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CURD_App.Models;

namespace CURD_App.Controllers
{
    public class ProductController : Controller
    {
        DataAccessLayer ProductDataAccessLayer = new DataAccessLayer();

        // GET: Product
        public ActionResult Index()
        {
            List<Product> listofProducts = new List<Product>();
            listofProducts = ProductDataAccessLayer.GetProducts().ToList();
            return View(listofProducts);
        }
    }
}