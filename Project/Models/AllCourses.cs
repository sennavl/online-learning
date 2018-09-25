using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class AllCourses
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public TimeSpan Length { get; set; }
        public string Introduction { get; set; }
        public DateTime Last_updated { get; set; }
        public double Rating { get; set; }
        public string Language { get; set; }
        public string Level { get; set; }
        public string Category { get; set; }
        public string Link { get; set; }

        public static IEnumerable<AllCourses> GetCourses()
        {
            List<Course> courses = new List<Course>();
            courses = Course.GetCourses();

            List<AllCourses> allCourses = new List<AllCourses>();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<Course>(s => s.Language);
                    options.LoadWith<Course>(s => s.Level);
                    options.LoadWith<Course>(s => s.Category);
                    db.LoadOptions = options;
                    foreach (var course in courses)
                    {
                        AllCourses apiCourse = new AllCourses();
                        apiCourse.Title = course.title;
                        apiCourse.Description = course.description;
                        apiCourse.Price = course.price;
                        apiCourse.Length = course.length;
                        apiCourse.Introduction = course.introduction;
                        apiCourse.Last_updated = course.last_updated;
                        apiCourse.Language = course.Language.name;
                        apiCourse.Level = course.Level.name;
                        apiCourse.Category = course.Category.name;
                        double rating = Math.Round(Models.Rating.GetRating(course.id), 2);
                        if (rating != 6) apiCourse.Rating = rating;
                        apiCourse.Link = "http://onlinelearning.sennavanlonders.ikdoeict.net/Courses/Detail/" + course.id;

                        allCourses.Add(apiCourse);
                    }
                    return allCourses;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return allCourses;
        }

        public static IEnumerable<AllCourses> GetCourses(string category)
        {
            List<Course> courses = new List<Course>();
            courses = Course.GetCourses(category);

            List<AllCourses> allCourses = new List<AllCourses>();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<Course>(s => s.Language);
                    options.LoadWith<Course>(s => s.Level);
                    options.LoadWith<Course>(s => s.Category);
                    db.LoadOptions = options;
                    foreach (var course in courses)
                    {
                        AllCourses apiCourse = new AllCourses();
                        apiCourse.Title = course.title;
                        apiCourse.Description = course.description;
                        apiCourse.Price = course.price;
                        apiCourse.Length = course.length;
                        apiCourse.Introduction = course.introduction;
                        apiCourse.Last_updated = course.last_updated;
                        apiCourse.Language = course.Language.name;
                        apiCourse.Level = course.Level.name;
                        apiCourse.Category = course.Category.name;
                        double rating = Math.Round(Models.Rating.GetRating(course.id), 2);
                        if (rating != 6) apiCourse.Rating = rating;
                        apiCourse.Link = "http://onlinelearning.sennavanlonders.ikdoeict.net/Courses/Detail/" + course.id;

                        allCourses.Add(apiCourse);
                    }
                    return allCourses;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return allCourses;
        }
    }
}