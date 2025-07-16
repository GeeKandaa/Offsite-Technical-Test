using CC_TechTest_Backend.Models;

namespace CC_TechTest_Backend.Services
{
    public static class InMemoryStorage
    {
        private static readonly List<RowData> entries = new();

        public static IEnumerable<RowData> GetAll() => entries;
        public static IEnumerable<RowData> GetByMpan(string mpan) => entries.Where(entry => entry.MPAN.ToString().StartsWith(mpan));
        public static IEnumerable<RowData> GetBySerial(string serial) => entries.Where(entry => entry.MeterSerial.StartsWith(serial));
        public static IEnumerable<RowData> GetByPostCode(string postCode) => entries.Where(entry => entry.Postcode.StartsWith(postCode));
        public static IEnumerable<RowData> GetByAddress(string address) => entries.Where(entry => entry.AddressLine1.Contains(address, StringComparison.OrdinalIgnoreCase));
        public static IEnumerable<RowData> GetByDate(DateTime date) => entries.Where(entry => entry.DateOfInstallation == date);
        public static void Add(RowData entry) => entries.Add(entry);
    }
}
