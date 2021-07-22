using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SummerBootCampTask2.Contexts;
using SummerBootCampTask2.CoreModels;
using SummerBootCampTask2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SummerBootCampTask2.Controllers
{
    public class AccountController : Controller
    {
        private readonly BootCampDbContext dbContext;

        public AccountController(BootCampDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = dbContext.Users.FirstOrDefault(user => user.Email == model.Email && user.Password == model.Password);

                if (user != null)
                {
                    await Authenticate(model.Email);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid password and(or) email");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = dbContext.Users.FirstOrDefault(user => user.Email == model.Email);

                if (user is null)
                {
                    var random = new Random();
                    dbContext.Users.Add(new User
                    {
                        UserName = model.UserName,
                        Password = model.Password,
                        Email = model.Email,
                        Identifier = random.Next(10000, 100000),
                        Visible = true,
                        Key = random.Next(20),
                    });

                    dbContext.SaveChanges();

                    await Authenticate(model.Email);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "User already exist!");
                }
            }

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            var user = dbContext.Users.FirstOrDefault(x => x.Email == User.Identity.Name);

            return View(new AccountViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Password = user.Password,
                Visible = user.Visible,
                Identifier = user.Identifier,
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Index(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = dbContext.Users.FirstOrDefault(x => x.Email == User.Identity.Name);

                if (user.Email != model.Email)
                {
                    if (!dbContext.Users.Any(x => x.Email == model.Email))
                    {
                        user.Email = model.Email;
                    }
                    else
                    {
                        ModelState.AddModelError("", "UserName is already taken!");
                    }
                }

                user.Visible = model.Visible;
                user.UserName = model.UserName;
                user.Password = model.Password;

                dbContext.SaveChanges();
            }

            await Authenticate(model.Email);

            return RedirectToAction("Index", "Account");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }

        private async Task Authenticate(string email)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, email),
            };

            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
