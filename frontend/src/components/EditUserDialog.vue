<script lang="ts" setup>
import { ref, watch } from "vue";
import { useUserStore, type UserDto } from "../services/userService";
import { useRoleStore } from "../services/roleService";
import { useUiStore } from "../stores/ui";

const userStore = useUserStore();
const roleStore = useRoleStore();
const ui = useUiStore();

const selectedRoleIds = ref<string[]>([]);

const props = defineProps<{
	modelValue: boolean;
	user: UserDto | null;
}>();

const form = ref({
	username: "",
	email: "",
});

const emit = defineEmits<{
	"update:modelValue": [boolean];
}>();

function cancel() {
	emit("update:modelValue", false);
}

async function save() {
	if (!props.user) return;
	ui.startLoading();
	try {
		await userStore.updateUser({
			...props.user,
			username: form.value.username,
			email: form.value.email,
		});
		await roleStore.setUserRoles(props.user.id, selectedRoleIds.value);
		await userStore.fetchUsersWithRoles();
		emit("update:modelValue", false);
	} finally {
		ui.stopLoading();
	}
}

watch(
	() => props.modelValue,
	async (open) => {
		if (open) await roleStore.fetchRoles();
	},
);

watch(
	() => props.user,
	(user) => {
		if (user) {
			form.value.username = user.username;
			form.value.email = user.email;
			selectedRoleIds.value = user.roles.map((r) => r.id);
		}
	},
);
</script>

<template>
	<v-dialog
		:model-value="modelValue"
		@update:model-value="$emit('update:modelValue', $event)"
		max-width="500px"
	>
		<v-card v-if="user">
			<v-card-title>Edit User: {{ user?.username }}</v-card-title>
			<v-card-text>
				<v-text-field v-model="form.username" label="Username" />
				<v-text-field v-model="form.email" label="Email" />
				<v-select
					v-model="selectedRoleIds"
					:items="roleStore.roles"
					item-title="name"
					item-value="id"
					label="Roles"
					multiple
					chips
				/>
			</v-card-text>
			<v-card-actions>
				<v-btn color="primary" @click="save">Save</v-btn>
				<v-btn color="error" @click="cancel">Cancel</v-btn>
			</v-card-actions>
		</v-card>
	</v-dialog>
</template>
