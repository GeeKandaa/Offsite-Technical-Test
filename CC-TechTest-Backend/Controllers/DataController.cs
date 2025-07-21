using CC_TechTest_Backend.Configuration;
using CC_TechTest_Backend.Data;
using CC_TechTest_Backend.Models;
using CC_TechTest_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CC_TechTest_Backend.Controllers
{
    /// <summary>
    /// Handles requests to "/data" route
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class DataController(IOptions<Config> config, IDataStore dataStore) : ControllerBase
    {
        private readonly Config Configuration = config.Value;
        private IDataStore DataStore = dataStore;

        /// <summary>
        /// Default behaviour of "/data" route.
        /// </summary>
        /// <returns>All stored dataRow entries</returns>
        [HttpGet("/data")]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(DataStore.GetAll());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Search-by MPAN endpoint: "/data/mpan/{mpan}"
        /// </summary>
        /// <param name="mpan">query string</param>
        /// <returns>List of data rows containing an MPAN that starts with query</returns>
        [HttpGet("/data/mpan/{mpan}")]
        public IActionResult GetByMpan(string mpan)
        {
            try
            {
                List<RowData> entries = DataStore.GetByMpan(mpan).ToList();

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

        /// <summary>
        /// Search-by Serial endpoint: "/data/serial/{serial}"
        /// </summary>
        /// <param name="serial">query string</param>
        /// <returns>List of data rows containing a serial that starts with query</returns>
        [HttpGet("/data/serial/{serial}")]
        public IActionResult GetBySerial(string serial)
        {        
            try
            {
                List<RowData> entries = DataStore.GetBySerial(serial).ToList();

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

        /// <summary>
        /// Search-by install date endpoint: "/data/installdate/{installDate}"
        /// </summary>
        /// <param name="installData">8-Digit date string: YYYYMMDD</param>
        /// <returns>List of data rows with date matching query</returns>
        [HttpGet("/data/installdate/{installDate}")]
        public IActionResult GetByDate(string installDate)
        {
            try
            {
                RowData rowData = new RowData();
                if (!Validation.TryValidateDateOfInstallation(installDate, ref rowData))
                    return BadRequest("Invalid date format. Expected format is YYYYMMDD.");

                List<RowData> entries = DataStore.GetByDate(rowData.DateOfInstallation).ToList();

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
        /// <summary>
        /// Search-by address endpoint: "/data/address/{address}"
        /// </summary>
        /// <param name="address">query string</param>
        /// <returns>List of data rows with an address containing query</returns>
        [HttpGet("/data/address/{address}")]
        public IActionResult GetByAddress(string address)
        {
            try
            {
                List<RowData> entries = DataStore.GetByAddress(address).ToList();

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

        /// <summary>
        /// Search-by postcode endpoint: "/data/postcode/{postcode}"
        /// </summary>
        /// <param name="serial">query string</param>
        /// <returns>List of data rows containing a postcode that starts with query</returns>
        [HttpGet("/data/postcode/{postcode}")]
        public IActionResult GetByPostcode(string postcode)
        {
            try
            {
                List<RowData> entries = DataStore.GetByPostCode(postcode).ToList();

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
