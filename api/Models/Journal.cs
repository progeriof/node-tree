public class Journal {
    public int id { get; set; }
    public string text { get; set; }
    public long eventId { get; set; }
    public string queryParams { get; set; }
    public string bodyParams { get; set; }
    public string stackTrace { get; set; }
    public DateTime createdAt { get; set; }
}