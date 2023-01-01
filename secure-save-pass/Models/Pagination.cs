namespace secure_save_pass.Models
{
    public class Pagination
    {
        public int AllItemsCount { get; set; }
        public int PageCount { get; set; }
        public int PageNumber { get; set; }
    }

    public interface IPaginatedResponse
    {
        abstract Pagination Pagination { get; set; }
    }
}
