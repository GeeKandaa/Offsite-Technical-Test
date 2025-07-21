using CC_TechTest_Backend.Models;

namespace CC_TechTest_Backend.Services
{
    public interface IDataStore
    {
        // GET-ALL
        public abstract IEnumerable<RowData> GetAll();

        // SEARCH-BY
        public abstract IEnumerable<RowData> GetByMpan(string mpan);
        public abstract IEnumerable<RowData> GetBySerial(string serial);
        public abstract IEnumerable<RowData> GetByPostCode(string postCode);
        public abstract IEnumerable<RowData> GetByAddress(string address);
        public abstract IEnumerable<RowData> GetByDate(DateTime date);

        // Helper Functions
        public abstract bool TryAdd(RowData entry);
        public abstract bool TryAddMany(IEnumerable<RowData> entries, out List<RowData> failedEntries);
        public abstract bool TryRemove(RowData entry);
    }
}
