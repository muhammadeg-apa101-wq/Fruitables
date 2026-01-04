using FruitablesFrontToBack.Models;
using FruitablesFrontToBack.ViewModels.AccountVMs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FruitablesFrontToBack.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            AppUser newUser = new()
            {
                Name = request.Name,
                Surname = request.Surname,
                UserName = request.Username,
                Email = request.Email
            };

            IdentityResult result = await _userManager.CreateAsync(newUser, request.Password);


            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(request);
            }

            await _signInManager.SignInAsync(newUser, false);

            return RedirectToAction("Login", "Account");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }


            AppUser? user = await _userManager.FindByNameAsync(request.UsernameOrEmail);

            user ??= await _userManager.FindByEmailAsync(request.UsernameOrEmail);


            if (user == null)
            {
                ModelState.AddModelError("", "Username or password is incorrect");
                return View(request);
            }


            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);


            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or password is incorrect");
                return View(request);
            }
            else 
            {
               await _signInManager.SignInAsync(user, false);
            }
                return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
