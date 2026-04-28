<script setup lang="ts">

import { useRouter } from 'vue-router'
import { useAuthStore } from './stores/auth'

const router = useRouter()
const auth = useAuthStore()

const logout = (): void => {
	auth.logout()
	router.push('/login')
}

</script>

<template>

	<div class="app-layout">

		<nav class="navbar">
			<div class="brand" @click="router.push('/')">Portfolio</div>
			<div class="nav-links">
				<template v-if="!auth.isLoggedIn">
					<RouterLink class="nav-btn" active-class="primary" to="/login">Login</RouterLink>
					<RouterLink class="nav-btn" active-class="primary" to="/register">Register</RouterLink>
					<RouterLink class="nav-btn" active-class="primary" to="/verify">Verify</RouterLink>
				</template>
				<template v-else>
					<div class="profile-avatar" @click="router.push('/profile')" title="Profile">
						<span>AVATAR</span>
					</div>
					<div>
						<div class="nav-links">
							<RouterLink v-if="auth.capabilities['roles']?.canRead" class="nav-btn"
								active-class="primary" to="/roles">Roles</RouterLink>
							<RouterLink v-if="auth.capabilities['user']?.canRead" class="nav-btn" active-class="primary"
								to="/users">Users</RouterLink>
						</div>
					</div>
					<button class="nav-btn logout" @click="logout">Logout</button>
				</template>
			</div>
		</nav>

		<main class="main-content">
			<router-view />
		</main>

		<footer class="footer">
			<p>&copy; 2026 Portfolio. All rights reserved.</p>
		</footer>

	</div>

</template>

<style scoped>
.app-layout {
	display: flex;
	flex-direction: column;
	min-height: 100vh;
	background-color: var(--bg-primary);
}

.navbar {
	display: flex;
	justify-content: space-between;
	align-items: center;
	padding: 1rem 2rem;
	background-color: #333;
	color: #fff;
}

.navbar .brand {
	font-size: 1.5rem;
	font-weight: bold;
	cursor: pointer;
}

.navbar .nav-links {
	display: flex;
	align-items: center;
}

.navbar .nav-btn {
	margin-left: 1rem;
	padding: 0.5rem 1rem;
	background-color: #007BFF;
	color: white;
	border: none;
	border-radius: 4px;
	cursor: pointer;
}

.navbar .nav-btn:hover {
	background-color: #0056b3;
}

.navbar .nav-btn.primary {
	background-color: #28a745;
}

.navbar .nav-btn.primary:hover {
	background-color: #1e7e34;
}

.navbar .nav-btn.logout {
	background-color: #dc3545;
}

.navbar .nav-btn.logout:hover {
	background-color: #c82333;
}

.profile-avatar {
	width: 40px;
	height: 40px;
	background-color: #007BFF;
	color: white;
	border-radius: 50%;
	display: flex;
	align-items: center;
	justify-content: center;
	cursor: pointer;
	font-weight: bold;
}

.profile-avatar:hover {
	background-color: #0056b3;
}

.main-content {
	flex: 1;
	padding: 2rem;
	display: flex;
	justify-content: center;
	flex-direction: column;
}

.footer {
	text-align: center;
	padding: 1rem;
	background-color: #f8f9fa;
	color: #333;
}
</style>