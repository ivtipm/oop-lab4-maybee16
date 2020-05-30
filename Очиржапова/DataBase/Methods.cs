using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;



namespace DataBase
{
    class Methods
    {
        public ArrayList BookFile { get; } = new ArrayList(); // массив из строк с данными о книгах
        public void AddRow(Book bookRow) // добавить новую строку в файл
        {
            BookFile.Add(bookRow);
        }
        public void DeleteRow(int number) => BookFile.RemoveAt(number); // удалить определённую строку из массива
        public void DeleteAllRows() => BookFile.Clear(); // удалить все строки из массива
        public void EditBookTitle(string title, int index) // изменить название книги
        {
            Book book = (Book)BookFile[index];
            book.Title = title;
        }
        public void EditBookAuthor(string author, int index) // изменить автора книги
        {
            Book book = (Book)BookFile[index];
            book.Author = author;
        }
        public void EditBookGenre(string genre, int index) // изменить жанр книги
        {
            Book book = (Book)BookFile[index];
            book.Genre = genre;
        }
        public void EditBookLanguge(string language, int index) // изменить язык книги
        {
            Book book = (Book)BookFile[index];
            book.Language = language;
        }
        public void EditBookYear(ushort year, int index) // изменить год издания книги
        {
            Book book = (Book)BookFile[index];
            if ((year < 1457) || (year > (DateTime.Now.Year) + 10))
            {
                throw new Exception("Год издания должен быть не раньше 1457 и не позднее " + (DateTime.Now.Year) + 10);
            }
            book.Year = year;
        }
        
        public void SaveToFile(string filename) // Сохранить массив в файл
        {
            using (StreamWriter sw = new StreamWriter(filename, false, System.Text.Encoding.Unicode))
            {
                foreach (Book s in BookFile)
                {
                    sw.WriteLine(s.ToString());
                }
            }
        }

        
        public void OpenFile(string filename) // Загрузить массив из файла
        {
            if (!File.Exists(filename))
            {
                throw new Exception("Файл не существует");
            }

            if (BookFile.Count != 0)
            {
                DeleteAllRows();
            }

            using (StreamReader sw = new StreamReader(filename))
            {
                while (!sw.EndOfStream)
                {
                    string str = sw.ReadLine();
                    string[] dataFromFile = str.Split(new String[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                    ushort id = (ushort)Convert.ToInt32(dataFromFile[0]);
                    string title = dataFromFile[1];
                    string author = dataFromFile[2];
                    string genre = dataFromFile[3];
                    string language = dataFromFile[4];
                    ushort year = (ushort)Convert.ToInt32(dataFromFile[5]);

                    Book filmRow = new Book(id, title, author, genre, language, year);
                    AddRow(filmRow);
                }
            }
        }

        // Поиск совпадений по базе, и получение ID всех найденных записей        
        // Возвращает -1, если совпадений не найдено
        public List<int> SearchRows(string query)
        {
            List<int> count = new List<int>();

            //Проверяются ID и год издания
            if (ushort.TryParse(query, out ushort num_query))
            {
                for (int i = 0; i < BookFile.Count; i++)
                {
                    Book book = (Book)BookFile[i];

                    if (book.BookID == num_query)
                    {
                        count.Add(i);
                        break; // Если нашли запись по уникальному ID, то закончить поиск
                    }
                    else
                    {
                        if (book.Year == num_query)
                        {
                            count.Add(i);
                        }
                    }
                }

                if (count.Count == 0)
                {
                    count.Add(-1);
                }
                return count;
            }

            // Поиск по текстовым полям записи
            query = query.ToLower(); // перевод в нижний регистр
            query = query.Replace(" ", "");

            for (int i = 0; i < BookFile.Count; i++)
            {
                Book book = (Book)BookFile[i];

                if (book.Title.ToLower().Replace(" ", "").Contains(query))
                {
                    count.Add(i);
                }

                else
                if (book.Author.ToLower().Replace(" ", "").Contains(query))
                {
                    count.Add(i);
                }

                else
                if (book.Genre.ToLower().Replace(" ", "").Contains(query))
                {
                    count.Add(i);
                }

                else
                if (book.Language.ToLower().Replace(" ", "").Contains(query))
                {
                    count.Add(i);
                }
            }

            if (count.Count == 0)
            {
                count.Add(-1);
            }
            return count;
        }

        // Сортировка по году издания
        public void Sort(SortDirection direction)
        {
            BookFile.Sort(new YearComparer(direction));
        }
    }

    // Сортировка по возрастанию/убыванию
    public enum SortDirection
    {
        Ascending, // возрастание
        Descending // убывание
    }

    public class YearComparer : IComparer
    {
        private SortDirection m_direction = SortDirection.Ascending;

        public YearComparer() : base() { }

        public YearComparer(SortDirection direction)
        {
            m_direction = direction;
        }

        int IComparer.Compare(object x, object y)
        {
            Book book1 = (Book)x;
            Book book2 = (Book)y;

            return (m_direction == SortDirection.Ascending) ?
                book1.Year.CompareTo(book2.Year) :
                book2.Year.CompareTo(book1.Year);
        }
    }
}

