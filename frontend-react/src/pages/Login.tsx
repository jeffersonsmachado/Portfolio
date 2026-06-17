import React, { useState } from "react";
import { useNavigate } from "react-router";
import { api } from "../services/api";
import { useAuthStore } from "../stores/useAuthStore";
import { isAxiosError } from "axios";

export function Login() {
	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");
	const [error, setError] = useState<string | undefined>("");

	// Pegamos a função de login do Zustand
	const login = useAuthStore((state) => state.login);
	const navigate = useNavigate();

	const handleLogin = async (e: React.SubmitEvent) => {
		e.preventDefault();
		setError("");

		try {
			const response = await api.post("/auth/login", { email, password });

			// O backend retorna: { token, capabilities, user }
			const { token, user, capabilities } = response.data;

			// Salva no estado global (e no localStorage automaticamente)
			login({ token, user, capabilities });

			// Redireciona para o painel principal
			navigate("/");
		} catch (err: unknown) {
			if (isAxiosError<{ detail?: string }>(err)) {
				setError(err.response?.data.detail);
			} else {
				setError("Ocorreu um problema no login.");
			}
		}
	};

	return (
		<div style={{ maxWidth: "400px", margin: "50px auto" }}>
			<h2>Login</h2>
			{error && <p style={{ color: "red" }}>{error}</p>}

			<form
				onSubmit={handleLogin}
				style={{ display: "flex", flexDirection: "column", gap: "1rem" }}
			>
				<div>
					<label>Email</label>
					<input
						type="email"
						value={email}
						onChange={(e) => setEmail(e.target.value)}
						required
						style={{ width: "100%", padding: "8px" }}
					/>
				</div>
				<div>
					<label>Senha</label>
					<input
						type="password"
						value={password}
						onChange={(e) => setPassword(e.target.value)}
						required
						style={{ width: "100%", padding: "8px" }}
					/>
				</div>
				<button type="submit" style={{ padding: "10px", cursor: "pointer" }}>
					Entrar
				</button>
			</form>
		</div>
	);
}
