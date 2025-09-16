﻿using AutoMapper;
using CoStudyCloud.Core.Constants;
using CoStudyCloud.Core.Models;
using CoStudyCloud.Core.Repositories;
using CoStudyCloud.Core.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoStudyCloud.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AccountController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string? returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(
                GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties()
                {
                    RedirectUri = Url.Action(nameof(ExternalLoginCallback), new { returnUrl })
                });
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl)
        {
            //Check authentication response as mentioned on startup file as options.DefaultSignInScheme = "External"
            var authenticateResult = await HttpContext.AuthenticateAsync("External");

            if (!authenticateResult.Succeeded)
            {
                return BadRequest();
            }

            //check if principal value exists or not 
            if (authenticateResult.Principal != null)
            {
                //Check if the redirection has been done via google or any other links
                if (authenticateResult.Principal.Identities.ToList()[0].AuthenticationType?.ToLower() == "google")
                {
                    //claim value initialization as mentioned on the startup file with options.DefaultScheme = "Application"
                    var claimsIdentity = new ClaimsIdentity("Application");

                    Claim idClaim = authenticateResult!.Principal.FindFirst(ClaimTypes.NameIdentifier)!;// Google Id of The User
                    Claim emailClaim = authenticateResult!.Principal.FindFirst(ClaimTypes.Email)!;// Email Address of The User
                    Claim firstNameClaim = authenticateResult!.Principal.FindFirst(ClaimTypes.GivenName)!;// Given Name Of The User
                    Claim lastNameClaim = authenticateResult.Principal.FindFirst(ClaimTypes.Surname)!;// Surname Of The User

                    claimsIdentity.AddClaim(emailClaim);
                    claimsIdentity.AddClaim(firstNameClaim);
                    claimsIdentity.AddClaim(lastNameClaim);

                    var role = emailClaim.Value == WhiteListAdminsConstant.Default
                        ? UserRolesConstant.SystemAdmin
                        : UserRolesConstant.Learner;

                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));

                    //Save the user in database for the first log in
                    if (!await _userRepository.Exists(emailClaim.Value))
                    {
                        var user = new User()
                        {
                            Email = emailClaim.Value,
                            FirstName = firstNameClaim.Value,
                            LastName = lastNameClaim.Value,
                            GoogleId = idClaim.Value,
                            ProfileImageUrl = authenticateResult.Principal.FindFirst("picture")?.Value,
                            UserRole = role,
                            CreateDate = DateTime.UtcNow,
                            LastEditDate = DateTime.UtcNow
                        };

                        await _userRepository.Add(user);
                    }

                    await HttpContext.SignInAsync("Application", new ClaimsPrincipal(claimsIdentity));

                    if (returnUrl == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    return Redirect(returnUrl);
                }
            }

            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            //Check if any cookie value is present
            if (HttpContext.Request.Cookies.Any())
            {
                //Check for the cookie value with the name mentioned for authentication and delete each cookie
                var siteCookies =
                    HttpContext.Request.Cookies.Where(
                        c => c.Key.Contains(".AspNetCore.") || c.Key.Contains("Microsoft.Authentication"));

                foreach (var cookie in siteCookies)
                {
                    Response.Cookies.Delete(cookie.Key);
                }
            }

            //sign out with any cookie present 
            await HttpContext.SignOutAsync("External");

            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> Profile()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value!;

            var user = await _userRepository.GetByEmail(email);

            if (user == null)
            {
                return NotFound();
            }

            var profileFormViewModel = _mapper.Map<User, ProfileFormViewModel>(user);

            return View(profileFormViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileFormViewModel profileFormViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", profileFormViewModel);
            }

            var existingUser = await _userRepository.GetByEmail(profileFormViewModel.Email!);

            if (existingUser == null)
            {
                return NotFound();
            }

            // Update properties of the existing user with the values from the view model
            existingUser.FirstName = profileFormViewModel.FirstName;
            existingUser.LastName = profileFormViewModel.LastName;
            existingUser.ProfileImageUrl = profileFormViewModel.ProfileImageUrl;
            existingUser.LastEditDate = DateTime.UtcNow;

            await _userRepository.Update(existingUser);

            return RedirectToAction(nameof(Profile));
        }
    }
}
