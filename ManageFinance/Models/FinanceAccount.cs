using System.Text.Json.Serialization;


public class FinanceAccount
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); // PK

    public string AccountName { get; set; } = null!;

    public string AccountType { get; set; } = null!; // Could be "Savings", "Checking", etc.

    public decimal Balance { get; set; }

   
    public string UserId { get; set; } = null!; // FK

    [JsonIgnore]
    public virtual AppUser? User { get; set; }// Navigation

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
