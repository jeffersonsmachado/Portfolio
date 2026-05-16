using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Aggregates.Permissions;
using Portfolio.Domain.Aggregates.Users;
namespace Portfolio.Infrastructure.Persistence.Repositories;

public class PermissionRepository(PortfolioDbContext context) : IPermissionRepository
{
	private readonly PortfolioDbContext _context = context;

	public Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return _context.Permissions.FindAsync([id], cancellationToken).AsTask();
	}

	public async Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Permissions.ToListAsync(cancellationToken);
	}

	public Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
	{
		return _context.Permissions.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
	}

	public Task AddAsync(Permission permission, CancellationToken cancellationToken = default)
	{
		_context.Permissions.Add(permission);
		return _context.SaveChangesAsync(cancellationToken);
	}

	public Task UpdateAsync(Permission permission, CancellationToken cancellationToken = default)
	{
		_context.Permissions.Update(permission);
		return _context.SaveChangesAsync(cancellationToken);
	}

	public Task DeleteAsync(Permission permission, CancellationToken cancellationToken = default)
	{
		_context.Permissions.Remove(permission);
		return _context.SaveChangesAsync(cancellationToken);
	}


}
