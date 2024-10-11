

public class Budget
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  public string Category { get; set; } = null!;

  public decimal Limit { get; set; }

  public string UserId { get; set; } = null!;

  public AppUser User { get; set; } = null!;
}