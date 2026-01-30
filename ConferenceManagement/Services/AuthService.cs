using System.Linq;

namespace ConferenceManagement.Services
{
    public enum UserRole
    {
        Guest,
        Participant,
        Moderator,
        Organizer,
        Jury
    }

    public class Session
    {
        public Users CurrentUser { get; set; }
        public UserRole Role { get; set; } = UserRole.Guest;
        public bool IsAuthenticated => CurrentUser != null;

        public void SignOut()
        {
            CurrentUser = null;
            Role = UserRole.Guest;
        }
    }

    public static class AuthService
    {
        public static Session Session { get; } = new Session();

        // Логин по Email (в модели Users нет IdNumber)
        public static bool SignIn(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return false;

            var db = ConferenceManagementDBEntities.GetContext();
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user == null) return false;

            Session.CurrentUser = user;
            Session.Role = ResolveRole(user);
            return true;
        }

        private static UserRole ResolveRole(Users user)
        {
            // Роль выводим по наличию связей
            if (user.Organizers != null && user.Organizers.Any()) return UserRole.Organizer;
            if (user.Moderators != null && user.Moderators.Any()) return UserRole.Moderator;
            if (user.Jury != null && user.Jury.Any()) return UserRole.Jury;
            if (user.Participants != null && user.Participants.Any()) return UserRole.Participant;
            return UserRole.Guest;
        }
    }
}
