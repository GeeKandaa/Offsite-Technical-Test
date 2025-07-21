using CC_TechTest_Backend.Models;

namespace CC_TechTest_Backend.Services
{
    public class InMemoryStorage : IDataStore
    {
        // Im-memory-store for data rows.
        private static readonly Dictionary<string, RowData> entries = new();

        // GET-ALL
        public IEnumerable<RowData> GetAll() => entries.Values;

        // SEARCH-BY
        public IEnumerable<RowData> GetByMpan(string mpan) => entries.Values.Where(entry => entry.MPAN.ToString().StartsWith(mpan));
        public IEnumerable<RowData> GetBySerial(string serial) => entries.Values.Where(entry => entry.MeterSerial.StartsWith(serial));
        public IEnumerable<RowData> GetByPostCode(string postCode) => entries.Values.Where(entry => entry.Postcode?.StartsWith(postCode) ?? false);
        public IEnumerable<RowData> GetByAddress(string address) => entries.Values.Where(entry => entry.AddressLine1?.Contains(address, StringComparison.OrdinalIgnoreCase) ?? false );
        public IEnumerable<RowData> GetByDate(DateTime date) => entries.Values.Where(entry => entry.DateOfInstallation == date);

        // Helper Functions
        public bool TryAdd(RowData entry)
        {
            // Validation better placed here?

            string primaryKey = entry.MPAN.ToString() + entry.MeterSerial.ToString() + entry.DateOfInstallation.ToString();
            if (entries.ContainsKey(primaryKey))
                return false;
            else
            {
                entries.Add(primaryKey, entry);
                return true;
            }
        }
        public bool TryAddMany(IEnumerable<RowData> entries, out List<RowData> failedRows)
        {
            failedRows = new();
            foreach (RowData entry in entries)
            {
                if (!TryAdd(entry))
                    failedRows.Append(entry);
            }
            if (failedRows.Count > 0)
                return false;
            else
                return true;
        }

        public bool TryRemove(RowData entry)
        {
            string primaryKey = entry.MPAN.ToString() + entry.MeterSerial.ToString() + entry.DateOfInstallation.ToString();
            if (entries.ContainsKey(primaryKey))
            {
                entries.Remove(primaryKey);
                return true;
            }
            else
                return false;
        }
    }
}
