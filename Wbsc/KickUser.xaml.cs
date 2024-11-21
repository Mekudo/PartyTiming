using System;
using System.Collections.Generic;
using System.Linq;
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
 
    public partial class KickUser : Window
    {
        public int IdAcount { get; set; }

        public KickUser()
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
        private void KickInvitationButton_Click(object sender, RoutedEventArgs e)
        {
            // Получение данных из текстового поля
            string receiverUsername = ReceiverTextBox.Text;

            // Подключение к базе данных
            if (DBConnection.ConnectionDB())
            {
                // SQL-запрос для проверки существования пользователя
                string userCheckQuery = $"SELECT COUNT(*) FROM account WHERE Login = '{receiverUsername}'";

                string userId = $"SELECT id_account FROM account WHERE Login = '{receiverUsername}'";

                // SQL-запрос для обновления значения party у получателя
                string updateReceiverPartyQuery = $"UPDATE account SET party = 0,partyName = 0 WHERE Login = '{receiverUsername}'";

                try
                {
                    // Проверяем, существуют ли строки, где id_account равен partyName
                    string checkOwnershipQuery = $"SELECT COUNT(*) FROM Account WHERE id_account = '{IdAcount}' AND partyName = id_account";
                    DBConnection.msCommand.CommandText = checkOwnershipQuery;
                    int check = Convert.ToInt32(DBConnection.msCommand.ExecuteScalar());

                    DBConnection.msCommand.CommandText = userId;
                    int chek = Convert.ToInt32(DBConnection.msCommand.ExecuteScalar());
                    if (chek != IdAcount)
                    {
                        if (check > 0)
                        {
                            // Выполнение SQL-запроса для проверки существования пользователя
                            DBConnection.msCommand.CommandText = userCheckQuery;
                            int count = Convert.ToInt32(DBConnection.msCommand.ExecuteScalar());

                            if (count > 0)
                            {

                                // Выполнение SQL-запроса для обновления значения party у получателя
                                DBConnection.msCommand.CommandText = updateReceiverPartyQuery;
                                DBConnection.msCommand.ExecuteNonQuery();

                                MessageBox.Show($"Пользователь {receiverUsername} успешно изгнан!");
                                this.Close();
                            }
                            else
                            {
                                // Пользователь не существует
                                MessageBox.Show("Пользователя не существует");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Это вы!");

                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при изгнании пользователя: " + ex.Message);
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

