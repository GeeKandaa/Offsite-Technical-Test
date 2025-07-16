using CC_TechTest_Backend.Models;
using CC_TechTest_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CC_TechTest_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private bool TrySimpleFileValidation(IFormFile file, out string errorMessage)
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

        private bool TryUnpackDataFromFile(IFormFile file, out string errorMessage, out string[] contentLines)
        {
            errorMessage = string.Empty;
            contentLines = [];

            using (StreamReader reader = new StreamReader(file.OpenReadStream()))
            {
                string content = reader.ReadToEnd();
                if (string.IsNullOrWhiteSpace(content))
                {
                    errorMessage = "File is empty.";
                    return false;
                }

                contentLines = content.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                if (contentLines.Length < 2)
                {
                    errorMessage = "File must contain headers and at least one data row.";
                    return false;
                }

                string[] contentHeaders = contentLines[0].Split('|').Select(header => header.Trim()).ToArray();
                if (!RowData.RequiredHeaders.SequenceEqual(contentHeaders))
                {
                    errorMessage = $"Mismatch between required headers and file headers. Required headers are: {string.Join(", ", RowData.RequiredHeaders)}";
                    return false;
                }
            }

            return true;
        }

        [HttpPost("/file")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (!TrySimpleFileValidation(file, out string errorMessage))
                    return BadRequest(errorMessage);

                if(!TryUnpackDataFromFile(file, out errorMessage, out string[] contentLines))
                    return BadRequest(errorMessage);

                List<RowData> contentFields = new();
                int successfulCount = 0;
                for (int i = 1; i < contentLines.Length; i++)
                {
                    string[] fieldData = contentLines[i].Split('|');

                    if (Validation.TryParseDataRow(contentLines[i], out RowData? rowData, out InvalidRowData _) == false)
                    {
                        // Skip invalid rows
                        continue;
                    }

                    contentFields.Add(rowData);
                    successfulCount++;
                }

                foreach (RowData row in contentFields)
                {
                    InMemoryStorage.Add(row);
                }

                return Ok(new {
                    Message = $"\"{file.FileName}\" uploaded successfully.",
                    SuccessCount = successfulCount,
                    FailureCount = contentLines.Length - successfulCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("/file/validate")]
        public async Task<IActionResult> ValidateFile(IFormFile file)
        {
            try
            {
                if (!TrySimpleFileValidation(file, out string errorMessage))
                return BadRequest(errorMessage);

                if(!TryUnpackDataFromFile(file, out errorMessage, out string[] contentLines))
                    return BadRequest(errorMessage);

                List<InvalidRowData> failedValidation = new();
                int successfulCount = 0;
                for (int i = 1; i < contentLines.Length; i++)
                {
                    if (Validation.TryParseDataRow(contentLines[i], out RowData _, out InvalidRowData invalidRowData) == false)
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
                    FailureCount = contentLines.Length - successfulCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}