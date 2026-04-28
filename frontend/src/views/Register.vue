<script setup lang="ts">

import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const username = ref<string>('')
const email = ref<string>('')
const password = ref<string>('')
const confirmPassword = ref<string>('')
const message = ref<string>('')
const isLoading = ref<boolean>(false)

const register = async (): Promise<void> => {
	isLoading.value = true
	message.value = ''

	try {
		const response = await fetch('https://localhost:7132/auth/register', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({
				username: username.value,
				email: email.value,
				password: password.value,
				confirmPassword: confirmPassword.value
			})
		})

		if (!response.ok) {
			const data = await response.json()
			throw new Error(data?.title || 'Registration failed.')
		}

		message.value = 'Registration successful! Please check your email to verify your account.'
		router.push({ path: '/verify', query: { email: email.value } })
	} catch (error) {
		message.value = error instanceof Error ? error.message : 'An error occurred during registration.'
	} finally {
		isLoading.value = false
	}
}

</script>

<template>

	<div class="container">

		<h1>Register</h1>

		<form @submit.prevent="register">
			<div>
				<label for="username">Username:</label>
				<input type="text" id="username" v-model="username" required />
			</div>
			<div>
				<label for="email">Email:</label>
				<input type="email" id="email" v-model="email" required />
			</div>
			<div>
				<label for="password">Password:</label>
				<input type="password" id="password" v-model="password" required />
			</div>
			<div>
				<label for="confirmPassword">Confirm Password:</label>
				<input type="password" id="confirmPassword" v-model="confirmPassword" required />
			</div>
			<button type="submit" :disabled="isLoading">
				{{ isLoading ? 'Registering...' : 'Register' }}
			</button>
		</form>

		<p v-if="message"
			:class="{ success: message.startsWith('Registration successful'), error: !message.startsWith('Registration successful') }">
			{{ message }}
		</p>

		<p class="login-link">
			Already have an account? <span @click="router.push('/login')">Login</span>
		</p>

	</div>

</template>

<style scoped>
.container {
	max-width: 400px;
	margin: 50px auto;
	padding: 20px;
	border: 1px solid #ccc;
	border-radius: 5px;
	box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
}

h1 {
	text-align: center;
	margin-bottom: 20px;
}

form div {
	margin-bottom: 15px;
}

label {
	display: block;
	margin-bottom: 5px;
	font-weight: bold;
}

input {
	width: 100%;
	padding: 8px;
	box-sizing: border-box;
	border: 1px solid #ccc;
	border-radius: 4px;
}

button {
	width: 100%;
	padding: 10px;
	background-color: #007BFF;
	color: white;
	border: none;
	border-radius: 4px;
	cursor: pointer;
}

button:hover:not(:disabled) {
	background-color: #0056b3;
}

.success {
	color: green;
}

.error {
	color: red;
}

.login-link {
	text-align: center;
	margin-top: 15px;
	font-size: 0.9rem;
}

.login-link span {
	color: #007BFF;
	cursor: pointer;
}

.login-link span:hover {
	text-decoration: underline;
}
</style>
