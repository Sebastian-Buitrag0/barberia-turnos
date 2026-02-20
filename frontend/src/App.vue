<script setup>
import { onMounted } from "vue";
import { RouterView, RouterLink } from "vue-router";
import { Ticket, Scissors, Settings, User } from "lucide-vue-next";
import { startConnection } from "./services/signalr";

onMounted(() => {
  startConnection();
});
</script>

<template>
  <div
    class="min-h-screen bg-slate-950 text-slate-100 font-sans selection:bg-red-500/30 selection:text-red-200"
  >
    <nav class="sticky top-0 z-50 glass border-b border-slate-800 shadow-sm">
      <div class="max-w-5xl mx-auto px-4">
        <div class="flex items-center justify-between h-16">
          <!-- Logo -->
          <div class="flex items-center gap-2">
            <div class="p-2 bg-red-600 rounded-lg shadow-lg shadow-red-900/20">
              <Scissors class="w-5 h-5 text-white transform -rotate-45" />
            </div>
            <a
              href="/"
              class="text-xl font-bold tracking-tight text-white hover:text-red-400 transition-colors"
            >
              Barberia <span class="text-red-500">El Paisa</span>
            </a>
          </div>

          <!-- Login Link -->
          <RouterLink
            to="/login"
            class="flex items-center gap-2 px-3 py-2 rounded-lg text-sm font-medium text-slate-400 hover:text-white hover:bg-slate-800 transition-all duration-200"
            title="Ingreso Staff"
          >
            <User class="w-5 h-5" />
          </RouterLink>
        </div>
      </div>
    </nav>

    <main class="max-w-5xl mx-auto px-4 py-8">
      <RouterView v-slot="{ Component }">
        <transition
          name="fade"
          mode="out-in"
          enter-active-class="transition duration-200 ease-out"
          enter-from-class="opacity-0 translate-y-2"
          enter-to-class="opacity-100 translate-y-0"
          leave-active-class="transition duration-150 ease-in"
          leave-from-class="opacity-100 translate-y-0"
          leave-to-class="opacity-0 translate-y-2"
        >
          <component :is="Component" />
        </transition>
      </RouterView>
    </main>
  </div>
</template>

<style scoped>
/* Scoped styles if needed */
</style>
