using Al_Web_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Al_Web_2.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        Al_WebEntities1 db = new Al_WebEntities1();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public ActionResult Login(Kullanicilar p)
        {

            string sifre1 = Sifre_Md5.MD5Olustur(p.Sifre);
            var bilgiler = db.Kullanicilar.FirstOrDefault(x => x.Email == p.Email && x.Sifre == sifre1);

            if (bilgiler != null)
            {
                FormsAuthentication.SetAuthCookie(bilgiler.Email, false);
                Session["Mail"] = bilgiler.Email.ToString();
                Session["Ad"] = bilgiler.Ad.ToString();
                Session["Soyad"] = bilgiler.Soyad.ToString();
                return RedirectToAction("Index", "KullaniciEkle");

            }
            else
            {
                ViewBag.hata = "Kullanıcı Adı veya Şifre Hatalı";
            }
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Kullanicilar data)
        {
            db.Kullanicilar.Add(data);
            data.Rol = "U";
            db.SaveChanges();
            return RedirectToAction("Login", "Account");
        }



        public ActionResult Guncelle()
        {
            var kullanicilar = (string)Session["Mail"];
            var degerler = db.Kullanicilar.FirstOrDefault(x => x.Email == kullanicilar);
            return View(degerler);
        }

        [HttpPost]
        public ActionResult Guncelle(Kullanicilar data)
        {
            var kullanicilar = (string)Session["Mail"];
            var user = db.Kullanicilar.Where(x => x.Email == kullanicilar).FirstOrDefault();
            user.Ad = data.Ad;
            user.Soyad = data.Soyad;
            user.Email = data.Email;
            user.KullaniciAd = data.KullaniciAd;
            user.Sifre = data.Sifre;
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}