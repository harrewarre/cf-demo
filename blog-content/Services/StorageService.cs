using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace blog_content.Services 
{
    public interface IStorageService
    {
        Task<string> GetBlogMarkdown(string slug);
    }

    public class StorageService : IStorageService
    {
        private readonly string _connectionString;

        private readonly CloudBlobClient _client;
        public StorageService(IOptions<CloudFoundryServicesOptions> serviceOptions)
        {   
            _connectionString = serviceOptions.Value.Services["user-provided"].First(s => s.Name == "content-storage").Credentials["connectionString"].Value;

            var account = CloudStorageAccount.Parse(_connectionString);
            _client = account.CreateCloudBlobClient();
        }

        public async Task<string> GetBlogMarkdown(string slug)
        {
            var container = _client.GetContainerReference("posts");
            var blobName = $"{slug}.md";

            var blob = container.GetBlobReference(blobName);

            if(!blob.Exists())
            {
                return null;
            }

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