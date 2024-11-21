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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Security.Principal;
using System.Reflection;


namespace Wbsc
{

    
    public partial class MainWindow : Window
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
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Days = new ObservableCollection<DayInfo>();
            GenerateCalendar(DateTime.Now);
            CalendarDate.SelectedDate = DateTime.Today;
//            DataContext = SharedViewModel.Instance;
            this.WindowState = WindowState.Maximized;
            CalendarItems.Visibility = Visibility.Visible;

            ThemeSettings.OnBackgroundColorChanged += ThemeSettings_OnBackgroundColorChanged;

            Loaded += (sender, e) =>
            {
                CalendarItems.Visibility = Visibility.Visible;
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
            // Подключение к базе данных
            if (DBConnection.ConnectionDB())
            {
                // SQL-запрос для получения мероприятий пользователя
                string query = $"SELECT eventName, eventDescription, eventDate FROM Event WHERE id_account = '{IdAccount}'";

                try
                {
                    // Выполнение SQL-запроса для получения мероприятий пользователя
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
                    // Закрытие соединения с базой данных
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

            // Поиск соответствующий объект DayInfo в коллекции Days
            DayInfo selectedDay = Days.FirstOrDefault(d => d.Day == day);

            // Обновление данных мероприятия 
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
                    sendParty sendParty = new sendParty();
                    sendParty.IdAcount = IdAccount;
                    sendParty.Show();

                }
                else if (clickedImage.Name == "image4")
                {
                    CreateEventWindow createEventWindow = new CreateEventWindow();
                    createEventWindow.IdAcount = IdAccount;
                    createEventWindow.Show();
                    
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

}
