using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        string username,
        string email,
        string password,
        string confirmPassword)
    {
        //If Empty UserName or Pass
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(string.Empty, "نام کاربری و رمز ورود را وارد نمایید");
            return View();
        }
        //If Pass Does Not Match
        if (password != confirmPassword)
        {
            ModelState.AddModelError("", "تکرار پسوورد اشتباه است");
            return View();
        }
        //Create and Add User
        var user = new IdentityUser
        {
            UserName = username ,
            Email = email
        };

        var result = await _userManager.CreateAsync(user, password);

        //Redirect To Index
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index", "Home");
        }
        //If Error
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View();
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        string username,
        string password,
        string? returnUrl = null)
    {
        //If UserName Or Pass Is Empty
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(
                string.Empty,
                "نام کاربری و رمز ورود را وارد نمایید");

            ViewBag.ReturnUrl = returnUrl;

            return View();
        }
        //Sign In User
        var result = await _signInManager.PasswordSignInAsync(
            username,
            password,
            false,
            false);

        //Redirect    
        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        //If Error
        ModelState.AddModelError("", "نام کاربری یا رمز عبور نامعتبر است");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("Login");
    }
}