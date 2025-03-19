using System;
using System.Collections.Generic;

namespace FileApi.Models  // Replace 'YourNamespace' with your actual namespace
{
    public class FileDetailsDto
    {
        public string ? Id { get; set; }
        public string ?FileName { get; set; }
        public DateTime ? UploadDate { get; set; }
        public MetadataDto ? Metadata { get; set; }
    }

    public class MetadataDto
    {
        public string ? Description { get; set; }
       
    }
}
