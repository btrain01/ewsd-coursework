using backend_app.Context;
using backend_app.DTOs;
using backend_app.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_app.Services
{
    public class BlogService(ApplicationDBContext applicationDBContext)
    {
        public async Task<BlogPost?> CreateBlogPost(int authorId, CreateBlogPostDTO dto)
        {
            var blogPost = new BlogPost
            {
                AuthorId = authorId,
                Title = dto.Title,
                Body = dto.Content,
            };

            applicationDBContext.BlogPosts.Add(blogPost);
            await applicationDBContext.SaveChangesAsync();
            return blogPost;
        }

        public async Task<List<BlogPost>> GetMyBlogPosts(int authorId)
        {
            return await applicationDBContext.BlogPosts
                .Where(bp => bp.AuthorId == authorId)
                .OrderByDescending(bp => bp.CreatedAt)
                .ToListAsync();
        }

        public async Task<BlogPost?> UpdateBlogPost(int authorId, int postId, UpdateBlogPostDTO dto)
        {
            var blogPost = await applicationDBContext.BlogPosts
                .Where(bp => bp.Id == postId && bp.AuthorId == authorId)
                .FirstOrDefaultAsync();

            if (blogPost == null)
                return null;

            blogPost.Title = dto.Title;
            blogPost.Body = dto.Content;

            await applicationDBContext.SaveChangesAsync();
            return blogPost;
        }

        public async Task<bool> DeleteBlogPost(int authorId, int postId)
        {
            var blogPost = await applicationDBContext.BlogPosts
                .Where(bp => bp.Id == postId && bp.AuthorId == authorId)
                .FirstOrDefaultAsync();

            applicationDBContext.BlogPosts.Remove(blogPost);
            await applicationDBContext.SaveChangesAsync();
            return true;
        }

        public async Task<BlogComment?> AddComment(int authorId, int postId, CreateBlogCommentDTO dto)
        {
            var comment = new BlogComment
            {
                PostId = postId,
                AuthorId = authorId,
                Body = dto.Content
            };

            applicationDBContext.BlogComments.Add(comment);
            await applicationDBContext.SaveChangesAsync();
            return comment;
        }
    }
}