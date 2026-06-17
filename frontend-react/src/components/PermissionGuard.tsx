import { Navigate, Outlet } from "react-router";

interface PermissionGuardProps {
	allowed: boolean | undefined;
}

export function PermissionGuard({ allowed }: PermissionGuardProps) {
	// Se a permissão não for válida/true, redireciona para a raiz de forma limpa
	if (!allowed) {
		return <Navigate to="/" replace />;
	}

	return <Outlet />;
}
