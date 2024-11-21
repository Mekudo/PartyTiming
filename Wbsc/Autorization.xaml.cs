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
using System.Data.SqlClient;
using System.Security.Principal;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;

namespace Wbsc
{

    public partial class Autorization : Window
    {

        public Autorization()
        {
            InitializeComponent();
            DataContext = SharedViewModel.Instance;

            ThemeSettings.OnBackgroundColorChanged += ThemeSettings_OnBackgroundColorChanged;

            Loaded += (sender, e) =>
            {
                // Обновить фон при загрузке окна
                this.Background = SharedViewModel.Instance.ThemeSettings.BackgroundColor;
            };
        }

        private void ThemeSettings_OnBackgroundColorChanged(object sender, EventArgs e)
        {
            // Обновить фон окна
            this.Background = SharedViewModel.Instance.ThemeSettings.BackgroundColor;
        }

        private void NewAutorization_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
        }

        private void TextBlock_Click(object sender, MouseButtonEventArgs e)
        {
            program.newRegistation.Show();
            this.Hide();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Получение данных из текстовых полей
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            // Подключение к базе данных
            if (DBConnection.ConnectionDB())
            {
                // SQL-запрос для проверки данных пользователя

                string loginQuery = $"SELECT COUNT(*) FROM account WHERE Login = '{username}' AND password = '{password}'";
                string userInfoQuery = $"SELECT id_account, party FROM account WHERE Login = '{username}'";

                try
                {
                    // Выполнение SQL-запроса для проверки данных пользователя
                    DBConnection.msCommand.CommandText = loginQuery;
                    int count = Convert.ToInt32(DBConnection.msCommand.ExecuteScalar());

                    if (count > 0)
                    {
                        // Выполнение SQL-запроса для получения id_account и party
                        DBConnection.msCommand.CommandText = userInfoQuery;

                        using (var reader = DBConnection.msCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                int partyValue = reader.GetInt32(1);

                                if (partyValue == 2)
                                {
                                    program.newPartyWindow.IdAccount = id;
                                    program.newPartyWindow.Show();
                                    this.Hide();

                                }
                                if (partyValue == 1 ) 
                                {
                                    program.newpartyWindowAccept.IdAccount = id;
                                    program.newpartyWindowAccept.Show();
                                    this.Hide();

                                }
                                if (partyValue == 0)
                                {
                                    program.newMainWindow.IdAccount = id;
                                    program.newMainWindow.Show();
                                    this.Hide();

                                }
                            }
                        }
                    }
                    else
                    {
                        // Пользователь не найден или данные неверны
                        MessageBox.Show("Неправильные учетные данные!");
                    }
                }
                catch (Exception ex)
                {
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
