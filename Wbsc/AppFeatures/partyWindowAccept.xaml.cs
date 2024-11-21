using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
  
    public partial class partyWindowAccept : Window
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
        public partyWindowAccept()
        {
            InitializeComponent();
            DataContext = this;
            Days = new ObservableCollection<DayInfo>();
            GenerateCalendar(DateTime.Now);
            CalendarDate.SelectedDate = DateTime.Today;
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
        private void ImageClickHandler(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image clickedImage)
            {
                if (clickedImage.Name == "image1")
                {
                    User newUser = new User();
                    newUser.Show();
                    this.Hide();
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
                        Autorization newvAutorization = new Autorization();
                        newvAutorization.Show();
                        this.Hide();
                    }
                    else if (result == MessageBoxResult.No)
                    {

                    }
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
        internal void eventLoad(object sender, RoutedEventArgs e)
        {

            // Подключение к базе данных
            if (DBConnection.ConnectionDB())
            {
                // SQL-запрос для получения мероприятий пользователя
                string query = $"SELECT eventName, eventDescription, eventDate FROM Event WHERE id_account = '{IdAccount}'";
                string updateReceiverPartyQuery = $"UPDATE account SET party = 2, id_role = 3 WHERE id_account = '{IdAccount}'";
                string updateReceiverPartyQuer = $"UPDATE account SET party = 0,partyName = 0 WHERE id_account = '{IdAccount}'";

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
                    if (Days.Any(d => d.Date.Date == DateTime.Now.Date))
                    {
                        MessageBoxResult result = MessageBox.Show("Вы хотите вступить в группу?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        MessageBox.Show($"{IdAccount}");
                        if (result == MessageBoxResult.Yes)
                        {
                            // Выполнение SQL-запроса для обновления значения party у получателя
                            DBConnection.msCommand.CommandText = updateReceiverPartyQuery;
                            DBConnection.msCommand.ExecuteNonQuery();

                            this.Close();

                        }

                        else if (result == MessageBoxResult.No)
                        {
  
                            this.Close();

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
               




        
    }
}
