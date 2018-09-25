using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public partial class Role
    {
        public static int GetRoleId(string name)
        {
            Role role = new Role();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    role = (from s in db.Roles
                            where s.name == name
                            select s).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return role.id;
        }

        public static List<Role> GetRoles()
        {
            List<Role> roles = new List<Role>();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    roles = (from s in db.Roles
                            select s).ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return roles;
        }
    }
}