using backend_app.Attributes;
using backend_app.DTOs;
using backend_app.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ServiceFilter(typeof(AuthenticationAttribute))]
    public class BlogController(BlogService blogService) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<ActionResult> CreateBlogPost([FromBody] CreateBlogPostDTO dto)
        {
            var authorId = (int)HttpContext.Items["UserId"]!;
            var post = await blogService.CreateBlogPost(authorId, dto);
            if (post == null)
                return BadRequest("Blog post could not be created");
            return Ok(post);
        }

        [HttpGet("my-posts")]
        public async Task<ActionResult> GetMyBlogPosts()
        {
            var authorId = (int)HttpContext.Items["UserId"]!;
            var posts = await blogService.GetMyBlogPosts(authorId);
            return Ok(posts);
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult> GetStudentBlogPosts(int studentId)
        {
            var tutorId = (int)HttpContext.Items["UserId"]!;
            var posts = await blogService.GetStudentBlogPosts(tutorId, studentId);
            return Ok(posts);
        }

        [HttpPut("update/{postId}")]
        public async Task<ActionResult> UpdateBlogPost(int postId, [FromBody] UpdateBlogPostDTO dto)
        {
            var authorId = (int)HttpContext.Items["UserId"]!;
            var post = await blogService.UpdateBlogPost(authorId, postId, dto);
            if (post == null)
                return NotFound("Blog post not found or you are not the author");
            return Ok(post);
        }

        [HttpDelete("delete/{postId}")]
        public async Task<ActionResult> DeleteBlogPost(int postId)
        {
            var authorId = (int)HttpContext.Items["UserId"]!;
            var result = await blogService.DeleteBlogPost(authorId, postId);
            if (!result)
                return NotFound("Blog post not found or you are not the author");
            return Ok("Blog post deleted successfully");
        }

        [HttpPost("{postId}/comment")]
        public async Task<ActionResult> AddComment(int postId, [FromBody] CreateBlogCommentDTO dto)
        {
            var authorId = (int)HttpContext.Items["UserId"]!;
            var comment = await blogService.AddComment(authorId, postId, dto);
            if (comment == null)
                return NotFound("Blog post not found");
            return Ok(comment);
        }
    }
}