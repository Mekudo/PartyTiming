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

    public partial class sendPartyWindow : Window
    {
        public int IdAcount { get; set; }

        public sendPartyWindow()
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

                // SQL-запрос для извлечения значения eventname
                string selectEventNameQuery = $"SELECT partyname FROM account WHERE id_account = {IdAcount}";

                string userId = $"SELECT id_account FROM account WHERE Login = '{receiverUsername}'";

                // SQL-запрос для обновления значения party и передачи eventname у получателя
                string updateReceiverPartyQuery = $"UPDATE account SET party = 1, partyname = @partyName WHERE Login = '{receiverUsername}'";

                try
                {
                    // Выполнение SQL-запроса для проверки существования пользователя
                    DBConnection.msCommand.CommandText = userCheckQuery;
                    int count = Convert.ToInt32(DBConnection.msCommand.ExecuteScalar());

                    if (count > 0)
                    {
                        DBConnection.msCommand.CommandText = userId;
                        int check = Convert.ToInt32(DBConnection.msCommand.ExecuteScalar());
                        // Выполнение SQL-запроса для извлечения значения eventname
                        DBConnection.msCommand.CommandText = selectEventNameQuery;
                        object eventNameObject = DBConnection.msCommand.ExecuteScalar();
                        string eventName = (eventNameObject != null) ? eventNameObject.ToString() : string.Empty;
                        if (check != IdAcount) 
                        {
                            DBConnection.msCommand.CommandText = updateReceiverPartyQuery;
                            DBConnection.msCommand.Parameters.AddWithValue("@eventName", eventName);
                            DBConnection.msCommand.ExecuteNonQuery();

                            MessageBox.Show($"Приглашение для {receiverUsername} отправлено успешно!");
                            program.newPartyWindow.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show($"Это вы!");

                        }
                        // Выполнение SQL-запроса для обновления значения party и передачи eventname у получателя

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

