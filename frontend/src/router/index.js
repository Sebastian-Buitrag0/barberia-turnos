import { createRouter, createWebHistory } from 'vue-router'
import ClienteView from '../views/ClienteView.vue'
import BarberoView from '../views/BarberoView.vue'
import AdminView from '../views/AdminView.vue'

const routes = [
  { path: '/', component: ClienteView, name: 'cliente' },
  { path: '/barbero', component: BarberoView, name: 'barbero' },
  { path: '/admin', component: AdminView, name: 'admin' },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
