using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blog_content.Config;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Microsoft.Extensions.Logging;

namespace blog_content.Services 
{
    public interface IStorageService
    {
        Task<string> GetBlogMarkdown(string slug);
        Task<string> GetBlogContentIndex();
    }

    public class StorageService : IStorageService
    {
        private readonly ILogger<StorageService> _logger;
        private readonly string _connectionString;
        private readonly CloudBlobClient _client;

        public StorageService(IOptions<StorageConfig> storageConfig, ILogger<StorageService> logger)
        {   
            _logger = logger;
            _connectionString = storageConfig.Value.ConnectionString;

            _logger.LogInformation(storageConfig.Value.ConnectionString);

            var account = CloudStorageAccount.Parse(_connectionString);
            _client = account.CreateCloudBlobClient();
        }

        public async Task<string> GetBlogContentIndex()
        {
            var container = _client.GetContainerReference("content");
            var blobName = "index.json";

            var blob = container.GetBlobReference(blobName);

            if (!blob.Exists())
            {
                _logger.LogWarning("Tried to load index.json but found nothing!");
                return null;
            }

            _logger.LogInformation("Loaded index.json!");
            return await GetBlobContentString(blob);
        }

        public async Task<string> GetBlogMarkdown(string slug)
        {
            var container = _client.GetContainerReference("posts");
            var blobName = $"{slug}.md";

            var blob = container.GetBlobReference(blobName);

            if (!blob.Exists())
            {
                _logger.LogWarning($"Tried to load post with slug {slug} but found nothing!");
                return null;
            }

            _logger.LogInformation($"Loaded post with slug {slug}");
            return await GetBlobContentString(blob);
        }

        private static async Task<string> GetBlobContentString(CloudBlob blob)
        {
            string content = string.Empty;
            using (var memoryStream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(memoryStream);
                content = Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            return content;
        }
    }
}