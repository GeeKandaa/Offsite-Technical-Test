using CC_TechTest_Backend.Configuration;
using CC_TechTest_Backend.Data;
using CC_TechTest_Backend.Models;
using CC_TechTest_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;

namespace CC_TechTest_Backend.Controllers
{
    /// <summary>
    /// Handles requests to "/file" route
    /// </summary>
    /// <param name="dataStore">injected IDataStore implementation</param>
    [ApiController]
    [Route("[controller]")]
    public class FileController(IDataStore dataStore) : ControllerBase
    {
        IDataStore DataStore = dataStore;

        /// <summary>
        /// Performs basic validation of file to ensure correct filetype and accessible content
        /// </summary>
        /// <param name="file">file to be validated</param>
        /// <param name="errorMessage">String outlining reason for failing validation</param>
        /// <returns>Boolean value indicating success</returns>
        private static bool TrySimpleFileValidation(IFormFile file, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Verify file exists and is correct filetype.
            // NB: This is purely defensive coding, these checks should be performed by the client which then cancels the API call.
            if (file == null || file.Length == 0)
            {
                errorMessage = "No file uploaded.";
                return false;
            }
            else if (!file.FileName.EndsWith(".txt"))
            {
                errorMessage = "Invalid file type. Only .txt files are allowed.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Performs basic validation of file contents and extracts row data from a file.
        /// </summary>
        /// <param name="file">file to be validated</param>
        /// <param name="errorMessage">String outlining reason for failing validation</param>
        /// <param name="contentLines">Array of data rows extracted from file</param>
        /// <returns>Boolean value indicating success</returns>
        private static bool TryUnpackDataFromFile(IFormFile file, out string errorMessage, out IEnumerable<string>? contentLines)
        {
            errorMessage = string.Empty;
            contentLines = [];

            try
            {
                contentLines = UnpackLinesLazily(file, out errorMessage);
                return (contentLines != null);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        private static IEnumerable<string>? UnpackLinesLazily(IFormFile file, out string errorMessage)
        {
            string errorBuffer;
            errorMessage = string.Empty;

            return ReadLines();

            IEnumerable<string> ReadLines()
            {
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);

                string? headerLine = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(headerLine))
                {
                    errorBuffer = "File must contain headers";
                    yield break;
                }

                string[] contentHeaders = headerLine.Split('|').Select(h => h.Trim()).ToArray();
                if (!RowData.RequiredHeaders.SequenceEqual(contentHeaders))
                {
                    errorBuffer = $"Mismatch between required headers and file headers. Required headers are: {string.Join(", ", RowData.RequiredHeaders)}";
                    yield break;
                }

                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                        yield return line;
                }
            }
        }

        /// <summary>
        /// Default behaviour of "/file" route.
        /// </summary>
        /// <param name="file">File to be uploaded, supplied in the request body</param>
        /// <returns>Object containing success message and the number of rows that passed/failed validation</returns>
        [HttpPost("/file")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UploadFile(IFormFile file)
        {
            try
            {
                if (!TrySimpleFileValidation(file, out string errorMessage))
                    return BadRequest(errorMessage);

                if(!TryUnpackDataFromFile(file, out errorMessage, out IEnumerable<string>? contentLines))
                    return BadRequest(errorMessage);

                List<RowData> contentFields = new();
                int successfulCount = 0;

                foreach(string line in contentLines)
                {
                    if (!Validation.TryParseDataRow(line, out RowData? rowData, out InvalidRowData? _))
                        continue;

                    contentFields.Add(rowData);
                    successfulCount++;
                }

                DataStore.TryAddMany(contentFields, out _);

                return Ok(new {
                    Message = $"\"{file.FileName}\" uploaded successfully.",
                    SuccessCount = successfulCount,
                    FailureCount = contentLines.Count() - successfulCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// File validation endpoint: "/file/validate"
        /// </summary>
        /// <param name="file">File to be validated, supplied in the request body</param>
        /// <returns>Object containing success message and the number of rows that passed/failed validation</returns>
        [HttpPost("/file/validate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ValidateFile(IFormFile file)
        {
            try
            {
                if (!TrySimpleFileValidation(file, out string errorMessage))
                return BadRequest(errorMessage);

                if(!TryUnpackDataFromFile(file, out errorMessage, out IEnumerable<string>? contentLines))
                    return BadRequest(errorMessage);

                List<InvalidRowData> failedValidation = new();
                int successfulCount = 0;
                foreach (string line in contentLines)
                {
                    if (Validation.TryParseDataRow(line, out RowData _, out InvalidRowData? invalidRowData) == false)
                    {
                        // Note invalid rows
                        failedValidation.Add(invalidRowData);
                        continue;
                    }

                    successfulCount++;
                }

                return Ok(new
                {
                    Message = $"Performed validation on \"{file.FileName}\".",
                    FailedRows = failedValidation,
                    SuccessCount = successfulCount,
                    FailureCount = contentLines.Count() - successfulCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}