<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useUserStore, type UserDto } from "../../services/userService";
import EditUserDialog from "../../components/EditUserDialog.vue";

const userStore = useUserStore();
const selectedUser = ref<UserDto | null>(null);
const dialogOpen = ref(false);

const headers = [
	{ title: "Username", key: "username", sortable: true },
	{ title: "Email", key: "email", sortable: true },
	{ title: "Roles", key: "roles", sortable: false },
	{ title: "Actions", key: "actions", sortable: false },
	// Add more fields as needed
];

function editUser(user: UserDto) {
	selectedUser.value = user;
	dialogOpen.value = true;
}

function deleteUser() {
	// todo
}

onMounted(async () => {
	await userStore.fetchUsersWithRoles();
});
</script>

<template>
	<div>
		<h1>Users Management</h1>
		<p>
			This is where you can manage users, view their details, and perform
			administrative actions.
		</p>
		<v-data-table :items="userStore.usersWithRoles" :headers="headers">
			<template #item.roles="{ item }">
				{{ item.roles.map((role) => role.name).join(", ") }}
			</template>
			<template #item.actions="{ item }">
				<v-btn color="primary" @click="editUser(item)">Edit</v-btn>
				<v-btn color="error" @click="deleteUser(item)">Delete</v-btn>
			</template>
		</v-data-table>
		<EditUserDialog v-model="dialogOpen" :user="selectedUser" />
	</div>
</template>

<style scoped></style>
