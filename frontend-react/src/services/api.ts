import axios from "axios";
import { useAuthStore } from "../stores/useAuthStore";

const envBaseUrl = import.meta.env.VITE_API_URL;
console.log("URL", envBaseUrl);
const baseURL = envBaseUrl?.trim() || "https://localhost:8080";

export const api = axios.create({
	baseURL: baseURL,
	headers: {
		Accept: "application/json",
	},
});

api.interceptors.request.use(
	(config) => {
		const token = useAuthStore.getState().token;

		if (token) {
			config.headers.Authorization = `Bearer ${token}`;
		}

		return config;
	},
	(error) => {
		return Promise.reject(error);
	},
);

api.interceptors.response.use(
	(response) => response,
	(error) => {
		if (error.responsee && error.response.status === 401) {
			useAuthStore.getState().logout();
			window.location.href = "/login";
		}
		return Promise.reject(error);
	},
);
