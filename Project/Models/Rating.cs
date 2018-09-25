using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Models
{
    [MetadataType(typeof(RatingValidationModel))]
    public partial class Rating
    {
        public static void AddRating(Rating rating)
        {
            Rating r = new Rating();

            r.score = rating.score;
            r.user_id = rating.user_id;
            r.course_id = rating.course_id;

            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    db.Ratings.InsertOnSubmit(r);
                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static double GetRating(int course_id)
        {
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    var ratings = db.Ratings.Where(m => m.course_id == course_id);
                    if(ratings.Count() == 0)
                    {
                        return 6;
                    }
                    double average = 0;
                    double counter = 0;
                    foreach (var rating in ratings)
                    {
                        average += rating.score;
                        counter++;
                    }
                    average = average / counter;
                    return average;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return 6;
        }

        public static bool AlreadyVoted(int user_id, int course_id)
        {
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    Rating rating = db.Ratings.Where(u => u.user_id == user_id && u.course_id == course_id).FirstOrDefault();
                    if (rating == null)
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
    }
    public class RatingValidationModel
    {
        [RegularExpression(@"^[0-5]{1}$",
        ErrorMessage = "Only numbers between 0 and 5 (including 5) allowed.")]
        [Required(ErrorMessage = "This field is required")]
        public int score;

        //[RegularExpression(@"^[a-zA-Z,.:; ]+$",
        //ErrorMessage = "Only alphabets, spaces and punctuation marks allowed")]
        //public string description;
    }
}