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

namespace Wbsc
{

    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Получение данных из текстовых полей
            string username = txtUsername.Text;
            string email = txtEmail.Text;
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;
            string role = "1";
            string party = "0";

            if (username.Length < 4 || username.Length > 16)
            {
                MessageBox.Show("Логин должен содержать от 4 до 16 символов!");
                return;
            }

            if (password.Length < 8 || password.Length > 16)
            {
                MessageBox.Show("Пароль должен содержать от 8 до 16 символов!");
                return;
            }

            // Проверка, чтобы пароль и подтверждение пароля совпадали
            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }
            // Проверка, чтобы пароль и подтверждение пароля совпадали

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }
            // Хэширование пароля ??
            // Подключение к базе данных
            if (DBConnection.ConnectionDB())
            {
                string userId = $"SELECT Login FROM account WHERE Login = '{username}'";

                // SQL-запрос для вставки данных пользователя в таблицу "Users"
                string query = $"INSERT INTO Account (Login, email, password, id_role, party) VALUES ('{username}', '{email}', '{confirmPassword}', '{role}', '{party}')";
                try
                {
                    DBConnection.msCommand.CommandText = userId;
                    string check = Convert.ToString(DBConnection.msCommand.ExecuteScalar());
                    if (check != username)
                    {
                        DBConnection.msCommand.CommandText = query;
                        DBConnection.msCommand.ExecuteNonQuery();

                        MessageBox.Show("Пользователь успешно зарегистрирован!");
                    }
                    else
                    {
                        MessageBox.Show("Логин занят!");
                    }
                    // Выполнение SQL-запроса

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при регистрации пользователя: " + ex.Message);
                }
                finally
                {
                    // Закрытие соединения с базой данных
                    DBConnection.CloseDb();
                }

            }


        }
        private void TextBlock_Click(object sender, MouseButtonEventArgs e)
        {
            program.newAutorization.Show();
            this.Hide();
        }
    }
}
