<script setup lang="ts">

import { ref } from 'vue'
import router from '../routes'
import { useAuthStore } from '../stores/auth'

const email = ref<string>('')
const password = ref<string>('')
const message = ref<string>('')
const isLoading = ref<boolean>(false)
const auth = useAuthStore()

const login = async (): Promise<void> => {

	isLoading.value = true
	message.value = ''

	try {
		await auth.login(email.value, password.value)
		message.value = 'Login successful!'
		router.push('/')
	} catch (error) {
		if (error instanceof Error)
			if (error.message.includes('EMAIL_NOT_VERIFIED')) {
				message.value = 'Your email is not verified. Please check your inbox for the verification email.'
				router.push({
					path: "/verify",
					query: {
						email: email.value,
						message:
							"Your email is not verified. Please check your inbox for the verification email.",
					},
				});
			} else if (error.message.includes('Login failed')) {
				message.value = 'Invalid email or password. Please try again.'
			} else {
				message.value = 'An unexpected error occurred. Please try again later.'
			}
	} finally {
		isLoading.value = false
	}
}

</script>

<template>

	<div class="container">

		<h1>LOGIN</h1>

		<form @submit.prevent="login">
			<div>
				<label for="email">Email:</label>
				<input type="email" id="email" v-model="email" required />
			</div>
			<div>
				<label for="password">Password:</label>
				<input type="password" id="password" v-model="password" required />
			</div>
			<button type="submit" :disabled="isLoading">
				{{ isLoading ? 'Logging in...' : 'Login' }}
			</button>
		</form>



		<div v-if="message" class="message">
			<p :class="{ 'success': message === 'Login successful!', 'error': message !== 'Login successful!' }">
				{{ message }}
			</p>
		</div>
	</div>


</template>


<style scoped>
.container {
	display: flex;
	flex-direction: column;
	align-items: center;
	height: 100%;
}

h1 {
	text-align: center;
	margin-bottom: 20px;
	color: #ccc;
}

form {
	display: flex;
	flex-direction: column;
	margin-bottom: 15px;
	margin: 50px auto;
	padding: 20px;
	border: 1px solid #ccc;
	border-radius: 5px;
	box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
	background-color: #333;
	color: #ccc;
	gap: 20px;
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

.message {
	margin-top: 20px;
	text-align: center;
	background-color: burlywood;
	padding: 10px;
	border-radius: 4px;
}
</style>