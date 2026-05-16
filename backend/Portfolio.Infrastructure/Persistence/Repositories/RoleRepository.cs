using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Aggregates.Roles;
using Portfolio.Domain.Aggregates.Users;

namespace Portfolio.Infrastructure.Persistence.Repositories;

public class RoleRepository(PortfolioDbContext context) : IRoleRepository
{
	private readonly PortfolioDbContext _context = context;

	public Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return _context.Roles
			.Include(r => r.Permissions)
			.Include(r => r.Users)
			.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
	}

	public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Roles
			.Include(r => r.Permissions)
			.ToListAsync(cancellationToken);
	}

	public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
	{
		return await _context.Roles
			.Include(r => r.Permissions)
			.FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
	}

	public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
	{
		await _context.Roles.AddAsync(role, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
	}

	public async Task UpdateAsync(Role role, CancellationToken cancellationToken = default)
	{
		_context.Roles.Update(role);
		await _context.SaveChangesAsync(cancellationToken);
	}

	public async Task DeleteAsync(Role role, CancellationToken cancellationToken = default)
	{
		_context.Roles.Remove(role);
		await _context.SaveChangesAsync(cancellationToken);
	}
}
