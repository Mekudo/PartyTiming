using System;
using System.Windows;
using System.Windows.Input;

namespace Wbsc;

public partial class Autorization : Window
{

    public Autorization()
    {
        InitializeComponent();
        DataContext = SharedViewModel.Instance;

        ThemeSettings.OnBackgroundColorChanged += ThemeSettings_OnBackgroundColorChanged;

        Loaded += (sender, e) =>
        {
            this.Background = SharedViewModel.Instance.ThemeSettings.BackgroundColor;
        };
    }

    private void ThemeSettings_OnBackgroundColorChanged(object sender, EventArgs e)
    {
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
        string username = txtUsername.Text;
        string password = txtPassword.Password;

        if (DBConnection.ConnectionDB())
        {

            string loginQuery = $"SELECT COUNT(*) FROM account WHERE Login = '{username}' AND password = '{password}'";
            string userInfoQuery = $"SELECT id_account, party FROM account WHERE Login = '{username}'";

            try
            {
                DBConnection.msCommand.CommandText = loginQuery;
                int count = Convert.ToInt32(DBConnection.msCommand.ExecuteScalar());

                if (count > 0)
                {
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
                    MessageBox.Show("Неправильные учетные данные!");
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                DBConnection.CloseDb();

            }
        }
    }
}