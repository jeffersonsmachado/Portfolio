import { defineStore } from "pinia";
import { ref } from "vue";

export interface RoleRef {
	id: string;
	name: string;
}

export interface UserDto {
	id: string;
	username: string;
	email: string;
	roles: RoleRef[];
}

export const useUserStore = defineStore("user", () => {
	const users = ref<UserDto[]>([]);
	const usersWithRoles = ref<UserDto[]>([]);

	async function fetchUsers(): Promise<UserDto[]> {
		const response = await fetch("https://localhost:7132/users", {
			method: "GET",
			headers: {
				"Content-Type": "application/json",
				Authorization: `Bearer ${localStorage.getItem("token")}`,
			},
		});

		if (!response.ok) {
			throw new Error("Failed to fetch users", {
				cause: response,
			});
		}

		const data = await response.json();
		users.value = data;
		return data;
	}

	async function fetchUsersWithRoles(): Promise<UserDto[]> {
		const response = await fetch("https://localhost:7132/users/with-roles", {
			method: "GET",
			headers: {
				"Content-Type": "application/json",
				Authorization: `Bearer ${localStorage.getItem("token")}`,
			},
		});

		if (!response.ok) {
			throw new Error("Failed to fetch users", {
				cause: response,
			});
		}

		const data = await response.json();
		usersWithRoles.value = data;
		return data;
	}

	return {
		users,
		usersWithRoles,
		fetchUsers,
		fetchUsersWithRoles,
	};
});
