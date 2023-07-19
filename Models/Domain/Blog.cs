namespace Coffeeroom.Models.Domain
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UrlHandle { get; set; }
        public int CategoryId { get; set; }
        public int AuthorId { get; set; }
        public string Tags{ get; set; }
        public bool IsActive { get; set; }
        public DateTime DatePosted { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
