using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System;

namespace BPN
{
    class PersonNameGenerator
    {
        #region noun and adjective creation
        static List<string> male = new List<string>();
        static List<string> female = new List<string>();
        static List<string> lastname = new List<string>();
        #endregion

        private static Random random = new Random();

        public static string[] NextPersonName()
        {
            string[] toReturn = new string[2];

            string firstName;
            if (random.Next(1) == 0)
            {
                firstName = male[random.Next(0, male.Count - 1)];
            }
            else
            {
                firstName = female[random.Next(0, female.Count - 1)];
            }
            string lastName = lastname[random.Next(0, lastname.Count - 1)];

            toReturn[0] = firstName;
            toReturn[1] = lastName;

            return toReturn;

        }

        public static void Initialize()
        {
            // males
            string f = @"Data/PersonNames/male.txt";
            using (StreamReader r = new StreamReader(f))
            {
                // Use while != null pattern for loop
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    male.Add(line);
                }
            }

            // females
            f = @"Data/PersonNames/female.txt";
            using (StreamReader r = new StreamReader(f))
            {
                // Use while != null pattern for loop
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    female.Add(line);
                }
            }

            // last names
            f = @"Data/PersonNames/lastname.txt";
            using (StreamReader r = new StreamReader(f))
            {
                // Use while != null pattern for loop
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    lastname.Add(line);
                }
            }

        }
    }
}
