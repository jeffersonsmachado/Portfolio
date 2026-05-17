<script setup lang="ts">
import { ref } from "vue";

// Interfaces
interface Experience {
	id: string;
	company: string;
	role: string;
	startMonth: string;
	startYear: string;
	endMonth: string;
	endYear: string;
	current: boolean;
	description: string;
}

interface Education {
	id: string;
	institution: string;
	degree: string;
	startMonth: string;
	startYear: string;
	endMonth: string;
	endYear: string;
}

const name = ref("Jeff");
const title = ref("Software Developer");
const bio = ref(
	"Passionate about building impactful software solutions. Experienced in full-stack development in .NET and JavaScript ecosystems. Always eager to learn and take on new challenges.",
);

const skills = ref(["C#", "JavaScript", "Vue.js", "ASP.NET Core"]);
const newSkill = ref("");

const experiences = ref<Experience[]>([
	{
		id: "1",
		company: "Tech Company A",
		role: ".NET Developer",
		startMonth: "January",
		startYear: "2020",
		endMonth: "Present",
		endYear: "",
		current: true,
		description:
			"Developed and maintained web applications using ASP.NET Core and Vue.js. Collaborated with cross-functional teams to design scalable solutions. Implemented new features and optimized performance, resulting in a 20% increase in user engagement.",
	},
]);

const educations = ref<Education[]>([
	{
		id: "1",
		institution: "University of Technology",
		degree: "B.Sc. in Computer Science",
		startMonth: "September",
		startYear: "2015",
		endMonth: "June",
		endYear: "2019",
	},
]);

const expDialogOpen = ref(false);
const editingExperience = ref<Experience | null>(null);
const expForm = ref<Omit<Experience, "id">>({
	company: "",
	role: "",
	startMonth: "",
	startYear: "",
	endMonth: "",
	endYear: "",
	current: false,
	description: "",
});

const eduDialogOpen = ref(false);
const editingEducation = ref<Education | null>(null);
const eduForm = ref<Omit<Education, "id">>({
	institution: "",
	degree: "",
	startMonth: "",
	startYear: "",
	endMonth: "",
	endYear: "",
});

function removeSkill(index: number) {
	skills.value.splice(index, 1);
}

function addSkill() {
	const skill = newSkill.value.trim();
	if (skill && !skills.value.includes(skill)) {
		skills.value.push(skill);
		newSkill.value = "";
	}
}

function openAddExp() {
	editingExperience.value = null;
	expForm.value = {
		company: "",
		role: "",
		startMonth: "",
		startYear: "",
		endMonth: "",
		endYear: "",
		current: false,
		description: "",
	};
	expDialogOpen.value = true;
}

function openAddEdu() {
	editingEducation.value = null;
	eduForm.value = {
		institution: "",
		degree: "",
		startMonth: "",
		startYear: "",
		endMonth: "",
		endYear: "",
	};
	eduDialogOpen.value = true;
}

function saveExp() {
	if (editingExperience.value) {
		const index = experiences.value.findIndex(
			(e) => e.id === editingExperience.value!.id,
		);

		if (index !== -1) {
			experiences.value[index] = {
				...expForm.value,
				id: editingExperience.value.id,
			};
		}
	} else {
		experiences.value.push({ ...expForm.value, id: Date.now().toString() });
	}
	expDialogOpen.value = false;
}

function openEditExp(exp: Experience) {
	editingExperience.value = exp;
	expForm.value = { ...exp };
	expDialogOpen.value = true;
}

function removeExp(id: string) {
	experiences.value = experiences.value.filter((e) => e.id !== id);
}

function saveEdu() {
	if (editingEducation.value) {
		const index = educations.value.findIndex(
			(e) => e.id === editingEducation.value!.id,
		);

		if (index !== -1) {
			educations.value[index] = {
				...eduForm.value,
				id: editingEducation.value.id,
			};
		}
	} else {
		educations.value.push({ ...eduForm.value, id: Date.now().toString() });
	}
	eduDialogOpen.value = false;
}

function openEditEdu(edu: Education) {
	editingEducation.value = edu;
	eduForm.value = { ...edu };
	eduDialogOpen.value = true;
}

function removeEdu(id: string) {
	educations.value = educations.value.filter((e) => e.id !== id);
}

// Helpers

function formatPeriod(
	startMonth: string,
	startYear: string,
	endMonth: string,
	endYear: string,
	current = false,
) {
	const start = startMonth && startYear ? `${startMonth}/${startYear}` : "";
	const end = current
		? "Atual"
		: endMonth && endYear
			? `${endMonth}/${endYear}`
			: "";
	return [start, end].filter(Boolean).join(" – ");
}

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
</script>

<template>
	<v-container max-width="800px">
		<!-- Basic info -->
		<v-card class="mb-4" elevation="2" rounded="lg">
			<v-card-title>Profile Information</v-card-title>
			<v-card-text>
				<v-text-field v-model="name" label="Name" />
				<v-text-field v-model="title" label="Title" />
				<v-textarea v-model="bio" label="Bio" rows="3" auto-grow />
			</v-card-text>
		</v-card>

		<!-- Skills -->

		<v-card class="mb-4" elevation="2" rounded="lg">
			<v-card-title>Skills</v-card-title>
			<v-card-text>
				<div class="d-flex flex-wrap gap-2 mb-4">
					<v-chip
						v-for="(skill, i) in skills"
						:key="i"
						closable
						@click:close="removeSkill(i)"
						color="primary"
						variant="tonal"
					>
						{{ skill }}
					</v-chip>
				</div>
				<div class="d-flex gap-2">
					<v-text-field
						v-model="newSkill"
						label="Add Skill"
						density="compact"
						hide-details
						@keyup.enter="addSkill"
					/>
					<v-btn color="primary" @click="addSkill">Add</v-btn>
				</div>
			</v-card-text>
		</v-card>

		<!-- Professional Experience -->

		<v-card class="mb-4" elevation="2" rounded="lg">
			<v-card-title class="d-flex justify-space-between align-center">
				Professional Experience
				<v-btn color="primary" size="small" @click="openAddExp">
					+ Add Experience
				</v-btn>
			</v-card-title>
			<v-card-text>
				<v-list>
					<v-list-item v-for="exp in experiences" :key="exp.id" class="mb-2">
						<v-card variant="outlined" width="100%">
							<v-card-text>
								<div class="d-flex justify-space-between">
									<div>
										<div class="font-weight-bold">{{ exp.role }}</div>
										<div class="text-subtitle-2">{{ exp.company }}</div>
										<div class="text-caption text-grey">
											{{
												formatPeriod(
													exp.startMonth,
													exp.startYear,
													exp.endMonth,
													exp.endYear,
													exp.current,
												)
											}}
										</div>
										<div class="mt-1">{{ exp.description }}</div>
									</div>

									<div class="d-flex gap-1">
										<v-btn
											icon
											size="x-small"
											variant="text"
											@click="openEditExp(exp)"
										>
											<v-icon>mdi-pencil</v-icon>
										</v-btn>
										<v-btn
											icon
											size="x-small"
											variant="text"
											color="error"
											@click="removeExp(exp.id)"
										>
											<v-icon>mdi-delete</v-icon>
										</v-btn>
									</div>
								</div>
							</v-card-text>
						</v-card>
					</v-list-item>
				</v-list>
			</v-card-text>
		</v-card>

		<!-- Education -->

		<v-card class="mb-4" elevation="2" rounded="lg">
			<v-card-title class="d-flex justify-space-between align-center">
				Academic Background
				<v-btn color="primary" size="small" @click="openAddEdu"
					>+ Add Education</v-btn
				>
			</v-card-title>
			<v-card-text>
				<v-list>
					<v-list-item v-for="edu in educations" :key="edu.id" class="mb-2">
						<v-card variant="outlined" width="100%">
							<v-card-text>
								<div class="d-flex justify-space-between">
									<div>
										<div class="font-weight-bold">{{ edu.degree }}</div>
										<div class="text-subtitle-2">{{ edu.institution }}</div>
										<div class="text-caption text-grey">
											{{
												formatPeriod(
													edu.startMonth,
													edu.startYear,
													edu.endMonth,
													edu.endYear,
												)
											}}
										</div>
									</div>
									<div class="d-flex gap-1">
										<v-btn
											icon
											size="x-small"
											variant="text"
											@click="openEditEdu(edu)"
										>
											<v-icon>mdi-pencil</v-icon>
										</v-btn>
										<v-btn
											icon
											size="x-small"
											variant="text"
											color="error"
											@click="removeEdu(edu.id)"
										>
											<v-icon>mdi-delete</v-icon>
										</v-btn>
									</div>
								</div>
							</v-card-text>
						</v-card>
					</v-list-item>
				</v-list>
			</v-card-text>
		</v-card>
	</v-container>

	<!-- Dialog experience -->
	<v-dialog v-model="expDialogOpen" max-width="600px">
		<v-card>
			<v-card-title
				>{{
					editingExperience ? "Editar" : "Adicionar"
				}}
				Experiência</v-card-title
			>
			<v-card-text>
				<v-text-field v-model="expForm.company" label="Empresa" />
				<v-text-field v-model="expForm.role" label="Cargo" />
				<v-row>
					<v-col cols="6"
						><v-select
							v-model="expForm.startMonth"
							:items="months"
							item-title="title"
							item-value="value"
							label="Mês de início"
					/></v-col>
					<v-col cols="6"
						><v-select
							v-model="expForm.startYear"
							:items="years"
							label="Ano de início"
					/></v-col>
				</v-row>
				<v-checkbox
					v-model="expForm.current"
					label="Trabalho aqui atualmente"
				/>
				<v-row v-if="!expForm.current">
					<v-col cols="6"
						><v-select
							v-model="expForm.endMonth"
							:items="months"
							item-title="title"
							item-value="value"
							label="Mês de término"
					/></v-col>
					<v-col cols="6"
						><v-select
							v-model="expForm.endYear"
							:items="years"
							label="Ano de término"
					/></v-col>
				</v-row>
				<v-textarea v-model="expForm.description" label="Descrição" rows="3" />
			</v-card-text>
			<v-card-actions>
				<v-spacer />
				<v-btn color="primary" @click="saveExp">Salvar</v-btn>
				<v-btn @click="expDialogOpen = false">Cancelar</v-btn>
			</v-card-actions>
		</v-card>
	</v-dialog>

	<!-- Dialog academic background -->
	<v-dialog v-model="eduDialogOpen" max-width="600px">
		<v-card>
			<v-card-title
				>{{ editingEducation ? "Editar" : "Adicionar" }} Formação</v-card-title
			>
			<v-card-text>
				<v-text-field v-model="eduForm.institution" label="Instituição" />
				<v-text-field v-model="eduForm.degree" label="Curso / Grau" />
				<v-row>
					<v-col cols="6"
						><v-select
							v-model="eduForm.startMonth"
							:items="months"
							item-title="title"
							item-value="value"
							label="Mês de início"
					/></v-col>
					<v-col cols="6"
						><v-select
							v-model="eduForm.startYear"
							:items="years"
							label="Ano de início"
					/></v-col>
				</v-row>
				<v-row>
					<v-col cols="6"
						><v-select
							v-model="eduForm.endMonth"
							:items="months"
							item-title="title"
							item-value="value"
							label="Mês de término"
					/></v-col>
					<v-col cols="6"
						><v-select
							v-model="eduForm.endYear"
							:items="years"
							label="Ano de término"
					/></v-col>
				</v-row>
			</v-card-text>
			<v-card-actions>
				<v-spacer />
				<v-btn color="primary" @click="saveEdu">Salvar</v-btn>
				<v-btn @click="eduDialogOpen = false">Cancelar</v-btn>
			</v-card-actions>
		</v-card>
	</v-dialog>
</template>

<style scoped></style>
