using System.Text.Json.Serialization;

public class Investment
{
  public string Id { get; set; } = Guid.NewGuid().ToString(); // PK

  public string AssetName { get; set; } = null!;

  public decimal AmountInvested { get; set; }

  public decimal CurrentValue { get; set; }

  public DateTime PurchaseDate { get; set; }

  
  public string UserId { get; set; } = null!; // FK

[JsonIgnore]  
  public virtual AppUser? User { get; set; } // Navigation

}