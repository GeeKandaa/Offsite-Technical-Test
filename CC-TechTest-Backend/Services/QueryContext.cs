using CC_TechTest_Backend.Data;
using CC_TechTest_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CC_TechTest_Backend.Services
{
    public static class QueryContext
    {
        public async static Task<List<RowData>> GetAll(MeterDbContext context) => await context.RegisteredMeters.ToListAsync();
        public async static Task<List<RowData>> GetByMpan(string mpan, MeterDbContext context) => await context.RegisteredMeters.Where(entry => EF.Functions.Like(entry.MPAN.ToString(), $"{mpan}%")).ToListAsync();
        public async static Task<List<RowData>> GetBySerial(string serial, MeterDbContext context) => await context.RegisteredMeters.Where(entry => EF.Functions.Like(entry.MeterSerial, $"{serial}%")).ToListAsync();
        public async static Task<List<RowData>> GetByDate(DateTime date, MeterDbContext context) => await context.RegisteredMeters.Where(entry => entry.DateOfInstallation == date).ToListAsync();
        public async static Task<List<RowData>> GetByAddress(string address, MeterDbContext context) => await context.RegisteredMeters.Where(entry => EF.Functions.Like(entry.AddressLine1, $"%{address}%")).ToListAsync();
        public async static Task<List<RowData>> GetByPostcode(string postcode, MeterDbContext context) => await context.RegisteredMeters.Where(entry => EF.Functions.Like(entry.Postcode, $"{postcode}%")).ToListAsync();
    }
}
