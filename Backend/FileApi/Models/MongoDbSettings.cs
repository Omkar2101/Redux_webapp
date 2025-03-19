namespace FileApi.Models;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string FilesCollectionName { get; set; } = null!; 
}