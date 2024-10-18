public class UpdateRoleSchema
{
  // Usage: Update a role for a user:
  // Like AuthModel (not in DB):
  public string RoleId { get; set; } = null!;
  public string NewRoleName { get; set; } = null!;
}