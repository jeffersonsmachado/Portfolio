namespace Portfolio.Application.Users.Dtos;

public class RoleDto
{
	public Guid Id { get; set; }
	public string Name { get; set; } = null!;
	public List<PermissionDto> Permissions { get; set; } = new();
}
