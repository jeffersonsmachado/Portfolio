<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useProfileStore } from '../../services/profileService';
import Form from './Form.vue';

const profile = useProfileStore();
const noProfile = ref<boolean>(false);
const profileData = ref<any>(null);


const fetchProfileData = async (): Promise<void> => {
	try {
		const profileData = await profile.fetchPlayerProfile()
		console.log('Fetched profile data:', profileData);
		if (profileData == null) {
			noProfile.value = true;
		}
	} catch (err) {
		if (err instanceof Error) {
			if (err.cause instanceof Response && err.cause.status === 404) {
				noProfile.value = true;
			} else {
				console.error('An error occurred while fetching profile data:', err.message);
			}
		} else {
			console.error('An unknown error occurred while fetching profile data.');
		}
	}
}

onMounted(() => {

	try {
		fetchProfileData()
	} catch (error) {
		console.error('Error fetching profile data:', error instanceof Error ? error.message : 'An unknown error occurred.')
	}
})



</script>


<template>

	<h1>Profile</h1>
	<div v-if="!noProfile">
		<p v-if="profile?.profileData">{{ profile?.profileData?.name }}</p>
		<p v-else-if="noProfile">No profile data found.</p>
		<p v-else>Loading...</p>
		<h3>{{ profile?.profileData?.bioTitle }}</h3>
		<p>{{ profile?.profileData?.bio }}</p>
	</div>
	<div v-else>
		<Form :profile="profileData" />
	</div>

</template>


<style scoped></style>