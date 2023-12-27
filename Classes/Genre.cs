using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Kylosov.Classes
{
    public class Genre
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public Genre(int Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }

        public static List<Genre> AllGenres()
        {
            List<Genre> allGenres = new List<Genre>();
            allGenres.Add(new Genre(1, "Современная русская литература"));
            allGenres.Add(new Genre(2, "Современные детективы"));
            allGenres.Add(new Genre(3, "Любовные фэнтези"));
            allGenres.Add(new Genre(4, "Попаданцы"));
            allGenres.Add(new Genre(5, "Юмористическое фэнтези"));
            return allGenres;
        }
    }
}
