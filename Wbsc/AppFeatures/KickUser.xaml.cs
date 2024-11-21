using System;
using System.Windows;

namespace Wbsc;

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
    private void KickInvitationButton_Click(object sender, RoutedEventArgs e)
    {
        string receiverUsername = ReceiverTextBox.Text;

        if (DBConnection.ConnectionDB())
        {
            string userCheckQuery = $"SELECT COUNT(*) FROM account WHERE Login = '{receiverUsername}'";

            string userId = $"SELECT id_account FROM account WHERE Login = '{receiverUsername}'";

            string updateReceiverPartyQuery = $"UPDATE account SET party = 0,partyName = 0 WHERE Login = '{receiverUsername}'";

            try
            {
                string checkOwnershipQuery = $"SELECT COUNT(*) FROM Account WHERE id_account = '{IdAcount}' AND partyName = id_account";
                DBConnection.msCommand.CommandText = checkOwnershipQuery;
                int check = Convert.ToInt32(DBConnection.msCommand.ExecuteScalar());

                DBConnection.msCommand.CommandText = userId;
                int chek = Convert.ToInt32(DBConnection.msCommand.ExecuteScalar());
                if (chek != IdAcount)
                {
                    if (check > 0)
                    {
                        DBConnection.msCommand.CommandText = userCheckQuery;
                        int count = Convert.ToInt32(DBConnection.msCommand.ExecuteScalar());

                        if (count > 0)
                        {
                            DBConnection.msCommand.CommandText = updateReceiverPartyQuery;
                            DBConnection.msCommand.ExecuteNonQuery();

                            MessageBox.Show($"Пользователь {receiverUsername} успешно изгнан!");
                            this.Close();
                        }
                        else
                        {
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
                DBConnection.CloseDb();
            }
           
        }
    }
}