using CC_TechTest_Backend.Data;
using CC_TechTest_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace CC_TechTest_Backend.Services
{
    public class QueryContext : IDataStore
    {
        private MeterDbContext Context;
        public QueryContext(MeterDbContext context)
        {
            Context = context;
        }

        // GET-ALL
        public async Task<List<RowData>> GetAllAsync() => await Context.RegisteredMeters.ToListAsync();

        // SEARCH_BY
        public async Task<List<RowData>> GetByMpanAsync(string mpan) => await Context.RegisteredMeters.Where(entry => EF.Functions.Like(entry.MPAN.ToString(), $"{mpan}%")).ToListAsync();
        public async Task<List<RowData>> GetBySerialAsync(string serial) => await Context.RegisteredMeters.Where(entry => EF.Functions.Like(entry.MeterSerial, $"{serial}%")).ToListAsync();
        public async Task<List<RowData>> GetByDateAsync(DateTime date) => await Context.RegisteredMeters.Where(entry => entry.DateOfInstallation == date).ToListAsync();
        public async Task<List<RowData>> GetByAddressAsync(string address) => await Context.RegisteredMeters.Where(entry => EF.Functions.Like(entry.AddressLine1, $"%{address}%")).ToListAsync();
        public async Task<List<RowData>> GetByPostcodeAsync(string postcode) => await Context.RegisteredMeters.Where(entry => EF.Functions.Like(entry.Postcode, $"{postcode}%")).ToListAsync();

        public IEnumerable<RowData> GetAll() => GetAllAsync().Result;

        public IEnumerable<RowData> GetByMpan(string mpan) => GetByMpanAsync(mpan).Result;


        public IEnumerable<RowData> GetBySerial(string serial) => GetBySerialAsync(serial).Result;

        public IEnumerable<RowData> GetByPostCode(string postCode) => GetByPostcodeAsync(postCode).Result;

        public IEnumerable<RowData> GetByAddress(string address) => GetByAddressAsync(address).Result;

        public IEnumerable<RowData> GetByDate(DateTime date) => GetByDateAsync(date).Result;

        public bool TryAdd(RowData entry)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> TryAddManyAsync(IEnumerable<RowData> entries)
        {
            try
            {
                Context.RegisteredMeters.AddRange(entries);
                await Context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }
        public bool TryAddMany(IEnumerable<RowData> entries, out List<RowData> failedEntries)
        {
            failedEntries = new();
            return TryAddManyAsync(entries).Result;
        }

        public bool TryRemove(RowData entry)
        {
            throw new NotImplementedException();
        }
    }
}
