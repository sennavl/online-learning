using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Models
{
    [MetadataType(typeof(SubsectionValidationModel))]
    public partial class Subsection
    {
        public static List<Subsection> GetSubsectionsForSection(int sectionId)
        {
            List<Subsection> subsections = new List<Subsection>();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    subsections = db.Subsections.Where(s => s.section_id == sectionId).ToList();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return subsections;
        }

        public static Subsection GetSubsectionById(int id)
        {
            Subsection subsection = new Subsection();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    subsection = db.Subsections.Where(s => s.id == id).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return subsection;
        }

        public static Subsection AddSubsection(Subsection subsection)
        {
            Subsection s = new Subsection();

            s.title = subsection.title;
            s.section_id = subsection.section_id;
            s.subsection_content = subsection.subsection_content;

            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    db.Subsections.InsertOnSubmit(s);
                    db.SubmitChanges();
                    return db.Subsections.FirstOrDefault(m => m.id == s.id);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public static Subsection EditSubsection(Subsection subsection)
        {
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    Subsection originalSubsection = db.Subsections.FirstOrDefault(m => m.id.Equals(subsection.id));

                    originalSubsection.title = subsection.title;
                    originalSubsection.section_id = subsection.section_id;
                    originalSubsection.subsection_content = subsection.subsection_content;

                    db.SubmitChanges();
                    return db.Subsections.FirstOrDefault(m => m.id == originalSubsection.id);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public static void DeleteSubsection(int subsection_id)
        {
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    Subsection delete_subsection = db.Subsections.Where(m => m.id == subsection_id).FirstOrDefault();
                    db.Subsections.DeleteOnSubmit(delete_subsection);
                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
    public class SubsectionValidationModel
    {
        [RegularExpression(@"^[a-zA-Z\u00C0-\u017F0-9:,()& ]+$",
        ErrorMessage = "Only alphabets (including accented characters), numbers, spaces, parenthesis, comma, & and double point allowed")]
        [Required(ErrorMessage = "This field is required")]
        public string title;

        [AllowHtml]
        [Required(ErrorMessage = "This field is required")]
        public string subsection_content { get; set; }
    }
}