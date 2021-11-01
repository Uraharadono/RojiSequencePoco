using System.Threading.Tasks;
using Core5ApiBoilerplate.Infrastructure;
using Core5ApiBoilerplate.Models;
using Core5ApiBoilerplate.Services.Blog;
using Core5ApiBoilerplate.Services.Blog.Dto;
using Core5ApiBoilerplate.Utility.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Core5ApiBoilerplate.Controllers
{
    // [Authorize] // TODO: Enable later
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : BaseController
    {
        private readonly IBlogService _blogService;

        public BlogsController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpPost, Route("AddBlogUsingUoWContext")]
        public async Task<IActionResult> AddBlogUsingUoWContext(BlogViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelStateExtensions.GetErrorMessage(ModelState));

            BlogDto dto = new BlogDto
            {
                Id = model.Id,
                Url = "AddBlogUsingUoWContext"
            };

            var addStatus = await _blogService.AddBlogUsingUoWContext(dto);
            return addStatus != null ? Ok() : StatusCode(500);
        }

        [HttpPost, Route("AddBlogUsingProperContext")]
        public async Task<IActionResult> AddBlogUsingProperContext(BlogViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelStateExtensions.GetErrorMessage(ModelState));

            BlogDto dto = new BlogDto
            {
                Id = model.Id,
                Url = "AddBlogUsingProperContext"
            };

            var addStatus = await _blogService.AddBlogUsingProperContext(dto);
            return addStatus != null ? Ok() : StatusCode(500);
        }

        [HttpPost, Route("AddBlogUsingUoW")]
        public async Task<IActionResult> AddBlogUsingUoW(BlogViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelStateExtensions.GetErrorMessage(ModelState));

            BlogDto dto = new BlogDto
            {
                Id = model.Id,
                Url = "AddBlogUsingUoW"
            };

            var addStatus = await _blogService.AddBlogUsingUoW(dto);
            return addStatus != null ? Ok() : StatusCode(500);
        }




        // Not important
        //[HttpGet, Route("GetBlogs")]
        //public async Task<IActionResult> GetBlogs()
        //{
        //    var blogs = await _blogService.GetBlogs();
        //    return blogs != null ? Ok(blogs) : StatusCode(500);
        //}

        //[HttpGet, Route("GetBlog")]
        //public async Task<IActionResult> GetBlog(long blogId)
        //{
        //    var blog = await _blogService.GetBlog(blogId);
        //    return blog != null ? Ok(blog) : StatusCode(500);
        //}

        //[HttpPost, Route("AddBlog")]
        //public async Task<IActionResult> AddBlog(BlogViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelStateExtensions.GetErrorMessage(ModelState));

        //    BlogDto dto = new BlogDto
        //    {
        //        Id = model.Id,
        //        Url = model.Url
        //    };

        //    var addStatus = await _blogService.AddBlog(dto);
        //    return addStatus != null ? Ok() : StatusCode(500);
        //}

        //[HttpPut, Route("UpdateBlog")]
        //public async Task<IActionResult> UpdateBlog(BlogViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelStateExtensions.GetErrorMessage(ModelState));

        //    BlogDto dto = new BlogDto
        //    {
        //        Id = model.Id,
        //        Url = model.Url
        //    };

        //    var addStatus = await _blogService.UpdateBlog(dto);
        //    return addStatus ? Ok() : StatusCode(500);
        //}
    }
}
