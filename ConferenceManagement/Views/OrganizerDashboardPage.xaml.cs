using System;
using System.Linq;
using System.Windows.Controls;
using ConferenceManagement.Services;

namespace ConferenceManagement.Views
{
    public partial class OrganizerDashboardPage : Page
    {
        public OrganizerDashboardPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var user = AuthService.Session.CurrentUser;
            var hours = DateTime.Now.Hour;
            var period = hours < 12 ? "Доброе утро" : hours < 18 ? "Добрый день" : "Добрый вечер";
            WelcomeText.Text = $"{period}, {user?.FullName}";
            Avatar.Source = ImageHelper.FromBytes(user?.ProfilePhoto);
        }

        private void OnProfile(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PublicEventsPage()); // заглушка, заменю на профиль
        }

        private void OnActivities(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService?.Navigate(new OrganizerActivitiesPage());
        }

        private void OnEvents(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PublicEventsPage()); // будет заменено на OrganizerEventsPage в следующей итерации
        }
    }
}
