using Project.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            FilterModel fm = new FilterModel();
            fm.TopThreeRecentCourses = Course.GetTopThreeRecentCourses();
            ViewBag.Categories = new SelectList(Category.GetCategories());

            List<string> filenames = new List<string>();
            List<double> ratings = new List<double>();
            foreach (var course in fm.TopThreeRecentCourses)
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