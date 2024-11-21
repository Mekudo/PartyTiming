using System;
using System.Windows;

namespace Wbsc;

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
            this.Background = SharedViewModel.Instance.ThemeSettings.BackgroundColor;
        };

        this.Activated += (sender, e) =>
        {
            SharedViewModel.Instance.ThemeSettings.BackgroundColor = SharedViewModel.Instance.ThemeSettings.SavedBackgroundColor;
        };
    }
    private void ThemeSettings_OnBackgroundColorChanged(object sender, EventArgs e)
    {
        this.Background = SharedViewModel.Instance.ThemeSettings.BackgroundColor;
    }
    private void SendInvitationButton_Click(object sender, RoutedEventArgs e)
    {
        string receiverUsername = ReceiverTextBox.Text;

        if (DBConnection.ConnectionDB())
        {
            string userCheckQuery = $"SELECT COUNT(*) FROM account WHERE Login = '{receiverUsername}'";

            string updateSenderPartyQuery = $"UPDATE account SET party = 2,id_role = 2,  partyName = '{IdAcount}' WHERE id_account = '{IdAcount}'";

            string userId = $"SELECT id_account FROM account WHERE Login = '{receiverUsername}'";

            string updateReceiverPartyQuery = $"UPDATE account SET party = 1,partyName = '{IdAcount}' WHERE Login = '{receiverUsername}'";

            try
            {
                DBConnection.msCommand.CommandText = userCheckQuery;
                int count = Convert.ToInt32(DBConnection.msCommand.ExecuteScalar());

                if (count > 0)
                {

                    DBConnection.msCommand.CommandText = userId;
                    int check = Convert.ToInt32(DBConnection.msCommand.ExecuteScalar());

                    if(check != IdAcount)
                    {
                        DBConnection.msCommand.CommandText = updateSenderPartyQuery;
                        DBConnection.msCommand.ExecuteNonQuery();

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
                    MessageBox.Show("Пользователя не существует");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при отправке приглашения: " + ex.Message);
            }
            finally
            {
                DBConnection.CloseDb();
            }
        }
    }
}