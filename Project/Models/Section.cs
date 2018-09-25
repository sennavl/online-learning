using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Models
{
    [MetadataType(typeof(SectionValidationModel))]
    public partial class Section
    {
        public static List<Section> GetSectionsForCourse(int courseId)
        {
            List<Section> sections = new List<Section>();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    sections = db.Sections.Where(s => s.course_id == courseId).ToList();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return sections;
        }

        public static Section GetSectionById(int id)
        {
            Section section = new Section();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    section = db.Sections.Where(s => s.id == id).FirstOrDefault();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return section;
        }

        public static void AddSection(Section section)
        {
            Section s = new Section();

            s.title = section.title;
            s.course_id = section.course_id;

            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    db.Sections.InsertOnSubmit(s);
                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void EditSection(Section section)
        {
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    Section originalSection = db.Sections.FirstOrDefault(m => m.id.Equals(section.id));

                    originalSection.title = section.title;
                    originalSection.course_id = section.course_id;

                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void DeleteSection(int section_id)
        {
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    Section delete_section = db.Sections.Where(m => m.id == section_id).FirstOrDefault();
                    db.Sections.DeleteOnSubmit(delete_section);
                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class SectionValidationModel
    {
        [RegularExpression(@"^[a-zA-Z\u00C0-\u017F0-9:,()& ]+$",
        ErrorMessage = "Only alphabets (including accented characters), numbers, spaces, parenthesis, comma, & and double point allowed")]
        [Required(ErrorMessage = "This field is required")]
        public string title;
    }
}