using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ConferenceManagement.Views
{
    public partial class OrganizerActivitiesPage : Page
    {
        private List<Activities> _all;
        private List<Events> _events;
        public OrganizerActivitiesPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var db = ConferenceManagementDBEntities.GetContext();
            _events = db.Events.OrderBy(e => e.EventName).ToList();
            var eventNames = new List<object> { new { EventID = 0, EventName = "Все мероприятия" } };
            eventNames.AddRange(_events.Select(e => new { e.EventID, e.EventName }));
            EventFilter.ItemsSource = eventNames;
            EventFilter.DisplayMemberPath = "EventName";
            EventFilter.SelectedIndex = 0;

            _all = db.Activities.ToList();
            Apply();
        }

        private void Apply()
        {
            var q = _all.AsEnumerable();
            // фильтр по мероприятию
            if (EventFilter.SelectedItem is dynamic sel && sel.EventID != 0)
            {
                int id = sel.EventID;
                q = q.Where(a => a.EventID == id);
            }
            // поиск по текстовым полям
            var search = (SearchBox.Text ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                q = q.Where(a =>
                    (a.ActivityName ?? string.Empty).IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (a.Events != null && (a.Events.EventName ?? string.Empty).IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                );
            }
            // сортировка по StartTime
            bool asc = SortAsc.IsChecked == true;
            q = asc ? q.OrderBy(a => a.StartTime) : q.OrderByDescending(a => a.StartTime);

            List.ItemsSource = q.ToList();
        }

        private void OnFilterChanged(object sender, RoutedEventArgs e)
        {
            Apply();
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            var wnd = ActivityEditorWindow.OpenNew();
            if (wnd.ShowDialog() == true) LoadData();
        }

        private void OnEdit(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (List.SelectedItem is Activities a)
            {
                var wnd = ActivityEditorWindow.OpenEdit(a.ActivityID);
                if (wnd.ShowDialog() == true) LoadData();
            }
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            if (List.SelectedItem is Activities a)
            {
                var db = ConferenceManagementDBEntities.GetContext();
                // запрет если есть жюри на активность
                bool hasJury = db.Jury.Any(j => j.ActivityID == a.ActivityID);
                if (hasJury)
                {
                    MessageBox.Show("Нельзя удалить активность: есть назначенное жюри.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (MessageBox.Show("Удалить активность?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    db.Activities.Remove(a);
                    db.SaveChanges();
                    LoadData();
                }
            }
        }
    }
}
