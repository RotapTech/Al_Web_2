using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Al_Web_2.Controllers
{
    [Authorize]
    public class IslemlerController : Controller
    {
        // GET: Islemler
        public ActionResult CozumOrtagiEkle()
        {
            return View();
        }

        public ActionResult CihazEkle()
        {
           
            return View();
        }
        public ActionResult KurumList()
        {
            return View();
        }
        public ActionResult UserList()
        {
            return View();
        }
    }
}