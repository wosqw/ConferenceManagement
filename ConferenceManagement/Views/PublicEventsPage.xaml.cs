using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ConferenceManagement.Views
{
    public partial class PublicEventsPage : Page
    {
        private List<Events> _allEvents;

        public PublicEventsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = ConferenceManagementDBEntities.GetContext())
            {
                _allEvents = db.Events.ToList();
                var directions = new List<string> { "Все направления" };
                // В модели нет явного поля Direction, используем City/Название как суррогат направления или пусто
                directions.AddRange(_allEvents.Select(e => e.Cities != null ? e.Cities.CityName : null).Distinct().Where(s => !string.IsNullOrWhiteSpace(s)));
                DirectionBox.ItemsSource = directions;
                DirectionBox.SelectedIndex = 0;
                ApplyFilter();
            }
        }

        private void ApplyFilter()
        {
            var filtered = _allEvents.AsEnumerable();
            var dir = DirectionBox.SelectedItem as string;
            if (!string.IsNullOrWhiteSpace(dir) && dir != "Все направления")
                filtered = filtered.Where(e => e.Cities != null && string.Equals(e.Cities.CityName, dir, StringComparison.OrdinalIgnoreCase));

            var from = DateFrom.SelectedDate;
            var to = DateTo.SelectedDate;
            if (from.HasValue)
                filtered = filtered.Where(e => e.EventDate >= from.Value);
            if (to.HasValue)
                filtered = filtered.Where(e => e.EventDate <= to.Value);

            EventsList.ItemsSource = filtered.ToList();
        }

        private void OnFilterChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void OnToLogin(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new LoginPage());
        }

        private void OnOpenDetails(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (EventsList.SelectedItem is Events ev)
            {
                NavigationService?.Navigate(new EventDetailsPage(ev.Id));
            }
        }
    }
}
