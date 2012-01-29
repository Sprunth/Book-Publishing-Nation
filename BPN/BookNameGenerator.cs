using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System;

namespace BPN
{
    class BookNameGenerator
    {
        #region noun and adjective creation
        static List<string> noun = new List<string>();
        static List<string> adjective = new List<string>();
        #endregion

        private static Random random = new Random();

        public static string NextBookTitle()
        {
            int structureType = random.Next(1, 6);
            string toReturn;

            string noun1 = noun[random.Next(0, noun.Count - 1)];
            string noun2 = noun[random.Next(0, noun.Count - 1)];
            string adjective1 = adjective[random.Next(0, adjective.Count - 1)];

            switch (structureType)
            {
                case 1:
                    {
                        toReturn = adjective1 + " " + noun1;
                        break;
                    }
                case 2:
                    {
                        toReturn = "The " + adjective1 + " " + noun1;
                        break;
                    }
                case 3:
                    {
                        toReturn = "The " + noun1 + " of " + noun2;
                        break;
                    }
                case 4:
                    {
                        toReturn = "The " + noun1 + "'s " + noun2;
                        break;
                    }
                case 5:
                    {
                        toReturn = "The " + noun1 + " of the " + noun2;
                        break;
                    }
                case 6:
                    {
                        toReturn = noun1 + " in the " + noun1;
                        break;
                    }
                case 7:
                case 8:
                    {
                        toReturn = "Making " + noun1;
                        break;
                    }
                default:
                    { throw new Exception("Book Generator Failed!"); }
            }

            return toReturn;

        }

        public static void Initialize()
        {
            //load the nouns, adjectives, etc from file

            // singular nouns
            string f = @"Data/BookNames/nouns-single.txt";
            using (StreamReader r = new StreamReader(f))
            {
                // Use while != null pattern for loop
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    noun.Add(line);
                }
            }

            //adjectives
            f = @"Data/BookNames/adjectives.txt";
            using (StreamReader r = new StreamReader(f))
            {
                // Use while != null pattern for loop
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    adjective.Add(line);
                }
            }

        }
    }
}
