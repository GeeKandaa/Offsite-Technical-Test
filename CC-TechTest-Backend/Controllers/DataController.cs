using CC_TechTest_Backend.Configuration;
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
        public DataController(IOptions<Config> config)
        {
            Configuration = config.Value;
        }

        [HttpGet("/data")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(InMemoryStorage.GetAll());
        }

        [HttpGet("/data/mpan/{mpan}")]
        public async Task<IActionResult> GetByMpan(string mpan)
        {
            var entries = InMemoryStorage.GetByMpan(mpan).ToList();
            if (entries.Count == 0)
                return NotFound();
            else
                return Ok(entries);
        }

        [HttpGet("/data/serial/{serial}")]
        public IActionResult GetBySerial(string serial)
        {
            var entries = InMemoryStorage.GetBySerial(serial).ToList();
            if (entries.Count == 0)
                return NotFound();
            else
                return Ok(entries);
        }
        [HttpGet("/data/installdate/{installDate}")]
        public IActionResult GetByDate(string installDate)
        {
            RowData rowData = new RowData();
            if (!Validation.TryValidateDateOfInstallation(installDate, ref rowData))
                return BadRequest("Invalid date format. Expected format is YYYYMMDD.");

            var entries = InMemoryStorage.GetByDate(rowData.DateOfInstallation).ToList();
            if (entries.Count == 0)
                return NotFound();
            else
                return Ok(entries);
        }
        [HttpGet("/data/address/{address}")]
        public IActionResult GetByAddress(string address)
        {
            var entries = InMemoryStorage.GetByAddress(address).ToList();
            if (entries.Count == 0)
                return NotFound();
            else
                return Ok(entries);
        }
        [HttpGet("/data/postcode/{postcode}")]
        public IActionResult GetByPostcode(string postcode)
        {
            var entries = InMemoryStorage.GetByPostCode(postcode).ToList();
            if (entries.Count == 0)
                return NotFound();
            else
                return Ok(entries);
        }
    }
}
