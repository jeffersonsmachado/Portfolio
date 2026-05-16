using System;
using Portfolio.Domain.Aggregates.Permissions;
using Portfolio.Domain.Aggregates.Users;
using Portfolio.Domain.Shared;

namespace Portfolio.Application.Users;

public class PermissionService(IPermissionRepository permissionRepository) : IPermissionService
{
	private readonly IPermissionRepository _permissionRepository = permissionRepository;
    public async Task<Result<PermissionDto>> CreatePermissionAsync(string permissionName, CancellationToken cancellationToken = default)
    {
        var existing = await _permissionRepository.GetByNameAsync(permissionName, cancellationToken);
        if (existing != null)
        {
            return Result<PermissionDto>.Failure(new Error("PERMISSION_ALREADY_EXISTS", $"Permission '{ permissionName }' already exists."));
        }
        var permission = Permission.Create(permissionName);
        await _permissionRepository.AddAsync(permission, cancellationToken);
        return Result<PermissionDto>.Success(new PermissionDto { Id = permission.Id, Name = permission.Name });
    }

	public async Task<Result<PermissionDto>> GetPermissionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var permission = await _permissionRepository.GetByIdAsync(id, cancellationToken);
        if (permission == null)
        {
            return Result<PermissionDto>.Failure(new Error("PERMISSION_NOT_FOUND", $"Permission with ID '{ id }' not found."));
        }
        return Result<PermissionDto>.Success(new PermissionDto { Id = permission.Id, Name = permission.Name });
    }

	public async Task<Result<PermissionDto>> GetPermissionByNameAsync(string permissionName, CancellationToken cancellationToken = default)
    {
        var permission = await _permissionRepository.GetByNameAsync(permissionName, cancellationToken);
        if (permission == null)
        {
            return Result<PermissionDto>.Failure(new Error("PERMISSION_NOT_FOUND", $"Permission with name '{ permissionName }' not found."));
        }
        return Result<PermissionDto>.Success(new PermissionDto { Id = permission.Id, Name = permission.Name });
    }

	public async Task<Result<IEnumerable<PermissionDto>>> GetAllPermissionsAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await _permissionRepository.GetAllAsync(cancellationToken);
        return Result<IEnumerable<PermissionDto>>.Success(permissions.Select(p => new PermissionDto { Id = p.Id, Name = p.Name }));
    }

	public async Task<Result<PermissionDto>> DeletePermissionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var permission = await _permissionRepository.GetByIdAsync(id, cancellationToken);
        if (permission == null)
        {
            return Result<PermissionDto>.Failure(new Error("PERMISSION_NOT_FOUND", $"Permission with ID '{ id }' not found."));
        }
        await _permissionRepository.DeleteAsync(permission, cancellationToken);
        return Result<PermissionDto>.Success(new PermissionDto { Id = permission.Id, Name = permission.Name });
    }


}
