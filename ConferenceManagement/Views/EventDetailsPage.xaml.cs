using System.Linq;
using System.Windows.Controls;

namespace ConferenceManagement.Views
{
    public partial class EventDetailsPage : Page
    {
        private readonly int _eventId;
        public EventDetailsPage(int eventId)
        {
            _eventId = eventId;
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = ConferenceManagementDBEntities.GetContext())
            {
                var ev = db.Events.FirstOrDefault(e => e.EventID == _eventId);
                if (ev == null) return;
                TitleText.Text = ev.EventName;
                DatesText.Text = $"{ev.EventDate:dd.MM.yyyy}";
                CityText.Text = $"Город: {ev.Cities?.CityName}";
                OrganizerText.Text = $"Организаторы: {string.Join(", ", ev.Organizers.Select(o => o.Users.FullName))}";
                DescriptionText.Text = string.Empty;
                Logo.Source = Services.ImageHelper.FromBytes(ev.Logo);

                var acts = db.Activities.Where(a => a.EventID == _eventId).OrderBy(a => a.StartTime).ToList();
                ActivitiesList.ItemsSource = acts;
            }
        }
    }
}
