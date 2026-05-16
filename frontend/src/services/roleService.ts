import { defineStore } from "pinia";
import { ref } from "vue";

export interface RoleDto {
	id: string;
	name: string;
	permissions: PermissionDto[];
}

export interface PermissionDto {
	id: string;
	name: string;
}

export const useRoleStore = defineStore("role", () => {
	const roles = ref<RoleDto[]>([]);

	const permissions = ref<PermissionDto[]>([]);

	async function fetchRoles(): Promise<RoleDto[]> {
		const response = await fetch("https://localhost:7132/roles", {
			method: "GET",
			headers: {
				"Content-Type": "application/json",
				Authorization: `Bearer ${localStorage.getItem("token")}`,
			},
		});

		if (!response.ok) {
			throw new Error("Failed to fetch roles", {
				cause: response,
			});
		}

		const data = await response.json();
		roles.value = data;
		return data;
	}

	async function assignRoleToUser(
		roleId: string,
		userId: string,
	): Promise<void> {
		const response = await fetch(
			`https://localhost:7132/roles/${roleId}/assign/${userId}`,
			{
				method: "POST",
				headers: {
					"Content-Type": "application/json",
					Authorization: `Bearer ${localStorage.getItem("token")}`,
				},
			},
		);

		console.log(response);

		if (!response.ok) {
			throw new Error("Failed to assign role to user", {
				cause: response,
			});
		}
	}

	async function setUserRoles(
		userId: string,
		roleIds: string[],
	): Promise<RoleDto[]> {
		const response = await fetch(
			`https://localhost:7132/users/${userId}/roles`,
			{
				method: "PUT",
				headers: {
					"Content-Type": "application/json",
					Authorization: `Bearer ${localStorage.getItem("token")}`,
				},
				body: JSON.stringify({ roleIds }),
			},
		);

		if (!response.ok) {
			throw new Error("Failed to set user roles", {
				cause: response,
			});
		}

		return response.json();
	}

	async function fetchPermissions(): Promise<PermissionDto[]> {
		const response = await fetch("https://localhost:7132/permissions", {
			method: "GET",
			headers: {
				"Content-Type": "application/json",
				Authorization: `Bearer ${localStorage.getItem("token")}`,
			},
		});

		if (!response.ok) {
			throw new Error("Failed to fetch permissions", {
				cause: response,
			});
		}
		
		const data = await response.json();
		permissions.value = data;
		return data;
	}

	async function setRolePermissions(roleId: string, permissionIds: string[]): Promise<RoleDto> {
		const response = await fetch(
			`https://localhost:7132/roles/${roleId}/permissions`,
			{
				method: "PUT",
				headers: {
					"Content-Type": "application/json",
					Authorization: `Bearer ${localStorage.getItem("token")}`,
				},
				body: JSON.stringify({ permissionIds }),
			},
		);

		if (!response.ok) {
			throw new Error("Failed to set role permissions", {
				cause: response,
			});
		}

		const data = await response.json();
		return data;
	}

	return {
		roles,
		permissions,
		fetchRoles,
		assignRoleToUser,
		setUserRoles,
		fetchPermissions,
		setRolePermissions,
	};
});
