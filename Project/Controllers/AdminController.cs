using Project.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            var user = Models.User.GetUserByEmail(User.Identity.Name);

            FilterModel fm = new FilterModel();
            fm.Courses = Course.GetCoursesOfUser(user, true);

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

        // CRUD COURSE
        public ActionResult AddCourse()
        {
            FilterModel fm = new FilterModel();
            return View(fm);
        }

        [HttpPost]
        public ActionResult AddCourse(Course course)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    if (file.ContentLength > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                        var checkExtension = Path.GetExtension(file.FileName).ToLower();

                        if (!allowedExtensions.Contains(checkExtension))
                        {
                            ViewBag.Notice = "You can only upload .jpg, .jpeg and .png";
                            FilterModel fm = new FilterModel();
                            return View(fm);
                        }
                        else
                        {
                            Course addedCourse = Course.AddCourse(course);

                            Subscription.AddSubscription(Models.User.GetUserByEmail(User.Identity.Name).id, addedCourse.id, true);

                            string fileName = addedCourse.id + Path.GetExtension(file.FileName);
                            Directory.CreateDirectory(Server.MapPath("~/Content/MainImages/" + addedCourse.id));
                            string path = Path.Combine(Server.MapPath("~/Content/MainImages/" + addedCourse.id), fileName);

                            file.SaveAs(path);
                        }
                    }
                    else
                    {
                        Course addedCourse = Course.AddCourse(course);

                        Subscription.AddSubscription(Models.User.GetUserByEmail(User.Identity.Name).id, addedCourse.id, true);
                    }
                }

                return RedirectToAction("Index", "Admin");
            }
            FilterModel fmDefault = new FilterModel();
            return View(fmDefault);
        }

        public ActionResult EditCourse(int id)
        {
            var user = Models.User.GetUserByEmail(User.Identity.Name);
            if (user.IsOwner(id))
            {
                FilterModel fm = new FilterModel();
                fm.Course = Course.GetCourse(id);
                return View(fm);
            }
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public ActionResult EditCourse(Course course, int id)
        {
            var user = Models.User.GetUserByEmail(User.Identity.Name);
            if (user.IsOwner(id))
            {
                if (ModelState.IsValid)
                {
                    if (Request.Files.Count > 0)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        if (file.ContentLength > 0)
                        {

                            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                            var checkExtension = Path.GetExtension(file.FileName).ToLower();

                            if (!allowedExtensions.Contains(checkExtension))
                            {
                                ViewBag.Notice = "You can only upload .jpg, .jpeg and .png";
                                FilterModel fm = new FilterModel();
                                fm.Course = Course.GetCourse(id);
                                return View(fm);
                            }
                            else
                            {
                                course.id = id;
                                Course editedCourse = Course.EditCourse(course);

                                string fileName = editedCourse.id + Path.GetExtension(file.FileName);
                                Directory.CreateDirectory(Server.MapPath("~/Content/MainImages/" + editedCourse.id));
                                string path = Path.Combine(Server.MapPath("~/Content/MainImages/" + editedCourse.id), fileName);

                                System.IO.File.Delete(path);

                                file.SaveAs(path);
                            }



                        }
                        else
                        {
                            course.id = id;
                            Course editedCourse = Course.EditCourse(course);
                        }
                    }
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    FilterModel fmDefault = new FilterModel();
                    fmDefault.Course = Course.GetCourse(id);
                    return View(fmDefault);
                }
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult DeleteCourse(int id)
        {
            var user = Models.User.GetUserByEmail(User.Identity.Name);

            if (user.IsOwner(id))
            {
                Course.DeleteCourse(id);
            }
            return RedirectToAction("Index", "Admin");
        }

        // CRUD SECTION
        public ActionResult Sections(int id)
        {
            var user = Models.User.GetUserByEmail(User.Identity.Name);

            if (user.IsOwner(id))
            {
                ViewBag.Sections = Section.GetSectionsForCourse(id);
                ViewBag.Course = Course.GetCourse(id);
                FilterModel fm = new FilterModel();
                return View(fm);
            }
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult AddSection(int id)
        {
            var user = Models.User.GetUserByEmail(User.Identity.Name);

            if (user.IsOwner(id))
            {
                ViewBag.CourseId = id;
                FilterModel fm = new FilterModel();
                return View(fm);
            }
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public ActionResult AddSection(Section section, int id)
        {
            var user = Models.User.GetUserByEmail(User.Identity.Name);
            if (user.IsOwner(id))
            {
                if (ModelState.IsValid)
                {
                    int course_id = id;
                    section.course_id = course_id;
                    Section.AddSection(section);

                    return RedirectToAction("Sections", "Admin", new { id = id });
                }
                else
                {
                    ViewBag.CourseId = id;
                    FilterModel fmDefault = new FilterModel();
                    return View(fmDefault);
                }
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        public ActionResult EditSection(int id)
        {
            var user = Models.User.GetUserByEmail(User.Identity.Name);
            if (user.IsOwner(Course.GetCourse(Section.GetSectionById(id).course_id).id))
            {
                FilterModel fm = new FilterModel();
                fm.Section = Section.GetSectionById(id);
                return View(fm);
            }
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public ActionResult EditSection(Section section, int id, int idCourse)
        {
            var user = Models.User.GetUserByEmail(User.Identity.Name);
            if (user.IsOwner(idCourse))
            {
                if (ModelState.IsValid)
                {
                    section.id = id;
                    section.course_id = idCourse;
                    Section.EditSection(section);
                    return RedirectToAction("Sections", "Admin", new { id = idCourse });
                }
                else
                {
                    FilterModel fm = new FilterModel();
                    fm.Section = Section.GetSectionById(id);
                    return View(fm);
                }
            }
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public ActionResult DeleteSection(int idSection)
        {
            var user = Models.User.GetUserByEmail(User.Identity.Name);
            int courseId = Section.GetSectionById(idSection).course_id;
            if (user.IsOwner(Course.GetCourse(courseId).id))
            {
                Section.DeleteSection(idSection);
                return RedirectToAction("Sections", "Admin", new { id = courseId });
            }
            return RedirectToAction("Index", "Admin");
        }

        // CRUD SUBSECTION
        public ActionResult Subsections(int id)
        {
            var user = Models.User.GetUserByEmail(User.Identity.Name);
            if (user.IsOwner(Course.GetCourse(Section.GetSectionById(id).course_id).id))
            {
                ViewBag.Subsections = Subsection.GetSubsectionsForSection(id);
                ViewBag.Section = Section.GetSectionById(id);
                ViewBag.CourseId = Section.GetSectionById(id).course_id;
                FilterModel fm = new FilterModel();
                return View(fm);
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        public ActionResult AddSubsection(int id)
        {
            var user = Models.User.GetUserByEmail(User.Identity.Name);
            if (user.IsOwner(Course.GetCourse(Section.GetSectionById(id).course_id).id))
            {
                ViewBag.SectionId = id;
                FilterModel fm = new FilterModel();
                return View(fm);
            }
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public ActionResult AddSubsection(Subsection subsection, int id)
        {
            var user = Models.User.GetUserByEmail(User.Identity.Name);
            if (user.IsOwner(Course.GetCourse(Section.GetSectionById(id).course_id).id))
            {
                if (ModelState.IsValid)
                {
                    int section_id = id;
                    subsection.section_id = section_id;
                    Subsection addedSubsection = Subsection.AddSubsection(subsection);

                    if (Request.Files.Count > 0)
                    {
                        string path;
                        for (var i = 0; i < Request.Files.Count; i++)
                        {
                            if (Request.Files[i].ContentLength > 0)
                            {
                                if (!Directory.Exists(Server.MapPath("~/Content/attatchments/" + addedSubsection.id)))
                                {
                                    Directory.CreateDirectory(Server.MapPath("~/Content/attatchments/" + addedSubsection.id));
                                }

                                path = Path.Combine(Server.MapPath("~/Content/attatchments/" + addedSubsection.id), Request.Files[i].FileName);

                                Request.Files[i].SaveAs(path);
                            }
                        }
                    }
                    return RedirectToAction("Subsections", "Admin", new { id = id });
                }
                else
                {
                    ViewBag.SectionId = id;
                    FilterModel fmDefault = new FilterModel();
                    return View(fmDefault);
                }
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        public ActionResult EditSubsection(int id)
        {
            var user = Models.User.GetUserByEmail(User.Identity.Name);
            if (user.IsOwner(Subsection.GetSubsectionById(id)))
            {
                FilterModel fm = new FilterModel();
                fm.Subsection = Subsection.GetSubsectionById(id);
                return View(fm);
            }
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public ActionResult EditSubsection(Subsection subsection, int id, int idSection)
        {
            var user = Models.User.GetUserByEmail(User.Identity.Name);
            if (user.IsOwner(Course.GetCourse(Section.GetSectionById(idSection).course_id).id))
            {
                if (ModelState.IsValid)
                {
                    subsection.id = id;
                    subsection.section_id = idSection;
                    Subsection editedSubsection = Subsection.EditSubsection(subsection);
                    if (Request.Files.Count > 0)
                    {
                        string path;
                        for (var i = 0; i < Request.Files.Count; i++)
                        {
                            if (Request.Files[i].ContentLength > 0)
                            {
                                if (!Directory.Exists(Server.MapPath("~/Content/attatchments/" + editedSubsection.id)))
                                {
                                    Directory.CreateDirectory(Server.MapPath("~/Content/attatchments/" + editedSubsection.id));
                                }

                                path = Path.Combine(Server.MapPath("~/Content/attatchments/" + editedSubsection.id), Request.Files[i].FileName);

                                Request.Files[i].SaveAs(path);
                            }
                        }
                    }
                    return RedirectToAction("Subsections", "Admin", new { id = idSection });
                }
                else
                {
                    FilterModel fm = new FilterModel();
                    fm.Subsection = Subsection.GetSubsectionById(id);
                    return View(fm);
                }
            }
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public ActionResult DeleteSubsection(int idSubsection)
        {
            var user = Models.User.GetUserByEmail(User.Identity.Name);
            Subsection subsection = Subsection.GetSubsectionById(idSubsection);
            if (user.IsOwner(subsection))
            {
                Subsection.DeleteSubsection(idSubsection);
                return RedirectToAction("Subsections", "Admin", new { id = subsection.section_id });
            }
            return RedirectToAction("Index", "Admin");
        }
    }
}