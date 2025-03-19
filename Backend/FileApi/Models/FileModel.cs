using System;

namespace FileApi.Models
{
    public class FileModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString(); // Unique identifier for the file
        public string FileName { get; set; } = null!;               // Name of the uploaded file
        public string FilePath { get; set; } = null!;               // Path where the file is stored
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow; // Timestamp of when the file was uploaded
        public string Description { get; set; } = null!;            // Optional description of the file
                      
    }
}