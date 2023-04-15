namespace InventarioViewModel
{
    public class Response
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public object? Result { get; set; }
        public int Id { get; set; }
        public string? Url { get; set; }
    }
}