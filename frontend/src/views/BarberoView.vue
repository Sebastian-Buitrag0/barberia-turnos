<script setup>
import { ref, onMounted, onUnmounted } from "vue";
import api from "../services/api";
import * as signalRService from "../services/signalr";
import { connectionState } from "../services/signalr"; // Import direct ref
import {
  Lock,
  Megaphone,
  CheckCircle,
  List,
  Scissors,
  X,
  Check,
  Loader2,
  User,
  Clock,
  Phone,
} from "lucide-vue-next";

const pin = ref("");
const usuario = ref(null);
const error = ref(null);
const colaEnEspera = ref([]);
const turnoActual = ref(null);
const showModal = ref(false);
const servicios = ref([]);
const serviciosSeleccionados = ref([]);
const loading = ref(false);
const processing = ref(false);

import { useRouter } from "vue-router";
const router = useRouter();

const logout = () => {
  usuario.value = null;
  localStorage.removeItem("barbero_user");
  localStorage.removeItem("staff_user");
  router.push("/login");
};

// Cargar estado inicial
const cargarDatos = async () => {
  if (!usuario.value) return;
  try {
    const data = await api.getCola();
    // Filtrar cola: Solo turnos sin barbero asignado o asignados a este barbero
    colaEnEspera.value = data.filter(
      (t) =>
        t.estado === "EnCola" &&
        (!t.barberoNombre || t.barberoNombre === usuario.value.nombre),
    );
    // Buscar si estoy atendiendo a alguien
    turnoActual.value = data.find(
      (t) =>
        (t.estado === "Llamado" || t.estado === "EnSilla") &&
        t.barberoNombre === usuario.value.nombre,
    );
  } catch (e) {
    console.error(e);
    showMessage("Error cargando la cola de turnos", "error");
  }
};

const isToggleLoading = ref(false);
const showConfirmModal = ref(false);

const confirmarDisponibilidad = () => {
  if (!usuario.value) return;
  showConfirmModal.value = true;
};

const executeToggleDisponibilidad = async () => {
  if (!usuario.value) return;
  showConfirmModal.value = false;

  isToggleLoading.value = true;
  try {
    const response = await api.toggleDisponibilidad();

    // Update local user state
    usuario.value.isAvailable = response.isAvailable;
    localStorage.setItem("barbero_user", JSON.stringify(usuario.value));

    if (response.isAvailable) {
      showMessage(
        "Has marcado tu entrada. Ahora estás disponible para recibir clientes.",
        "success",
      );
    } else {
      showMessage(
        "Has marcado tu salida. Ya no recibirás nuevos clientes.",
        "info",
      );
    }
  } catch (error) {
    console.error("Error al cambiar disponibilidad:", error);
    showMessage("Ocurrió un error al intentar cambiar tu estado.", "error");
  } finally {
    isToggleLoading.value = false;
  }
};

// Feedback Modal
const showFeedback = ref(false);
const feedbackMessage = ref("");
const feedbackType = ref("info"); // info, error, success

const showMessage = (msg, type = "info") => {
  feedbackMessage.value = msg;
  feedbackType.value = type;
  showFeedback.value = true;
  // Auto-close success messages
  if (type === "success") {
    setTimeout(() => {
      showFeedback.value = false;
    }, 2000);
  }
};

// Acciones
const llamarSiguiente = async () => {
  processing.value = true;
  try {
    const turno = await api.llamar(usuario.value.id);
    turnoActual.value = turno; // Update immediately
    // SignalR will also update, but this is faster
  } catch (e) {
    showMessage(
      "No se pudo llamar al siguiente (¿Quizás la cola está vacía?)",
      "error",
    );
  } finally {
    processing.value = false;
  }
};

const ponerEnSilla = async () => {
  if (!turnoActual.value) return;
  processing.value = true;
  try {
    await api.enSilla(turnoActual.value.id);
    turnoActual.value.estado = "EnSilla"; // Update immediately
  } finally {
    processing.value = false;
  }
};

const abrirModalFinalizar = async () => {
  showModal.value = true;
  try {
    servicios.value = await api.getServicios();
  } catch (e) {
    console.error(e);
  }
};

const toggleServicio = (id) => {
  const index = serviciosSeleccionados.value.indexOf(id);
  if (index === -1) serviciosSeleccionados.value.push(id);
  else serviciosSeleccionados.value.splice(index, 1);
};

const finalizarTurno = async () => {
  if (serviciosSeleccionados.value.length === 0) {
    showMessage("Selecciona al menos un servicio", "error");
    return;
  }
  processing.value = true;
  try {
    await api.finalizar(turnoActual.value.id, serviciosSeleccionados.value);
    showModal.value = false;
    serviciosSeleccionados.value = [];
    turnoActual.value = null; // Se limpia localmente, SignalR confirmará
    showMessage("Turno finalizado correctamente", "success");
  } catch (e) {
    showMessage("Error al finalizar el turno", "error");
  } finally {
    processing.value = false;
  }
};

// SignalR
const handleQueueUpdate = () => {
  cargarDatos();
  // showMessage("Lista actualizada", 'success') // Feedback opcional para confirmar que funciona
};

onMounted(() => {
  const stored = localStorage.getItem("barbero_user");
  if (stored) {
    usuario.value = JSON.parse(stored);
    cargarDatos();
  }
  signalRService.onQueueUpdated(handleQueueUpdate);
});

onUnmounted(() => {
  signalRService.offQueueUpdated(handleQueueUpdate);
});
</script>

<template>
  <div class="max-w-4xl mx-auto">
    <!-- Dashboard -->
    <div v-if="usuario" class="space-y-6">
      <!-- Top Bar -->
      <div class="flex items-center justify-between mb-6">
        <h1 class="text-2xl font-bold text-white flex items-center gap-2">
          Hola, <span class="text-red-500">{{ usuario.nombre }}</span>
        </h1>

        <div class="flex items-center gap-4">
          <!-- Connection Status -->
          <div
            class="flex items-center gap-2 px-3 py-1 rounded-full text-xs font-bold border transition-colors"
            :class="
              connectionState === 'Conectado'
                ? 'bg-green-500/10 text-green-500 border-green-500/20'
                : 'bg-red-500/10 text-red-500 border-red-500/20 animate-pulse'
            "
          >
            <div
              class="w-2 h-2 rounded-full"
              :class="
                connectionState === 'Conectado' ? 'bg-green-500' : 'bg-red-500'
              "
            ></div>
            {{ connectionState }}
          </div>

          <button
            @click="confirmarDisponibilidad"
            :disabled="isToggleLoading"
            class="flex items-center gap-2 px-4 py-2 rounded-xl text-sm font-bold border transition-all disabled:opacity-50"
            :class="
              usuario.isAvailable
                ? 'bg-emerald-500/10 text-emerald-500 border-emerald-500/20 hover:bg-emerald-500/20'
                : 'bg-slate-800 text-slate-400 border-slate-700 hover:bg-slate-700'
            "
          >
            <Clock class="w-4 h-4" />
            {{ usuario.isAvailable ? "Registrar Salida" : "Registrar Llegada" }}
          </button>

          <button
            @click="logout"
            class="text-sm text-slate-400 hover:text-white underline decoration-slate-700 hover:decoration-white transition-all ml-2"
          >
            Salir
          </button>
        </div>
      </div>

      <!-- Turno Actual Card -->
      <div
        v-if="turnoActual"
        class="glass rounded-2xl p-6 border border-slate-700 shadow-lg relative overflow-hidden group"
      >
        <div
          class="absolute top-0 right-0 p-24 bg-red-600/5 group-hover:bg-red-600/10 transition-colors blur-3xl rounded-full pointer-events-none"
        ></div>

        <div class="relative z-10">
          <div
            class="text-xs uppercase font-bold text-red-500 tracking-wider mb-2"
          >
            {{
              turnoActual.estado === "Llamado"
                ? "LLAMADO - ESPERANDO CLIENTE"
                : "ATENDIENDO AHORA"
            }}
          </div>

          <div class="flex items-center justify-between mb-8">
            <div class="flex items-center gap-4">
              <div class="text-5xl font-black text-white">
                #{{ turnoActual.turnoDiario }}
              </div>
              <div>
                <div class="text-2xl font-bold text-white">
                  {{ turnoActual.clienteNombre }}
                </div>
                <div class="text-slate-400 flex items-center gap-1.5 text-sm">
                  <Phone class="w-3.5 h-3.5" />
                  {{ turnoActual.clienteTelefono }}
                </div>
              </div>
            </div>

            <div v-if="turnoActual.estado === 'Llamado'" class="animate-pulse">
              <span
                class="px-3 py-1 bg-yellow-500/20 text-yellow-400 text-xs font-bold rounded-full border border-yellow-500/20"
                >Esperando...</span
              >
            </div>
          </div>

          <!-- Actions -->
          <div class="grid grid-cols-2 gap-4">
            <button
              v-if="turnoActual.estado === 'Llamado'"
              @click="ponerEnSilla"
              :disabled="processing"
              class="col-span-2 bg-green-600 hover:bg-green-700 text-white font-bold py-4 rounded-xl shadow-lg shadow-green-900/20 transition-all flex items-center justify-center gap-2"
            >
              <CheckCircle class="w-5 h-5" /> Confirmar En Silla
            </button>

            <template v-else>
              <button
                disabled
                class="bg-slate-800 text-green-500 font-bold py-4 rounded-xl border border-green-500/30 flex items-center justify-center gap-2 cursor-default"
              >
                <Check class="w-5 h-5" /> En Silla
              </button>
              <button
                @click="abrirModalFinalizar"
                class="bg-slate-700 hover:bg-slate-600 text-white font-bold py-4 rounded-xl transition-all flex items-center justify-center gap-2 border border-slate-600"
              >
                <CheckCircle class="w-5 h-5 text-red-500" /> Finalizar
              </button>
            </template>
          </div>
        </div>
      </div>

      <!-- Empty State / Call Button -->
      <div
        v-else
        class="glass rounded-2xl p-8 border border-slate-800 text-center py-16"
      >
        <div class="mb-6 inline-flex p-4 rounded-full bg-slate-900/50">
          <Megaphone class="w-8 h-8 text-slate-500" />
        </div>
        <h3 class="text-xl font-bold text-white mb-2">Sin cliente asignado</h3>
        <p class="text-slate-400 mb-8">
          La silla está vacía. Llama al siguiente cliente.
        </p>

        <button
          @click="llamarSiguiente"
          :disabled="processing"
          class="w-full max-w-sm mx-auto bg-red-600 hover:bg-red-700 text-white font-bold py-4 px-8 rounded-xl shadow-lg shadow-red-900/30 transition-all flex items-center justify-center gap-3 text-lg"
        >
          <Megaphone class="w-6 h-6" /> LLAMAR SIGUIENTE
        </button>
      </div>

      <!-- Queue List -->
      <div class="mt-8">
        <h3
          class="text-sm font-bold text-slate-400 uppercase tracking-wider mb-4 flex items-center gap-2"
        >
          <List class="w-4 h-4" /> Cola de Espera ({{ colaEnEspera.length }})
        </h3>

        <div class="space-y-3">
          <div
            v-for="turno in colaEnEspera"
            :key="turno.id"
            class="glass rounded-xl p-4 flex items-center justify-between border border-slate-800 hover:border-slate-700 transition-colors"
          >
            <div class="flex items-center gap-4">
              <span class="text-xl font-black text-slate-500 w-8"
                >#{{ turno.turnoDiario }}</span
              >
              <div>
                <div class="font-bold text-slate-200">
                  {{ turno.clienteNombre }}
                </div>
                <div class="text-xs text-slate-500">
                  {{ turno.clienteTelefono }}
                </div>
              </div>
            </div>
            <div
              class="px-3 py-1 bg-slate-900 rounded-full text-xs text-slate-400 border border-slate-800"
            >
              En cola
            </div>
          </div>

          <div
            v-if="colaEnEspera.length === 0"
            class="text-center py-8 text-slate-500 border-2 border-dashed border-slate-800 rounded-xl"
          >
            No hay clientes esperando
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Finalizar -->
    <div
      v-if="showModal"
      class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/80 backdrop-blur-sm"
    >
      <div
        class="glass w-full max-w-lg rounded-2xl p-6 border border-slate-700 shadow-2xl animate-in zoom-in-95 duration-200"
      >
        <div class="flex items-center justify-between mb-6">
          <h3 class="text-xl font-bold text-white flex items-center gap-2">
            <Scissors class="w-5 h-5 text-red-500" />
            Servicios Realizados
          </h3>
          <button
            @click="showModal = false"
            class="p-2 hover:bg-slate-800 rounded-lg transition-colors"
          >
            <X class="w-5 h-5 text-slate-400" />
          </button>
        </div>

        <div class="space-y-2 mb-8 max-h-[60vh] overflow-y-auto pr-2">
          <div
            v-for="servicio in servicios"
            :key="servicio.id"
            @click="toggleServicio(servicio.id)"
            class="flex items-center justify-between p-4 rounded-xl cursor-pointer border transition-all"
            :class="
              serviciosSeleccionados.includes(servicio.id)
                ? 'bg-red-600/10 border-red-500/50'
                : 'bg-slate-900/50 border-slate-800 hover:border-slate-700'
            "
          >
            <div class="flex items-center gap-3">
              <div
                class="w-5 h-5 rounded-full border flex items-center justify-center transition-colors"
                :class="
                  serviciosSeleccionados.includes(servicio.id)
                    ? 'bg-red-500 border-red-500'
                    : 'border-slate-600'
                "
              >
                <Check
                  v-if="serviciosSeleccionados.includes(servicio.id)"
                  class="w-3 h-3 text-white"
                />
              </div>
              <span class="font-medium text-slate-200">{{
                servicio.nombre
              }}</span>
            </div>
            <span class="font-bold text-slate-300">${{ servicio.precio }}</span>
          </div>
        </div>

        <button
          @click="finalizarTurno"
          :disabled="processing"
          class="w-full bg-red-600 hover:bg-red-700 text-white font-bold py-4 rounded-xl shadow-lg shadow-red-900/30 transition-all"
        >
          Guardar y Finalizar
        </button>
      </div>
    </div>

    <!-- Confirmation Modal for Arrival/Departure -->
    <div
      v-if="showConfirmModal"
      class="fixed inset-0 z-[60] flex items-center justify-center p-4 bg-black/50 backdrop-blur-sm"
    >
      <div
        class="glass w-full max-w-sm rounded-2xl p-6 border border-slate-700 shadow-2xl animate-in zoom-in-95 text-center"
      >
        <div
          class="w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4 bg-slate-800 text-slate-400"
        >
          <Clock class="w-8 h-8" />
        </div>
        <h3 class="text-xl font-bold text-white mb-2">Confirmar Acción</h3>
        <p class="text-slate-400 mb-6 font-medium">
          ¿Estás seguro de que deseas
          <strong class="text-white">{{
            usuario?.isAvailable
              ? "registrar tu SALIDA"
              : "registrar tu LLEGADA"
          }}</strong
          >?
          <span class="block mt-2 text-sm text-slate-500 font-normal"
            >Tu estado se actualizará en la pantalla principal de
            clientes.</span
          >
        </p>

        <div class="grid grid-cols-2 gap-3">
          <button
            @click="showConfirmModal = false"
            class="py-3 rounded-xl font-bold transition-all bg-slate-800 hover:bg-slate-700 text-slate-300"
          >
            Cancelar
          </button>
          <button
            @click="executeToggleDisponibilidad"
            class="py-3 rounded-xl font-bold transition-all text-white shadow-lg"
            :class="
              usuario?.isAvailable
                ? 'bg-slate-600 hover:bg-slate-500 border border-slate-500 shadow-slate-900/50'
                : 'bg-emerald-600 hover:bg-emerald-700 shadow-emerald-900/30'
            "
          >
            Confirmar
          </button>
        </div>
      </div>
    </div>

    <!-- Feedback Modal -->
    <div
      v-if="showFeedback"
      class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/80 backdrop-blur-sm"
    >
      <div
        class="glass w-full max-w-sm rounded-2xl p-6 border border-slate-700 shadow-2xl animate-in zoom-in-95 duration-200 text-center"
      >
        <div
          class="w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4"
          :class="
            feedbackType === 'error'
              ? 'bg-red-500/10 text-red-500'
              : 'bg-green-500/10 text-green-500'
          "
        >
          <component
            :is="feedbackType === 'error' ? X : Check"
            class="w-8 h-8"
          />
        </div>
        <h3 class="text-xl font-bold text-white mb-2">
          {{ feedbackType === "error" ? "Atención" : "¡Listo!" }}
        </h3>
        <p class="text-slate-400 mb-6">{{ feedbackMessage }}</p>

        <button
          @click="showFeedback = false"
          class="w-full py-3 rounded-xl font-bold transition-all"
          :class="
            feedbackType === 'error'
              ? 'bg-red-600 hover:bg-red-700 text-white'
              : 'bg-green-600 hover:bg-green-700 text-white'
          "
        >
          Entendido
        </button>
      </div>
    </div>
  </div>
</template>
