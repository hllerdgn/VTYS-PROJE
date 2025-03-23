using Microsoft.AspNetCore.Mvc;

namespace KullaniciGirisKayit.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // Giriş işlemleri burada yapılacak
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Register(string username, string email, string password, string confirmPassword)
        {
            // Kayıt işlemleri burada yapılacak
            return RedirectToAction("Login");
        }
    }
}