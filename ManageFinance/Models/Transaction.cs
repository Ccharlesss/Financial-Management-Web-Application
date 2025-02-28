using System.Text.Json.Serialization;

public class Transaction
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); // PK

    public string Description { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateOnly Date { get; set; }

    public bool IsExpense { get; set; }

    // Foreign key to FinanceAccount
    public string FinanceAccountId { get; set; } = null!; 

    // Navigation property for FinanceAccount:
    // property in an entity class that allows you to navigate to related entities.
    // Usage: Data Relationship Representation, Easier Data Access
    [JsonIgnore]
    public virtual FinanceAccount? FinanceAccount { get; set; }
}
