<script setup>
import { ref } from "vue";
import { useRouter } from "vue-router";
import api from "../services/api";
import { Lock, Loader2 } from "lucide-vue-next";

const router = useRouter();
const pin = ref("");
const error = ref(null);
const loading = ref(false);

const login = async () => {
  if (!pin.value) return;
  loading.value = true;
  error.value = null;

  try {
    const response = await api.login(pin.value);

    if (response.token) {
      const userData = { ...response.user, token: response.token };

      // Guardar sesión unificada
      localStorage.setItem("staff_user", JSON.stringify(userData));

      // Para compatibilidad con interceptor actual si se usaba 'barbero_user' y 'admin_user'
      localStorage.setItem("barbero_user", JSON.stringify(userData));
      localStorage.setItem("admin_user", JSON.stringify(userData));

      // Redirigir según el rol
      const role = userData.rol?.trim();
      if (role === "Admin") {
        router.push("/admin");
      } else if (role === "Barbero") {
        router.push("/barbero");
      } else {
        error.value = "Rol desconocido";
      }
    } else {
      error.value = "PIN incorrecto o acceso no autorizado";
    }
  } catch (e) {
    error.value = "PIN incorrecto";
  } finally {
    loading.value = false;
  }
};
</script>

<template>
  <div
    class="max-w-md mx-auto glass rounded-2xl p-8 border border-slate-800 shadow-xl mt-10"
  >
    <div class="text-center mb-8">
      <div
        class="inline-flex items-center justify-center p-3 bg-red-600/10 rounded-xl mb-4 text-red-500"
      >
        <Lock class="w-8 h-8" />
      </div>
      <h2 class="text-2xl font-bold text-white">Acceso al Sistema</h2>
      <p class="text-slate-400">Ingresa tu PIN</p>
    </div>

    <form @submit.prevent="login" class="space-y-4">
      <input
        v-model="pin"
        type="password"
        maxlength="4"
        class="block w-full text-center text-4xl tracking-[0.5em] py-4 bg-slate-900/50 border border-slate-700 rounded-xl text-white focus:ring-2 focus:ring-red-500/20 focus:border-red-500 transition-all font-mono placeholder-slate-700"
        placeholder="••••"
      />

      <div v-if="error" class="text-red-400 text-sm text-center">
        {{ error }}
      </div>

      <button
        type="submit"
        :disabled="loading"
        class="w-full bg-red-600 hover:bg-red-700 text-white font-bold py-3.5 rounded-xl shadow-lg shadow-red-900/30 transition-all disabled:opacity-50 disabled:cursor-not-allowed"
      >
        <Loader2 v-if="loading" class="w-5 h-5 animate-spin mx-auto" />
        <span v-else>Entrar</span>
      </button>
    </form>
  </div>
</template>
