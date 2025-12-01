namespace Explorer.Blog.API.Dtos
{
    public class CreateCommentDto
    {
        public long BlogId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}