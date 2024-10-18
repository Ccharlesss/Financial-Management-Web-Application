// using System.Text.Json.Serialization;

public class Budget
{
  public string Id { get; set; } = Guid.NewGuid().ToString(); // PK

  public string Category { get; set; } = null!;

  public decimal Limit { get; set; }

//   public string UserId { get; set; } = null!; // FK

// [JsonIgnore]
//   public AppUser User { get; set; } = null!; // NavigationLink
}