import { defineStore } from "pinia";
import { ref } from "vue";

const API_BASE = "https://localhost:7132/profiles";

const headers = () => ({
	"Content-Type": "application/json",
	Authorization: `Bearer ${localStorage.getItem("token")}`,
});

export interface SkillDto {
	id: string;
	name: string;
}

export interface ExperienceDto {
	id: string;
	company: string;
	role: string;
	startMonth: number;
	startYear: number;
	endMonth: number | null;
	endYear: number | null;
	current: boolean;
	description: string;
}

export interface EducationDto {
	id: string;
	institution: string;
	degree: string;
	startMonth: number;
	startYear: number;
	endMonth: number | null;
	endYear: number | null;
}

export interface ProfileDto {
	id: string;
	name: string;
	userName: string;
	bio: string;
	bioTitle: string;
	avatarUrl: string;
	skills: SkillDto[];
	experiences: ExperienceDto[];
	educations: EducationDto[];
}

export const useProfileStore = defineStore("profile", () => {
	const profileData = ref<ProfileDto | null>(null);

	async function fetchProfile(): Promise<void> {
		const res = await fetch(`${API_BASE}/me`, { headers: headers() });

		if (!res.ok) {
			throw new Error("Failed to fetch profile data");
		}

		profileData.value = await res.json();
	}

	async function updateProfile(data: {
		name: string;
		bio?: string;
		bioTitle?: string;
		avatarUrl?: string;
	}): Promise<void> {
		const res = await fetch(`${API_BASE}/me`, {
			method: "PUT",
			headers: headers(),
			body: JSON.stringify(data),
		});

		if (!res.ok) {
			throw new Error("Failed to update profile data");
		}
		profileData.value = await res.json();
	}

	async function addSkill(name: string): Promise<void> {
		const res = await fetch(`${API_BASE}/me/skills`, {
			method: "POST",
			headers: headers(),
			body: JSON.stringify({ name }),
		});

		if (!res.ok) {
			throw new Error("Failed to add skill");
		}

		const skill: SkillDto = await res.json();

		profileData.value?.skills.push(skill);
	}

	async function removeSkill(skillId: string): Promise<void> {
		const res = await fetch(`${API_BASE}/me/skills/${skillId}`, {
			method: "DELETE",
			headers: headers(),
		});

		if (!res.ok) {
			throw new Error("Failed to remove skill");
		}

		if (profileData.value) {
			profileData.value.skills = profileData.value.skills.filter(
				(s: SkillDto) => s.id !== skillId,
			);
		}
	}

	async function addExperience(data: Omit<ExperienceDto, "id">): Promise<void> {
		const res = await fetch(`${API_BASE}/me/experiences`, {
			method: "POST",
			headers: headers(),
			body: JSON.stringify(data),
		});

		if (!res.ok) {
			throw new Error("Failed to add experience");
		}

		const experience: ExperienceDto = await res.json();

		profileData.value?.experiences.push(experience);
	}

	async function updateExperience(
		experienceId: string,
		data: Omit<ExperienceDto, "id">,
	): Promise<void> {
		const res = await fetch(`${API_BASE}/me/experiences/${experienceId}`, {
			method: "PUT",
			headers: headers(),
			body: JSON.stringify(data),
		});

		if (!res.ok) {
			throw new Error("Failed to update experience");
		}

		const updatedExperience: ExperienceDto = await res.json();

		if (profileData.value) {
			const index = profileData.value.experiences.findIndex(
				(e: ExperienceDto) => e.id === experienceId,
			);
			if (index !== -1) {
				profileData.value.experiences[index] = updatedExperience;
			}
		}
	}

	async function removeExperience(experienceId: string): Promise<void> {
		const res = await fetch(`${API_BASE}/me/experiences/${experienceId}`, {
			method: "DELETE",
			headers: headers(),
		});
		if (!res.ok) throw new Error("Failed to remove experience");
		if (profileData.value) {
			profileData.value.experiences = profileData.value.experiences.filter(
				(e) => e.id !== experienceId,
			);
		}
	}

	async function addEducation(data: Omit<EducationDto, "id">): Promise<void> {
		const res = await fetch(`${API_BASE}/me/educations`, {
			method: "POST",
			headers: headers(),
			body: JSON.stringify(data),
		});
		if (!res.ok) throw new Error("Failed to add education");
		const edu: EducationDto = await res.json();
		profileData.value?.educations.push(edu);
	}

	async function updateEducation(
		educationId: string,
		data: Omit<EducationDto, "id">,
	): Promise<void> {
		const res = await fetch(`${API_BASE}/me/educations/${educationId}`, {
			method: "PUT",
			headers: headers(),
			body: JSON.stringify(data),
		});
		if (!res.ok) throw new Error("Failed to update education");
		const updated: EducationDto = await res.json();
		if (profileData.value) {
			const i = profileData.value.educations.findIndex(
				(e) => e.id === educationId,
			);
			if (i !== -1) profileData.value.educations[i] = updated;
		}
	}

	async function removeEducation(educationId: string): Promise<void> {
		const res = await fetch(`${API_BASE}/me/educations/${educationId}`, {
			method: "DELETE",
			headers: headers(),
		});
		if (!res.ok) throw new Error("Failed to remove education");
		if (profileData.value) {
			profileData.value.educations = profileData.value.educations.filter(
				(e) => e.id !== educationId,
			);
		}
	}

	return {
		profileData,
		fetchProfile,
		updateProfile,
		addSkill,
		removeSkill,
		addExperience,
		updateExperience,
		removeExperience,
		addEducation,
		updateEducation,
		removeEducation,
	};
});
