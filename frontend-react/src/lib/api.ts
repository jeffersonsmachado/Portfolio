import axios from "axios";

const envBaseUrl = import.meta.env.VITE_API_URL;
console.log("URL", envBaseUrl);
const baseURL = envBaseUrl?.trim() || "https://localhost:8080";

export const api = axios.create({
	baseURL,
	headers: {
		Accept: "application/json",
	},
});

api.interceptors.request.use((config) => {
	const token = localStorage.getItem("jwt_token");
	if (token && config.headers) {
		config.headers.Authorization = `Bearer ${token}`;
	}
	return config;
});
