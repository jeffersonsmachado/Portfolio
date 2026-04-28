<script setup>
import { defineProps, defineEmits } from 'vue';

const props = defineProps({
	users: { type: Array, required: true },    // Ex: [{ id: '1', username: 'João', email: '...' }]
	roles: { type: Array, required: true }, // Ex: [{ id: 'admin', name: 'Administrador' }]
	userRoles: { type: Array, required: true },  // Ex: [{ id: '1', roles: [{ id: 'admin', name: 'Admin' }] }]
	sourceLabel: { type: String, default: 'Item de Origem' },
	targetLabel: { type: String, default: 'Atribuição' }
});

const emit = defineEmits(['update:userRoles']);

// Retrieve assigned role IDs for a given user ID
const getAssignedTargets = (sourceId) => {
	const entry = props.userRoles.find((u) => u.id === sourceId);
	if (!entry || !Array.isArray(entry.roles)) return [];
	return entry.roles.map((r) => r.id);
};

// Emits updated userRoles when a selection changes
const updateAssignment = (sourceId, event) => {
	const selectedIds = Array.from(event.target.selectedOptions).map((opt) => opt.value);
	const selectedRoles = props.roles.filter((r) => selectedIds.includes(r.id));
	const newAssignments = props.userRoles.map((u) =>
		u.id === sourceId ? { ...u, roles: selectedRoles } : u
	);

	emit('update:userRoles', newAssignments);
};
</script>

<template>
	<div class="dynamic-assigner">
		<table border="1" style="width: 100%; text-align: left; border-collapse: collapse;">
			<thead>
				<tr>
					<th>{{ sourceLabel }}</th>
					<th>{{ targetLabel }}</th>
				</tr>
			</thead>
			<tbody>
				<tr v-for="user in users" :key="user.id">
					<td>{{ user.username }}</td>
					<td>
						<select multiple @change="updateAssignment(user.id, $event)">
							<option v-for="role in roles" :key="role.id" :value="role.id"
								:selected="getAssignedTargets(user.id).includes(role.id)">
								{{ role.name }}
							</option>
						</select>
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</template>