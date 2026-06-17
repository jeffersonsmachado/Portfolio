import { create } from "zustand";
import { api } from "../services/api";
import { isAxiosError } from "axios";

export interface Skill {
	id: string;
	name: string;
}

export interface Experience {
	id: string;
	company: string;
	role: string;
	startMonth: number | string;
	startYear: number | string;
	endMonth: number | string;
	endYear: number | string;
	current: boolean;
	description: string;
}

export interface Education {
	id: string;
	institution: string;
	degree: string;
	startMonth: number | string;
	startYear: number | string;
	endMonth: number | string;
	endYear: number | string;
}

export interface ProfileData {
	id: string;
	name: string;
	bioTitle: string;
	bio: string;
	skills: Skill[];
	experiences: Experience[];
	educations: Education[];
}

interface ProfileState {
	profileData: ProfileData | null;
	isLoading: boolean;
	error: string | null;

	fetchProfile: () => Promise<void>;
	updateProfile: (data: {
		name: string;
		bioTitle: string;
		bio: string;
	}) => Promise<void>;

	addSkill: (skillName: string) => Promise<void>;
	removeSkill: (id: string) => Promise<void>;

	addExperience: (data: Omit<Experience, "id">) => Promise<void>;
	updateExperience: (id: string, data: Omit<Experience, "id">) => Promise<void>;
	removeExperience: (id: string) => Promise<void>;

	addEducation: (data: Omit<Education, "id">) => Promise<void>;
	updateEducation: (id: string, data: Omit<Education, "id">) => Promise<void>;
	removeEducation: (id: string) => Promise<void>;
}

export const useProfileStore = create<ProfileState>((set) => ({
	profileData: null,
	isLoading: false,
	error: null,

	fetchProfile: async () => {
		set({ isLoading: true, error: null });
		try {
			const response = await api.get<ProfileData>("/profiles/me");
			set({ profileData: response.data, isLoading: false });
		} catch (error: unknown) {
			if (isAxiosError<{ detail?: string }>(error)) {
				set({
					error: error.response?.data.detail || "Erro ao buscar perfil",
					isLoading: false,
				});
			} else {
				set({
					error: "Erro ao buscar perfil",
					isLoading: false,
				});
			}
		}
	},
	updateProfile: async (data) => {
		try {
			await api.put("/profiles/me", data);
			set((state) => ({
				profileData: state.profileData
					? { ...state.profileData, ...data }
					: null,
			}));
		} catch (error) {
			console.error("Erro ao atualizar perfil", error);
		}
	},
	addSkill: async (name) => {
		try {
			const response = await api.post<Skill>("/profiles/me/skills", { name });
			set((state) => ({
				profileData: state.profileData
					? {
							...state.profileData,
							skills: [...state.profileData.skills, response.data],
						}
					: null,
			}));
		} catch (error) {
			console.error("Erro ao remover skill", error);
		}
	},
	removeSkill: async (id) => {
		try {
			await api.delete(`/profiles/me/skills/${id}`);

			set((state) => ({
				profileData: state.profileData
					? {
							...state.profileData,
							skills: state.profileData.skills.filter((s) => s.id !== id),
						}
					: null,
			}));
		} catch (error) {
			console.error("Erro ao remover skill", error);
		}
	},
	addExperience: async (data) => {
		try {
			const response = await api.post<Experience>(
				"/profiles/me/experiences",
				data,
			);
			set((state) => ({
				profileData: state.profileData
					? {
							...state.profileData,
							experiences: [...state.profileData.experiences, response.data],
						}
					: null,
			}));
		} catch (error) {
			console.error("Erro ao adicionar experiência", error);
		}
	},
	updateExperience: async (id, data) => {
		try {
			await api.put(`/profiles/me/experiences/${id}`, data);
			set((state) => ({
				profileData: state.profileData
					? {
							...state.profileData,
							experiences: state.profileData.experiences.map((exp) =>
								exp.id === id ? { ...exp, ...data } : exp,
							),
						}
					: null,
			}));
		} catch (error) {
			console.error("Erro ao atualizar experiência", error);
		}
	},
	removeExperience: async (id) => {
		try {
			await api.delete(`/profiles/me/experiences/${id}`);
			set((state) => ({
				profileData: state.profileData
					? {
							...state.profileData,
							experiences: state.profileData.experiences.filter(
								(exp) => exp.id !== id,
							),
						}
					: null,
			}));
		} catch (error) {
			console.error("Erro ao remover experiência", error);
		}
	},
	addEducation: async (data) => {
		try {
			const response = await api.post<Education>(
				"/profiles/me/educations",
				data,
			);
			set((state) => ({
				profileData: state.profileData
					? {
							...state.profileData,
							educations: [...state.profileData.educations, response.data],
						}
					: null,
			}));
		} catch (error) {
			console.error("Erro ao adicionar educação", error);
		}
	},
	updateEducation: async (id, data) => {
		try {
			await api.put(`/profiles/me/educations/${id}`, data);
			set((state) => ({
				profileData: state.profileData
					? {
							...state.profileData,
							educations: state.profileData.educations.map((edu) =>
								edu.id === id ? { ...edu, ...data } : edu,
							),
						}
					: null,
			}));
		} catch (error) {
			console.error("Erro ao atualizar educação", error);
		}
	},
	removeEducation: async (id) => {
		try {
			await api.delete(`/profiles/me/educations/${id}`);
			set((state) => ({
				profileData: state.profileData
					? {
							...state.profileData,
							educations: state.profileData.educations.filter(
								(edu) => edu.id !== id,
							),
						}
					: null,
			}));
		} catch (error) {
			console.error("Erro ao remover educação", error);
		}
	},
}));
