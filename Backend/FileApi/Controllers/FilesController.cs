using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using FileApi.Services;
using Microsoft.AspNetCore.Http;
using FileApi.Models;

namespace FileApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;

        public FilesController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        /// <summary>
        /// Retrieves all uploaded files with their metadata.
        /// </summary>
        /// <returns>A list of file metadata.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllFiles()
        {
            try
            {
                var files = await _mongoDbService.GetAllFilesAsync();

                if (files == null || files.Count == 0)
                    return NotFound(new { Message = "No files found." });

                return Ok(files);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Uploads a file to the server and stores it in MongoDB GridFS.
        /// </summary>
        /// <param name="file">The file to upload.</param>
        /// <param name="description">Optional description for the file.</param>
        /// <returns>Success message with file ID.</returns>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] string? description)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                using var stream = file.OpenReadStream();
                var fileId = await _mongoDbService.UploadFileAsync(stream, file.FileName, description ?? "");

                return Ok(new { Message = "File uploaded successfully.", FileId = fileId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Downloads a file by its ID.
        /// </summary>
        /// <param name="id">The ID of the file to download.</param>
        /// <returns>The file as a binary stream.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> DownloadFile(string id)
        {
            try
            {
                var (stream, fileName) = await _mongoDbService.DownloadFileAsync(id);
                return File(stream, "application/octet-stream", fileName);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves metadata for a specific file.
        /// </summary>
        /// <param name="id">The ID of the file.</param>
        /// <returns>File metadata including name, upload date, and description.</returns>
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetFileDetails(string id)
        {
            try
            {
                var fileDetails = await _mongoDbService.GetFileDetailsAsync(id);
                if (fileDetails == null)
                {
                    return NotFound(new { Message = "File not found" });
                }
                return Ok(fileDetails); // Return JSON response with file details
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Deletes a file by its ID.
        /// </summary>
        /// <param name="id">The ID of the file to delete.</param>
        /// <returns>Success message if deletion is successful.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(string id)
        {
            try
            {
                await _mongoDbService.DeleteFileAsync(id);
                return Ok(new { Message = "File deleted successfully." });
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
