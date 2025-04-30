using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Moq;
using FileApi.Models;
using FileApi.Services;

namespace FileApi.Tests
{
    public class MongoDbServiceTests : IDisposable
    {
        private readonly MongoDbService _service;
        private readonly string _connectionString = "mongodb://localhost:27017";
        private readonly string _databaseName = "TestFileDb";
        private readonly List<string> _uploadedFileIds = new List<string>();

        public MongoDbServiceTests()
        {
            // Create a real MongoDB service for integration testing
            _service = new MongoDbService(_connectionString, _databaseName);
        }

        [Fact]
        public async Task UploadFileAsync_ShouldReturnValidId()
        {
            // Arrange
            var fileName = "test-file.txt";
            var description = "Test file description";
            var content = "This is a test file content";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            // Act
            var fileId = await _service.UploadFileAsync(stream, fileName, description);

            // Assert
            Assert.NotNull(fileId);
            Assert.NotEmpty(fileId);
            _uploadedFileIds.Add(fileId); // Track for cleanup
        }

        [Fact]
        public async Task GetAllFilesAsync_ShouldReturnListOfFiles()
        {
            // Arrange
            var fileName = "test-getall-file.txt";
            var description = "Test file for GetAll";
            var content = "This is a test file content for GetAll test";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var fileId = await _service.UploadFileAsync(stream, fileName, description);
            _uploadedFileIds.Add(fileId);

            // Act
            var files = await _service.GetAllFilesAsync();

            // Assert
            Assert.NotNull(files);
            Assert.NotEmpty(files);
            Assert.Contains(files, f => GetPropertyValue(f, "FileName")?.ToString() == fileName);
        }

        [Fact]
        public async Task GetFileDetailsAsync_ShouldReturnFileDetails_WhenFileExists()
        {
            // Arrange
            var fileName = "test-details-file.txt";
            var description = "Test file for details";
            var content = "This is a test file content for details test";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var fileId = await _service.UploadFileAsync(stream, fileName, description);
            _uploadedFileIds.Add(fileId);

            // Act
            var fileDetails = await _service.GetFileDetailsAsync(fileId);

            // Assert
            Assert.NotNull(fileDetails);
            Assert.Equal(fileId, fileDetails.Id);
            Assert.Equal(fileName, fileDetails.FileName);
            Assert.NotNull(fileDetails.UploadDate);
            Assert.Equal(description, fileDetails.Metadata.Description);
        }

        [Fact]
        public async Task GetFileDetailsAsync_ShouldReturnNull_WhenFileDoesNotExist()
        {
            // Arrange
            var nonExistentId = ObjectId.GenerateNewId().ToString();

            // Act
            var fileDetails = await _service.GetFileDetailsAsync(nonExistentId);

            // Assert
            Assert.Null(fileDetails);
        }

        [Fact]
        public async Task DownloadFileAsync_ShouldReturnFileStreamAndName_WhenFileExists()
        {
            // Arrange
            var fileName = "test-download-file.txt";
            var description = "Test file for download";
            var content = "This is a test file content for download test";
            using var uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var fileId = await _service.UploadFileAsync(uploadStream, fileName, description);
            _uploadedFileIds.Add(fileId);

            // Act
            var (downloadStream, downloadedFileName) = await _service.DownloadFileAsync(fileId);

            // Assert
            Assert.NotNull(downloadStream);
            Assert.Equal(fileName, downloadedFileName);
            
            // Verify content
            using var reader = new StreamReader(downloadStream);
            var downloadedContent = await reader.ReadToEndAsync();
            Assert.Equal(content, downloadedContent);
        }

        [Fact]
        public async Task DownloadFileAsync_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
        {
            // Arrange
            var nonExistentId = ObjectId.GenerateNewId().ToString();

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => 
                _service.DownloadFileAsync(nonExistentId));
        }

        [Fact]
        public async Task GetFileMetadataAsync_ShouldReturnMetadata_WhenFileExists()
        {
            // Arrange
            var fileName = "test-metadata-file.txt";
            var description = "Test file for metadata";
            var content = "This is a test file content for metadata test";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var fileId = await _service.UploadFileAsync(stream, fileName, description);
            _uploadedFileIds.Add(fileId);

            // Act
            var metadata = await _service.GetFileMetadataAsync(fileId);

            // Assert
            Assert.NotNull(metadata);
            Assert.True(metadata.Contains("description"));
            Assert.Equal(description, metadata["description"].AsString);
        }

        [Fact]
        public async Task GetFileMetadataAsync_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
        {
            // Arrange
            var nonExistentId = ObjectId.GenerateNewId().ToString();

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => 
                _service.GetFileMetadataAsync(nonExistentId));
        }

        [Fact]
        public async Task DeleteFileAsync_ShouldRemoveFile()
        {
            // Arrange
            var fileName = "test-delete-file.txt";
            var description = "Test file for delete";
            var content = "This is a test file content for delete test";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var fileId = await _service.UploadFileAsync(stream, fileName, description);

            // Act
            await _service.DeleteFileAsync(fileId);

            // Assert - This should throw as the file no longer exists
            await Assert.ThrowsAsync<FileNotFoundException>(() => 
                _service.DownloadFileAsync(fileId));
        }

        // Helper method to get property value using reflection
        private static object GetPropertyValue(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            return property?.GetValue(obj);
        }

        // Clean up after tests
        public void Dispose()
        {
            // Delete any files created during tests
            foreach (var fileId in _uploadedFileIds)
            {
                try
                {
                    _service.DeleteFileAsync(fileId).Wait();
                }
                catch
                {
                    // Ignore errors during cleanup
                }
            }
        }
    }
}