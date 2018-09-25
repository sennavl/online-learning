using Project.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Project.Controllers
{
    public class CoursesController : Controller
    {
        public ActionResult Overview()
        {
            FilterModel fm = new FilterModel();

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

        [HttpPost]
        public ActionResult Overview(FilterModel fm)
        {
            if (ModelState.IsValid)
            {
                FilterModel fm2 = new FilterModel(fm);

                List<string> filenames = new List<string>();
                List<double> ratings = new List<double>();
                foreach (var course in fm2.Courses)
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
                return View(fm2);
            }
            else
            {
                FilterModel fmDefault = new FilterModel();
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
                return View(fmDefault);
            }
        }

        public ActionResult Detail(int id)
        {
            FilterModel fm = new FilterModel();

            ViewBag.Course = Course.GetCourse(id);

            ViewBag.IsSubscribed = false;

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.IsSubscribed = Subscription.ExistsForUser(Models.User.GetUserByEmail(User.Identity.Name).id, id);
                ViewBag.IsVisible = Subscription.IsVisible(Models.User.GetUserByEmail(User.Identity.Name).id, id);
            }

            User user = Models.User.GetUserById(Subscription.GetOwnerId(id));
            ViewBag.Owner = user.first_name + " " + user.last_name;

            if (Directory.Exists(Server.MapPath("~/Content/MainImages/" + ViewBag.Course.id)))
            {
                DirectoryInfo directory = new DirectoryInfo(Server.MapPath("~/Content/MainImages/" + ViewBag.Course.id));
                FileInfo[] files = directory.GetFiles("*.*");
                ViewBag.File = files[0].Name;
            }
            else
            {
                ViewBag.File = "default.jpg";
            }

            ViewBag.Rating = Math.Round(Rating.GetRating(id), 2);
            ViewBag.LastUpdated = Course.GetCourse(id).last_updated.ToString("dd/MM/yyyy");

            return View(fm);
        }

        [Authorize]
        public ActionResult Sections(int id)
        {
            var user = Models.User.GetUserByEmail(User.Identity.Name);

            if (user.IsSubscribed(id) || user.IsOwner(id))
            {
                ViewBag.AlreadyVoted = Rating.AlreadyVoted(user.id, id);
                ViewBag.IsOwner = user.IsOwner(id);
                FilterModel fm = new FilterModel();
                ViewBag.Sections = Section.GetSectionsForCourse(id);
                ViewBag.Course = Course.GetCourse(id);
                return View(fm);
            }
            return RedirectToAction("Profile", "Account");
        }

        [Authorize]
        public ActionResult Subsections(int id)
        {
            FilterModel fm = new FilterModel();
            ViewBag.Section = Section.GetSectionById(id);
            ViewBag.Subsections = Subsection.GetSubsectionsForSection(id);
            return View(fm);
        }

        [Authorize]
        public ActionResult SubsectionDetail(int id)
        {
            FilterModel fm = new FilterModel();
            ViewBag.Subsection = Subsection.GetSubsectionById(id);

            if (Directory.Exists(Server.MapPath("~/Content/attatchments/" + id)))
            {
                var dir = new DirectoryInfo(Server.MapPath("~/Content/attatchments/" + id + "/"));
                FileInfo[] fileNames = dir.GetFiles("*.*");
                List<string> items = new List<string>();

                foreach (var file in fileNames)
                {
                    items.Add(file.Name);
                }

                fm.Items = items;
            }

            return View(fm);
        }

        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }

        [Authorize]
        [HttpPost]
        public ActionResult Register(int id)
        {
            Subscription.AddSubscription(Models.User.GetUserByEmail(User.Identity.Name).id, id, false);
            return RedirectToAction("Profile", "Account");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Hide(int id)
        {
            int user_id = Models.User.GetUserByEmail(User.Identity.Name).id;
            Subscription.ToggleSubscription(user_id, id, false);
            return RedirectToAction("Profile", "Account");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Show(int id)
        {
            int user_id = Models.User.GetUserByEmail(User.Identity.Name).id;
            Subscription.ToggleSubscription(user_id, id, true);
            return RedirectToAction("Sections", "Courses", new { id = id });
        }

        [Authorize]
        public ActionResult Review(int id)
        {
            var user = Models.User.GetUserByEmail(User.Identity.Name);

            if (user.IsSubscribed(id) && !Rating.AlreadyVoted(user.id, id))
            {
                ViewBag.CourseId = id;
                FilterModel fm = new FilterModel();
                return View(fm);
            }
            return RedirectToAction("Profile", "Account");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Review(Rating rating, string id)
        {
            if (ModelState.IsValid)
            {
                int course_id = Int32.Parse(id);
                var user = Models.User.GetUserByEmail(User.Identity.Name);
                if (user.IsSubscribed(course_id) && !Rating.AlreadyVoted(user.id, course_id))
                {
                    rating.user_id = user.id;
                    rating.course_id = course_id;
                    Rating.AddRating(rating);
                    return RedirectToAction("Profile", "Account");
                }
            }
            ViewBag.CourseId = id;
            FilterModel fmDefault = new FilterModel();
            return View(fmDefault);
        }

        [Authorize]
        public FileResult Download(int id, string itemName)
        {
            string extension = Path.GetExtension("~/Content/attatchments/" + id + "/" + itemName);
            return File("~/Content/attatchments/" + id + "/" + itemName, System.Net.Mime.MediaTypeNames.Application.Octet, itemName);
        }
    }
}