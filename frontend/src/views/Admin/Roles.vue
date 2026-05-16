<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useUserStore, type UserDto } from "../../services/userService";
import { useRoleStore, type RoleDto } from "../../services/roleService";
import EditRoleDialog from "../../components/EditRoleDialog.vue";

const userStore = useUserStore();
const roleStore = useRoleStore();
const sourceLabel = "Users";
const targetLabel = "Roles";

const dialogOpen = ref(false);
const selectedRole = ref<RoleDto | null>(null);

const headers = [
	{ title: "Name", key: "name", sortable: true },
	{ title: "Permissions", key: "permissions", sortable: false },
	{ title: "Actions", key: "actions", sortable: false },
];

onMounted(async () => {
	await userStore.fetchUsers();
	await roleStore.fetchRoles();
	await userStore.fetchUsersWithRoles();
});

function editRole(role: RoleDto) {
	selectedRole.value = role;
	dialogOpen.value = true;
}
</script>

<template>
	<div>
		<h1>Roles Management</h1>
		<p>This is where you can manage user roles and permissions.</p>
		<v-data-table :items="roleStore.roles" :headers="headers">
			<template #item.permissions="{ item }">
				{{ item.permissions.map((p) => p.name).join(", ") }}
			</template>
			<template #item.actions="{ item }">
				<v-btn color="primary" @click="editRole(item)">Edit</v-btn>
				<v-btn color="error" @click="deleteRole(item)">Delete</v-btn>
			</template>
		</v-data-table>
		<EditRoleDialog v-model="dialogOpen" :role="selectedRole" />
	</div>
</template>

<style scoped></style>
