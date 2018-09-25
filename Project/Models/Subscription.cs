using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public partial class Subscription
    {
        public static void AddSubscription(int userId, int courseId, bool owner)
        {
            Subscription sub = new Subscription();

            sub.course_id = courseId;
            sub.time_registered = DateTime.Now;
            sub.user_id = userId;
            sub.owner = owner;
            sub.visible = true;

            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    db.Subscriptions.InsertOnSubmit(sub);
                    db.SubmitChanges();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }            
        }

        public static void ToggleSubscription(int userId, int courseId, bool visible)
        {
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    Subscription subscription = db.Subscriptions.FirstOrDefault(m => m.user_id == userId && m.course_id == courseId);
                    subscription.visible = visible;
                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static bool ExistsForUser(int userId, int courseId)
        {
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    Subscription subscription = db.Subscriptions.Where(u => u.user_id == userId && u.course_id == courseId).FirstOrDefault();
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

        public static bool IsVisible(int userId, int courseId)
        {
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    Subscription subscription = db.Subscriptions.Where(u => u.user_id == userId && u.course_id == courseId).FirstOrDefault();
                    return subscription.visible;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return true;
        }

        public static int GetOwnerId(int courseId)
        {
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    int owner_id = db.Subscriptions.Where(c => c.course_id == courseId && c.owner == true).FirstOrDefault().user_id;
                    return owner_id;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return 0;
        }

        public static List<int> GetTopThreePopularCoursesIds()
        {
            try
            {
                List<int> course_ids = new List<int>();
                using (var db = new OnlineLearningDataContext())
                {
                    var subscriptions = db.Subscriptions.GroupBy(m => m.course_id).OrderByDescending(m => m.Count());
                    IEnumerable<Subscription> subs = new List<Subscription>();
                    subs = subscriptions.SelectMany(m => m);
                    foreach (var subscription in subs)
                    {
                        course_ids.Add(subscription.course_id);
                    }
                    return course_ids;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }
    }
}