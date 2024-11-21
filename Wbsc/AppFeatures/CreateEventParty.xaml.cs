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
using System.Windows.Shapes;

namespace Wbsc
{

    public partial class CreateEventParty : Window
    {
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public DateTime SelectedDate { get; set; }
        public int IdAcount { get; set; }

        public CreateEventParty()
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
                try
                {
                    string getNamePartyQuery = $"SELECT partyName FROM account WHERE id_account = '{IdAcount}'";
                    DBConnection.msCommand.CommandText = getNamePartyQuery;
                    string nameparty = DBConnection.msCommand.ExecuteScalar()?.ToString();
                    // Проверка существования записи
                    string checkQuery = $"SELECT COUNT(*) FROM partyevent WHERE eventDate = '{SelectedDate:yyyy-MM-dd}' AND id_account = {nameparty}";
                    DBConnection.msCommand.CommandText = checkQuery;
                    int count = Convert.ToInt32(DBConnection.msCommand.ExecuteScalar());

                    if (count > 0)
                    {
                        // Запись существует, выполнить операцию обновления
                        string updateQuery = $@"UPDATE partyevent SET eventName = '{EventName}', eventDescription = '{EventDescription}' WHERE eventDate = '{SelectedDate:yyyy-MM-dd}'";
                        DBConnection.msCommand.CommandText = updateQuery;
                        DBConnection.msCommand.ExecuteNonQuery();
                        MessageBox.Show("Информация успешно обновлена!");
                    }
                    else
                    {
                        // Получение nameparty из таблицы account


                        if (!string.IsNullOrEmpty(nameparty))
                        {
                            string insertQuery = $"INSERT INTO partyevent (id_account, eventName, eventDescription, eventDate) VALUES ('{nameparty}', '{EventName}', '{EventDescription}', '{SelectedDate:yyyy-MM-dd}')";
                            DBConnection.msCommand.CommandText = insertQuery;
                            DBConnection.msCommand.ExecuteNonQuery();
                            MessageBox.Show("Мероприятие создано");

                            program.newPartyWindow = Application.Current.Windows.OfType<partyWindow>().FirstOrDefault();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось получить данные из бд.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при создании мероприятия: " + ex.Message);
                }
                finally
                {
                    program.newPartyWindow.eventLoad(this, null);
                    DBConnection.CloseDb();
                }
            }
            this.Close();

            // Закройте окно

        }
    }
}
