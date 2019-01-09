using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog_content.Services;
using Microsoft.AspNetCore.Mvc;

namespace blog_content.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public ContentController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        // GET api/values
        [HttpGet]
        [Route("{slug}")]
        public async Task<ActionResult<string>> Get(string slug)
        {
            var content = await _storageService.GetBlogMarkdown(slug);

            if (content == null)
            {
                return NotFound();
            }

            return content;
        }

        [HttpGet]
        [Route("index")]
        public async Task<ActionResult<string>> GetIndex()
        {
            var content = await _storageService.GetBlogContentIndex();

            if (content == null)
            {
                return NotFound();
            }

            return content;
        }
    }
}
