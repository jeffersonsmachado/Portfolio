import { create } from "zustand";
import { persist } from "zustand/middleware";

interface User {
	id: string;
	username: string;
	email: string;
}

interface Capabilities {
	roles: Record<string, boolean>;
	profile: Record<string, boolean>;
	user: Record<string, boolean>;
}

interface AuthState {
	token: string | null;
	user: User | null;
	capabilities: Capabilities | null;
	isAuthenticated: boolean;
	login: (data: {
		token: string;
		user: User;
		capabilities: Capabilities;
	}) => void;
	logout: () => void;
}

export const useAuthStore = create<AuthState>()(
	persist(
		(set) => ({
			token: null,
			user: null,
			capabilities: null,
			isAuthenticated: false,

			login: (data) =>
				set({
					token: data.token,
					user: data.user,
					capabilities: data.capabilities,
					isAuthenticated: true,
				}),

			logout: () =>
				set({
					token: null,
					user: null,
					capabilities: null,
					isAuthenticated: false,
				}),
		}),
		{
			name: "auth-storage",
		},
	),
);
