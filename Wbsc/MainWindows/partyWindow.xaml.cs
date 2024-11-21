using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Wbsc;

public partial class partyWindow : Window
{
    private int _idAccount;

    public int IdAccount
    {
        get { return _idAccount; }
        set
        {
            if (_idAccount != value)
            {
                _idAccount = value;
                RaisePropertyChanged(nameof(IdAccount));
                eventLoad(this, null);
            }
        }
    }
    public partyWindow()
    {
        InitializeComponent();
        DataContext = this;
        Days = new ObservableCollection<DayInfo>();
        GenerateCalendar(DateTime.Now);
        CalendarDate.SelectedDate = DateTime.Today;
        eventLoad(this, null);
        this.WindowState = WindowState.Maximized;

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

    public class DayInfo : INotifyPropertyChanged
    {
        private DateTime _date;
        public int Day { get; set; }

        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged(nameof(Date));
                }
            }
        }

        private string _eventName;
        public string EventName
        {
            get { return _eventName; }
            set
            {
                if (_eventName != value)
                {
                    _eventName = value;
                    OnPropertyChanged(nameof(EventName));
                }
            }
        }

        private string _eventDescription;
        public string EventDescription
        {
            get { return _eventDescription; }
            set
            {
                if (_eventDescription != value)
                {
                    _eventDescription = value;
                    OnPropertyChanged(nameof(EventDescription));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public ObservableCollection<DayInfo> Days { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    private void GenerateCalendar(DateTime targetDate)
    {
        Days.Clear();

        DateTime firstDayOfMonth = new DateTime(targetDate.Year, targetDate.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(targetDate.Year, targetDate.Month);

        for (int i = 1; i <= daysInMonth; i++)
        {
            Days.Add(new DayInfo { Day = i, Date = new DateTime(targetDate.Year, targetDate.Month, i) });
        }

    }
    internal void eventLoad(object sender, RoutedEventArgs e)
    {
        if (DBConnection.ConnectionDB())
        {
            string getNamePartyQuery = $"SELECT partyName FROM account WHERE id_account = '{IdAccount}'";
            DBConnection.msCommand.CommandText = getNamePartyQuery;
            string nameparty = DBConnection.msCommand.ExecuteScalar()?.ToString();
            string query = $"SELECT eventName, eventDescription, eventDate FROM partyevent WHERE id_account = '{nameparty}'";

            try
            {
                DBConnection.msCommand.CommandText = query;

                using (var reader = DBConnection.msCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string eventName = reader["eventName"].ToString();
                        string eventDescription = reader["eventDescription"].ToString();
                        DateTime eventDate = Convert.ToDateTime(reader["eventDate"]);
                        DayInfo selectedDay = Days.FirstOrDefault(d => d.Date.Date == eventDate.Date);

                        if (selectedDay != null)
                        {
                            selectedDay.EventName = eventName;
                            selectedDay.EventDescription = eventDescription;
                        }
                        else
                        {
                            Days.Add(new DayInfo
                            {
                                Day = eventDate.Day,
                                Date = eventDate.Date, 
                                EventName = eventName,
                                EventDescription = eventDescription
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке мероприятий: " + ex.Message);
            }
            finally
            {
                DBConnection.CloseDb();
            }
        }
    }


    private void PrevMonthButton_Click(object sender, RoutedEventArgs e)
    {
        CalendarDate.SelectedDate = CalendarDate.SelectedDate?.AddMonths(-1);
        GenerateCalendar(CalendarDate.SelectedDate ?? DateTime.Now);
    }

    private void NextMonthButton_Click(object sender, RoutedEventArgs e)
    {
        CalendarDate.SelectedDate = CalendarDate.SelectedDate?.AddMonths(1);
        GenerateCalendar(CalendarDate.SelectedDate ?? DateTime.Now);
    }

    private void CalendarDate_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        GenerateCalendar(CalendarDate.SelectedDate ?? DateTime.Now);
    }

    public void OnEventCreated(DateTime selectedDate, string eventName, string eventDescription)
    {

        int day = selectedDate.Day;
        int month = selectedDate.Month;
        int year = selectedDate.Year;

        DayInfo selectedDay = Days.FirstOrDefault(d => d.Day == day);

        if (selectedDay != null)
        {
            selectedDay.EventName = eventName;
            selectedDay.EventDescription = eventDescription;
            selectedDay.Date = selectedDate;

        }
    }


    private void ImageClickHandler(object sender, MouseButtonEventArgs e)
    {
        if (sender is Image clickedImage)
        {
            if (clickedImage.Name == "image1")
            {
                User newUser = new User();
                newUser.Show();
                this.Close();
            }

            if (clickedImage.Name == "image2")
            {
                program.newUser.UpdateUserData(IdAccount);
                program.newUser.Show();
                this.Hide();
            }
            else if (clickedImage.Name == "image3")
            {
                sendPartyWindow sendPartyWindow = new sendPartyWindow();
                sendPartyWindow.IdAcount = IdAccount;
                sendPartyWindow.Show();
            }
            else if (clickedImage.Name == "image4")
            {
                CreateEventParty CreateEventParty = new CreateEventParty();
                CreateEventParty.IdAcount = IdAccount;
                CreateEventParty.Show();

            }
            else if (clickedImage.Name == "image7")
            {
                if (DBConnection.ConnectionDB())
                {
                    try
                    {
                        string checkOwnershipQuery = $"SELECT COUNT(*) FROM Account WHERE id_account = '{IdAccount}' AND partyName = id_account";
                        DBConnection.msCommand.CommandText = checkOwnershipQuery;
                        int count = Convert.ToInt32(DBConnection.msCommand.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBoxResult result = MessageBox.Show("Вы точно хотите удалить группу?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                            if (result == MessageBoxResult.Yes)
                            {
                                string updateGroupQuery = $"UPDATE Account SET partyName = '0', party = '0' WHERE partyName = '{IdAccount}'";

                                DBConnection.msCommand.CommandText = updateGroupQuery;
                                DBConnection.msCommand.ExecuteNonQuery();

                                MessageBox.Show("Группа успешно удалена!");
                                program.newMainWindow.Show();
                                this.Close();
                            }
                        }
                        else
                        {
                            MessageBoxResult result = MessageBox.Show("Вы точно хотите выйти из группы?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                            if (result == MessageBoxResult.Yes)
                            {
                                string leaveGroupQuery = $"UPDATE Account SET partyName = '0', party = '0' WHERE id_account = '{IdAccount}'";

                                DBConnection.msCommand.CommandText = leaveGroupQuery;
                                DBConnection.msCommand.ExecuteNonQuery();
                                MessageBox.Show("Вы успешно вышли из группы!");
                                MainWindow newMainWindow = new MainWindow();
                                newMainWindow.Show();
                                this.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при выполнении запроса: " + ex.Message);
                    }
                    finally
                    {
                        DBConnection.CloseDb();
                    }
                }
            }

            else if (clickedImage.Name == "image6")
            {

                KickUser KickUser = new KickUser();
                KickUser.IdAcount = IdAccount;
                KickUser.Show();
            }

            else if (clickedImage.Name == "image5")
            {
                MessageBoxResult result = MessageBox.Show("Вы точно хотите выйти?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    Autorization newAutorization = new Autorization();
                    newAutorization.Show();
                    this.Hide();

                }
                else if (result == MessageBoxResult.No)
                {

                }
            }
        }
    }
}