using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Aggregates.Users;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for User entity
/// </summary>
public class UserRepository(PortfolioDbContext context) : IUserRepository
{
	private readonly PortfolioDbContext _context = context;

	public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return _context.Users
			.Include(u => u.Roles)
			.ThenInclude(u => u.Permissions).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
	}

	public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Users.ToListAsync(cancellationToken);
	}

	public async Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
	{
		var query = _context.Users.Where(u => u.Name == name);
		if (await query.AnyAsync(cancellationToken))
		{
			return await query.FirstAsync(cancellationToken);
		}
		return null;
	}

	public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		var emailObj = Email.Create(email);
		return await _context.Users.Include(u => u.Roles)
								   .ThenInclude(r => r.Permissions)
								   .FirstOrDefaultAsync(u => u.Email == emailObj, cancellationToken);
	}

	public Task AddAsync(User user, CancellationToken cancellationToken = default)
	{
		_context.Users.Add(user);
		return _context.SaveChangesAsync(cancellationToken);
	}

	public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
	{
		_context.Users.Update(user);
		return _context.SaveChangesAsync(cancellationToken);
	}

	public Task DeleteAsync(User user, CancellationToken cancellationToken = default)
	{
		_context.Users.Remove(user);
		return _context.SaveChangesAsync(cancellationToken);
	}

	public async Task<IEnumerable<User>> GetUsersInRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.Where(u => u.Roles.Any(r => r.Id == roleId))
			.ToListAsync(cancellationToken);
	}

	public async Task<IEnumerable<User>> GetAllWithRolesAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.Include(u => u.Roles)
			.ThenInclude(r => r.Permissions)
			.ToListAsync(cancellationToken);
	}

	public Task<User?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return _context.Users
			.Include(u => u.Roles)
			.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
	}
}
