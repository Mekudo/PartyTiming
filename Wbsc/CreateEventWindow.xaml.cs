using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Wbsc
{

    public partial class CreateEventWindow : Window
    {
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public DateTime SelectedDate { get; set; }
        public int IdAcount { get; set; }

        public CreateEventWindow()
        {
            InitializeComponent();
            SelectedDate = DateTime.Today; // Устанавливаем начальную дату на сегодняшний день
            DataContext = this;
            this.Width = 300;
            this.Height = 350;

        }


        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (DBConnection.ConnectionDB())
            {
                string checkQuery = $"SELECT COUNT(*) FROM Event WHERE EventDate = '{SelectedDate:yyyy-MM-dd}' AND id_account = {IdAcount}";

                // SQL-запрос для вставки данных пользователя в таблицу Users
                try
                {
                    DBConnection.msCommand.CommandText = checkQuery;
                    int count = Convert.ToInt32(DBConnection.msCommand.ExecuteScalar());

                    if (count > 0)
                    {
                        // Запись существует, выполнить операцию обновления
                        string updateQuery = $@"UPDATE Event SET eventName = '{EventName}', eventDescription = '{EventDescription}' WHERE EventDate = '{SelectedDate:yyyy-MM-dd}'";

                        DBConnection.msCommand.CommandText = updateQuery;
                        DBConnection.msCommand.ExecuteNonQuery();

                        MessageBox.Show("Информация успешно обновлена!");
                    }
                    else
                    {
                        string query = $"INSERT INTO Event (id_account, eventName, eventDescription, eventDate) VALUES ('{IdAcount}', '{EventName}', '{EventDescription}', '{SelectedDate:yyyy-MM-dd}')";

                        // Выполнение SQL-запроса
                        DBConnection.msCommand.CommandText = query;
                        DBConnection.msCommand.ExecuteNonQuery();

                        MessageBox.Show("Мероприятие создано");

                        program.newMainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при создании мероприятия: " + ex.Message);
                }
                finally
                {
                    // Закрытие соединения с базой данных
                    program.newMainWindow.eventLoad(this, null);
                    DBConnection.CloseDb();
                }
            }
            this.Close();

        }





    }
}
