using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Wbsc;

public class ThemeSettings
{
    private SolidColorBrush _backgroundColor = Brushes.White;

    public SolidColorBrush BackgroundColor
    {
        get { return _backgroundColor; }
        set
        {
            _backgroundColor = value;
            OnBackgroundColorChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public SolidColorBrush SavedBackgroundColor { get; set; }

    public static event EventHandler OnBackgroundColorChanged;
    public ThemeSettings()
    {
        _backgroundColor = Brushes.White;
        SavedBackgroundColor = Brushes.White;
    }
}
public class SharedViewModel
{
    private static SharedViewModel _instance;
    public ThemeSettings ThemeSettings { get; private set; }

    public static SharedViewModel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SharedViewModel();
            }
            return _instance;
        }
    }

    private SharedViewModel()
    {
        ThemeSettings = new ThemeSettings();
    }
}
public partial class User : Window
{
    public int IdAcount { get; set; }
    bool isClosed = false;
    private string originalEmail;
    private string originalpol;
    private string originalDat_rod;
    private string originalNumber_phone;
    private string originalGorod;
    private string originalFamilia;
    private string originalImy;
    private string originalOtche;
    private bool textChangedByFamilia = false;

    public User()
    {
        InitializeComponent();
        originalEmail = email.Text;
        originalpol = pol.Text;
        originalDat_rod = dat_rod.Text;
        originalNumber_phone = number_phone.Text;
        originalGorod = gorod.Text;
        originalFamilia = familia.Text;
        originalImy = imy.Text;
        originalOtche = otche.Text;
        this.WindowState = WindowState.Maximized;
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

    private void ImageClickHandler(object sender, MouseButtonEventArgs e)
    {
        if (sender is Image clickedImage)
        {
            if (clickedImage.Name == "image1")
            {
                program.newMainWindow.Show();
                this.Close();
            }
            else if (clickedImage.Name == "image2")
            {
                CreateEventParty newCreateEventParty = new CreateEventParty();
                newCreateEventParty.Show();
                this.Close();
            }
            else if (clickedImage.Name == "image3")
            {
                partyWindow newpartyWindow = new partyWindow();
                newpartyWindow.Show();
                this.Close();
            }
            else if (clickedImage.Name == "image4")
            {
                MessageBoxResult result = MessageBox.Show("Вы точно хотите выйти?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    Autorization newAutorization = new Autorization();
                    newAutorization.Show();
                    this.Close();
                }
                else if (result == MessageBoxResult.No)
                {

                }
            }
        }
    }

    private void EditProfileTextBlock_Click_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        originalEmail = email.Text;
        email.IsEnabled = true;
        email.Focus();

        originalpol = pol.Text;
        pol.IsEnabled = true;
        pol.Focus();

        originalDat_rod = dat_rod.Text;
        dat_rod.IsEnabled = true;
        dat_rod.Focus();

        originalNumber_phone = number_phone.Text;
        number_phone.IsEnabled = true;
        number_phone.Focus();

        originalGorod = gorod.Text;
        gorod.IsEnabled = true;
        gorod.Focus();

        originalFamilia = familia.Text;
        familia.IsEnabled = true;
        familia.Focus();

        originalImy = imy.Text;
        imy.IsEnabled = true;
        imy.Focus();

        originalOtche = otche.Text;
        otche.IsEnabled = true;
        otche.Focus();

        SaveButton.Visibility = Visibility.Visible;
        NoSaveButton.Visibility = Visibility.Visible;
    }

    public void UpdateUserData(int accountId)
    {
        IdAcount = accountId;
        LoadUserData();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        string Email = email.Text;
        string Gender = pol.Text;
        string Birthday = dat_rod.Text;
        string Phone = number_phone.Text;
        string City = gorod.Text;
        string SecondName = familia.Text;
        string FirstName = imy.Text;
        string FatherName = otche.Text;
        string UserName = fio1.Text;

        if (pol.Text == "мужской")
        {
            avatarImage.Source = new BitmapImage(new Uri("Images/m.png", UriKind.RelativeOrAbsolute));
        }
        else if (pol.Text == "женский")
        {
            avatarImage.Source = new BitmapImage(new Uri("Images/g.png", UriKind.RelativeOrAbsolute));
        }
        if (DBConnection.ConnectionDB())
        {
            string checkQuery = $"SELECT COUNT(*) FROM User WHERE IdAccount = {IdAcount}";

            try
            {
                DBConnection.msCommand.CommandText = checkQuery;
                int count = Convert.ToInt32(DBConnection.msCommand.ExecuteScalar());

                if (count > 0)
                {
                    string updateQuery = $@"UPDATE User 
                                        SET Email = '{Email}', 
                                            Gender = '{Gender}', 
                                            Birthday = '{Birthday}', 
                                            Phone = '{Phone}', 
                                            City = '{City}', 
                                            SecondName = '{SecondName}', 
                                            FirstName = '{FirstName}', 
                                            FatherName = '{FatherName}', 
                                            UserName = '{UserName}' 
                                        WHERE IdAccount = {IdAcount}";

                    DBConnection.msCommand.CommandText = updateQuery;
                    DBConnection.msCommand.ExecuteNonQuery();

                    MessageBox.Show("Информация успешно обновлена!");
                }
                else
                {
                    string insertQuery = $@"INSERT INTO User (Email, Gender, Birthday, Phone, City, SecondName, FirstName, FatherName, UserName, IdAccount) 
                                        VALUES ('{Email}', '{Gender}', '{Birthday}', '{Phone}', '{City}', '{SecondName}', '{FirstName}', '{FatherName}', '{UserName}', '{IdAcount}')";

                    DBConnection.msCommand.CommandText = insertQuery;
                    DBConnection.msCommand.ExecuteNonQuery();

                    MessageBox.Show("Информация успешно записана!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обработке данных пользователя: " + ex.Message);
            }
            finally
            {
                DBConnection.CloseDb();
            }
        }

        SaveButton.Visibility = Visibility.Hidden;
        NoSaveButton.Visibility = Visibility.Hidden;
        email.IsEnabled = false;
        pol.IsEnabled = false;
        dat_rod.IsEnabled = false;
        number_phone.IsEnabled = false;
        gorod.IsEnabled = false;
        familia.IsEnabled = false;
        imy.IsEnabled = false;
        otche.IsEnabled = false;
    }

   private void LoadUserData()
    {
        if (DBConnection.ConnectionDB())
        {
            try
            {
                string query = $"SELECT * FROM Account WHERE id_account = {IdAcount}";
                DBConnection.msCommand.CommandText = query;

                using (var reader = DBConnection.msCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        email.Text = reader["email"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            finally
            {
                DBConnection.CloseDb();
            }
        }
        if (DBConnection.ConnectionDB())
        {
            try
            {
                string query = $"SELECT * FROM User WHERE IdAccount = {IdAcount}";
                DBConnection.msCommand.CommandText = query;

                using (var reader = DBConnection.msCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        email.Text = reader["Email"].ToString();
                        pol.Text = reader["Gender"].ToString();
                        dat_rod.Text = reader["Birthday"].ToString();
                        number_phone.Text = reader["Phone"].ToString();
                        gorod.Text = reader["City"].ToString();
                        familia.Text = reader["SecondName"].ToString();
                        imy.Text = reader["FirstName"].ToString();
                        otche.Text = reader["FatherName"].ToString();
                        fio1.Text = reader["UserName"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            finally
            {
                DBConnection.CloseDb();
            }
        }
        if (pol.Text == "мужской")
        {
            avatarImage.Source = new BitmapImage(new Uri("Images/g.png", UriKind.RelativeOrAbsolute));
        }
        else if (pol.Text == "женский")
        {
            avatarImage.Source = new BitmapImage(new Uri("Images/m.png", UriKind.RelativeOrAbsolute));
        }
    }

    private void NoSaveButton_Click(object sender, RoutedEventArgs e)
    {
        email.Text = originalEmail;
        email.IsEnabled = false;

        pol.Text = originalpol;
        pol.IsEnabled = false;

        dat_rod.Text = originalDat_rod;
        dat_rod.IsEnabled = false;

        number_phone.Text = originalNumber_phone;
        number_phone.IsEnabled = false;

        gorod.Text = originalGorod;
        gorod.IsEnabled = false;

        familia.Text = originalFamilia;
        familia.IsEnabled = false;

        imy.Text = originalImy;
        imy.IsEnabled = false;

        otche.Text = originalOtche;
        otche.IsEnabled = false;

        SaveButton.Visibility = Visibility.Hidden;
        NoSaveButton.Visibility = Visibility.Hidden;
    }

    private void pol_GotFocus(object sender, RoutedEventArgs e)
    {
        if (pol.Text == "мужской или женский")
        {
            pol.Text = "";
        }
    }
    private void pol_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(pol.Text))
        {
            pol.Text = "мужской или женский";
        }
        else
        {
            string lowerCaseInput = pol.Text.ToLower();
            if (lowerCaseInput != "мужской" && lowerCaseInput != "женский")
            {
                string originalValue = pol.Text;
                MessageBox.Show("Пожалуйста, введите только 'мужской' или 'женский'.");
                pol.Text = originalValue;
            }
        }
    }

    private void dat_rod_LostFocus(object sender, RoutedEventArgs e)
    {

        if (string.IsNullOrWhiteSpace(dat_rod.Text))
        {
            dat_rod.Text = "дд.мм.гггг";
        }
        else
        {
            string dateFormat = "dd.MM.yyyy";
            DateTime parsedDate;

            if (!DateTime.TryParseExact(dat_rod.Text, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                MessageBox.Show("Пожалуйста, введите дату в формате 'дд.мм.гггг'.");
                dat_rod.Text = "дд.мм.гггг";
            }
            else
            {
                int minYear = 1900;
                int maxYear = DateTime.Now.Year;

                if (parsedDate.Year < minYear || parsedDate.Year > maxYear)
                {
                    MessageBox.Show($"Пожалуйста, введите год в пределах от {minYear} до {maxYear}.");
                    dat_rod.Text = "дд.мм.гггг";
                }
            }
        }
    }
    private void dat_rod_GotFocus(object sender, RoutedEventArgs e)
    {
        if (dat_rod.Text == "дд.мм.гггг")
        {
            dat_rod.Text = "";
        }
    }
    private void number_phone_GotFocus(object sender, RoutedEventArgs e)
    {
        if (number_phone.Text == "+7(___)___*__*__")
        {
            number_phone.Text = "";
        }
    }
    private void number_phone_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(number_phone.Text))
        {
            number_phone.Text = "+7(___)___*__*__";
        }
        else
        {
            string numericInput = new string(number_phone.Text.Where(char.IsDigit).ToArray());
            if (numericInput.Length == 10)
            {
                string formattedPhoneNumber = $"+7({numericInput.Substring(0, 3)}){numericInput.Substring(3, 3)}*{numericInput.Substring(6, 2)}*{numericInput.Substring(8, 2)}";
                number_phone.Text = formattedPhoneNumber;
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите 10 цифр телефонного номера.");
                number_phone.Text = "+7(___)___*__*__";
            }
        }
    }


    private void UpdateFioTextBlock()
    {
        string surname = familia.Text;
        string name = imy.Text;
        string patronymic = otche.Text;
        string fio = $"{surname} {(!string.IsNullOrWhiteSpace(name) ? name.Substring(0, 1) + "." : "")}{(!string.IsNullOrWhiteSpace(patronymic) ? patronymic.Substring(0, 1) + "." : "")}";
        fio1.Text = fio;
    }
    private void familia_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox familia)
        {
            if (!string.IsNullOrEmpty(familia.Text))
            {
                familia.Text = familia.Text[0].ToString().ToUpper() + familia.Text.Substring(1).ToLower();
                familia.CaretIndex = familia.Text.Length;
            }
            UpdateFioTextBlock();
        }
    }
    private void imy_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox imy)
        {
            if (!string.IsNullOrEmpty(imy.Text))
            {
                imy.Text = imy.Text[0].ToString().ToUpper() + imy.Text.Substring(1).ToLower();
                imy.CaretIndex = imy.Text.Length;
            }
            UpdateFioTextBlock();
        }
    }
    private void otche_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox otche)
        {
            if (!string.IsNullOrEmpty(otche.Text))
            {
                otche.Text = otche.Text[0].ToString().ToUpper() + otche.Text.Substring(1).ToLower();
                otche.CaretIndex = otche.Text.Length;
            }
            UpdateFioTextBlock();
        }
    }


    private void gorod_TextChanged(object sender, TextChangedEventArgs e)
    {
        StringBuilder result = new StringBuilder(gorod.Text.Length);

        bool makeUpperCase = true;

        foreach (char c in gorod.Text)
        {
            if (char.IsLetter(c))
            {
                if (makeUpperCase)
                {
                    result.Append(char.ToUpper(c));
                    makeUpperCase = false;
                }
                else
                {
                    result.Append(char.ToLower(c));
                }
            }
            else
            {
                result.Append(c);
                makeUpperCase = true;
            }
        }

        gorod.Text = result.ToString();
        gorod.CaretIndex = gorod.Text.Length;
    }
    private void gorod_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        char inputChar = e.Text[0];

        if (char.IsLetter(inputChar) || char.IsWhiteSpace(inputChar) || inputChar == '-')
        {
            e.Handled = false;
        }
        else
        {
            e.Handled = true;
        }
    }
    private void otche_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        char inputChar = e.Text[0];

        if (char.IsLetter(inputChar) || char.IsWhiteSpace(inputChar))
        {
            e.Handled = false;
        }
        else
        {
            e.Handled = true;
        }
    }
    private void imy_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        char inputChar = e.Text[0];

        if (char.IsLetter(inputChar) || char.IsWhiteSpace(inputChar))
        {
            e.Handled = false;
        }
        else
        {
            e.Handled = true;
        }
    }
    private void familia_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        char inputChar = e.Text[0];

        if (char.IsLetter(inputChar) || char.IsWhiteSpace(inputChar))
        {
            e.Handled = false;
        }
        else
        {
            e.Handled = true;
        }
    }

    private void color_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ComboBoxItem selectedItem = (ComboBoxItem)color.SelectedItem;

        if (selectedItem != null)
        {
            SolidColorBrush selectedColor = (SolidColorBrush)selectedItem.Background;

            SharedViewModel.Instance.ThemeSettings.SavedBackgroundColor = selectedColor;

            SharedViewModel.Instance.ThemeSettings.BackgroundColor = selectedColor;
        }
    }
}