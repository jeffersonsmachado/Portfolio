using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Aggregates.Profiles;

namespace Portfolio.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Profile aggregate
/// </summary>
/// <param name="context"></param>
public class ProfileRepository(PortfolioDbContext context) : IProfileRepository
{
	private readonly PortfolioDbContext _context = context;

	public async Task<Profile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await _context.Profiles.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
	}

	public async Task<IEnumerable<Profile>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Profiles.Include(p => p.User).ToListAsync(cancellationToken);
	}

	public async Task<Profile?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
	{
		return await _context.Profiles.Include(p => p.User).FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
	}

	public async Task AddAsync(Profile profile, CancellationToken cancellationToken = default)
	{
		await _context.Profiles.AddAsync(profile, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
	}

	public async Task UpdateAsync(Profile profile, CancellationToken cancellationToken = default)
	{
		_context.Profiles.Update(profile);
		await _context.SaveChangesAsync(cancellationToken);
	}

	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var profile = await GetByIdAsync(id, cancellationToken);
		if (profile != null)
		{
			_context.Profiles.Remove(profile);
			await _context.SaveChangesAsync(cancellationToken);
		}
	}

	public async Task<Profile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		return await _context.Profiles.Include(p => p.User).FirstOrDefaultAsync(p => p.User.Id == userId, cancellationToken);
	}
}