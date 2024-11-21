using System;
using System.Linq;
using System.Windows;

namespace Wbsc;

public partial class CreateEventWindow : Window
{
    public string EventName { get; set; }
    public string EventDescription { get; set; }
    public DateTime SelectedDate { get; set; }
    public int IdAcount { get; set; }

    public CreateEventWindow()
    {
        InitializeComponent();
        SelectedDate = DateTime.Today;
        DataContext = this;
        this.Width = 300;
        this.Height = 350;
    }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        if (DBConnection.ConnectionDB())
        {
            string checkQuery = $"SELECT COUNT(*) FROM Event WHERE EventDate = '{SelectedDate:yyyy-MM-dd}' AND id_account = {IdAcount}";

            try
            {
                DBConnection.msCommand.CommandText = checkQuery;
                int count = Convert.ToInt32(DBConnection.msCommand.ExecuteScalar());

                if (count > 0)
                {
                    string updateQuery = $@"UPDATE Event SET eventName = '{EventName}', eventDescription = '{EventDescription}' WHERE EventDate = '{SelectedDate:yyyy-MM-dd}'";

                    DBConnection.msCommand.CommandText = updateQuery;
                    DBConnection.msCommand.ExecuteNonQuery();

                    MessageBox.Show("Информация успешно обновлена!");
                }
                else
                {
                    string query = $"INSERT INTO Event (id_account, eventName, eventDescription, eventDate) VALUES ('{IdAcount}', '{EventName}', '{EventDescription}', '{SelectedDate:yyyy-MM-dd}')";

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
                program.newMainWindow.eventLoad(this, null);
                DBConnection.CloseDb();
            }
        }
        this.Close();
    }
}