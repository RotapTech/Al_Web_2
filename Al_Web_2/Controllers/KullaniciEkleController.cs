using Al_Web_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace Al_Web_2.Controllers
{
    [Authorize(Roles = "A")]
    public class KullaniciEkleController : Controller
    {
        // GET: KullaniciEkle
        Al_WebEntities1 db = new Al_WebEntities1();

        public ActionResult Index()
        {
            var list = db.Kullanicilars.Include("SirketEkle").Include("KanalEkles").Include("CihazEkles").Where(x => x.Rol != "A" && x.Silindi == false).ToList();

            return View(list);
        }
        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            if (formCollection.Count != 0)
            {
                string[] ids = formCollection["userId"].Split(new char[] { ',' });
                foreach (string id in ids)
                {
                    var user = db.Kullanicilars.Find(int.Parse(id));

                    user.Silindi = true;
                    user.KanalEkles.Clear();
                    user.CihazEkles.Clear();

                    //db.Kullanicilars.Remove(user);
                }
                db.SaveChanges();
            }


            var list = db.Kullanicilars.Where(x => x.Rol != "A" && x.Silindi == false).ToList();

            return View(list);
        }

        public ActionResult Ekle()
        {
            List<SelectListItem> deger = (from x in db.KanalEkles.ToList()
                                          where x.Silindi == false
                                          select new SelectListItem
                                          {
                                              Text = x.Name + " " + x.Type,
                                              Value = x.Id.ToString()
                                          }).ToList();
            ViewBag.knl = deger;

            List<SelectListItem> deger1 = (from x in db.CihazEkles.ToList()
                                           where x.Silindi == false
                                           select new SelectListItem
                                           {
                                               Text = x.CihazAd + x.CihazSeriNo + x.SimCard,
                                               Value = x.Id.ToString()
                                           }).ToList();
            ViewBag.chz = deger1;

            var deger2 = (from x in db.SirketEkles
                          where x.Silindi == false && !(db.Kullanicilars.Any(z => z.SirketEkleId == x.Id))
                          select x).AsEnumerable().Select(x => new SelectListItem
                          {
                              Text = x.SirketIsim,
                              Value = x.Id.ToString()
                          });
            var deger2list = deger2.ToList();
            ViewBag.Error = TempData["Error"];
            deger2list.Insert(0,new SelectListItem
            {
                Text = "Boş",
                Value = "0"
            });
            ViewBag.frm = deger2list;

            return View();
        }

        [HttpPost]
        public ActionResult Ekle(Kullanicilar Data, List<string> KanalEkle, List<string> CihazEkle, FormCollection frm)
        {
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
            if (frm["SirketEkle"] != null)
            {
                //Data.SirketEkleId = Convert.ToInt16(frm["FirmaEkle"].ToString());

                int firmaId = Convert.ToInt16(frm["SirketEkle"].ToString());
                var sirket = db.SirketEkles.Where(x => x.Id == firmaId).FirstOrDefault();

                Data.SirketEkle = sirket;

            }


            Data.Silindi = false;
            Data.Sifre = Sifre_Md5.MD5Olustur(Data.Sifre);
            Data.AbonelikTarih = DateTime.Now;
            Data.Rol = "U";
            db.Kullanicilars.Add(Data);

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
            var user = db.Kullanicilars.Where(x => x.Id == id).FirstOrDefault();
            user.Silindi = true;
            user.KanalEkles.Clear();
            user.CihazEkles.Clear();



            //db.Kullanicilars.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Guncelle(int id)
        {

            var guncelle = db.Kullanicilars.Include("SirketEkle").Where(x => x.Id == id).FirstOrDefault();

            guncelle.Sifre = "";


            List<SelectListItem> deger = (from x in db.KanalEkles.ToList()
                                          where x.Silindi == false
                                          select new SelectListItem
                                          {
                                              Text = x.Name + " " + x.Type,
                                              Value = x.Id.ToString()
                                          }).ToList();

            foreach (var kullanici in guncelle.KanalEkles)
            {
                deger.Where(x => x.Value == kullanici.Id.ToString()).FirstOrDefault().Selected = true;
            }

            ViewBag.knl = deger;


            List<SelectListItem> deger1 = (from x in db.CihazEkles.ToList()
                                           where x.Silindi == false
                                           select new SelectListItem
                                           {
                                               Text = x.CihazAd + x.CihazSeriNo + x.SimCard,
                                               Value = x.Id.ToString()
                                           }).ToList();

            foreach (var kullanici in guncelle.CihazEkles)
            {
                deger1.Where(x => x.Value == kullanici.Id.ToString()).FirstOrDefault().Selected = true;
            }

            ViewBag.chz = deger1;




            var deger2 = (from x in db.SirketEkles
                          where x.Silindi == false && !(db.Kullanicilars.Any(z => z.SirketEkleId == x.Id && z.SirketEkleId != guncelle.SirketEkleId))
                          select x).AsEnumerable().Select(x => new SelectListItem
                          {
                              Text = x.SirketIsim,
                              Value = x.Id.ToString()
                          });
            var deger2list = deger2.ToList();

            if (guncelle.SirketEkle != null)
            {
                deger2list.Where(x => x.Value == guncelle.SirketEkle.Id.ToString()).FirstOrDefault().Selected = true;

            }



            deger2list.Insert(0, new SelectListItem
            {
                Text = "Boş",
                Value = "0"
            });
            ViewBag.frm = deger2list;

            return View(guncelle);
        }

        [HttpPost]
        public ActionResult Guncelle(Kullanicilar model, List<string> KanalEkle, List<string> CihazEkle, FormCollection frm)
        {
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


            if (frm["SirketEkle"] != null)
            {
                //Data.SirketEkleId = Convert.ToInt16(frm["FirmaEkle"].ToString());

                int firmaId = Convert.ToInt16(frm["SirketEkle"].ToString());
                var sirket = db.SirketEkles.Where(x => x.Id == firmaId).FirstOrDefault();

                model.SirketEkle = sirket;

            }


            var user = db.Kullanicilars.Find(model.Id);

            user.Sifre = Sifre_Md5.MD5Olustur(user.Sifre);
            user.Rol = "U";
            user.Ad = model.Ad;
            user.Soyad = model.Soyad;
            user.Email = model.Email;
            user.KullaniciAd = model.KullaniciAd;
            user.SirketEkle = model.SirketEkle;

            user.KanalEkles.Clear();
            user.CihazEkles.Clear();
            user.KanalEkles = model.KanalEkles;
            user.CihazEkles = model.CihazEkles;

            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}