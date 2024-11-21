using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
  
    public partial class sendParty : Window
    {
        public int IdAcount { get; set; }

        public sendParty()
        {
            InitializeComponent();
            DataContext = SharedViewModel.Instance;

            ThemeSettings.OnBackgroundColorChanged += ThemeSettings_OnBackgroundColorChanged;

            Loaded += (sender, e) =>
            {
                // Установить фон при загрузке окна
                this.Background = SharedViewModel.Instance.ThemeSettings.BackgroundColor;
            };

            // Установить фон при каждом открытии окна
            this.Activated += (sender, e) =>
            {
                SharedViewModel.Instance.ThemeSettings.BackgroundColor = SharedViewModel.Instance.ThemeSettings.SavedBackgroundColor;
            };
        }
        private void ThemeSettings_OnBackgroundColorChanged(object sender, EventArgs e)
        {
            // Обновить фон окна
            this.Background = SharedViewModel.Instance.ThemeSettings.BackgroundColor;
        }
        private void SendInvitationButton_Click(object sender, RoutedEventArgs e)
        {
            // Получение данных из текстового поля
            string receiverUsername = ReceiverTextBox.Text;

            // Подключение к базе данных
            if (DBConnection.ConnectionDB())
            {
                // SQL-запрос для проверки существования пользователя
                string userCheckQuery = $"SELECT COUNT(*) FROM account WHERE Login = '{receiverUsername}'";

                // SQL-запрос для обновления значения party у отправителя
                string updateSenderPartyQuery = $"UPDATE account SET party = 2,id_role = 2,  partyName = '{IdAcount}' WHERE id_account = '{IdAcount}'";

                string userId = $"SELECT id_account FROM account WHERE Login = '{receiverUsername}'";

                // SQL-запрос для обновления значения party у получателя
                string updateReceiverPartyQuery = $"UPDATE account SET party = 1,partyName = '{IdAcount}' WHERE Login = '{receiverUsername}'";

                try
                {
                    // Выполнение SQL-запроса для проверки существования пользователя
                    DBConnection.msCommand.CommandText = userCheckQuery;
                    int count = Convert.ToInt32(DBConnection.msCommand.ExecuteScalar());

                    if (count > 0)
                    {

                        DBConnection.msCommand.CommandText = userId;
                        int check = Convert.ToInt32(DBConnection.msCommand.ExecuteScalar());
                        // Выполнение SQL-запроса для обновления значения party у отправителя
                        if(check != IdAcount)
                        {
                            DBConnection.msCommand.CommandText = updateSenderPartyQuery;
                            DBConnection.msCommand.ExecuteNonQuery();

                            // Выполнение SQL-запроса для обновления значения party у получателя
                            DBConnection.msCommand.CommandText = updateReceiverPartyQuery;
                            DBConnection.msCommand.ExecuteNonQuery();

                            MessageBox.Show($"Приглашение для {receiverUsername} отправлено успешно!");
                            program.newMainWindow.Close();
                            program.newPartyWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show($"Это вы!");

                        }

                    }
                    else
                    {
                        // Пользователь не существует
                        MessageBox.Show("Пользователя не существует");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при отправке приглашения: " + ex.Message);
                }
                finally
                {
                    // Закрытие соединения с базой данных
                    DBConnection.CloseDb();
                }
            }
        }



    }
}