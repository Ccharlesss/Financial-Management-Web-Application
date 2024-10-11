public class FinanceAccount
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); // Primary key

    public string AccountName { get; set; } = null!;

    public string AccountType { get; set; } = null!; // Could be "Savings", "Checking", etc.

    public decimal Balance { get; set; }

    // Foreign key to ApplicationUser (custom user class)
    public string UserId { get; set; } = null!; 

    // Navigation property for ApplicationUser (custom user class)
    public AppUser User { get; set; } = null!; 

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
