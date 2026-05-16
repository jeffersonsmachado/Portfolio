<script lang="ts" setup>
import { ref, watch } from "vue";
import { useRoleStore, type RoleDto } from "../services/roleService";
import { useUiStore } from "../stores/ui";

const roleStore = useRoleStore();
const ui = useUiStore();

const props = defineProps<{
	modelValue: boolean;
	role: RoleDto | null;
}>();

const emit = defineEmits<{
	"update:modelValue": [boolean];
}>();

const form = ref({ name: "" });
const selectedPermissionsIds = ref<string[]>([]);

async function save() {
	if (!props.role) return;
	ui.startLoading();
	try {
		await roleStore.setRolePermissions(
			props.role.id,
			selectedPermissionsIds.value,
		);
		await roleStore.fetchRoles();
		emit("update:modelValue", false);
	} finally {
		ui.stopLoading();
	}
}

function cancel() {
	emit("update:modelValue", false);
}

watch(
	() => props.modelValue,
	async (open) => {
		if (open) {
			await roleStore.fetchPermissions();
		}
	},
);

watch(
	() => props.role,
	(role) => {
		if (role) {
			form.value.name = role.name;
			selectedPermissionsIds.value = role.permissions.map((p) => p.id);
		}
	},
);
</script>

<template>
	<v-dialog
		:model-value="modelValue"
		max-width="500px"
		@update:model-value="$emit('update:modelValue', $event)"
	>
		<v-card v-if="role">
			<v-card-title>Edit Role: {{ role.name }}</v-card-title>
			<v-card-text>
				<v-text-field v-model="form.name" label="Role Name" />
				<v-select
					v-model="selectedPermissionsIds"
					:items="roleStore.permissions"
					item-title="name"
					item-value="id"
					label="Permissions"
					multiple
					chips
				/>
			</v-card-text>
			<v-card-actions>
				<v-btn color="primary" @click="save">Save</v-btn>
				<v-btn color="secondary" @click="cancel">Cancel</v-btn>
			</v-card-actions>
		</v-card>
	</v-dialog>
</template>
