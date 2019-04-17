using System;
using System.Collections.Generic;
using System.Text;

namespace DeicingApp
{
    public class City
    {
        private static City c;
        public static string name;

        public string Name { get; set; }

        private City()
        {
            Name = "";
        }

        public static City GetMyCity()
        {
            if (c == null)
            {
                c = new City();
            }
            return c;
        }
    }
}
