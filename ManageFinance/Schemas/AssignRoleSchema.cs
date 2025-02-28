public class AssignRoleSchema
{ // Usage: Assign a role for a new user:
  // Like AuthModel (not in DB):
  public string UserId { get; set; } = null!;
  public string RoleName { get; set; } = null!;
}