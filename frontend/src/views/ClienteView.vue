<script setup>
import { ref, onMounted, onUnmounted, watch } from "vue";
import api from "../services/api";
import * as signalRService from "../services/signalr";
import {
  Ticket,
  User,
  Phone,
  Loader2,
  PlayCircle,
  Clock,
  BellRing,
  Armchair,
  AlertCircle,
  Scissors,
  CheckCircle2,
} from "lucide-vue-next";

const nombre = ref("");
// Turno State
const miTurno = ref(null);
const personasDelante = ref(0);
const loading = ref(true); // Changed initial loading state to true
const error = ref(null);
const registering = ref(false);
const barberos = ref([]);
const selectedBarberoId = ref(null);

const showLlamadoAlert = ref(false);

// Formulario
const telefonoArea = ref("+57");
const telefonoNumero = ref("");

// Lista de Indicativos Comunes (Ejemplo)
const countryCodes = [
  { code: "+57", flag: "🇨🇴", name: "Colombia" },
  { code: "+1", flag: "🇺🇸", name: "USA/Canada" },
  { code: "+52", flag: "🇲🇽", name: "Mexico" },
  { code: "+34", flag: "🇪🇸", name: "España" },
  { code: "+54", flag: "🇦🇷", name: "Argentina" },
  { code: "+56", flag: "🇨🇱", name: "Chile" },
  { code: "+51", flag: "🇵🇪", name: "Perú" },
];

// Cargar estado inicial
const cargarEstado = async () => {
  const storedPhone = localStorage.getItem("cliente_telefono");
  if (storedPhone) {
    loading.value = true;
    try {
      const data = await api.miTurno(storedPhone);
      if (data) {
        miTurno.value = data; // The endpoint returns the turn object directly with extras
        personasDelante.value = data.personasDelante;
        // Attempt to pre-fill phone number if stored
        const matchedCode = countryCodes.find((c) =>
          storedPhone.startsWith(c.code),
        );
        if (matchedCode) {
          telefonoArea.value = matchedCode.code;
          telefonoNumero.value = storedPhone.substring(matchedCode.code.length);
        } else {
          // Fallback if no country code matches, just put the whole number in numero
          telefonoNumero.value = storedPhone;
        }
      }
    } catch (e) {
      // Si no hay turno 404, limpiamos storage
      if (e.response && e.response.status === 404) {
        localStorage.removeItem("cliente_telefono");
        miTurno.value = null;
      }
    } finally {
      loading.value = false;
    }
  } else {
    loading.value = false; // No stored phone, so not loading a turn
  }

  // Find available barbers
  try {
    const list = await api.getBarberos();
    barberos.value = list;
  } catch (e) {
    console.error("No se pudieron cargar los barberos", e);
  }
};

// Registrar Turno
const sacarTurno = async () => {
  if (!nombre.value || !telefonoNumero.value) {
    error.value = "Por favor completa todos los campos";
    return;
  }

  registering.value = true;
  error.value = null;

  const fullPhone = `${telefonoArea.value}${telefonoNumero.value.replace(/\D/g, "")}`;

  try {
    await api.registrar(nombre.value, fullPhone, selectedBarberoId.value);

    localStorage.setItem("cliente_telefono", fullPhone);
    await cargarEstado();
  } catch (e) {
    error.value = e.response?.data?.message || "Error al sacar turno";
  } finally {
    registering.value = false;
  }
};

// Registrar otro turno (reset)
const reset = () => {
  localStorage.removeItem("cliente_telefono");
  miTurno.value = null;
  nombre.value = "";
  telefonoArea.value = "+57"; // Reset to default country code
  telefonoNumero.value = "";
  selectedBarberoId.value = null;
  error.value = null;
};

// Cancelar Turno Actual
const cancelarTurno = async () => {
  if (
    !confirm(
      "¿Estás seguro de que quieres cancelar tu turno? Tendrás que volver a sacar uno nuevo si cambias de opinión.",
    )
  )
    return;

  const storedPhone = localStorage.getItem("cliente_telefono");
  if (!storedPhone) return;

  try {
    loading.value = true;
    await api.cancelar(storedPhone);
    // Remove local data to show the registration form again
    reset();
  } catch (e) {
    alert("Hubo un error al intentar cancelar el turno.");
    console.error(e);
  } finally {
    loading.value = false;
  }
};

// SignalR Events
const handleQueueUpdate = async () => {
  const storedPhone = localStorage.getItem("cliente_telefono");
  if (storedPhone) {
    const data = await api.miTurno(storedPhone);
    if (data) {
      miTurno.value = data;
      personasDelante.value = data.personasDelante;
    }
  }
};

// Watcher for "Llamado" state to trigger alert
watch(
  () => miTurno.value?.estado,
  (newEstado, oldEstado) => {
    if (newEstado === "Llamado" && oldEstado !== "Llamado") {
      showLlamadoAlert.value = true;
      // Play a sound
      try {
        const audio = new Audio(
          "https://actions.google.com/sounds/v1/alarms/beep_short.ogg",
        );
        audio
          .play()
          .catch((e) => console.log("Audio play blocked by browser:", e));

        // Vibrate if supported
        if (navigator.vibrate) {
          navigator.vibrate([200, 100, 200, 100, 500]);
        }
      } catch (e) {
        console.error("No se pudo reproducir el sonido o vibración", e);
      }
    }
  },
);

onMounted(() => {
  cargarEstado();
  signalRService.onQueueUpdated(handleQueueUpdate);
});

onUnmounted(() => {
  signalRService.offQueueUpdated(handleQueueUpdate);
});
</script>

<template>
  <div class="max-w-md mx-auto">
    <!-- Header -->
    <div class="text-center mb-10">
      <div
        class="inline-flex items-center justify-center p-4 bg-slate-900 rounded-2xl mb-4 border border-slate-800 shadow-xl shadow-black/40"
      >
        <Armchair class="w-12 h-12 text-red-500" />
      </div>
      <h1 class="text-3xl font-bold mb-2">Bienvenido a Barberia El Paisa</h1>
      <p class="text-slate-400">Tu turno, sin esperas innecesarias.</p>
    </div>

    <!-- Loading State -->
    <div
      v-if="loading"
      class="flex flex-col items-center justify-center py-12 text-slate-400"
    >
      <Loader2 class="w-8 h-8 animate-spin mb-4 text-red-500" />
      <p>Cargando tu información...</p>
    </div>

    <!-- Waiting Screen -->
    <div
      v-else-if="miTurno"
      class="glass rounded-2xl p-8 border border-slate-700 shadow-2xl relative overflow-hidden"
    >
      <!-- Background Accents -->
      <div
        class="absolute top-0 right-0 p-32 bg-red-600/5 blur-[80px] rounded-full pointer-events-none"
      ></div>

      <div class="relative z-10 text-center">
        <!-- Status Indicator -->
        <div class="mb-8">
          <span
            v-if="miTurno.estado === 'EnCola'"
            class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-blue-500/10 text-blue-400 border border-blue-500/20 text-sm font-medium"
          >
            <Clock class="w-4 h-4" /> En Espera
          </span>
          <span
            v-else-if="miTurno.estado === 'Llamado'"
            class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-yellow-500/10 text-yellow-400 border border-yellow-500/20 text-sm font-medium animate-pulse"
          >
            <BellRing class="w-4 h-4" /> ¡Es tu Turno!
          </span>
          <span
            v-else
            class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-green-500/10 text-green-400 border border-green-500/20 text-sm font-medium"
          >
            <Scissors class="w-4 h-4" /> Siendo Atendido
          </span>
        </div>

        <h2
          class="text-slate-400 text-sm uppercase tracking-wider font-semibold mb-2"
        >
          Tu Número
        </h2>
        <div class="text-7xl font-black text-white mb-8 tracking-tighter">
          #{{ miTurno.turnoDiario }}
        </div>

        <!-- Waiting Info -->
        <div class="grid grid-cols-1 gap-4 mb-8">
          <div class="bg-slate-900/50 rounded-xl p-4 border border-slate-800">
            <div class="text-slate-400 text-xs uppercase font-bold mb-1">
              Personas delante
            </div>
            <div class="text-2xl font-bold text-white">
              {{ personasDelante }}
            </div>
          </div>

          <div
            v-if="miTurno.barberoNombre"
            class="bg-slate-900/50 rounded-xl p-4 border border-slate-800"
          >
            <div class="text-slate-400 text-xs uppercase font-bold mb-1">
              Barbero Seleccionado
            </div>
            <div class="text-xl font-bold text-white">
              {{ miTurno.barberoNombre }}
            </div>
          </div>
        </div>

        <p
          class="text-slate-400 text-sm mb-6 flex items-center justify-center gap-2"
        >
          Te avisaremos cuando sea tu momento <br />
          <span class="text-xs text-slate-500"
            >(Mantén esta pantalla abierta)</span
          >
        </p>

        <div class="flex flex-col gap-3 max-w-xs mx-auto">
          <button
            @click="cancelarTurno"
            class="w-full flex items-center justify-center gap-2 py-3 px-4 bg-red-500/10 hover:bg-red-500/20 border border-red-500/20 text-red-500 rounded-xl font-medium transition-all"
          >
            <svg
              class="w-4 h-4"
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M6 18L18 6M6 6l12 12"
              />
            </svg>
            Cancelar Mi Turno
          </button>

          <button
            @click="reset"
            class="text-slate-500 hover:text-white text-sm underline decoration-slate-700 hover:decoration-white transition-all"
          >
            Registrar otro turno (Debug)
          </button>
        </div>
      </div>
    </div>

    <!-- Registration Form -->
    <div v-else class="glass rounded-2xl p-8 border border-slate-800 shadow-xl">
      <h2 class="text-xl font-bold mb-6 flex items-center gap-2 text-white">
        <Ticket class="w-5 h-5 text-red-500" />
        Nuevo Turno
      </h2>

      <form @submit.prevent="sacarTurno" class="space-y-5">
        <!-- Nombre Input -->
        <div class="space-y-2">
          <label class="block text-sm font-medium text-slate-300"
            >Nombre Completo</label
          >
          <div class="relative group">
            <div
              class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none"
            >
              <User
                class="h-5 w-5 text-slate-500 group-focus-within:text-red-500 transition-colors"
              />
            </div>
            <input
              v-model="nombre"
              type="text"
              class="block w-full pl-10 pr-3 py-3 bg-slate-900/50 border border-slate-700 rounded-xl text-white placeholder-slate-500 focus:ring-2 focus:ring-red-500/20 focus:border-red-500 transition-all"
              placeholder="Ej. Juan Pérez"
              required
            />
          </div>
        </div>

        <!-- Telefono Input -->
        <div class="space-y-2">
          <label class="block text-sm font-medium text-slate-300"
            >Teléfono (WhatsApp)</label
          >
          <div class="flex gap-2 relative">
            <!-- Indicativo -->
            <div class="relative w-1/3">
              <select
                v-model="telefonoArea"
                class="block w-full pl-3 pr-8 py-3 bg-slate-900/50 border border-slate-700 rounded-xl text-white focus:ring-2 focus:ring-red-500/20 focus:border-red-500 transition-all appearance-none text-sm"
              >
                <option v-for="c in countryCodes" :key="c.code" :value="c.code">
                  {{ c.flag }} {{ c.code }}
                </option>
              </select>
              <div
                class="pointer-events-none absolute inset-y-0 right-0 flex items-center px-2 text-slate-400"
              >
                <svg
                  class="fill-current h-4 w-4"
                  xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 20 20"
                >
                  <path
                    d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z"
                  />
                </svg>
              </div>
            </div>

            <!-- Número -->
            <div class="relative group flex-1">
              <div
                class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none"
              >
                <Phone
                  class="h-5 w-5 text-slate-500 group-focus-within:text-red-500 transition-colors"
                />
              </div>
              <input
                v-model="telefonoNumero"
                @input="telefonoNumero = $event.target.value.replace(/\D/g, '')"
                type="tel"
                class="block w-full pl-10 pr-3 py-3 bg-slate-900/50 border border-slate-700 rounded-xl text-white placeholder-slate-500 focus:ring-2 focus:ring-red-500/20 focus:border-red-500 transition-all text-sm"
                placeholder="Ej. 300 123 4567"
                pattern="[0-9]{10}"
                title="Debe ser un número de 10 dígitos sin espacios ni guiones"
                required
              />
            </div>
          </div>
          <p class="text-xs text-slate-500 mt-1">
            Requerido para el recordatorio de asistencia (WhatsApp)
          </p>
        </div>

        <!-- Barbero Selection -->
        <div class="space-y-2">
          <label class="block text-sm font-medium text-slate-300"
            >Barbero de Preferencia (Opcional)</label
          >
          <div class="relative group">
            <div
              class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none"
            >
              <Scissors
                class="h-5 w-5 text-slate-500 group-focus-within:text-red-500 transition-colors"
              />
            </div>
            <select
              v-model="selectedBarberoId"
              class="block w-full pl-10 pr-3 py-3 bg-slate-900/50 border border-slate-700 rounded-xl text-white focus:ring-2 focus:ring-red-500/20 focus:border-red-500 transition-all appearance-none"
            >
              <option :value="null">Cualquier Barbero</option>
              <option v-for="b in barberos" :key="b.id" :value="b.id">
                {{ b.nombre }}
              </option>
            </select>
          </div>
        </div>

        <!-- Error Message -->
        <div
          v-if="error"
          class="flex items-center gap-2 p-3 bg-red-500/10 border border-red-500/20 rounded-lg text-red-400 text-sm"
        >
          <AlertCircle class="w-4 h-4 shrink-0" />
          {{ error }}
        </div>

        <!-- Submit Button -->
        <button
          type="submit"
          :disabled="registering"
          class="w-full flex items-center justify-center gap-2 bg-red-600 hover:bg-red-700 disabled:opacity-50 disabled:cursor-not-allowed text-white font-bold py-3.5 px-4 rounded-xl shadow-lg shadow-red-900/30 transition-all duration-200 transform hover:-translate-y-0.5"
        >
          <Loader2 v-if="registering" class="w-5 h-5 animate-spin" />
          <span v-else>Sacar Turno</span>
        </button>
      </form>
    </div>

    <!-- Llamado Fullscreen Alert -->
    <transition
      name="fade"
      enter-active-class="transition duration-300 ease-out"
      enter-from-class="opacity-0 scale-95"
      enter-to-class="opacity-100 scale-100"
      leave-active-class="transition duration-200 ease-in"
      leave-from-class="opacity-100 scale-100"
      leave-to-class="opacity-0 scale-95"
    >
      <div
        v-if="showLlamadoAlert && miTurno"
        class="fixed inset-0 z-[100] flex flex-col items-center justify-center p-6 bg-slate-950/95 backdrop-blur-md"
      >
        <div class="absolute inset-0 overflow-hidden pointer-events-none">
          <div class="absolute inset-0 bg-red-500/10 animate-pulse"></div>
          <div
            class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-96 h-96 bg-red-500/20 rounded-full blur-[100px] animate-ping"
          ></div>
        </div>

        <div
          class="relative z-10 max-w-sm w-full bg-slate-900 border border-red-500/30 rounded-3xl p-8 text-center shadow-2xl shadow-red-500/20"
        >
          <div
            class="inline-flex items-center justify-center p-5 bg-red-500 rounded-2xl mb-6 shadow-lg shadow-red-500/40 animate-bounce"
          >
            <BellRing class="w-12 h-12 text-white" />
          </div>

          <h2
            class="text-4xl font-black text-white mb-2 tracking-tight uppercase"
          >
            ¡Es tu turno!
          </h2>
          <p class="text-xl text-slate-300 mb-8 font-medium">
            Por favor, acércate a la silla.
          </p>

          <div
            v-if="miTurno.barberoNombre"
            class="bg-slate-800/50 rounded-xl p-4 mb-8 border border-slate-700"
          >
            <p
              class="text-sm text-slate-400 font-medium uppercase tracking-wider mb-1"
            >
              Te espera
            </p>
            <p class="text-2xl font-bold text-white">
              {{ miTurno.barberoNombre }}
            </p>
          </div>

          <button
            @click="showLlamadoAlert = false"
            class="w-full py-4 px-6 bg-white text-slate-900 hover:bg-slate-100 rounded-xl font-bold text-lg transition-colors flex items-center justify-center gap-2"
          >
            <CheckCircle2 class="w-6 h-6" />
            ¡Voy para allá!
          </button>
        </div>
      </div>
    </transition>
  </div>
</template>
