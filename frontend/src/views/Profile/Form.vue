<script setup lang="ts">
import { ref } from 'vue';
import { useProfileStore } from '../../services/profileService';

const props = defineProps<{
	profile: any
}>()

const name = ref(props.profile?.name ?? '')
const bio = ref(props.profile?.bio ?? '')
const bioTitle = ref(props.profile?.bioTitle ?? '')
const avatarUrl = ref(props.profile?.avatarUrl ?? '')

const profile = useProfileStore();


const saveProfile = () => {
	profile.updatePlayerProfile({
		id: props.profile?.id,
		name: name.value,
		bio: bio.value,
		bioTitle: bioTitle.value,
		avatarUrl: avatarUrl.value,
	});
}

</script>

<template>
	<form @submit.prevent="saveProfile" class="profile-form">

		<h1 class="title">Profile Form</h1>

		<input class="name" type="text" placeholder="Enter your name" v-model="name" />
		<input class="bioTitle" type="text" placeholder="Enter your bio title" v-model="bioTitle" />
		<textarea class="bio" type="text" placeholder="Enter your bio" v-model="bio" />
		<input class="avatarUrl" type="text" placeholder="Enter your avatar URL" v-model="avatarUrl" />

		<button class="btn-primary" type="submit">
			Save Profile
		</button>

	</form>


</template>

<style scoped>
.profile-form {
	display: flex;
	flex-direction: column;
	gap: 1rem;
}

.title {
	font-size: 1.5rem;
	font-weight: bold;
}

.name,
.bioTitle {
	padding: 0.5rem;
	font-size: 1rem;
	border: 1px solid #ccc;
	border-radius: 4px;
}

.bio {
	padding: 0.5rem;
	font-size: 1rem;
	border: 1px solid #ccc;
	border-radius: 4px;
	min-height: 100px;
}

.avatarUrl {
	padding: 0.5rem;
	font-size: 1rem;
	border: 1px solid #ccc;
	border-radius: 4px;
}
</style>