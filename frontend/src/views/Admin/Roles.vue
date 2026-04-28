<template>
	<div>
		<h1>Roles Management</h1>
		<p>This is where you can manage user roles and permissions.</p>
		<DynamicAssigner :users="userStore.users" :roles="roleStore.roles" :userRoles="userStore.usersWithRoles"
			:sourceLabel="sourceLabel" :targetLabel="targetLabel" @update:userRoles="handleRolesChanged" />
	</div>
</template>

<script setup lang="ts">

import { onMounted } from 'vue';
import DynamicAssigner from '../../components/DynamicAssigner.vue';
import { useUserStore, type UserDto } from '../../services/userService';
import { useRoleStore } from '../../services/roleService';

const userStore = useUserStore();
const roleStore = useRoleStore();
const sourceLabel = 'Users';
const targetLabel = 'Roles';


const handleRolesChanged = async (updatedUsers: UserDto[]) => {
	// Check which user has changed roles by comparing with the original data
	const changed = updatedUsers.find((updated) => {
		const original = userStore.usersWithRoles.find((u) => u.id === updated.id);
		const originalIds = (original?.roles ?? []).map((r) => r.id).sort();
		const updatedIds = updated.roles.map((r) => r.id).sort();
		return JSON.stringify(originalIds) !== JSON.stringify(updatedIds);
	});

	if (!changed) return;

	console.log('Usuário alterado:', changed);
	const userId = changed.id;
	const newRoleIds = changed.roles.map((r) => r.id);

	await roleStore.setUserRoles(userId, newRoleIds);
};

onMounted(async () => {

	await userStore.fetchUsers();
	await roleStore.fetchRoles();
	await userStore.fetchUsersWithRoles();


});


</script>

<style scoped></style>