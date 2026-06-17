import { BrowserRouter, Routes, Route, Navigate, Outlet } from "react-router";
import { useAuthStore } from "./stores/useAuthStore";
import { Login } from "./pages/Login";
import { MainLayout } from "./components/MainLayout";
import { PermissionGuard } from "./components/PermissionGuard";
import { Profile } from "./pages/Profile";
import { ProfilePage } from "./pages/ProfileForm";
import { Users } from "./pages/Users";

// Guard de Autenticação Simples (Se não logado, vai pro Login)
function PrivateRoute() {
	const isAuthenticated = useAuthStore((state) => state.isAuthenticated);
	return isAuthenticated ? <Outlet /> : <Navigate to="/login" replace />;
}

// Tela de Dashboard (Home) interna
function Dashboard() {
	return (
		<div>
			<h2>Dashboard Inicial</h2>
			<p>Bem-vindo ao painel principal do sistema.</p>
		</div>
	);
}

export default function App() {
	// Buscamos a capability diretamente para injetar no Guard de rotas
	const canReadUsers = useAuthStore(
		(state) => state.capabilities?.user?.canRead,
	);

	return (
		<BrowserRouter>
			<Routes>
				{/* Rota Pública */}
				<Route path="/login" element={<Login />} />

				{/* Todas as rotas dentro deste bloco exigem LOGIN */}
				<Route element={<PrivateRoute />}>
					{/* Todas as rotas dentro deste bloco compartilham o Menu Superior */}
					<Route element={<MainLayout />}>
						<Route path="/" element={<Dashboard />} />
						<Route path="/profile" element={<Profile />} />
						<Route path="/profile/form" element={<ProfilePage />} />

						{/* Rota Protegida por CAPABILITY (RBAC) */}
						<Route element={<PermissionGuard allowed={canReadUsers} />}>
							<Route path="/users" element={<Users />} />
						</Route>
					</Route>
				</Route>

				{/* Fallback para rotas não encontradas */}
				<Route path="*" element={<Navigate to="/" replace />} />
			</Routes>
		</BrowserRouter>
	);
}
