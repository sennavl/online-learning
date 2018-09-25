using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Models
{
    [MetadataType(typeof(CourseValidationModel))]
    public partial class Course
    {
        public static List<Course> GetCourses()
        {
            List<Course> courses = new List<Course>();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<Course>(s => s.Language);
                    options.LoadWith<Course>(s => s.Level);
                    options.LoadWith<Course>(s => s.Category);
                    db.LoadOptions = options;

                    courses = db.Courses.ToList();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return courses;
        }

        public static List<Course> GetCourses(string category)
        {
            List<Course> courses = new List<Course>();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<Course>(s => s.Language);
                    options.LoadWith<Course>(s => s.Level);
                    options.LoadWith<Course>(s => s.Category);
                    db.LoadOptions = options;

                    courses = db.Courses.Where(m => m.category_id == Category.GetCategoryId(category)).ToList();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return courses;
        }

        public static List<Course> GetTopThreeRecentCourses()
        {
            List<Course> courses = new List<Course>();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<Course>(s => s.Language);
                    options.LoadWith<Course>(s => s.Level);
                    options.LoadWith<Course>(s => s.Category);
                    db.LoadOptions = options;

                    courses = db.Courses.OrderByDescending(c => c.last_updated).Take(3).ToList();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return courses;
        }

        public static List<Course> GetTopThreePopularCourses()
        {
            List<Course> courses = new List<Course>();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    List<int> course_ids = Subscription.GetTopThreePopularCoursesIds();

                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<Course>(s => s.Language);
                    options.LoadWith<Course>(s => s.Level);
                    options.LoadWith<Course>(s => s.Category);
                    db.LoadOptions = options;

                    foreach(var courseId in course_ids)
                    {
                        courses.Add(db.Courses.Where(m => m.id == courseId).FirstOrDefault());
                    }
                    return courses;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return courses;
        }

        public static Course GetCourse(int id)
        {
            Course course = new Course();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<Course>(s => s.Language);
                    options.LoadWith<Course>(s => s.Level);
                    options.LoadWith<Course>(s => s.Category);
                    options.LoadWith<Course>(s => s.Sections);
                    db.LoadOptions = options;

                    course = db.Courses.Where(s => s.id == id).FirstOrDefault();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return course;
        }

        public static Course AddCourse(Course course)
        {
            Course c = new Course();

            c.category_id = course.category_id;
            c.description = course.description;
            c.introduction = course.introduction;
            c.language_id = course.language_id;
            c.last_updated = DateTime.Now;
            c.length = course.length;
            c.level_id = course.level_id;
            c.price = course.price;
            c.title = course.title;

            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    db.Courses.InsertOnSubmit(c);
                    db.SubmitChanges();
                    return db.Courses.FirstOrDefault(m => m.id == c.id);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public static Course EditCourse(Course course)
        {
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    Course originalCourse = db.Courses.FirstOrDefault(m => m.id.Equals(course.id));

                    originalCourse.category_id = course.category_id;
                    originalCourse.description = course.description;
                    originalCourse.introduction = course.introduction;
                    originalCourse.language_id = course.language_id;
                    originalCourse.last_updated = DateTime.Now;
                    originalCourse.length = course.length;
                    originalCourse.level_id = course.level_id;
                    originalCourse.price = course.price;
                    originalCourse.title = course.title;

                    db.SubmitChanges();
                    return db.Courses.FirstOrDefault(m => m.id == originalCourse.id);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public static void DeleteCourse(int course_id)
        {
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    Course delete_course = db.Courses.Where(m => m.id == course_id).FirstOrDefault();
                    db.Courses.DeleteOnSubmit(delete_course);
                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static List<Course> GetCoursesOfUser(Models.User u, bool owner)
        {
            List<Course> courses = new List<Course>();
            using (var db = new OnlineLearningDataContext())
            {
                DataLoadOptions options = new DataLoadOptions();
                options.LoadWith<Course>(s => s.Language);
                options.LoadWith<Course>(s => s.Level);
                options.LoadWith<Course>(s => s.Category);
                db.LoadOptions = options;

                var courseIds = (from a in db.Subscriptions
                                 where a.user_id == u.id
                                 && a.owner == owner
                                 && a.visible == true
                                 select a.course_id).ToList();

                foreach (var id in courseIds)
                {
                    courses.Add((from a in db.Courses
                                 where a.id == id
                                 select a).FirstOrDefault());
                }
            }
            return courses;
        }

        public static List<Course> GetCourses(FilterModel fm)
        {
            List<Course> courses = new List<Course>();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<Course>(s => s.Language);
                    options.LoadWith<Course>(s => s.Level);
                    options.LoadWith<Course>(s => s.Category);
                    db.LoadOptions = options;

                    courses = (from s in db.Courses
                               where fm.CategoryId == null || s.category_id == fm.CategoryId
                               where fm.LevelId == null || s.level_id == fm.LevelId
                               where fm.LanguageId == null || s.language_id == fm.LanguageId
                               where fm.SearchTerm == null || s.title.Contains(fm.SearchTerm)
                               where fm.PriceFrom == null || s.price > fm.PriceFrom
                               where fm.PriceTo == null || s.price < fm.PriceTo
                               select s).ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return courses;
        }
    }

    public class CourseValidationModel
    {
        [RegularExpression(@"^[a-zA-Z\u00C0-\u017F0-9:,()& ]+$",
        ErrorMessage = "Only alphabets (including accented characters), numbers, spaces, parenthesis, comma, & and double point allowed")]
        [Required(ErrorMessage = "This field is required")]
        public string title;
        
        [AllowHtml]
        [Required(ErrorMessage = "This field is required")]
        public string description { get; set; }

        [RegularExpression(@"^[0-9,]+$",
        ErrorMessage = "Only numbers and comma allowed, use the following format: xxx,xx")]
        [Required(ErrorMessage = "This field is required")]
        public int price;

        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$",
        ErrorMessage = "Follow this notation HH:MM:SS")]
        public TimeSpan length;

        [RegularExpression(@"^[0-9a-zA-Z\u00C0-\u017F,.:;!? ]+$",
        ErrorMessage = "Only alphabets, spaces, numbers and punctuation marks allowed")]
        [Required(ErrorMessage = "This field is required")]
        public string introduction;

        [RegularExpression(@"^[0-9]+$",
        ErrorMessage = "Only numbers allowed")]
        [Required(ErrorMessage = "This field is required")]
        public int language_id;

        [RegularExpression(@"^[0-9]+$",
        ErrorMessage = "Only numbers allowed")]
        [Required(ErrorMessage = "This field is required")]
        public int level_id;

        [RegularExpression(@"^[0-9]+$",
        ErrorMessage = "Only numbers allowed")]
        [Required(ErrorMessage = "This field is required")]
        public int category_id;
    }
}