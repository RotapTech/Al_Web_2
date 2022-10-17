using Al_Web_2.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Al_Web_2.Controllers
{
    [Authorize(Roles = "A")]
    public class CihazEkleController : Controller
    {
        // GET: CihazEkle
        Al_WebEntities db = new Al_WebEntities();

        public ActionResult Index()
        {
            var list = db.CihazEkle.Where(x => x.Silindi == false).ToList();
            return View(list);
        }
        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            if (formCollection.Count != 0)
            {
                string[] ids = formCollection["cihazId"].Split(new char[] { ',' });
                foreach (string id in ids)
                {
                    var cihaz = db.CihazEkle.Find(int.Parse(id));

                    cihaz.Silindi = true;
                   cihaz.Kullanicilar.Clear();

                    //db.CihazEkle.Remove(cihaz);
                }
                db.SaveChanges();
            }
            var list = db.CihazEkle.Where(x => x.Silindi == false).ToList();

            return View(list);
        }
        public ActionResult Ekle()
        {
            List<SelectListItem> deger1 = (from x in db.Kullanicilars.Where(x => x.Rol != "A").ToList()
                                           select new SelectListItem
                                           {

                                               Text = x.Ad + " " + x.Soyad,
                                               Value = x.Id.ToString()
                                           }).ToList();

            ViewBag.kllnc = deger1;
            return View();
        }

        [HttpPost]
        public ActionResult Ekle(CihazEkle Data, List<string> Kullanicilar)
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
            db.CihazEkle.Add(Data);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public ActionResult Sil(int id)
        {
            var cihaz = db.CihazEkle.Where(x => x.Id == id).FirstOrDefault();
            cihaz.Kullanicilar.Clear();

            cihaz.Silindi = true;
            //db.CihazEkle.Remove(cihaz);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Guncelle(int id)
        {
            var cihaz = db.CihazEkle.Where(x => x.Id == id).FirstOrDefault();

            List<SelectListItem> deger1 = (from x in db.Kullanicilars.Where(x => x.Rol != "A").ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.Ad + " " + x.Soyad,
                                               Value = x.Id.ToString(),
                                           }).ToList();


            foreach (var kullanici in cihaz.Kullanicilar)
            {
                deger1.Where(x => x.Value == kullanici.Id.ToString()).FirstOrDefault().Selected = true;
            }
            ViewBag.kllnc = deger1;
            return View(cihaz);
        }
        [HttpPost]
        public ActionResult Guncelle(CihazEkle model, List<string> Kullanicilar)
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
                    //var kullaniciId = int.Parse(item);
                    //var kullanici = db.Kullanicilars.Where(x => x.Id == kullaniciId).FirstOrDefault();
                    //model.Kullanicilar.Add(kullanici);
                }
            }
            var cihaz = db.CihazEkle.Find(model.Id);
            cihaz.CihazAd = model.CihazAd;
            cihaz.CihazSeriNo = model.CihazSeriNo;
            cihaz.SimCard = model.SimCard;
            cihaz.Kullanicilar.Clear();
            cihaz.Kullanicilar = model.Kullanicilar;

            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}