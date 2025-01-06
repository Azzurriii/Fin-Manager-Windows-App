namespace Fin_Manager_v2.DTO;

public class MailerDto
{
    public int UserId { get; set; }
    public string UserEmail { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public string Period { get; set; }
    public DateTime StartDate { get; set; }
    public string PaymentLink { get; set; }
}