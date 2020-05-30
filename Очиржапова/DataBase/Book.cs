using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    class Book
    {
        public Book(ushort bookID, string title, string author, string genre, string language, ushort year)
        {
            if ((title == "") || (author == "") || (genre == "") || (language == ""))
            {
                throw new Exception("Заполните все поля");
            }
            Title = title;
            Author = author;
            Genre = genre;
            Language = language;
            if ((year < 1457)||(year> (DateTime.Now.Year) + 10))
            {
                throw new Exception("Год издания должен быть не раньше 1457 и не позднее " + (DateTime.Now.Year) + 10);
            }
            Year = year;
            BookID = bookID; 
        }

        public string Title { get; set; } //название книги
        public string Author { get; set; } //имя автора
        public string Genre { get; set; } //жанр книги
        public string Language { get; set; } //язык книги
        public ushort Year { get; set; } //год публикации книги
        public ushort BookID { get; set; } //ID книги

        public override string ToString()
        {
            return Title + "|" + Author + "|" + Genre + "|" + Language + "|" + Year + "|";
        }
    }
}
