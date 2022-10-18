using Al_Web_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Al_Web_2.Controllers
{
    [Authorize(Roles = "A")]
    public class KanalEkleController : Controller
    {
        // GET: KanalEkle
        Al_WebEntities db = new Al_WebEntities();
        public ActionResult Index()
        {
            var list = db.KanalEkles.Include("SirketEkle").Where(x => x.Silindi == false).ToList();

            
            return View(list);
        }
        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            if (formCollection.Count != 0)
            {
                string[] ids = formCollection["kanalId"].Split(new char[] { ',' });
                foreach (string id in ids)
                {
                    var kanal = db.KanalEkles.Find(int.Parse(id));

                   
                    kanal.Silindi = true;
                    kanal.Kullanicilar.Clear();
                    

                    //db.KanalEkles.Remove(kanal);
                }
                db.SaveChanges();
            }
            var list = db.KanalEkles.Where(x => x.Silindi == false).ToList();

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

            List<SelectListItem> deger2 = (from x in db.SirketEkle.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.SirketIsim,
                                               Value = x.Id.ToString()
                                           }).ToList();
            ViewBag.frm = deger2;

            return View();

        }


        [HttpPost]
        public ActionResult Ekle(KanalEkle Data, List<string> Kullanicilar, FormCollection frm)
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
            if (frm["SirketEkle"] != null)
            {
                //Data.SirketEkleId = Convert.ToInt16(frm["FirmaEkle"].ToString());

                int firmaId = Convert.ToInt16(frm["SirketEkle"].ToString());
                var sirket = db.SirketEkle.Where(x => x.Id == firmaId).FirstOrDefault();

                Data.SirketEkle = sirket;

            }

            Data.Silindi = false;
            db.KanalEkles.Add(Data);
            db.SaveChanges();

            return RedirectToAction("Index");

        }

        public ActionResult Sil(int id)
        {
            var kanal = db.KanalEkles.Where(x => x.Id == id).FirstOrDefault();
            kanal.Silindi = true;
            kanal.Kullanicilar.Clear();

            //db.KanalEkles.Remove(kanal);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Guncelle(int id)
        {
            var kanal = db.KanalEkles.Include("SirketEkle").Where(x => x.Id == id).FirstOrDefault();


            List<SelectListItem> deger1 = (from x in db.Kullanicilars.Where(x => x.Rol != "A").ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.Ad + " " + x.Soyad,
                                               Value = x.Id.ToString(),
                                               //Selected = int.Equals(x.Id, id)

                                           }).ToList();


            foreach (var kullanici in kanal.Kullanicilar)
            {
                deger1.Where(x => x.Value == kullanici.Id.ToString()).FirstOrDefault().Selected = true;
            }
            ViewBag.kllnc = deger1;

            List<SelectListItem> deger2 = (from x in db.SirketEkle.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.SirketIsim,
                                               Value = x.Id.ToString()
                                           }).ToList();

            ViewBag.frm = deger2;

            return View(kanal);
        }

        [HttpPost]
        public ActionResult Guncelle(KanalEkle model, List<string> Kullanicilar, FormCollection frm)
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

                    //try
                    //{
                    //    var kullaniciId = int.Parse(item);
                    //    var kullanici = db.Kullanicilars.Where(x => x.Id == kullaniciId).FirstOrDefault();
                    //    model.Kullanicilar.Add(kullanici);


                    //}
                    //catch (System.Exception)
                    //{

                    //    continue;
                    //}

                }
            }

            if (frm["SirketEkle"] != null)
            {
                //Data.SirketEkleId = Convert.ToInt16(frm["FirmaEkle"].ToString());

                int firmaId = Convert.ToInt16(frm["SirketEkle"].ToString());
                var sirket = db.SirketEkle.Where(x => x.Id == firmaId).FirstOrDefault();

                model.SirketEkle = sirket;

            }


            var kanal = db.KanalEkles.Find(model.Id);
            kanal.Name = model.Name;
            kanal.Type = model.Type;
            kanal.SirketEkle = model.SirketEkle;
            kanal.Kullanicilar.Clear();
            kanal.Kullanicilar = model.Kullanicilar;


            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}


