using System;
using System.Linq;
using System.Windows;

namespace ConferenceManagement.Views
{
    public partial class ActivityEditorWindow : Window
    {
        private static bool _opened;
        private readonly int? _activityId;
        private Events _selectedEvent;

        private ActivityEditorWindow(int? activityId = null)
        {
            InitializeComponent();
            _activityId = activityId;
            LoadEvents();
            if (_activityId.HasValue)
                LoadActivity(_activityId.Value);
        }

        public static ActivityEditorWindow OpenNew()
        {
            if (_opened) { MessageBox.Show("Окно редактирования уже открыто."); return null; }
            _opened = true;
            var w = new ActivityEditorWindow();
            w.Closed += (_, __) => _opened = false;
            return w;
        }

        public static ActivityEditorWindow OpenEdit(int activityId)
        {
            if (_opened) { MessageBox.Show("Окно редактирования уже открыто."); return null; }
            _opened = true;
            var w = new ActivityEditorWindow(activityId);
            w.Closed += (_, __) => _opened = false;
            return w;
        }

        private void LoadEvents()
        {
            var db = ConferenceManagementDBEntities.GetContext();
            var list = db.Events.OrderBy(e => e.EventName).Select(e => new { e.EventID, e.EventName }).ToList();
            EventBox.ItemsSource = list;
            EventBox.DisplayMemberPath = "EventName";
            EventBox.SelectionChanged += (s, e) =>
            {
                if (EventBox.SelectedItem is dynamic d)
                {
                    _selectedEvent = db.Events.First(ev => ev.EventID == (int)d.EventID);
                    FillStartTimes();
                }
            };
            if (list.Any())
            {
                EventBox.SelectedIndex = 0;
            }
        }

        private void LoadActivity(int id)
        {
            var db = ConferenceManagementDBEntities.GetContext();
            var a = db.Activities.First(x => x.ActivityID == id);
            _selectedEvent = a.Events;
            EventBox.SelectedItem = new { a.Events.EventID, a.Events.EventName };
            NameBox.Text = a.ActivityName;
            DescBox.Text = a.Description;
            FillStartTimes();
            if (a.StartTime.HasValue)
                StartBox.SelectedItem = a.StartTime.Value;
            DurationBox.Text = (a.Duration ?? 90).ToString();
        }

        private void FillStartTimes()
        {
            StartBox.ItemsSource = null;
            if (_selectedEvent == null) return;
            var db = ConferenceManagementDBEntities.GetContext();
            var busy = db.Activities.Where(a => a.EventID == _selectedEvent.EventID && a.StartTime.HasValue)
                                    .Select(a => a.StartTime.Value)
                                    .OrderBy(d => d)
                                    .ToList();
            // Генерация слотов: ориентируемся на дату мероприятия (EventDate), шаг 90 + 15
            var day = _selectedEvent.EventDate.Date;
            var start = day.AddHours(9); // условное начало дня 09:00
            var end = day.AddHours(20);  // условное окончание 20:00
            var slots = SlotGenerator(day, start, end, busy);
            StartBox.ItemsSource = slots;
        }

        private static System.Collections.Generic.List<DateTime> SlotGenerator(DateTime day, DateTime start, DateTime end, System.Collections.Generic.List<DateTime> busy)
        {
            var res = new System.Collections.Generic.List<DateTime>();
            var t = start;
            var duration = TimeSpan.FromMinutes(90);
            var gap = TimeSpan.FromMinutes(15);
            while (t + duration <= end)
            {
                bool conflict = busy.Any(b => Overlaps(t, t + duration, b, b + duration));
                if (!conflict)
                    res.Add(t);
                t = t + duration + gap;
            }
            return res;
        }

        private static bool Overlaps(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd)
        {
            return aStart < bEnd && bStart < aEnd;
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            if (_selectedEvent == null) { MessageBox.Show("Выберите мероприятие"); return; }
            var name = (NameBox.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name)) { MessageBox.Show("Название обязательно"); return; }
            if (!(StartBox.SelectedItem is DateTime start)) { MessageBox.Show("Выберите время начала"); return; }
            int duration = 90;

            var db = ConferenceManagementDBEntities.GetContext();
            Activities a;
            if (_activityId.HasValue)
            {
                a = db.Activities.First(x => x.ActivityID == _activityId.Value);
            }
            else
            {
                a = new Activities();
                db.Activities.Add(a);
            }
            a.EventID = _selectedEvent.EventID;
            a.ActivityName = name;
            a.StartTime = start;
            a.Duration = duration;
            a.Description = string.IsNullOrWhiteSpace(DescBox.Text) ? null : DescBox.Text.Trim();

            db.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
