using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Models
{
    public class FilterModel
    {
        // Courses
        public IList<Course> Courses { get; set; }

        // Lists for dropdown
        public SelectList CategoryList { get; set; }
        public SelectList LevelList { get; set; }
        public SelectList LanguageList { get; set; }

        // Filter values
        [RegularExpression(@"^[0-9]+$",
        ErrorMessage = "Only numbers allowed")]
        public int? PriceFrom { get; set; }
        [RegularExpression(@"^[0-9]+$",
        ErrorMessage = "Only numbers allowed")]
        public int? PriceTo { get; set; }
        [RegularExpression(@"^[a-zA-Z ]+$",
        ErrorMessage = "Only alphabets and spaces allowed")]
        public string SearchTerm { get; set; }
        [RegularExpression(@"^[0-9]+$",
        ErrorMessage = "Only numbers allowed")]
        public int? LanguageId { get; set; }
        [RegularExpression(@"^[0-9]+$",
        ErrorMessage = "Only numbers allowed")]
        public int? CategoryId { get; set; }
        [RegularExpression(@"^[0-9]+$",
        ErrorMessage = "Only numbers allowed")]
        public int? LevelId { get; set; }

        // Users
        public Models.User User { get; set; }
        public UserLogin UserLogin { get; set; }

        // Course lists for homepage
        public IList<Course> TopThreeRecentCourses { get; set; }
        public IList<Course> TopThreePopularCourses { get; set; }

        // Course
        public Course Course { get; set; }

        // Section
        public Section Section { get; set; }

        // Subsection
        public Subsection Subsection { get; set; }

        // Review
        public Rating Rating { get; set; } 


        public List<string> Items { get; set; }

        public FilterModel()
        {            
            Items = new List<string>();
            this.Courses = Course.GetCourses();
            CategoryList = new SelectList(Category.GetCategories(), "id", "name");
            LevelList = new SelectList(Models.Level.GetLevels(), "id", "name");
            LanguageList = new SelectList(Language.GetLanguages(), "id", "name");
            this.User = new User();
        }

        public FilterModel(FilterModel fm)
        {
            //filter            
            this.Courses = Course.GetCourses(fm);
            CategoryList = new SelectList(Category.GetCategories(), "id", "name");
            LevelList = new SelectList(Models.Level.GetLevels(), "id", "name");
            LanguageList = new SelectList(Language.GetLanguages(), "id", "name");
            this.User = new User();
        }
    }
}