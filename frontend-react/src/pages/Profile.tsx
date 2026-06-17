import { Link } from "react-router";

export function Profile() {
	return (
		<div>
			<h2>Minha Conta / Perfil</h2>
			<p>Área destinada às configurações do perfil do usuário logado.</p>
			<Link
				to="/profile/form"
				style={{ color: "#014ead", textDecoration: "none" }}
			>
				Form
			</Link>
		</div>
	);
}
