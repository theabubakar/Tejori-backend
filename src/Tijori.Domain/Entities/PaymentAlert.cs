using Tijori.Domain.Common;
using Tijori.Domain.Enums;

namespace Tijori.Domain.Entities;

public class PaymentAlert : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "RS";
    public int ProgressPercentage { get; set; }
    public PaymentAlertStatus Status { get; set; } = PaymentAlertStatus.Next;

    public User User { get; set; } = null!;
    public Project? Project { get; set; }
}
