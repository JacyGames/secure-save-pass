namespace secure_save_pass.Models
{

    public interface IResponse
    {
        abstract int StatusCode { get; set; }
        abstract string Message { get; set; }
    }

    public class DeletedEntityResponse : IResponse
    {
        public virtual IEnumerable<Guid> DeletedEntitiesId { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }

}
