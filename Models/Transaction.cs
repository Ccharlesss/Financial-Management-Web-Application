// using System.Text.Json.Serialization;

public class Transaction
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); // PK

    public string Description { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    public string Category { get; set; } = null!;

    public bool IsExpense { get; set; }

    // Foreign key to FinanceAccount
    public string FinanceAccountId { get; set; } = null!; 

    // Navigation property for FinanceAccount:
    // property in an entity class that allows you to navigate to related entities.
    // Usage: Data Relationship Representation, Easier Data Access
    // [JsonIgnore]
    public FinanceAccount FinanceAccount { get; set; } = null!; 
}
