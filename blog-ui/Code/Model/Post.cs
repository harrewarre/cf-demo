using System;

namespace netcore_blog.Code.Model
{
    public class Post : PostMeta
    {
        public string MarkdownContent { get; set; }
    }

    public class PostMeta
    {
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public string[] Tags { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; }
    }
}