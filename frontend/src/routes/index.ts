import {
	createRouter,
	createWebHistory,
	type RouteRecordRaw,
} from "vue-router";

const routes: Array<RouteRecordRaw> = [
	{
		path: "/",
		name: "Home",
		component: () => import("../views/Home.vue"),
	},
	{
		path: "/login",
		name: "Login",
		component: () => import("../views/Login.vue"),
	},
	{
		path: "/register",
		name: "Register",
		component: () => import("../views/Register.vue"),
	},
	{
		path: "/verify",
		name: "Verify",
		component: () => import("../views/Verify.vue"),
		props: (route) => ({
			email: route.query.email,
			message: route.query.message,
		}),
	},
	{
		path: "/profile",
		component: () => import("../views/Profile/Home.vue"),
	},
	{
		path: "/roles",
		name: "Roles",
		component: () => import("../views/Admin/Roles.vue"),
	},
	{
		path: "/users",
		name: "Users",
		component: () => import("../views/Admin/Users.vue"),
	},
];

const router = createRouter({
	history: createWebHistory(),
	routes,
});

export default router;
