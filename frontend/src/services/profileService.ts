import { defineStore } from "pinia";
import { ref } from "vue";

const API_BASE = "https://localhost:7132/profile";

export interface PlayerProfileDto {
	id: string;
	name: string;
	bio: string;
	bioTitle: string;
	avatarUrl: string;
}

export const useProfileStore = defineStore("profile", () => {
	const profileData = ref<PlayerProfileDto | null>(null);

	async function fetchPlayerProfile(): Promise<PlayerProfileDto> {
		const response = await fetch(`${API_BASE}/me`, {
			method: "GET",
			headers: {
				"Content-Type": "application/json",
				Authorization: `Bearer ${localStorage.getItem("token")}`,
			},
		});

		if (!response.ok) {
			throw new Error("Failed to fetch player profile", {
				cause: response,
			});
		}
		const profile = await response.json();
		profileData.value = profile;
		return profile;
	}

	async function updatePlayerProfile(
		updatedProfile: PlayerProfileDto,
	): Promise<PlayerProfileDto> {
		const response = await fetch(`${API_BASE}/me`, {
			method: "PUT",
			headers: {
				"Content-Type": "application/json",
				Authorization: `Bearer ${localStorage.getItem("token")}`,
			},
			body: JSON.stringify(updatedProfile),
		});

		if (!response.ok) {
			throw new Error("Failed to update player profile", {
				cause: response,
			});
		}
		const profile = await response.json();
		profileData.value = profile;
		return profile;
	}

	return {
		profileData,
		fetchPlayerProfile,
		updatePlayerProfile,
	};
});
