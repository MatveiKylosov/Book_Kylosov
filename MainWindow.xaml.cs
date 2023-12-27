using Book_Kylosov.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Book_Kylosov
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
#if true
        List<Classes.Author> AllAuthors = new List<Classes.Author>();
        List<Classes.Genre> AllGenres = new List<Classes.Genre>();
        List<Classes.Book> AllBooks = new List<Classes.Book>();
#else
        List<Classes.Author> AllAuthors = Classes.Author.AllAuthors();
        List<Classes.Genre> AllGenres = Classes.Genre.AllGenres();
        List<Classes.Book> AllBooks = Classes.Book.AllBook();
#endif

        public MainWindow()
        {
            InitializeComponent();

            AddAuthors();
            AddGenres();
            AddYears();

            CreateUI(AllBooks);
        }

        public void AddAuthors() {
            cbAuthors.Items.Add("Выберите...");

            foreach (Classes.Author author in AllAuthors)
                cbAuthors.Items.Add(author.FIO);
        }

        public void AddGenres()
        {
            cbGenres.Items.Add("Выберите...");

            foreach (Classes.Genre genre in AllGenres)
                cbGenres.Items.Add(genre.Name);
        }

        public void AddYears()
        {
            cbYear.Items.Add("Выберите...");

            List<int> AllYears = new List<int>();

            foreach(Classes.Book book in AllBooks)
                if(AllYears.Find(x => x == book.Year) == 0)
                {
                    AllYears.Add(book.Year);
                    cbYear.Items.Add(book.Year);
                }
        }

        public void CreateUI(List<Classes.Book> AllBooks)
        {
            parent.Children.Clear();

            foreach (Classes.Book book in AllBooks)
                parent.Children.Add(new Elements.Element(book));
        }

        private void Search_Book(object sender, KeyEventArgs e) => Search();

        public void Search()
        {
            List<Classes.Book> FindBook = AllBooks.FindAll(x => x.Name.ToLower().Contains(tbSearch.Text.ToLower()));

            if(cbAuthors.SelectedIndex > 0)
            {
                Classes.Author SelectAuthor = AllAuthors.Find(x => x.FIO == cbAuthors.SelectedItem.ToString());

                FindBook = FindBook.FindAll(x => x.Authors.Find(y => y.Id == SelectAuthor.Id) != null);
            }

            if(cbGenres.SelectedIndex > 0)
            {
                Classes.Genre SelectGender = AllGenres.Find(x => x.Name == cbGenres.SelectedItem.ToString());

                FindBook = FindBook.FindAll(x => x.Genres.Find(y => y.Id == SelectGender.Id) != null);
            }

            if (cbYear.SelectedIndex > 0)
                FindBook = FindBook.FindAll(x => x.Year == Convert.ToInt32(cbYear.SelectedItem.ToString()));

            CreateUI(FindBook);
        }

        private void SelectAuthor(object sender, SelectionChangedEventArgs e) => Search();

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Book| *.book";

            if (saveFileDialog.ShowDialog() == true)
            {
                string fio = $"";
                string genre = $"";
                string book = $"";

                for (int i = 0; i < AllAuthors.Count; i++)
                    fio += $"{AllAuthors[i].FIO}{(AllAuthors.Count - 1 == i ? "" : "|")}";

                for (int i = 0; i < AllGenres.Count; i++)
                    genre += $"{AllGenres[i].Name}{(AllGenres.Count - 1 == i ? "" : "|")}";

                for (int i = 0; i < AllBooks.Count; i++)
                {
                    string aut = "";
                    string genr = "";

                    for(int f = 0; f < AllBooks[i].Authors.Count; f++)
                        aut += $"{AllBooks[i].Authors[f].FIO}{(f == AllBooks[i].Authors.Count - 1 ? "" : ",")}";

                    for (int f = 0; f < AllBooks[i].Genres.Count; f++)
                        genr += $"{AllBooks[i].Genres[f].Name}{(f == AllBooks[i].Genres.Count- 1 ? "" : ",")}";

                    book += $"{AllBooks[i].Name};{aut};{genr};{AllBooks[i].Year}{(AllBooks.Count - 1 == i ? "" : "|")}";
                }

                var fileStream = saveFileDialog.FileName;
                StreamWriter streamWriter = new StreamWriter(fileStream);

                streamWriter.WriteLine(fio);
                streamWriter.WriteLine(genre);
                streamWriter.WriteLine(book);

                streamWriter.Close();
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Book| *.book";

            if (openFileDialog.ShowDialog() == true)
            {
                string booksStr;

                using (StreamReader readde = new StreamReader(openFileDialog.FileName))
                    booksStr = readde.ReadToEnd();

                string[] strings = booksStr.Split('\n');


                foreach (string x in strings[0].Split('|'))
                    if (AllAuthors.Find(f => f.FIO == x) == null)
                    {
                        AllAuthors.Add(new Classes.Author(AllAuthors.Count() + 1, x));
                    }


                foreach (string x in strings[1].Split('|'))
                    if (AllGenres.Find(f => f.Name == x) == null)
                        AllGenres.Add(new Classes.Genre(AllGenres.Count() + 1, x));

                foreach (string x in strings[2].Split('|'))
                {
                    if (AllBooks.Find(f => f.Name == x.Split(';')[0]) == null)
                    {

                        List<Author> aut = new List<Author>();
                        List<Classes.Genre> genres = new List<Classes.Genre>();
                    
                        string[] fio = x.Split(';')[1].Split(',');
                        string[] genre = x.Split(';')[2].Split(',');

                        foreach (string f in fio)
                        {
                            Classes.Author a = AllAuthors.Find(item => item.FIO.Contains(f));
                            if (a != null)
                                aut.Add(a);
                        }

                        for (int i = 0; i < genre.Length; i++)
                        {
                            Classes.Genre ge = AllGenres.Find(item => item.Name.Contains(genre[i]));
                            if (ge != null)
                                genres.Add(ge);
                        }

                        AllBooks.Add(new Book(AllBooks.Count, x.Split(';')[0], genres, aut, 1337));
                    }
                }

                CreateUI(AllBooks);
            }
        }
    }
}
