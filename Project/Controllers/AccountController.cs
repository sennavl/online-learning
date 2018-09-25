using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Project.Models;
using Project.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Project.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            if (!User.Identity.IsAuthenticated)
            {
                FilterModel fm = new FilterModel();
                return View(fm);
            }
            else
            {
                if (this.User.IsInRole("Teacher"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin user)
        {
            if (ModelState.IsValid)
            {
                if (PasswordHelper.IsValid(user.Email, user.Password))
                {
                    User u = Models.User.GetUserByEmail(user.Email);

                    var ID = new ClaimsIdentity(
                      new[] {
                      // adding following 2 claim just for supporting default antiforgery provider
                      new Claim(ClaimTypes.NameIdentifier, u.email),
                      new Claim(ClaimTypes.Name, u.email),
                      new Claim(ClaimTypes.Email, u.email),

                      new Claim(ClaimTypes.Role, u.Role.name),
                      },
                      DefaultAuthenticationTypes.ApplicationCookie);

                    HttpContext.GetOwinContext().Authentication.SignIn(
                        new AuthenticationProperties { IsPersistent = false }, ID);
                    return RedirectToAction("Index", "Home"); // auth succeed 
                }
                else
                {
                    ViewBag.WrongCredentials = "Email and password combination is not correct";
                }
                // invalid username or password
                ModelState.AddModelError("", "invalid username or password");

                FilterModel fm = new FilterModel();

                return View(fm);
            }

            FilterModel fmDefault = new FilterModel();

            return View(fmDefault);
        }

        public ActionResult SignUp()
        {
            FilterModel fm = new FilterModel();
            fm.User.RoleList = Models.User.GetRoles();

            return View(fm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                if (Models.User.GetUserByEmail(user.email) == null)
                {
                    Models.User.AddUser(user);
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ViewBag.AlreadyExists = "There is already an account using this email address";
                }
            }
            FilterModel fm2 = new FilterModel();
            fm2.User.RoleList = Models.User.GetRoles();
            return View(fm2);
        }

        [Authorize]
        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Login");
        }

        [Authorize]
        public ActionResult Profile()
        {
            var user = Models.User.GetUserByEmail(User.Identity.Name);

            FilterModel fm = new FilterModel();
            fm.Courses = Course.GetCoursesOfUser(user, false);

            List<string> filenames = new List<string>();
            List<double> ratings = new List<double>();
            foreach (var course in fm.Courses)
            {
                if (Directory.Exists(Server.MapPath("~/Content/MainImages/" + course.id)))
                {
                    DirectoryInfo directory = new DirectoryInfo(Server.MapPath("~/Content/MainImages/" + course.id));
                    FileInfo[] files = directory.GetFiles("*.*");
                    filenames.Add(files[0].Name);
                    ratings.Add(Math.Round(Rating.GetRating(course.id), 2));
                }
                else
                {
                    filenames.Add("default.jpg");
                    ratings.Add(Math.Round(Rating.GetRating(course.id), 2));
                }
            }

            ViewBag.Files = filenames;
            ViewBag.Ratings = ratings;
            return View(fm);
        }
    }
}