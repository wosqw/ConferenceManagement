using System.Windows;
using System.Windows.Controls;
using ConferenceManagement.Services;

namespace ConferenceManagement.Views
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void OnLoginClick(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = string.Empty;
            var email = EmailBox.Text?.Trim();
            var pwd = PasswordBox.Password;
            if (AuthService.SignIn(email, pwd))
            {
                NavigateAfterLogin();
            }
            else
            {
                ErrorText.Text = "Неверный Email или пароль";
            }
        }

        private void OnGuestClick(object sender, RoutedEventArgs e)
        {
            AuthService.Session.SignOut();
            // Навигация на публичный экран мероприятий
            NavigationService?.Navigate(new PublicEventsPage());
        }

        private void NavigateAfterLogin()
        {
            switch (AuthService.Session.Role)
            {
                case UserRole.Organizer:
                    NavigationService?.Navigate(new OrganizerDashboardPage());
                    break;
                default:
                    NavigationService?.Navigate(new PublicEventsPage());
                    break;
            }
        }
    }
}
