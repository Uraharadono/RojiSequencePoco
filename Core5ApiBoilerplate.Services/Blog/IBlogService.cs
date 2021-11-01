using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core5ApiBoilerplate.DbContext.Infrastructure;
using Core5ApiBoilerplate.Infrastructure.Repository;
using Core5ApiBoilerplate.Infrastructure.Services;
using Core5ApiBoilerplate.Services.Blog.Dto;
using NLog;

namespace Core5ApiBoilerplate.Services.Blog
{
    public interface IBlogService : IService
    {
        Task<BlogDto> AddBlogUsingUoWContext(BlogDto dto);
        Task<BlogDto> AddBlogUsingProperContext(BlogDto dto);
        Task<BlogDto> AddBlogUsingUoW(BlogDto dto);


        // Not important stuff
        Task<List<BlogDto>> GetBlogs();
        Task<BlogDto> GetBlog(long blogId);
        Task<BlogDto> AddBlog(BlogDto dto);
        Task<bool> UpdateBlog(BlogDto dto);
    }

    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _uow;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public BlogService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<BlogDto> AddBlogUsingUoWContext(BlogDto dto)
        {
            try
            {
                var ctx = _uow.Context as Net5BoilerplateContext;
                ctx.Blogs.Add(new() { Url = dto.Url });
                // _uow.Commit(); // doesn't work
                ctx.SaveChanges(); // doesn't work, either

                return dto;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }
        }

        public async Task<BlogDto> AddBlogUsingProperContext(BlogDto dto)
        {
            try
            {
                await using var ctx = Net5BoilerplateContext.Create("Server=.;Database=BloggingDb;Trusted_Connection=True;MultipleActiveResultSets=True;", 1);
                // ctx.Database.EnsureDeletedAsync();
                // await ctx.Database.EnsureCreatedAsync();

                ctx.Blogs.Add(new() { Url = dto.Url });
                await ctx.SaveChangesAsync();

                return dto;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }
        }

        public async Task<BlogDto> AddBlogUsingUoW(BlogDto dto)
        {
            try
            {
                // var nextId = _uow.Query<DbContext.Entities.Blog>().OrderByDescending(s => s.Oid).First().Oid + 1;

                var dbBlog = new DbContext.Entities.Blog
                {
                    // Oid = nextId,
                    Url = dto.Url,
                };
                _uow.Context.Set<DbContext.Entities.Blog>().Add(dbBlog);
                _uow.Commit();

                return dto;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }
        }


        // Not important
        public async Task<List<BlogDto>> GetBlogs()
        {
            try
            {
                var blogs = _uow.Query<DbContext.Entities.Blog>().ToList();

                if (!blogs.Any())
                    return null;

                return blogs.Select(s => new BlogDto { Id = s.Oid, Url = s.Url }).ToList();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }
        }

        public async Task<BlogDto> GetBlog(long blogId)
        {
            try
            {
                var blog = _uow.Query<DbContext.Entities.Blog>(s => s.Oid == blogId).FirstOrDefault();
                if (blog == null)
                    return null;

                return new BlogDto { Id = blog.Oid, Url = blog.Url };
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }
        }

        public async Task<BlogDto> AddBlog(BlogDto dto)
        {
            try
            {
                // var nextId = _uow.Query<DbContext.Entities.Blog>().OrderByDescending(s => s.Oid).First().Oid + 1;

                //var dbBlog = new DbContext.Entities.Blog
                //{
                //    // Oid = nextId,
                //    Url = dto.Url,
                //};

                //_uow.Context.Set<DbContext.Entities.Blog>().Add(dbBlog);
                //_uow.Commit();
                
                // dto.Id = dbBlog.Oid;


                var ctx = _uow.Context as Net5BoilerplateContext;
                ctx.Blogs.Add(new() {Url = "foo"});

                _uow.Commit();

                return dto;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }
        }

        public async Task<bool> UpdateBlog(BlogDto dto)
        {
            try
            {
                var dbBlog = _uow.Query<DbContext.Entities.Blog>(s => s.Oid == dto.Id).FirstOrDefault();

                if (dbBlog == null)
                    return false;

                dbBlog.Oid = dto.Id;
                dbBlog.Url = dto.Url;
                _uow.Commit();

                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return false;
            }
        }
    }
}
