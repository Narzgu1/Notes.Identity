﻿using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Notes.Identity.Models;

namespace Notes.Identity.Controllers
{
    public class AuthController : Controller
    {
        readonly SignInManager<AppUser> _signInManager;
        readonly UserManager<AppUser> _userManager;
        readonly IIdentityServerInteractionService _interactionService;

        public AuthController(SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager, 
            IIdentityServerInteractionService interactionService) => 
            (_signInManager, _userManager, _interactionService) = 
            (signInManager, userManager, interactionService);

        [HttpGet]   
        public IActionResult Login(string returnUrl)
        {
            var viewModel =  new LoginViewModel
            { 
                ReturnUrl = returnUrl 
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            var result = await _signInManager.PasswordSignInAsync(viewModel.UserName,
                viewModel.Password, false, false);
            if (result.Succeeded)
            {
                return Redirect(viewModel.ReturnUrl);
            }
            ModelState.AddModelError(string.Empty, "Login Error");
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Register(string returnUrl)
        {
            var viewModel = new RegisterViewModel { ReturnUrl = returnUrl };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        { 
            if(!ModelState.IsValid) 
                return View(viewModel);
            var user = new AppUser
            {
                UserName = viewModel.UserName
            };
            var result = await _userManager.CreateAsync(user, viewModel.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return Redirect(viewModel.ReturnUrl);
            }
            ModelState.AddModelError(string.Empty, "Error occurred");
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();
            var logoutRequest = await _interactionService.GetLogoutContextAsync(logoutId);
            return Redirect(logoutRequest.PostLogoutRedirectUri);
        }
    }
}
