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
        Al_WebEntities1 db = new Al_WebEntities1();

        public ActionResult Index()
        {
            var list = db.SirketEkles.Where(x => x.Silindi == false).ToList();

            foreach (var item in list)
            {
                foreach (var items in item.Kullanicilars.ToList())
                {
                    if (items.Silindi == true)
                    {
                        item.Kullanicilars.Remove(items);
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
                string[] ids = formCollection["firmaId"].Split(new char[] { ',' });
                foreach (string id in ids)
                {
                    var firma = db.SirketEkles.Find(int.Parse(id));

                    firma.Silindi = true;
                    firma.Kullanicilars.Clear();

                    //db.SirketEkle.Remove(firma);
                }
                db.SaveChanges();
            }
            var list = db.SirketEkles.Where(x => x.Silindi == false).ToList();

            return View(list);
        }

        public ActionResult Ekle()
        {
            var kllnc = (from x in db.Kullanicilars
                          where x.Silindi == false && x.Rol != "A" && !(db.SirketEkles.Any(z => z.Id == x.SirketEkleId))
                          select x).AsEnumerable().Select(x => new SelectListItem
                          {
                              Text = x.Ad + " " + x.Soyad,
                              Value = x.Id.ToString()
                          });
            var kllnclist = kllnc.ToList();
            ViewBag.Error = TempData["Error"];
            ViewBag.kllnc = kllnclist;

            var deger2 = (from x in db.KanalEkles
                          where x.Silindi == false && !(db.SirketEkles.Any(z => z.Id == x.SirketEkleId ))
                          select x).AsEnumerable().Select(x => new SelectListItem
                          {
                              Text = x.Name + " " + x.Type,
                              Value = x.Id.ToString()
                          });
            var deger2list = deger2.ToList();
            ViewBag.knl = deger2list;

            List<SelectListItem> deger1 = (from x in db.CihazEkles.ToList()
                                           where x.Silindi == false && ( x.SirketEkleId == null || x.SirketEkleId.ToString() =="")
                                           select new SelectListItem
                                           {
                                               Text = x.CihazAd + x.CihazSeriNo + x.SimCard,
                                               Value = x.Id.ToString()
                                           }).ToList();
            ViewBag.chz = deger1;



            return View();
        }

        [HttpPost]
        public ActionResult Ekle(SirketEkle Data, List<string> Kullanicilar, List<string> KanalEkle, List<string> CihazEkle)
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
            if (KanalEkle != null)
            {
                foreach (var item in KanalEkle)
                {
                    int kanalId1;
                    bool success = int.TryParse(item, out kanalId1);

                    if (success)
                    {
                        var kanalId = int.Parse(item);
                        var kanal = db.KanalEkles.Where(x => x.Id == kanalId).FirstOrDefault();
                        Data.KanalEkles.Add(kanal);
                    }
                    else
                    {
                        continue;
                    }

                }
            }

            if (CihazEkle != null)
            {
                foreach (var item in CihazEkle)
                {
                    int cihazId1;
                    bool success = int.TryParse(item, out cihazId1);

                    if (success)
                    {
                        var cihazId = int.Parse(item);
                        var cihaz = db.CihazEkles.Where(x => x.Id == cihazId).FirstOrDefault();
                        Data.CihazEkles.Add(cihaz);
                    }
                    else
                    {
                        continue;
                    }

                }
            }

            Data.Silindi = false;
            db.SirketEkles.Add(Data);

            try
            {
                db.SaveChanges();
            }
            catch (System.Exception)
            {

                TempData["Error"] = "Aynı isim var!";

                return RedirectToAction("Ekle");
            }


            return RedirectToAction("Index");

        }

        public ActionResult Sil(int id)
        {
            var sirket = db.SirketEkles.Where(x => x.Id == id).FirstOrDefault();

            sirket.Silindi = true;
            sirket.Kullanicilars.Clear();


            //db.SirketEkle.Remove(sirket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Guncelle(int id)
        {
            var sirket = db.SirketEkles.Where(x => x.Id == id).FirstOrDefault();

            var Kullanicilar = (from x in db.Kullanicilars
                          where x.Silindi == false && x.Rol != "A" && !(db.Kullanicilars.Any(z => z.SirketEkleId == sirket.Id && z.SirketEkleId != sirket.Id))
                          select x).AsEnumerable().Select(x => new SelectListItem
                          {
                              Text = x.Ad + " "+ x.Soyad,
                              Value = x.Id.ToString()
                          });
            var Kullanicilarlist = Kullanicilar.ToList();



            foreach (var kullanici in sirket.Kullanicilars)
            {
                Kullanicilarlist.Where(x => x.Value == kullanici.Id.ToString()).FirstOrDefault().Selected = true;
            }
            ViewBag.kllnc = Kullanicilarlist;

            var deger11 = (from x in db.KanalEkles
                           where x.Silindi == false && !(db.SirketEkles.Any(z => z.Id == x.SirketEkleId && x.SirketEkleId != sirket.Id))
                          select x).AsEnumerable().Select(x => new SelectListItem
                          {
                              Text = x.Name + " " + x.Type,
                              Value = x.Id.ToString()
                          });
            var deger11list = deger11.ToList();


            foreach (var kullanici in sirket.KanalEkles)
            {
                deger11list.Where(x => x.Value == kullanici.Id.ToString()).FirstOrDefault().Selected = true;
            }

            ViewBag.knl = deger11list;

            var deger1 = (from x in db.CihazEkles
                          where x.Silindi == false && !(db.SirketEkles.Any(z => z.Id == x.SirketEkleId && x.SirketEkleId != sirket.Id))
                          select x).AsEnumerable().Select(x => new SelectListItem
                          {
                              Text = x.CihazAd + x.CihazSeriNo + x.SimCard,
                              Value = x.Id.ToString()
                          });
            var deger1list = deger1.ToList();



            foreach (var cihazlar in sirket.CihazEkles)
            {
                deger1list.Where(x => x.Value == cihazlar.Id.ToString()).FirstOrDefault().Selected = true;
            }

            ViewBag.chz = deger1list;


            return View(sirket);
        }

        [HttpPost]
        public ActionResult Guncelle(SirketEkle model, List<string> Kullanicilar, List<string> KanalEkle, List<string> CihazEkle)
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
                        model.Kullanicilars.Add(kullanici);
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            if (KanalEkle != null)
            {
                foreach (var item in KanalEkle)
                {
                    int kanalId1;
                    bool success = int.TryParse(item, out kanalId1);

                    if (success)
                    {
                        var kanalId = int.Parse(item);
                        var kanal = db.KanalEkles.Where(x => x.Id == kanalId).FirstOrDefault();
                        model.KanalEkles.Add(kanal);
                    }
                    else
                    {
                        continue;
                    }

                }
            }

            if (CihazEkle != null)
            {
                foreach (var item in CihazEkle)
                {
                    int cihazId1;
                    bool success = int.TryParse(item, out cihazId1);
                    if (success)
                    {
                        var cihazId = int.Parse(item);
                        var cihaz = db.CihazEkles.Where(x => x.Id == cihazId).FirstOrDefault();
                        model.CihazEkles.Add(cihaz);
                    }
                    else
                    {
                        continue;
                    }
                }
            }



            //UPDATE DE VAR İSE EKLEMEYECEK DUPLICATE GİRİYOR!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            var firma = db.SirketEkles.Find(model.Id);
            firma.KanalEkles = model.KanalEkles;
            firma.SirketIsim = model.SirketIsim;
            firma.CihazEkles = model.CihazEkles;
            firma.Kullanicilars.Clear();
            firma.Kullanicilars = model.Kullanicilars;


            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}


