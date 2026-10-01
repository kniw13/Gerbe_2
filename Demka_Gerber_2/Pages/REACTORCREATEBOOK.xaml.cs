using Demka_Gerber_2.Classes;
using Demka_Gerber_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Demka_Gerber_2.Pages
{
    public partial class REACTORCREATEBOOK : Page
    {
        public static BookDisplay _currentBook;
        public static Gerber_Entitites db = Helper.Help.GetContext();

        public REACTORCREATEBOOK(BookDisplay currentBook)
        {
            InitializeComponent();
            _currentBook = currentBook;

            lb_Name.Content = _currentBook != null
                ? "Редактирование книги"
                : "Создание книги";

            Load_Boxes();
            Load_Data();
        }

        // Загружаем в ComboBox сами сущности, а не строки
        public void Load_Boxes()
        {
            cmb_autor.ItemsSource = db.Authors.ToList();
            cmb_genre.ItemsSource = db.Genre.ToList();
            cmb_status.ItemsSource = db.Book_Statuses.ToList();

            // Подсказываем WPF, какие свойства показывать
            cmb_autor.DisplayMemberPath = "FullName";
            cmb_genre.DisplayMemberPath = "Genre_Name";
            cmb_status.DisplayMemberPath = "Status_Name";
        }

        public void Load_Data()
        {
            if (_currentBook == null) return;

            txt_name.Text = _currentBook.BookName;
            txt_Date.Text = _currentBook.Date.ToString();

            cmb_autor.SelectedItem = cmb_autor.Items
                .Cast<Authors>()
                .FirstOrDefault(a => a.FullName == _currentBook.AuthorName);

            cmb_genre.SelectedItem = cmb_genre.Items
                .Cast<Genre>()
                .FirstOrDefault(g => g.Genre_Name == _currentBook.Genre);

            cmb_status.SelectedItem = cmb_status.Items
                .Cast<Book_Statuses>()
                .FirstOrDefault(s => s.Status_Name == _currentBook.StatusBook);
        }

        private void btn_Nazad_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBook == null) return;

            var existedBook = db.Books.FirstOrDefault(ex => ex.Book_ID == _currentBook.BookID);
            if (existedBook == null)
            {
                MessageBox.Show("Книга не найдена.");
                return;
            }

            db.Books.Remove(existedBook);
            db.SaveChanges();
            NavigationService.GoBack();
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txt_Date.Text, out int date))
            {
                MessageBox.Show("Введите корректную дату", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var autor = cmb_autor.SelectedItem as Authors;
            var genre = cmb_genre.SelectedItem as Genre;
            var status = cmb_status.SelectedItem as Book_Statuses;

            if (autor == null || genre == null || status == null)
            {
                MessageBox.Show("Выберите автора, жанр и статус.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_name.Text))
            {
                MessageBox.Show("Введите название книги.");
                return;
            }

            if (_currentBook != null)
            {
                var existedBook = db.Books.FirstOrDefault(ex => ex.Book_ID == _currentBook.BookID);
                if (existedBook == null)
                {
                    MessageBox.Show("Редактируемая книга не найдена в БД.");
                    return;
                }

                existedBook.Book_Name = txt_name.Text;
                existedBook.Date_Of_Writing = date;
                existedBook.Author_ID = autor.Author_ID;
                existedBook.GenreID = genre.Genre_ID;
                existedBook.Book_StatusID = status.Status_ID;
            }
            else
            {
                var newBook = new Books
                {
                    Book_ID=GetNextBookID(),
                    Book_Name = txt_name.Text,
                    Date_Of_Writing = date,
                    Author_ID = autor.Author_ID,
                    GenreID = genre.Genre_ID,
                    Book_StatusID = status.Status_ID
                };
                db.Books.Add(newBook);
            }

            db.SaveChanges();
            NavigationService.GoBack();
        }
        public int GetNextBookID()
        {
            var maxid = db.Books.Any() ? db.Books.Max(m=>m.Book_ID) : 0;
            return maxid + 1;
        }
    }
}