import { createRouter, createWebHistory } from "vue-router";
import Login from "@/views/Login.vue";
import Register from "@/views/Register.vue";
import Dashboard from "@/views/Dashboard.vue";
import Members from "@/views/Members.vue";
import NotFound from "@/views/NotFound.vue";

const routes = [
  { path: "/login", component: Login },
  { path: "/register", component: Register },
  { path: "/dashboard", component: Dashboard },
  { path: "/members", component: Members },
  { path: "/clubs", component: () => import("@/views/Clubs.vue") },
  { path: "/activities", component: () => import("@/views/Activities.vue") },
  { path: "/wallet", component: () => import("@/views/Wallet.vue") },
  { path: "/booking", component: () => import("@/views/BookingView.vue") },
  { path: "/tournaments", component: () => import("@/views/TournamentsView.vue") },
  { path: "/tournaments/:id", component: () => import("@/views/TournamentDetail.vue") },
  { path: "/matches", component: () => import("@/views/MatchesView.vue") },
  { path: "/", redirect: "/dashboard" },
  { path: "/:pathMatch(.*)*", component: NotFound },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// 🔐 GUARD
router.beforeEach((to, _from, next) => {
  const token = localStorage.getItem("token");
  const publicRoutes = ["/login", "/register"];

  if (!publicRoutes.includes(to.path) && !token) {
    next("/login");
  } else {
    next();
  }
});

export default router;
