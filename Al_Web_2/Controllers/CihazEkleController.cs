using Al_Web_2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Al_Web_2.Controllers
{
    [Authorize(Roles = "A")]
    public class CihazEkleController : Controller
    {
        // GET: CihazEkle
        Al_WebEntities1 db = new Al_WebEntities1();

        public ActionResult Index()
        {
            var list = db.CihazEkle.Where(x => x.Silindi == false).ToList();

            foreach (var item in list)
            {
                foreach (var items in item.SirketEkles.ToList())
                {
                    if (items.Silindi == true)
                    {
                        item.SirketEkles.Remove(items);
                    }
                }
            }


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
                    cihaz.Kullanicilars.Clear();


                    //db.CihazEkle.Remove(cihaz);
                }
                db.SaveChanges();
            }
            var list = db.CihazEkle.Where(x => x.Silindi == false).ToList();

            return View(list);
        }
        public ActionResult Ekle()
        {
            List<SelectListItem> deger1 = (from x in db.Kullanicilar.Where(x => x.Rol != "A" && x.Silindi == false).ToList()
                                           select new SelectListItem
                                           {

                                               Text = x.Ad + " " + x.Soyad,
                                               Value = x.Id.ToString()
                                           }).ToList();

            ViewBag.kllnc = deger1;

            List<SelectListItem> deger2 = (from x in db.SirketEkle.ToList() where x.Silindi == false
                                           select new SelectListItem
                                           {
                                               Text = x.SirketIsim,
                                               Value = x.Id.ToString()
                                           }).ToList();
            ViewBag.Error = TempData["Error"];
            ViewBag.frm = deger2;
            return View();
        }

        [HttpPost]
        public ActionResult Ekle(CihazEkle Data, List<string> Kullanicilar, List<string> SirketEkle)
        {
            if (Kullanicilar != null)
            {
                foreach (var item in Kullanicilar)
                {
                    int kullaniciId;

                    bool success = int.TryParse(item, out kullaniciId);

                    if (success)
                    {
                        var kullanici = db.Kullanicilar.Where(x => x.Id == kullaniciId).FirstOrDefault();
                        Data.Kullanicilars.Add(kullanici);
                    }
                    else
                    {
                        continue;
                    }
                }

            }
            if (SirketEkle != null)
            {
                //Data.SirketEkleId = Convert.ToInt16(frm["FirmaEkle"].ToString());
                foreach (var item in SirketEkle)
                {

                    int firmaId;

                    bool success = int.TryParse(item, out firmaId);

                    if (success)
                    {
                        var sirket = db.SirketEkle.Where(x => x.Id == firmaId && x.Silindi == false).FirstOrDefault();
                        Data.SirketEkles.Add(sirket);
                    }
                    else
                    {
                        continue;
                    }

                }

            }
            Data.Silindi = false;
            db.CihazEkle.Add(Data);
            try
            {
                db.SaveChanges();

            }
            catch (Exception)
            {
                TempData["Error"] = "Aynı isim var!";

                return RedirectToAction("Ekle");
            }



            return RedirectToAction("Index");
        }
        public ActionResult Sil(int id)
        {
            var cihaz = db.CihazEkle.Where(x => x.Id == id).FirstOrDefault();
            cihaz.Kullanicilars.Clear();

            cihaz.Silindi = true;
            //db.CihazEkle.Remove(cihaz);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Guncelle(int id)
        {
            var cihaz = db.CihazEkle.Where(x => x.Id == id).FirstOrDefault();

            List<SelectListItem> Kullanicilar = (from x in db.Kullanicilar.Where(x => x.Rol != "A" && x.Silindi == false).ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.Ad + " " + x.Soyad,
                                               Value = x.Id.ToString(),
                                           }).ToList();


            foreach (var kullanici in cihaz.Kullanicilars)
            {
                Kullanicilar.Where(x => x.Value == kullanici.Id.ToString()).FirstOrDefault().Selected = true;
            }
            ViewBag.kllnc = Kullanicilar;

            List<SelectListItem> deger2 = (from x in db.SirketEkle.ToList() where x.Silindi == false
                                           select new SelectListItem
                                           {
                                               Text = x.SirketIsim,
                                               Value = x.Id.ToString()
                                           }).ToList();
            
                foreach (var firma in cihaz.SirketEkles)
                {
                if (firma.Silindi == false)
                {
                    deger2.Where(x => x.Value == firma.Id.ToString()).FirstOrDefault().Selected = true;

                }
            }
            

            ViewBag.frm = deger2;
            return View(cihaz);
        }
        [HttpPost]
        public ActionResult Guncelle(CihazEkle model, List<string> Kullanicilar, List<string> SirketEkle)
        {
            var cihaz = db.CihazEkle.Find(model.Id);

            if (Kullanicilar != null)
            {
                foreach (var item in Kullanicilar)
                {
                    int kullaniciId;

                    bool success = int.TryParse(item, out kullaniciId);

                    if (success)
                    {
                        var kullanici = db.Kullanicilar.Where(x => x.Id == kullaniciId).FirstOrDefault();
                        model.Kullanicilars.Add(kullanici);
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
            if (SirketEkle != null)
            {
                cihaz.SirketEkles.Clear();
                //Data.SirketEkleId = Convert.ToInt16(frm["FirmaEkle"].ToString());
                foreach (var item in SirketEkle)
                {

                    int firmaId;

                    bool success = int.TryParse(item, out firmaId);

                    if (success)
                    {
                        var sirket = db.SirketEkle.Where(x => x.Id == firmaId).FirstOrDefault();
                        cihaz.SirketEkles.Add(sirket);
                    }
                    else
                    {
                        continue;
                    }

                }

            }
            else
            {
                cihaz.SirketEkles.Clear();
            }

            cihaz.CihazAd = model.CihazAd;
            cihaz.CihazSeriNo = model.CihazSeriNo;
            cihaz.SimCard = model.SimCard;
            cihaz.Kullanicilars.Clear();
            cihaz.Kullanicilars = model.Kullanicilars;

            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}