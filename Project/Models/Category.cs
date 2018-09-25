using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public partial class Category
    {
        public static List<Category> GetCategories()
        {
            List<Category> categories = new List<Category>();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    categories = db.Categories.ToList();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return categories;
        }

        public static int GetCategoryId(string categoryName)
        {
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    return db.Categories.Where(m => m.name == categoryName).FirstOrDefault().id;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
    }
}