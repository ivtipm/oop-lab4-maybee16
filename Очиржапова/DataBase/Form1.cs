using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataBase
{
    public partial class Form1 : Form
    {
        Methods data = new Methods();
        public string oldValue = "";
        public string filename = "";
        public string BookTitle;
        public string BookGenre;
        public string BookAuthor;
        public string BookLanguage;
        public string BookYear;
        public string BookID;
        public Form1()
        {
            InitializeComponent();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            openFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            saveFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            InitializeTimers();

        }
        private void InitializeTimers()
        {
            // Таймер для автосохранения данных в файл раз в 1 минуту
            timer1.Interval = 60000;
            timer1.Tick += new EventHandler(timer1_Tick);

            // Таймер для отображения надписи, свидетельствующей о сохранении файла
            // длительностью 3 сек
            timer2.Interval = 3000;
            timer2.Tick += new EventHandler(timer2_Tick);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            data.SaveToFile(filename);
            labelAutoSave.Visible = true;
            timer2.Enabled = true;
            timer2.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (labelAutoSave.Visible)
            {
                labelAutoSave.Visible = false;
            }
            else
            {
                labelSave.Visible = false;
            }
            timer2.Enabled = false;
            timer2.Stop();
        }
        //Создаёт порядковый ID
        public ushort generateID()
        {
            try
            {
                Book book = (Book)data.BookFile[data.BookFile.Count - 1];
                ushort NewID = book.BookID;
                NewID++;
                return NewID;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }

        private void добавитьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                panelAdd.Visible = true;
                panelSearch.Visible = false;
                TextBoxsClear();
                TextBoxsEnabled();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        //Запрет редактирования последней строки
        public void BanChangeColumn(int index)
        {
            dataGridView1.Rows[index].Cells[0].ReadOnly = true;
        }

        //Добавить новую запись в базу
        private void button_add_Click(object sender, EventArgs e)
        {
            try
            {
                BookID = textBox_ID.Text = generateID().ToString();
                BookTitle = textBox_Title.Text;
                BookGenre = textBox_Genre.Text;
                BookAuthor = textBox_Author.Text;
                BookYear = textBox_Year.Text;
                BookLanguage = textBox_Language.Text;

                Book book = new Book(
                    (ushort)Convert.ToInt32(textBox_ID.Text),
                    textBox_Title.Text,
                    textBox_Author.Text,
                    textBox_Genre.Text,
                    textBox_Language.Text,
                    (ushort)Convert.ToInt32(textBox_Year.Text));

                data.AddRow(book);
                int n = data.BookFile.Count;

                Book book1 = (Book)data.BookFile[n - 1];
                dataGridView1.Rows.Add
                    (book1.BookID,
                    textBox_Title.Text,
                    textBox_Author.Text,
                    textBox_Genre.Text,
                    textBox_Language.Text,
                    textBox_Year.Text
                    );

                BanChangeColumn(n - 1);

                TextBoxsClear();

                if ((!timer1.Enabled) && (filename != ""))
                {
                    timer1.Enabled = true;
                    timer1.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        //Режим поиска данных
        private void ПоискЗаписейToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                panelSearch.Visible = true;
                panelAdd.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        //Полностью очистить базу
        private void ОчиститьБазуДанныхToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (data.BookFile.Count != 0)
            {
                DialogResult dialogResult = MessageBox.Show
                    ("Уверены," +
                    "что хотите удалить все элементы?",
                    "Подтверждение", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    data.DeleteAllRows();
                    dataGridView1.Rows.Clear();
                }
            }
        }

        //Удаление отдельных записей в базе
        private void dataGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            try
            {
                int count = dataGridView1.Rows.Count;
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    int index = row.Index; // индекс выбранной строки
                    if (index == count - 1) return;

                    data.DeleteRow(index);
                    dataGridView1.Rows.RemoveAt(index);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        //Завершение редактирование ячейки
        private void dataGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int indRow = dataGridView1.Rows[e.RowIndex].Index;
                int indColumn = dataGridView1.Columns[e.ColumnIndex].Index;
                object value = dataGridView1.Rows[indRow].Cells[indColumn].Value;

                // Если значение не было введено, то оставляем старое
                if (value is null)
                {
                    MessageBox.Show("Вы не ввели значение.");
                    dataGridView1.Rows[indRow].Cells[indColumn].Value = oldValue;
                    return;
                }

                if (indColumn == 1)
                {
                    data.EditBookTitle((string)value, indRow);
                }

                else
                if (indColumn == 2)
                {
                    data.EditBookGenre((string)value, indRow);
                }

                else
                if (indColumn == 3)
                {
                    data.EditBookAuthor((string)value, indRow);
                }

                else
                if (indColumn == 4)
                {
                    try
                    {
                        data.EditBookYear((ushort)Convert.ToInt32(value), indRow);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show($"Ошибка: {exception.Message}");
                        dataGridView1.Rows[indRow].Cells[indColumn].Value = oldValue;
                    }
                }

                else
                if (indColumn == 6)
                {
                    data.EditBookLanguge((string)value, indRow);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        //Начало редактирования ячейки
        private void dataGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                oldValue = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        }

        //Сохранить базу в текстовый файл
        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (filename == "")
                {
                    if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                        return;
                    filename = saveFileDialog1.FileName;
                    this.Text = filename + " - База данных фильмов";
                }
                // Если сохранение произошло первый раз, то запускаем 
                // таймер для автосохранения
                if (!timer1.Enabled)
                {
                    timer1.Enabled = true;
                    timer1.Start();
                }
                // При каждом сохранении будет появляться надпись "Сохранение..."
                labelSave.Visible = true;
                timer2.Enabled = true;
                timer2.Start();

                data.SaveToFile(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        //Считать базу из файла
        private void Open_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }

                filename = openFileDialog1.FileName;
                this.Text = filename + " - База данных фильмов";
                dataGridView1.Rows.Clear();
                data.OpenFile(filename);
                WriteToDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        //"Создать новый файл"
        private void Create_Click_1(object sender, EventArgs e)
        {
            if ((data.BookFile.Count != 0) || (filename != ""))
            {
                DialogResult dialogResult = MessageBox.Show
                    ("Уверены," +
                    "что хотите создать новый файл?" + "\r\n" +
                    "Изменения в текущем не сохранятся!",
                    "Подтверждение", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    this.Text = "База данных фильмов";
                    filename = "";
                    data.DeleteAllRows();
                    dataGridView1.Rows.Clear();
                    timer1.Stop();
                    timer1.Enabled = false;
                    timer2.Stop();
                    timer2.Enabled = false;
                }
            }
        }

        //Найти данные в базе
        private void ButtonSearch_Click(object sender, EventArgs e)
        {
            if ((data.BookFile.Count == 0) || (textBoxSearch.Text == ""))
                return;

            dataGridView1.ClearSelection();
            List<int> foundElements = data.SearchRows(textBoxSearch.Text);

            if (foundElements[0] == -1)
            {
                MessageBox.Show("Ничего не удалось найти!");
                return;
            }

            dataGridView1.CurrentCell = dataGridView1[0, foundElements[0]];
            for (int i = 0; i < foundElements.Count; i++)
            {
                dataGridView1.Rows[foundElements[i]].Selected = true;
            }
        }
        //Записать базу в таблицу
        private void WriteToDataGrid()
        {
            try
            {
                for (int i = 0; i < data.BookFile.Count; i++)
                {
                    Book book = (Book)data.BookFile[i];
                    dataGridView1.Rows.Add(
                        book.BookID,
                        book.Title,
                        book.Author,
                        book.Genre,
                        book.Language,
                        book.Year);

                    BanChangeColumn(i);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        //Очистить все поля
        private void TextBoxsClear()
        {
            textBox_ID.Text = "";
            textBox_Title.Text = "";
            textBox_Author.Text = "";
            textBox_Genre.Text = "";
            textBox_Language.Text = "";
            textBox_Year.Text = "";
        }

        //Сортировать записи
        private void ПоУбываниюToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (data.BookFile.Count != 0)
            {
                dataGridView1.Rows.Clear();
                data.Sort(SortDirection.Descending);
                WriteToDataGrid();
            }
        }
        private void поВозрастаниюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (data.BookFile.Count != 0)
            {
                dataGridView1.Rows.Clear();
                data.Sort(SortDirection.Ascending);
                WriteToDataGrid();
            }
        }

        //"Справка"
        private void ОПрограммеToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            string Info = "База данных библиотеки" + "\n\n" +
                "Powered by Visual Studio 2017, C#" + "\n\n" +
                "2020 г.";
            MessageBox.Show(Info, "Справка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //Нажали на ячейку/строку
        private void dataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int indRow = dataGridView1.Rows[e.RowIndex].Index;
            Book _book = (Book)data.BookFile[indRow];

            CellClick_TextBoxInfo(_book);
        }

        //И данные из строки поместились в поля на форме
        private void CellClick_TextBoxInfo(Book book)
        {
            добавитьЗаписьToolStripMenuItem_Click(добавитьЗаписьToolStripMenuItem, null);

            textBox_ID.Text = book.BookID.ToString();
            textBox_Title.Text = book.Title;
            textBox_Genre.Text = book.Genre;
            textBox_Author.Text = book.Author;
            textBox_Year.Text = book.Year.ToString();
            textBox_Language.Text = book.Language;

            TextBoxsDisabled();
        }

        //"открыть доступ к редактированию полей"
        private void TextBoxsEnabled()
        {
            panelAdd.Enabled = true;
        }

        //"закрыть доступ к редактированию полей"
        private void TextBoxsDisabled()
        {
            panelAdd.Enabled = false;
        }
        //При закрытии формы сказать про несохранённые данные
        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            if ((data.BookFile.Count != 0) || (filename != ""))
            {
                DialogResult dialogResult = MessageBox.Show
                    ("Уверены," +
                    "что хотите выйти из программы?" + "\r\n" +
                    "Изменения ещё не сохранены!",
                    "Подтверждение", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    this.Text = "База данных библиотеки";
                    filename = "";
                    data.DeleteAllRows();
                    dataGridView1.Rows.Clear();
                    timer1.Stop();
                    timer1.Enabled = false;
                    timer2.Stop();
                    timer2.Enabled = false;
                }
            }
        }
    }
}
