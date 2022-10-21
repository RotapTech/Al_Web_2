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
        Al_WebEntities1 db = new Al_WebEntities1();
        public ActionResult Index()
        {
            var list = db.KanalEkles.Where(x => x.Silindi == false).ToList();

            //foreach (var item in list)
            //{
            //    foreach (var items in item.SirketEkles.ToList())
            //    {
            //        if (items.Silindi == true)
            //        {
            //            item.SirketEkles.Remove(items);
            //        }
            //    }
            //}

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
                    kanal.Kullanicilars.Clear();


                    //db.KanalEkles.Remove(kanal);
                }
                db.SaveChanges();
            }
            var list = db.KanalEkles.Where(x => x.Silindi == false).ToList();

            return View(list);
        }

        public ActionResult Ekle()
        {
            List<SelectListItem> deger1 = (from x in db.Kullanicilars.Where(x => x.Rol != "A" && x.Silindi == false).ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.Ad + " " + x.Soyad,
                                               Value = x.Id.ToString()
                                           }).ToList();
            ViewBag.kllnc = deger1;


            var deger2 = (from x in db.SirketEkles
                          where x.Silindi == false && !(db.KanalEkles.Any(z => z.SirketEkleId == x.Id ))
                          select x).AsEnumerable().Select(x => new SelectListItem
                          {
                              Text = x.SirketIsim,
                              Value = x.Id.ToString()
                          });
            var deger2list = deger2.ToList();

            ViewBag.Error = TempData["Error"];

            deger2list.Insert(0, new SelectListItem
            {
                Text = "Boş",
                Value = "0"
            });
            ViewBag.frm = deger2list;

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
                        Data.Kullanicilars.Add(kullanici);
                    }
                    else
                    {
                        continue;
                    }

                }
            }

            if (frm["SirketEkle"] != null)
            {
                if (Convert.ToInt32(frm["SirketEkle"]) != 0)
                {
                    Data.SirketEkleId = Convert.ToInt16(frm["SirketEkle"].ToString());

                }
                //foreach (var item in SirketEkle)
                //{

                //    int firmaId;

                //    bool success = int.TryParse(item, out firmaId);

                //    if (success)
                //    {
                //        var sirket = db.SirketEkle.Where(x => x.Id == firmaId && x.Silindi == false).FirstOrDefault();
                //        Data.SirketEkles.Add(sirket);
                //    }
                //    else
                //    {
                //        continue;
                //    }

                //}

            }

            Data.Silindi = false;
            db.KanalEkles.Add(Data);
            try
            {
                db.SaveChanges();

            }
            catch (Exception ex)
            {

                TempData["Error"] = "Aynı isim var!";

                return RedirectToAction("Ekle");
            }

            return RedirectToAction("Index");

        }

        public ActionResult Sil(int id)
        {
            var kanal = db.KanalEkles.Where(x => x.Id == id).FirstOrDefault();
            kanal.Silindi = true;
            kanal.Kullanicilars.Clear();
          

            //db.KanalEkles.Remove(kanal);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Guncelle(int id)
        {
            var kanal = db.KanalEkles.Where(x => x.Id == id).FirstOrDefault();


            List<SelectListItem> deger1 = (from x in db.Kullanicilars.Where(x => x.Rol != "A").ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.Ad + " " + x.Soyad,
                                               Value = x.Id.ToString(),
                                               //Selected = int.Equals(x.Id, id)

                                           }).ToList();


            foreach (var kullanici in kanal.Kullanicilars)
            {
                deger1.Where(x => x.Value == kullanici.Id.ToString()).FirstOrDefault().Selected = true;
            }
            ViewBag.kllnc = deger1;
            var deger2 = (from x in db.SirketEkles
                          where x.Silindi == false && !(db.KanalEkles.Any(z => z.SirketEkleId == x.Id && z.SirketEkleId != kanal.SirketEkleId)) 
                          select x).AsEnumerable().Select(x => new SelectListItem
                          {
                              Text = x.SirketIsim,
                              Value = x.Id.ToString()
                          });
            var deger2list = deger2.ToList();
            if (kanal.SirketEkleId != null)
                deger2list.Where(x => x.Value == kanal.SirketEkleId.ToString()).FirstOrDefault().Selected = true;

            deger2list.Insert(0, new SelectListItem
            {
                Text = "Boş",
                Value = "0"
            });


            ViewBag.frm = deger2list;

            return View(kanal);
        }

        [HttpPost]
        public ActionResult Guncelle(KanalEkle model, List<string> Kullanicilar, FormCollection frm)
        {
            var kanal = db.KanalEkles.Find(model.Id);

            if (Kullanicilar != null)
            {
                foreach (var item in Kullanicilar)
                {
                    int kullaniciId;

                    bool success = int.TryParse(item, out kullaniciId);

                    if (success)
                    {
                        var kullanici = db.Kullanicilars.Where(x => x.Id == kullaniciId).FirstOrDefault();
                        model.Kullanicilars.Add(kullanici);
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
            if (frm["FirmaEkle"] != null)
            {
                if (Convert.ToInt32(frm["FirmaEkle"]) != 0)
                {
                    kanal.SirketEkleId = Convert.ToInt16(frm["Sirket"].ToString());


                }
                //cihaz.SirketEkles.Clear();
                //foreach (var item in SirketEkle)
                //{

                //    int firmaId;

                //    bool success = int.TryParse(item, out firmaId);

                //    if (success)
                //    {
                //        var sirket = db.SirketEkle.Where(x => x.Id == firmaId).FirstOrDefault();
                //        cihaz.SirketEkles.Add(sirket);
                //    }
                //    else
                //    {
                //        continue;
                //    }

                //}

            }
            else
            {
                kanal.SirketEkles.Clear();
            }
          
            //BOŞ GÖNDERİNCE GÜNCELLEMİYOR!!!!!!!!!!!!!!!!!!!!!!!!
            kanal.Name = model.Name;
            kanal.Type = model.Type;

            kanal.Kullanicilars.Clear();
            kanal.Kullanicilars = model.Kullanicilars;


            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}


