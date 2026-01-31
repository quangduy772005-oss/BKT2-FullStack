import { createRouter, createWebHistory } from "vue-router";
import LoginView from "../views/LoginView.vue";
import MembersView from "../views/MembersView.vue";

const routes = [
  { path: "/", redirect: "/login" },
  { path: "/login", component: LoginView },
  { path: "/members", component: MembersView },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
