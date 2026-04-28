<script setup lang="ts">

import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const email = ref<string>('')
const serverMessage = ref<string>('')
const isLoading = ref<boolean>(false)
const isVerified = ref<boolean>(false)
const tokenResent = ref<boolean>(false)
const isLoadingResend = ref<boolean>(false)

const digits = ref<string[]>(['', '', '', '', '', ''])
const digitRefs = ref<HTMLInputElement[]>([])

const token = computed(() => digits.value.join(''))

const props = defineProps<{
	email?: string
	message?: string
}>()

if (props.email) {
	email.value = props.email
}

const onDigitInput = (index: number, event: Event): void => {
	const input = event.target as HTMLInputElement
	const value = input.value.replace(/\D/g, '')
	digits.value[index] = value.slice(-1)
	input.value = digits.value[index]
	if (digits.value[index] && index < 5) {
		digitRefs.value[index + 1]?.focus()
	}
}

const onDigitKeydown = (index: number, event: KeyboardEvent): void => {
	if (event.key === 'Backspace' && !digits.value[index] && index > 0) {
		digitRefs.value[index - 1]?.focus()
	}
}

const onDigitPaste = (event: ClipboardEvent): void => {
	event.preventDefault()
	const pasted = (event.clipboardData?.getData('text') ?? '').replace(/\D/g, '').slice(0, 6)
	pasted.split('').forEach((char, i) => { digits.value[i] = char })
	const nextEmpty = digits.value.findIndex(d => !d)
	digitRefs.value[nextEmpty === -1 ? 5 : nextEmpty]?.focus()
}

const verify = async (): Promise<void> => {
	isLoading.value = true
	serverMessage.value = ''

	try {
		const response = await fetch('https://localhost:7132/auth/verify', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({
				email: email.value,
				token: token.value
			})
		})

		if (!response.ok) {
			const data = await response.json()
			throw new Error(data?.title || 'Verification failed.')
		}

		serverMessage.value = 'Verification successful! You can now log in.'

		isVerified.value = true

	} catch (error) {
		serverMessage.value = error instanceof Error ? error.message : 'An error occurred during verification.'
	} finally {
		isLoading.value = false
	}
}

const resendToken = async (): Promise<void> => {
	isLoadingResend.value = true
	serverMessage.value = ''

	try {
		const response = await fetch('https://localhost:7132/users/resend-verification', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({ email: email.value })
		})

		if (!response.ok) {
			throw new Error('Failed to resend token.')
		}

		serverMessage.value = 'Verification token resent! Please check your email.'
		tokenResent.value = true
	} catch (error) {
		serverMessage.value = error instanceof Error ? error.message : 'An error occurred while resending the token.'
	} finally {
		isLoadingResend.value = false
	}
}

</script>

<template>

	<div v-if="!isVerified" class="container bg-primary text-primary">

		<h1 v-if="!message">Verify Account</h1>

		<p v-else="message" :class="{
			success: message.startsWith('Verification successful'),
			error: !message.startsWith('Verification successful')
		}">
			{{ message }}
		</p>

		<form @submit.prevent="verify">
			<div v-if="props.email === undefined" class="">
				<label for="email">Email:</label>
				<input type="email" id="email" v-model="email" required />
			</div>
			<div>
				<label>Verification Token:</label>
				<div class="digits-row">
					<input v-for="(_, i) in digits" :key="i"
						:ref="el => { if (el) digitRefs[i] = el as HTMLInputElement }" type="text" inputmode="numeric"
						maxlength="1" class="digit-box" :value="digits[i]" @input="onDigitInput(i, $event)"
						@keydown="onDigitKeydown(i, $event)" @paste="onDigitPaste" required />
				</div>
			</div>
			<button class="btn-primary" type="submit" :disabled="isLoading">
				{{ isLoading ? 'Verifying...' : 'Verify' }}
			</button>
		</form>

		<div class="sub-group">
			<form @submit.prevent="resendToken">
				<button class="btn-secondary" v-if="!tokenResent" type="submit"
					:disabled="isLoadingResend || tokenResent">
					{{ isLoadingResend ? 'Resending...' : 'Resend Token' }}
				</button>
			</form>
		</div>



		<p v-if="serverMessage"
			:class="{ success: serverMessage.startsWith('Verification successful'), error: !serverMessage.startsWith('Verification successful') }">
			{{ serverMessage }}
		</p>
	</div>
	<div v-else class="login-link">
		<p>You are now verified <a @click.prevent="router.push('/login')" href="#">Login here!!</a>.</p>
	</div>

</template>

<style scoped>
.container {
	max-width: 400px;
	margin: 50px auto;
	padding: 20px;
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

.digits-row {
	display: flex;
	gap: 8px;
	justify-content: center;
}

.digit-box {
	width: 42px;
	height: 52px;
	padding: 0;
	text-align: center;
	font-size: 1.4rem;
	font-weight: bold;
	border: 2px solid #ccc;
	border-radius: 6px;
	box-sizing: border-box;
	transition: border-color 0.2s;
}

.digit-box:focus {
	outline: none;
}

button {
	width: 100%;
	padding: 10px;
	border: none;
	border-radius: 4px;
	cursor: pointer;
}


.sub-group {
	display: flex;
	justify-content: center;
	margin-top: 15px;
}

.sub-group button {
	width: auto;
	padding: 8px 16px;
	width: auto;
}
</style>
