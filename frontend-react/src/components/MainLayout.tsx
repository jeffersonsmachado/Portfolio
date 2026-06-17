import { Link, Outlet, useNavigate } from "react-router";
import { useAuthStore } from "../stores/useAuthStore";

export function MainLayout() {
	const { user, capabilities, logout } = useAuthStore();
	const navigate = useNavigate();

	const handleLogout = () => {
		logout();
		navigate("/login");
	};

	return (
		<div
			style={{ minHeight: "100vh", display: "flex", flexDirection: "column" }}
		>
			{/* Menu de Navegação Superior */}
			<header
				style={{
					background: "#1e293b",
					color: "#f8fafc",
					padding: "1rem 2rem",
					display: "flex",
					justifyContent: "space-between",
					alignItems: "center",
					boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
				}}
			>
				<div style={{ display: "flex", alignItems: "center", gap: "2rem" }}>
					<span style={{ fontWeight: "bold", fontSize: "1.2rem" }}>
						Meu Sistema
					</span>
					<nav style={{ display: "flex", gap: "1.5rem" }}>
						<Link to="/" style={{ color: "#cbd5e1", textDecoration: "none" }}>
							Dashboard
						</Link>
						<Link
							to="/profile"
							style={{ color: "#cbd5e1", textDecoration: "none" }}
						>
							Perfil
						</Link>

						{/* Segurança Visual: Só exibe o link se canRead de user for true */}
						{capabilities?.user?.canRead && (
							<Link
								to="/users"
								style={{ color: "#cbd5e1", textDecoration: "none" }}
							>
								Usuários
							</Link>
						)}
					</nav>
				</div>

				<div style={{ display: "flex", alignItems: "center", gap: "1rem" }}>
					<span>
						Olá, <strong>{user?.username}</strong>
					</span>
					<button
						onClick={handleLogout}
						style={{
							background: "#ef4444",
							color: "white",
							border: "none",
							padding: "0.5rem 1rem",
							borderRadius: "4px",
							cursor: "pointer",
						}}
					>
						Sair
					</button>
				</div>
			</header>

			{/* Conteúdo da Página Ativa */}
			<main style={{ flex: 1, padding: "2rem", background: "#f1f5f9" }}>
				<Outlet />
			</main>
		</div>
	);
}
