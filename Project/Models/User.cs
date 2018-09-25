using Project.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace Project.Models
{
    [MetadataType(typeof(UserValidationModel))]
    public partial class User
    {
        public List<Role> RoleList { get; set; }
        public static User GetUserByEmail(string email)
        {
            User user = new User();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    DataLoadOptions option = new DataLoadOptions();
                    option.LoadWith<User>(s => s.Role);
                    db.LoadOptions = option;
                    user = (from s in db.Users
                            where s.email == email
                            select s).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return user;
        }

        public static User GetUserById(int id)
        {
            User user = new User();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    DataLoadOptions option = new DataLoadOptions();
                    option.LoadWith<User>(s => s.Role);
                    db.LoadOptions = option;
                    user = (from s in db.Users
                            where s.id == id
                            select s).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return user;
        }

        public static void AddUser(User usr)
        {
            Models.User u = new User();

            u.first_name = usr.first_name;
            u.last_name = usr.last_name;
            u.email = usr.email;
            u.password = PasswordHelper.EncodePasswordMd5(usr.password);
            u.role_id = usr.role_id;

            using (var db = new OnlineLearningDataContext())
            {
                db.Users.InsertOnSubmit(u);
                db.SubmitChanges();
            }
        }

        public static List<Role> GetRoles()
        {
            List<Role> roles = new List<Role>();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<User>(s => s.Role);
                    db.LoadOptions = options;

                    roles = db.Roles.ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return roles;
        }

        public bool IsOwner(int course_id)
        {
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    Subscription subscription = db.Subscriptions.Where(u => u.user_id == this.id && u.course_id == course_id && u.owner == true).FirstOrDefault();
                    if (subscription == null)
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return true;
        }

        public bool IsOwner(Subsection subsection)
        {
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    Section section = Section.GetSectionById(subsection.section_id);
                    Course course = Course.GetCourse(section.course_id);
                    Subscription subscription = db.Subscriptions.Where(u => u.user_id == this.id && u.course_id == course.id && u.owner == true).FirstOrDefault();
                    if (subscription == null)
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return true;
        }

        public bool IsSubscribed(int course_id)
        {
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    Subscription subscription = db.Subscriptions.Where(u => u.user_id == this.id && u.course_id == course_id && u.owner == false).FirstOrDefault();
                    if (subscription == null)
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return true;
        }

        public class UserValidationModel
        {
            [Required(ErrorMessage = "This field is required")]
            [RegularExpression(@"^[a-zA-Z ]+$",
            ErrorMessage = "Only alphabets and spaces allowed")]
            public string first_name { get; set; }
            [Required(ErrorMessage = "This field is required")]
            [RegularExpression(@"^[a-zA-Z ]+$",
            ErrorMessage = "Only alphabets and spaces allowed")]
            public string last_name { get; set; }
            [Required(ErrorMessage = "This field is required")]
            [RegularExpression(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                + "@"
                                + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$",
            ErrorMessage = "Only valid email allowed")]
            public string email { get; set; }
            [Required(ErrorMessage = "This field is required")]
            public string password { get; set; }

            [RegularExpression(@"^[1-9]+$",
            ErrorMessage = "This field is required")]
            [Required(ErrorMessage = "This field is required")]
            public int role_id { get; set; }
        }
    }
}