public class QueryDto
{
    public int UserId { get; set; }
    public int? AccountId { get; set; }
    public DateTime? StartDate { get; set; } = DateTime.Now;
    public DateTime? EndDate { get; set; } = DateTime.Now;
    public List<int>? TagIds { get; set; }
}