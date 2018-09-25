using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Models
{
    public partial class Level
    {
        public static List<Level> GetLevels()
        {
            List<Level> levels = new List<Level>();
            try
            {
                using (var db = new OnlineLearningDataContext())
                {
                    levels = db.Levels.ToList();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return levels;
        }
    }
}