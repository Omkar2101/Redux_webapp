using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileApi.Models;

namespace FileApi.Services
{
    /// <summary>
    /// Service for managing file operations in MongoDB using GridFS.
    /// Provides functionalities to upload, download, retrieve metadata, and delete files.
    /// </summary>
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;
        private readonly GridFSBucket _gridFsBucket;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbService"/> class.
        /// Establishes a connection to MongoDB and configures GridFS.
        /// </summary>
        public MongoDbService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client.GetDatabase("FileDatabase");
            _gridFsBucket = new GridFSBucket(_database);
        }

        /// <summary>
        /// Uploads a file to MongoDB GridFS.
        /// </summary>
        /// <param name="fileStream">The file stream to upload.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="description">An optional description for the file.</param>
        /// <returns>The ID of the uploaded file.</returns>
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string description)
        {
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument
                {
                    { "description", description }
                }
            };

            var fileId = await _gridFsBucket.UploadFromStreamAsync(fileName, fileStream, options);
            return fileId.ToString();
        }

        /// <summary>
        /// Retrieves all stored files from GridFS along with their metadata.
        /// </summary>
        /// <returns>A list of file metadata objects.</returns>
        public async Task<List<object>> GetAllFilesAsync()
        {
            var filter = Builders<GridFSFileInfo<ObjectId>>.Filter.Empty;
            using var cursor = await _gridFsBucket.FindAsync(filter);
            var files = await cursor.ToListAsync();

            if (files == null || files.Count == 0)
            {
                return new List<object>(); // Return empty list instead of null
            }

            var result = files.Select(file => new
            {
                Id = file.Id.ToString(),
                FileName = file.Filename,
                UploadDate = file.UploadDateTime,
                Metadata = new
                {
                    Description = file.Metadata.Contains("description") && file.Metadata["description"].IsString
                        ? file.Metadata["description"].AsString
                        : ""
                }
            }).ToList();

            return result.Cast<object>().ToList();
        }

        /// <summary>
        /// Retrieves details of a specific file from GridFS, including metadata.
        /// </summary>
        /// <param name="id">The ID of the file.</param>
        /// <returns>A <see cref="FileDetailsDto"/> containing file details and metadata.</returns>
        public async Task<FileDetailsDto> GetFileDetailsAsync(string id)
        {
            var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", new ObjectId(id));
            var fileInfo = await _gridFsBucket.Find(filter).FirstOrDefaultAsync();

            if (fileInfo == null) return null;

            return new FileDetailsDto
            {
                Id = fileInfo.Id.ToString(),
                FileName = fileInfo.Filename,
                UploadDate = fileInfo.UploadDateTime,
                Metadata = new MetadataDto // MetadataDto instead of an anonymous object
                {
                    Description = fileInfo.Metadata.Contains("description") ? fileInfo.Metadata["description"].AsString : ""
                }
            };
        }

        /// <summary>
        /// Downloads a file from GridFS.
        /// </summary>
        /// <param name="fileId">The ID of the file to download.</param>
        /// <returns>A tuple containing the file stream and filename.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the file is not found in GridFS.</exception>
        public async Task<(Stream, string)> DownloadFileAsync(string fileId)
        {
            var objectId = ObjectId.Parse(fileId);
            var fileInfo = await _gridFsBucket.Find(Builders<GridFSFileInfo<ObjectId>>.Filter.Eq("_id", objectId)).FirstOrDefaultAsync();

            if (fileInfo == null)
                throw new FileNotFoundException("File not found in GridFS");

            var stream = await _gridFsBucket.OpenDownloadStreamAsync(objectId);
            return (stream, fileInfo.Filename);
        }

        /// <summary>
        /// Retrieves metadata of a specific file from GridFS.
        /// </summary>
        /// <param name="fileId">The ID of the file.</param>
        /// <returns>A BSON document containing file metadata.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the file is not found.</exception>
        public async Task<BsonDocument> GetFileMetadataAsync(string fileId)
        {
            var objectId = ObjectId.Parse(fileId);
            var fileInfo = await _gridFsBucket.Find(Builders<GridFSFileInfo<ObjectId>>.Filter.Eq("_id", objectId)).FirstOrDefaultAsync();

            if (fileInfo == null)
                throw new FileNotFoundException("File not found");

            return fileInfo.Metadata;
        }

        /// <summary>
        /// Deletes a file from GridFS.
        /// </summary>
        /// <param name="fileId">The ID of the file to delete.</param>
        public async Task DeleteFileAsync(string fileId)
        {
            var objectId = ObjectId.Parse(fileId);
            await _gridFsBucket.DeleteAsync(objectId);
        }
    }
}
