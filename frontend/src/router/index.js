import { createRouter, createWebHistory } from "vue-router";
import ClienteView from "../views/ClienteView.vue";
import BarberoView from "../views/BarberoView.vue";
import AdminView from "../views/AdminView.vue";

import LoginView from "../views/LoginView.vue";

const routes = [
  { path: "/", component: ClienteView, name: "cliente" },
  { path: "/login", component: LoginView, name: "login" },
  {
    path: "/barbero",
    component: BarberoView,
    name: "barbero",
    meta: { requiresAuth: true, role: "Barbero" },
  },
  {
    path: "/admin",
    component: AdminView,
    name: "admin",
    meta: { requiresAuth: true, role: "Admin" },
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, from, next) => {
  if (to.meta.requiresAuth) {
    const userDataStr = localStorage.getItem("staff_user");
    if (!userDataStr) {
      next("/login");
    } else {
      try {
        const user = JSON.parse(userDataStr);
        if (to.meta.role && user.rol?.trim() !== to.meta.role) {
          // If trying to access admin but is barber, redirect to barber (and vice versa)
          next(user.rol?.trim() === "Admin" ? "/admin" : "/barbero");
        } else {
          next();
        }
      } catch (e) {
        localStorage.removeItem("staff_user");
        next("/login");
      }
    }
  } else {
    next();
  }
});

export default router;
