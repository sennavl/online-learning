using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Models
{
    [MetadataType(typeof(LanguageModelValidation))]
    public partial class Language
    {
        public static List<Language> GetLanguages()
        {
            List<Language> languages = new List<Language>();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    languages = db.Languages.ToList();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return languages;
        }
    }

    public class LanguageModelValidation
    {
        
        public int? id;
    }
}