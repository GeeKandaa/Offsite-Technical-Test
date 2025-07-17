using CC_TechTest_Backend.Configuration;
using CC_TechTest_Backend.Data;
using CC_TechTest_Backend.Models;
using CC_TechTest_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CC_TechTest_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly Config Configuration;
        private MeterDbContext Context;

        public DataController(IOptions<Config> config, MeterDbContext context)
        {
            Configuration = config.Value;
            Context = context;
        }

        [HttpGet("/data")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                if (Configuration.useInMemoryStorage)
                    return Ok(InMemoryStorage.GetAll());
                else
                    return Ok(await QueryContext.GetAll(Context));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("/data/mpan/{mpan}")]
        public async Task<IActionResult> GetByMpan(string mpan)
        {
            try
            {
                List<RowData> entries = new();
                if (Configuration.useInMemoryStorage)
                    entries = InMemoryStorage.GetByMpan(mpan).ToList();
                else
                    entries = await QueryContext.GetByMpan(mpan, Context);

                if (entries.Count == 0)
                    return NotFound();
                else
                    return Ok(entries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("/data/serial/{serial}")]
        public async Task<IActionResult> GetBySerial(string serial)
        {        
            try
            {
                List<RowData> entries = new();
                if (Configuration.useInMemoryStorage)
                    entries = InMemoryStorage.GetBySerial(serial).ToList();
                else
                    entries = await QueryContext.GetBySerial(serial, Context);
                
                if (entries.Count == 0)
                    return NotFound();
                else
                    return Ok(entries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("/data/installdate/{installDate}")]
        public async Task<IActionResult> GetByDate(string installDate)
        {
            try
            {
                RowData rowData = new RowData();
                if (!Validation.TryValidateDateOfInstallation(installDate, ref rowData))
                    return BadRequest("Invalid date format. Expected format is YYYYMMDD.");

                List<RowData> entries = new();
                if (Configuration.useInMemoryStorage)
                    entries = InMemoryStorage.GetByDate(rowData.DateOfInstallation).ToList();
                else
                    entries = await QueryContext.GetByDate(rowData.DateOfInstallation, Context);
                
                if (entries.Count == 0)
                    return NotFound();
                else
                    return Ok(entries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("/data/address/{address}")]
        public async Task<IActionResult> GetByAddress(string address)
        {
            try
            {
                List<RowData> entries = new();
                if (Configuration.useInMemoryStorage)
                    entries = InMemoryStorage.GetByAddress(address).ToList();
                else
                    entries = await QueryContext.GetByAddress(address, Context);

                if (entries.Count == 0)
                    return NotFound();
                else
                    return Ok(entries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("/data/postcode/{postcode}")]
        public async Task<IActionResult> GetByPostcode(string postcode)
        {
            try
            {
                List<RowData> entries = new();
                if (Configuration.useInMemoryStorage)
                    entries = InMemoryStorage.GetByPostCode(postcode).ToList();
                else
                    entries = await QueryContext.GetByPostcode(postcode, Context);
                
                if (entries.Count == 0)
                    return NotFound();
                else
                    return Ok(entries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
