using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Project.Controllers
{
    public class ApiCoursesController : ApiController
    {
        [HttpGet]
        [Route("api/ApiCourses")]
        public IEnumerable<AllCourses> GetCourses()
        {
            try
            {
                return AllCourses.GetCourses();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        [HttpGet]
        [Route("api/ApiCourses/Category/{category}")]
        public IEnumerable<AllCourses> GetCoursesByCategory([FromUri] string category)
        {
            try
            {
                return AllCourses.GetCourses(category);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}