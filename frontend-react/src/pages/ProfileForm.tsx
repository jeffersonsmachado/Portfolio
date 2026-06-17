import { useState, useEffect } from "react";
// Importe a sua store do Zustand aqui
import { useProfileStore } from "../stores/useProfileStore";
import type {
	ProfileData,
	Skill,
	Experience,
	Education,
} from "../stores/useProfileStore";

// Helpers
const months = [
	{ title: "Janeiro", value: "01" },
	{ title: "Fevereiro", value: "02" },
	{ title: "Março", value: "03" },
	{ title: "Abril", value: "04" },
	{ title: "Maio", value: "05" },
	{ title: "Junho", value: "06" },
	{ title: "Julho", value: "07" },
	{ title: "Agosto", value: "08" },
	{ title: "Setembro", value: "09" },
	{ title: "Outubro", value: "10" },
	{ title: "Novembro", value: "11" },
	{ title: "Dezembro", value: "12" },
];

const currentYear = new Date().getFullYear();
const years = Array.from({ length: 50 }, (_, i) => String(currentYear - i));

function formatPeriod(
	startMonth: number | string | null,
	startYear: number | string | null,
	endMonth: number | string | null,
	endYear: number | string | null,
	current = false,
) {
	const monthName = (m: number | string | null) =>
		months.find((mo) => mo.value === String(m ?? "").padStart(2, "0"))?.title ??
		String(m ?? "");
	const start =
		startMonth && startYear ? `${monthName(startMonth)}/${startYear}` : "";
	const end = current
		? "Atual"
		: endMonth && endYear
			? `${monthName(endMonth)}/${endYear}`
			: "";
	return [start, end].filter(Boolean).join(" – ");
}

export function ProfilePage() {
	const { profileData, fetchProfile, isLoading } = useProfileStore();

	useEffect(() => {
		fetchProfile();
	}, [fetchProfile]);

	if (isLoading || !profileData) {
		return (
			<div style={{ textAlign: "center", padding: "50px" }}>Carregando...</div>
		);
	}

	return <ProfileForm key={profileData.id} profileData={profileData} />;
}

function ProfileForm({ profileData }: { profileData: ProfileData }) {
	const {
		updateProfile,
		addSkill,
		removeSkill,
		addExperience,
		updateExperience,
		removeExperience,
		addEducation,
		updateEducation,
		removeEducation,
	} = useProfileStore();

	const [name, setName] = useState(profileData.name || "");
	const [bioTitle, setBioTitle] = useState(profileData.bioTitle || "");
	const [bio, setBio] = useState(profileData.bio || "");
	const [newSkill, setNewSkill] = useState("");

	const [expDialogOpen, setExpDialogOpen] = useState(false);
	const [editingExperience, setEditingExperience] = useState<string | null>(
		null,
	);
	const [expForm, setExpForm] = useState<Omit<Experience, "id">>({
		company: "",
		role: "",
		startMonth: "",
		startYear: "",
		endMonth: "",
		endYear: "",
		current: false,
		description: "",
	});

	const [eduDialogOpen, setEduDialogOpen] = useState(false);
	const [editingEducation, setEditingEducation] = useState<string | null>(null);
	const [eduForm, setEduForm] = useState<Omit<Education, "id">>({
		institution: "",
		degree: "",
		startMonth: "",
		startYear: "",
		endMonth: "",
		endYear: "",
	});

	const handleSaveProfile = async () => {
		await updateProfile({ name, bioTitle, bio });
	};

	const handleAddSkill = async () => {
		const skill = newSkill.trim();
		if (!skill) return;
		await addSkill(skill);
		setNewSkill("");
	};

	const openAddExp = () => {
		setEditingExperience(null);
		setExpForm({
			company: "",
			role: "",
			startMonth: "",
			startYear: "",
			endMonth: "",
			endYear: "",
			current: false,
			description: "",
		});
		setExpDialogOpen(true);
	};

	const openEditExp = (exp: Experience) => {
		setEditingExperience(exp.id);
		setExpForm({
			company: exp.company,
			role: exp.role,
			startMonth: String(exp.startMonth).padStart(2, "0"),
			startYear: String(exp.startYear),
			endMonth: exp.endMonth ? String(exp.endMonth).padStart(2, "0") : "",
			endYear: exp.endYear ? String(exp.endYear) : "",
			current: exp.current,
			description: exp.description,
		});
		setExpDialogOpen(true);
	};

	const saveExp = async () => {
		const data = {
			...expForm,
			startMonth: Number(expForm.startMonth),
			startYear: Number(expForm.startYear),
			endMonth: expForm.endMonth ? Number(expForm.endMonth) : "",
			endYear: expForm.endYear ? Number(expForm.endYear) : "",
		};

		if (editingExperience) {
			await updateExperience(editingExperience, data);
		} else {
			await addExperience(data);
		}
		setExpDialogOpen(false);
	};

	const openAddEdu = () => {
		setEditingEducation(null);
		setEduForm({
			institution: "",
			degree: "",
			startMonth: "",
			startYear: "",
			endMonth: "",
			endYear: "",
		});
		setEduDialogOpen(true);
	};

	const openEditEdu = (edu: Education) => {
		setEditingEducation(edu.id);
		setEduForm({
			institution: edu.institution,
			degree: edu.degree,
			startMonth: String(edu.startMonth).padStart(2, "0"),
			startYear: String(edu.startYear),
			endMonth: edu.endMonth ? String(edu.endMonth).padStart(2, "0") : "",
			endYear: edu.endYear ? String(edu.endYear) : "",
		});
		setEduDialogOpen(true);
	};

	const saveEdu = async () => {
		const data = {
			...eduForm,
			startMonth: Number(eduForm.startMonth),
			startYear: Number(eduForm.startYear),
			endMonth: eduForm.endMonth ? Number(eduForm.endMonth) : "",
			endYear: eduForm.endYear ? Number(eduForm.endYear) : "",
		};

		if (editingEducation) {
			await updateEducation(editingEducation, data);
		} else {
			await addEducation(data);
		}
		setEduDialogOpen(false);
	};

	return (
		<div style={{ maxWidth: "800px", margin: "0 auto", padding: "20px" }}>
			{/* Informações Básicas */}
			<section
				style={{
					border: "1px solid #ddd",
					padding: "20px",
					borderRadius: "8px",
					marginBottom: "20px",
				}}
			>
				<h3>Informações do Perfil</h3>
				<div
					style={{
						display: "flex",
						flexDirection: "column",
						gap: "10px",
						marginTop: "10px",
					}}
				>
					<label>
						Nome:
						<input
							type="text"
							value={name}
							onChange={(e) => setName(e.target.value)}
							style={{ width: "100%" }}
						/>
					</label>
					<label>
						Título:
						<input
							type="text"
							value={bioTitle}
							onChange={(e) => setBioTitle(e.target.value)}
							style={{ width: "100%" }}
						/>
					</label>
					<label
						style={{
							display: "flex",
							flexDirection: "column",
							alignItems: "center",
						}}
					>
						Bio:
						<textarea
							value={bio}
							onChange={(e) => setBio(e.target.value)}
							rows={3}
							style={{ width: "100%", maxWidth: "100%" }}
						/>
					</label>
				</div>
			</section>

			{/* Skills */}
			<section
				style={{
					border: "1px solid #ddd",
					padding: "20px",
					borderRadius: "8px",
					marginBottom: "20px",
				}}
			>
				<h3>Skills</h3>
				<div
					style={{
						display: "flex",
						flexWrap: "wrap",
						gap: "10px",
						margin: "10px 0",
					}}
				>
					{/* Lemos diretamente de profileData (que vem via props). Sempre que a store mudar, o React injeta a prop nova aqui e a lista atualiza instantaneamente */}
					{profileData.skills?.map((skill: Skill) => (
						<span
							key={skill.id}
							style={{
								background: "#eee",
								padding: "5px 10px",
								borderRadius: "16px",
							}}
						>
							{skill.name}{" "}
							<button
								onClick={() => removeSkill(skill.id)}
								style={{
									border: "none",
									background: "none",
									cursor: "pointer",
									color: "red",
								}}
							>
								&times;
							</button>
						</span>
					))}
				</div>
				<div style={{ display: "flex", gap: "10px" }}>
					<input
						type="text"
						value={newSkill}
						onChange={(e) => setNewSkill(e.target.value)}
						onKeyDown={(e) => e.key === "Enter" && handleAddSkill()}
						placeholder="Nova skill"
					/>
					<button onClick={handleAddSkill}>Adicionar</button>
				</div>
			</section>

			{/* Experiência Profissional */}
			<section
				style={{
					border: "1px solid #ddd",
					padding: "20px",
					borderRadius: "8px",
					marginBottom: "20px",
				}}
			>
				<div
					style={{
						display: "flex",
						justifyContent: "space-between",
						alignItems: "center",
					}}
				>
					<h3>Experiência Profissional</h3>
					<button onClick={openAddExp}>+ Adicionar Experiência</button>
				</div>
				<ul style={{ listStyle: "none", padding: 0 }}>
					{profileData.experiences?.map((exp: Experience) => (
						<li
							key={exp.id}
							style={{
								borderBottom: "1px solid #eee",
								padding: "10px 0",
								display: "flex",
								justifyContent: "space-between",
							}}
						>
							<div>
								<strong>{exp.role}</strong>
								<div>{exp.company}</div>
								<div style={{ fontSize: "12px", color: "gray" }}>
									{formatPeriod(
										exp.startMonth,
										exp.startYear,
										exp.endMonth,
										exp.endYear,
										exp.current,
									)}
								</div>
								<div>{exp.description}</div>
							</div>
							<div>
								<button onClick={() => openEditExp(exp)}>Editar</button>
								<button
									onClick={() => removeExperience(exp.id)}
									style={{ color: "red", marginLeft: "10px" }}
								>
									Excluir
								</button>
							</div>
						</li>
					))}
				</ul>
			</section>

			{/* Formação Acadêmica */}
			<section
				style={{
					border: "1px solid #ddd",
					padding: "20px",
					borderRadius: "8px",
					marginBottom: "20px",
				}}
			>
				<div
					style={{
						display: "flex",
						justifyContent: "space-between",
						alignItems: "center",
					}}
				>
					<h3>Formação Acadêmica</h3>
					<button onClick={openAddEdu}>+ Adicionar Formação</button>
				</div>
				<ul style={{ listStyle: "none", padding: 0 }}>
					{profileData.educations?.map((edu: Education) => (
						<li
							key={edu.id}
							style={{
								borderBottom: "1px solid #eee",
								padding: "10px 0",
								display: "flex",
								justifyContent: "space-between",
							}}
						>
							<div>
								<strong>{edu.degree}</strong>
								<div>{edu.institution}</div>
								<div style={{ fontSize: "12px", color: "gray" }}>
									{formatPeriod(
										edu.startMonth,
										edu.startYear,
										edu.endMonth,
										edu.endYear,
									)}
								</div>
							</div>
							<div>
								<button onClick={() => openEditEdu(edu)}>Editar</button>
								<button
									onClick={() => removeEducation(edu.id)}
									style={{ color: "red", marginLeft: "10px" }}
								>
									Excluir
								</button>
							</div>
						</li>
					))}
				</ul>
			</section>

			<div style={{ display: "flex", justifyContent: "flex-end" }}>
				<button
					onClick={handleSaveProfile}
					style={{
						padding: "10px 20px",
						background: "blue",
						color: "white",
						cursor: "pointer",
					}}
				>
					Salvar Perfil
				</button>
			</div>

			{/* Modal de Experiência */}
			{expDialogOpen && (
				<div style={modalOverlayStyle}>
					<div style={modalContentStyle}>
						<h3>{editingExperience ? "Editar" : "Adicionar"} Experiência</h3>
						<div
							style={{
								display: "flex",
								flexDirection: "column",
								gap: "10px",
								margin: "15px 0",
							}}
						>
							<input
								placeholder="Empresa"
								value={expForm.company}
								onChange={(e) =>
									setExpForm({ ...expForm, company: e.target.value })
								}
							/>
							<input
								placeholder="Cargo"
								value={expForm.role}
								onChange={(e) =>
									setExpForm({ ...expForm, role: e.target.value })
								}
							/>

							<div style={{ display: "flex", gap: "10px" }}>
								<select
									value={expForm.startMonth}
									onChange={(e) =>
										setExpForm({ ...expForm, startMonth: e.target.value })
									}
								>
									<option value="">Mês de início</option>
									{months.map((m) => (
										<option key={m.value} value={m.value}>
											{m.title}
										</option>
									))}
								</select>
								<select
									value={expForm.startYear}
									onChange={(e) =>
										setExpForm({ ...expForm, startYear: e.target.value })
									}
								>
									<option value="">Ano de início</option>
									{years.map((y) => (
										<option key={y} value={y}>
											{y}
										</option>
									))}
								</select>
							</div>

							<label>
								<input
									type="checkbox"
									checked={expForm.current}
									onChange={(e) =>
										setExpForm({ ...expForm, current: e.target.checked })
									}
								/>
								Trabalho aqui atualmente
							</label>

							{!expForm.current && (
								<div style={{ display: "flex", gap: "10px" }}>
									<select
										value={expForm.endMonth}
										onChange={(e) =>
											setExpForm({ ...expForm, endMonth: e.target.value })
										}
									>
										<option value="">Mês de término</option>
										{months.map((m) => (
											<option key={m.value} value={m.value}>
												{m.title}
											</option>
										))}
									</select>
									<select
										value={expForm.endYear}
										onChange={(e) =>
											setExpForm({ ...expForm, endYear: e.target.value })
										}
									>
										<option value="">Ano de término</option>
										{years.map((y) => (
											<option key={y} value={y}>
												{y}
											</option>
										))}
									</select>
								</div>
							)}

							<textarea
								placeholder="Descrição"
								value={expForm.description}
								onChange={(e) =>
									setExpForm({ ...expForm, description: e.target.value })
								}
								rows={3}
							/>
						</div>
						<div
							style={{
								display: "flex",
								justifyContent: "flex-end",
								gap: "10px",
							}}
						>
							<button onClick={() => setExpDialogOpen(false)}>Cancelar</button>
							<button
								onClick={saveExp}
								style={{ background: "blue", color: "white" }}
							>
								Salvar
							</button>
						</div>
					</div>
				</div>
			)}

			{/* Modal de Educação */}
			{eduDialogOpen && (
				<div style={modalOverlayStyle}>
					<div style={modalContentStyle}>
						<h3>{editingEducation ? "Editar" : "Adicionar"} Formação</h3>
						<div
							style={{
								display: "flex",
								flexDirection: "column",
								gap: "10px",
								margin: "15px 0",
							}}
						>
							<input
								placeholder="Instituição"
								value={eduForm.institution}
								onChange={(e) =>
									setEduForm({ ...eduForm, institution: e.target.value })
								}
							/>
							<input
								placeholder="Curso / Grau"
								value={eduForm.degree}
								onChange={(e) =>
									setEduForm({ ...eduForm, degree: e.target.value })
								}
							/>
							<div style={{ display: "flex", gap: "10px" }}>
								<select
									value={eduForm.startMonth}
									onChange={(e) =>
										setEduForm({ ...eduForm, startMonth: e.target.value })
									}
								>
									<option value="">Mês de início</option>
									{months.map((m) => (
										<option key={m.value} value={m.value}>
											{m.title}
										</option>
									))}
								</select>
								<select
									value={eduForm.startYear}
									onChange={(e) =>
										setEduForm({ ...eduForm, startYear: e.target.value })
									}
								>
									<option value="">Ano de início</option>
									{years.map((y) => (
										<option key={y} value={y}>
											{y}
										</option>
									))}
								</select>
							</div>
							<div style={{ display: "flex", gap: "10px" }}>
								<select
									value={eduForm.endMonth}
									onChange={(e) =>
										setEduForm({ ...eduForm, endMonth: e.target.value })
									}
								>
									<option value="">Mês de término</option>
									{months.map((m) => (
										<option key={m.value} value={m.value}>
											{m.title}
										</option>
									))}
								</select>
								<select
									value={eduForm.endYear}
									onChange={(e) =>
										setEduForm({ ...eduForm, endYear: e.target.value })
									}
								>
									<option value="">Ano de término</option>
									{years.map((y) => (
										<option key={y} value={y}>
											{y}
										</option>
									))}
								</select>
							</div>
						</div>
						<div
							style={{
								display: "flex",
								justifyContent: "flex-end",
								gap: "10px",
							}}
						>
							<button onClick={() => setEduDialogOpen(false)}>Cancelar</button>
							<button
								onClick={saveEdu}
								style={{ background: "blue", color: "white" }}
							>
								Salvar
							</button>
						</div>
					</div>
				</div>
			)}
		</div>
	);
}

const modalOverlayStyle: React.CSSProperties = {
	position: "fixed",
	top: 0,
	left: 0,
	right: 0,
	bottom: 0,
	backgroundColor: "rgba(0,0,0,0.5)",
	display: "flex",
	justifyContent: "center",
	alignItems: "center",
	zIndex: 1000,
};

const modalContentStyle: React.CSSProperties = {
	backgroundColor: "#fff",
	padding: "20px",
	borderRadius: "8px",
	width: "100%",
	maxWidth: "500px",
};
