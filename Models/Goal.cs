

public class Goal
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  public string GoalTitle { get; set; } = null!;

  public decimal TargetAmount { get; set; }

  public decimal CurrentAmount  { get; set; }

  public DateTime TargetDate { get; set; }

  public string UserId { get; set; } = null!;

  public AppUser User { get; set; } =null!; 
}