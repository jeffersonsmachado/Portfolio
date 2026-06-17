import { create } from "zustand";
import { persist } from "zustand/middleware";
import { api } from "./api";
import { isAxiosError } from "axios";

interface Credentials {
	email: string;
	password: string;
}

interface ResourceCapabilities {
	canCreate: boolean;
	canRead: boolean;
	canUpdate: boolean;
	canDelete: boolean;
}

interface User {
	id: string;
	username: string;
	email: string;
}

interface LoginResponse {
	token: string;
	capabilities: ResourceCapabilities;
	user: User;
}

interface AuthState {
	user: User | null;
	isAuthenticated: boolean;
	isLoading: boolean;
	error: string | null;
	token: string | null;
	capabilities: ResourceCapabilities | null;
	login: (credentials: Credentials) => Promise<boolean>;
	logout: () => void;
}

export const useAuthStore = create<AuthState>()(
	persist(
		(set) => ({
			user: null,
			isAuthenticated: false,
			isLoading: false,
			error: null,
			token: null,
			capabilities: null,
			login: async (credentials: Credentials) => {
				set({ isLoading: true, error: null });

				try {
					const response = await api.post<LoginResponse>(`/auth/login`, {
						email: credentials.email,
						password: credentials.password,
					});

					const data = response.data;

					set({
						user: data.user,
						isAuthenticated: true,
						isLoading: false,
						token: data.token,
						capabilities: data.capabilities,
					});

					return true;
				} catch (err: unknown) {
					if (isAxiosError<{ detail?: string }>(err)) {
						set({ error: err.response?.data.detail });
					}
					return false;
				}
			},
			logout: () => {
				console.log("logout");
				set({
					user: null,
					isAuthenticated: false,
					isLoading: false,
					token: null,
					capabilities: null,
				});
			},
		}),
		{
			name: "auth-session",
		},
	),
);

// const useAuth = create((set: any) => ({
// 	token: localStorage.getItem("jwt_token"),
// 	setToken: () => set((state: object) => ({ token: state.token })),
// 	cleanToken: () => set((state: object) => ({ token: null })),
// }));
