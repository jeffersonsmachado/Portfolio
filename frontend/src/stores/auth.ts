import { defineStore } from "pinia";
import { computed, ref } from "vue";

export interface ResourceCapabilities {
	canCreate: boolean;
	canRead: boolean;
	canUpdate: boolean;
	canDelete: boolean;
}

export type Capabilities = Record<string, ResourceCapabilities>;

export const useAuthStore = defineStore("auth", () => {
	const token = ref<string | null>(localStorage.getItem("token") || null);
	const capabilities = ref<Capabilities>({});

	const isLoggedIn = computed(() => !!token.value);

	function setToken(newToken: string | null) {
		token.value = newToken;
		if (newToken) {
			localStorage.setItem("token", newToken);
		} else {
			localStorage.removeItem("token");
		}
	}

	function clearToken() {
		setToken(null);
	}

	function logout() {
		setToken(null);
		localStorage.removeItem("token");
	}

	async function login(email: string, password: string) {
		const response = await fetch("https://localhost:7132/auth/login", {
			method: "POST",
			headers: {
				"Content-Type": "application/json",
			},
			body: JSON.stringify({
				email: email,
				password: password,
			}),
		});

		const data = await response.json();

		if (!response.ok) {
			if (data?.title) {
				if (data.title === "EMAIL_NOT_VERIFIED") {
					throw new Error(data.title);
				}
			} else if (data?.detail) {
				throw new Error(data.detail);
			} else {
				throw new Error("Login failed.");
			}
		}

		if (data?.token) {
			setToken(data.token);
		} else {
			clearToken();
			throw new Error("Login failed. No token received.");
		}

		capabilities.value = (data.capabilities as Capabilities) ?? {};
	}

	return {
		token,
		isLoggedIn,
		setToken,
		clearToken,
		logout,
		login,
		capabilities,
	};
});
