namespace ConferenceManagement
{
    public partial class ConferenceManagementDBEntities
    {
        private static ConferenceManagementDBEntities _context;
        public static ConferenceManagementDBEntities GetContext()
        {
            if (_context == null)
                _context = new ConferenceManagementDBEntities();
            return _context;
        }
    }
}
