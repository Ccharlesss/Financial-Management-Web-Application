

public class Investment
{
  public string Id { get; set; } = Guid.NewGuid().ToString(); 

  public string AssetName { get; set; } = null!;

  public decimal AmountInvested { get; set; }

  public decimal CurrentValue { get; set; }

  public DateTime PurchaseDate { get; set; }

  // Foreign key to ApplicationUser (custom user class)
  public string UserId { get; set; } = null!; 

  // Navigation property for ApplicationUser (custom user class)
  public AppUser User { get; set; } = null!; 

}