using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using netcore_blog.Code.Model;
using Newtonsoft.Json;

namespace netcore_blog.Code
{
    public interface IBlog
    {
        Task<Post> GetPost(string slug);
        Task<PostMeta[]> GetLastestPosts(int count);
    }

    public class Blog : IBlog
    {
        private readonly string _webRootPath;
        private readonly string _postsPath;

        public Blog(IHostingEnvironment hostingEnvironment)
        {
            _webRootPath = hostingEnvironment.WebRootPath;
            _postsPath = Path.Join(_webRootPath, "content", "posts");
        }

        public async Task<Post> GetPost(string slug)
        {
            var postMeta = await GetSinglePostMeta(slug);

            if(postMeta == null) {
                return null;
            }

            var postContent = await File.ReadAllTextAsync(Path.Join(_postsPath, $"{slug}.md"));

            postMeta.MarkdownContent = postContent;
            return postMeta;
        }

        private async Task<Post> GetSinglePostMeta(string slug)
        {
            var indexJson = await File.ReadAllTextAsync(Path.Join(_webRootPath, "content", "index.json"));
            var posts = JsonConvert.DeserializeObject<Post[]>(indexJson);

            var meta = posts.OfType<Post>().FirstOrDefault(p => p.Slug == slug);

            return meta;
        }

        public async Task<PostMeta[]> GetLastestPosts(int count)
        {
            var indexJson = await File.ReadAllTextAsync(Path.Join(_webRootPath, "content", "index.json"));
            var posts = JsonConvert.DeserializeObject<PostMeta[]>(indexJson)
                .OrderByDescending(p => p.Created)
                .Take(count)
                .ToArray();

            return posts;
        }
    }
}