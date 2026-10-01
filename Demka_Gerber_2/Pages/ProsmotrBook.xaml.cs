using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Demka_Gerber_2.Classes;
using Demka_Gerber_2.Models;

namespace Demka_Gerber_2.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProsmotrBook.xaml
    /// </summary>
    public partial class ProsmotrBook : Page
    {
        public static Gerber_Entitites db = Helper.Help.GetContext();
        public ProsmotrBook()
        {
            InitializeComponent();
            Load_Data();
            Load_Boxes();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Load_Boxes();
            Load_Data();
        }
        public void Apply_Search()
        {
            var query = db.Books
                .Include("Genre")
                .Include("Authors")
                .Include("Book_Statuses")
                .ToList()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(txt_name.Text))
            {
                string search = txt_name.Text.Trim().ToLower();
                query = query.Where(q => q.Book_Name != null &&
                                         q.Book_Name.ToLower().Contains(search));
            }

            
            if (cmb_Authors.SelectedItem is string selectedAuthor
                && !string.IsNullOrWhiteSpace(selectedAuthor)
                && selectedAuthor != "Все авторы")           
            {
                string target = selectedAuthor.Trim();
                query = query.Where(q =>
                    ((q.Authors.FirstName ?? "") + " " +
                     (q.Authors.LastName ?? "") + " " +
                     (q.Authors.Patronimyc ?? "")).Trim() == target);
            }


            if (cmb_Genre.SelectedItem is string selectedGenre
                && !string.IsNullOrWhiteSpace(selectedGenre)
                && selectedGenre != "Все жанры")            
            {
                query = query.Where(q => q.Genre.Genre_Name == selectedGenre);
            }

            var result = query.ToList().Select(l => new BookDisplay
            {
                BookID=l.Book_ID,
                BookName = l.Book_Name,
                AuthorName = $"{l.Authors.FirstName} {l.Authors.LastName} {l.Authors.Patronimyc}".Trim(),
                Genre = l.Genre?.Genre_Name,
                StatusBook = l.Book_Statuses?.Status_Name,
                Date = (int)l.Date_Of_Writing
            }).ToList();

            lv_Books.ItemsSource = result;
        }
        public void Load_Data()
            
        {
            lv_Books.ItemsSource = null;
            var books = db.Books.Include("Genre").Include("Authors").Include("Book_Statuses").ToList();

            var result = books.Select(r => new BookDisplay
            {
                BookID = r.Book_ID,
                BookName = r.Book_Name,

                
                AuthorName = r.Authors != null
                    ? $"{r.Authors.FirstName} {r.Authors.LastName} {r.Authors.Patronimyc?.Trim()}".Trim()
                    : "Автор не указан",

             
                Genre = r.Genre?.Genre_Name ?? "Жанр не указан",

               
                StatusBook = r.Book_Statuses?.Status_Name ?? "Статус не указан",

                
                Date = r.Date_Of_Writing != null ? (int)r.Date_Of_Writing : 0,
            }).ToList();

            lv_Books.ItemsSource = result;
        }
        public void Load_Boxes()
        {
            cmb_Authors.Items.Clear();
            cmb_Authors.Items.Add("Все авторы");
            foreach (var a in db.Authors.ToList())
            {
                cmb_Authors.Items.Add(
                    $"{a.FirstName} {a.LastName} {a.Patronimyc?.Trim()}".Trim());
            }
            cmb_Authors.SelectedIndex = 0;

            cmb_Genre.Items.Clear();
            cmb_Genre.Items.Add("Все жанры");
            foreach (var g in db.Genre.ToList())
            {
                cmb_Genre.Items.Add(g.Genre_Name);
            }
            cmb_Genre.SelectedIndex = 0;
        }

        private void txt_name_TextChanged(object sender, TextChangedEventArgs e)
        {
            Apply_Search();
        }

        private void cmb_Authors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Apply_Search();
        }

        private void cmb_Genre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Apply_Search();
        }

        private void lv_Books_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedBook = lv_Books.SelectedItem as BookDisplay;
            NavigationService.Navigate(new REACTORCREATEBOOK(selectedBook));
        }

        private void btn_Dobavit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new REACTORCREATEBOOK(null));
        }
    }
}
