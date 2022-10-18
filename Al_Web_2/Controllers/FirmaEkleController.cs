using Al_Web_2.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Al_Web_2.Controllers
{
    [Authorize(Roles = "A")]

    public class FirmaEkleController : Controller
    {
        // GET: FirmaEkle
        Al_WebEntities db = new Al_WebEntities();

        public ActionResult Index()
        {
            var list = db.SirketEkle.Where(x => x.Silindi == false).ToList();
            return View(list);
        }
        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            if (formCollection.Count != 0)
            {
                string[] ids = formCollection["firmaId"].Split(new char[] { ',' });
                foreach (string id in ids)
                {
                    var firma = db.SirketEkle.Find(int.Parse(id));

                    firma.Silindi = true;
                    firma.Kullanicilar.Clear();

                    //db.SirketEkle.Remove(firma);
                }
                db.SaveChanges();
            }
            var list = db.SirketEkle.Where(x => x.Silindi == false).ToList();

            return View(list);
        }

        public ActionResult Ekle()
        {
            List<SelectListItem> firma = (from x in db.Kullanicilars.Where(x => x.Rol != "A").ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.Ad + " " + x.Soyad,
                                               Value = x.Id.ToString()
                                           }).ToList();
            ViewBag.kllnc= firma;
            return View();
        }

        [HttpPost]
        public ActionResult Ekle(SirketEkle Data, List<string> Kullanicilar)
        {

            if (Kullanicilar != null)
            {
                foreach (var item in Kullanicilar)
                {

                    int kullaniciId;

                    bool success = int.TryParse(item, out kullaniciId);

                    if (success)
                    {
                        var kullanici = db.Kullanicilars.Where(x => x.Id == kullaniciId).FirstOrDefault();
                        Data.Kullanicilar.Add(kullanici);
                    }
                    else
                    {
                        continue;
                    }

                }
            }

            Data.Silindi = false;
            db.SirketEkle.Add(Data);
            db.SaveChanges();

            return RedirectToAction("Index");

        }

        public ActionResult Sil(int id)
        {
            var sirket = db.SirketEkle.Where(x => x.Id == id).FirstOrDefault();

            sirket.Silindi = true;
            sirket.Kullanicilar.Clear();

           
            //db.SirketEkle.Remove(sirket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Guncelle(int id)
        {
            var sirket = db.SirketEkle.Include("Kullanicilar").Where(x => x.Id == id).FirstOrDefault();


            List<SelectListItem> firma = (from x in db.Kullanicilars.Where(x => x.Rol != "A").ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.Ad + " " + x.Soyad,
                                               Value = x.Id.ToString(),
                                               //Selected = int.Equals(x.Id, id)

                                           }).ToList();


            foreach (var kullanici in sirket.Kullanicilar)
            {
                firma.Where(x => x.Value == kullanici.Id.ToString()).FirstOrDefault().Selected = true;
            }
            ViewBag.kllnc = firma;
            return View(sirket);
        }

        [HttpPost]
        public ActionResult Guncelle(SirketEkle model, List<string> Kullanicilar)
        {
            if (Kullanicilar != null)
            {
                foreach (var item in Kullanicilar)
                {
                    int kullaniciId;

                    bool success = int.TryParse(item, out kullaniciId);

                    if (success)
                    {
                        var kullanici = db.Kullanicilars.Where(x => x.Id == kullaniciId).FirstOrDefault();
                        model.Kullanicilar.Add(kullanici);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            var firma = db.SirketEkle.Find(model.Id);
            firma.SirketIsim = model.SirketIsim;
          


            firma.Kullanicilar.Clear();
            firma.Kullanicilar = model.Kullanicilar;


            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}


