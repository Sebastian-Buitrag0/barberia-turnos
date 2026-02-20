<script setup>
import { ref, onMounted, onUnmounted, computed } from "vue";
import api from "../services/api";
import * as signalRService from "../services/signalr";
import QrcodeVue from "qrcode.vue";
import {
  Lock,
  Loader2,
  CreditCard,
  Plus,
  QrCode,
  List,
  Check,
  Trash2,
  Calendar,
  User,
  Phone,
  CheckSquare,
  BarChart3,
  TrendingUp,
  Users,
  Wallet,
} from "lucide-vue-next";

const pin = ref("");
const usuario = ref(null);
const error = ref(null);
const activeTab = ref("porpagar");
const loading = ref(false);

// Data
const turnosPorPagar = ref([]);
const colaCompleta = ref([]); // Ahora trae todos los del día
// Form admin registra
const form = ref({ nombre: "", telefono: "" });
const formSuccess = ref(null);

// Barberos Gestión Data
const listaBarberos = ref([]);
const barberoForm = ref({ id: null, nombre: "", pin: "" });
const showBarberoModal = ref(false);
const showDeleteConfirmModal = ref(false);
const barberoToDelete = ref(null);
const barberoFormError = ref(null);

// Computed
const qrValue = window.location.origin;

const stats = computed(() => {
  const totalIngresos = colaCompleta.value
    .filter((t) => t.estado === "Finalizado" || t.estado === "PorPagar") // Incluimos PorPagar como ingreso esperado
    .reduce((sum, t) => sum + (t.total || 0), 0);

  const clientesAtendidos = colaCompleta.value.filter(
    (t) => t.estado === "Finalizado",
  ).length;
  const clientesEnCola = colaCompleta.value.filter(
    (t) => t.estado === "EnCola",
  ).length;

  // Rendimiento por barbero
  const porBarbero = {};
  colaCompleta.value.forEach((t) => {
    if (
      t.barberoNombre &&
      (t.estado === "Finalizado" || t.estado === "PorPagar")
    ) {
      if (!porBarbero[t.barberoNombre])
        porBarbero[t.barberoNombre] = { count: 0, total: 0 };
      porBarbero[t.barberoNombre].count++;
      porBarbero[t.barberoNombre].total += t.total || 0;
    }
  });

  return {
    totalIngresos,
    clientesAtendidos,
    clientesEnCola,
    porBarbero,
  };
});

import { useRouter } from "vue-router";
const router = useRouter();

const logout = () => {
  usuario.value = null;
  localStorage.removeItem("admin_user");
  localStorage.removeItem("staff_user");
  router.push("/login");
};

// Cargar estado inicial
const cargarDatos = async () => {
  if (!usuario.value) return;
  try {
    // Usamos el nuevo endpoint para obtener TODO el historial del día
    const [cola, barberos] = await Promise.all([
      api.getTurnosHoy(),
      api.adminGetBarberos(),
    ]);
    colaCompleta.value = cola;
    listaBarberos.value = barberos;

    // Filtrar localmente los por pagar
    turnosPorPagar.value = cola.filter((t) => t.estado === "PorPagar");
  } catch (e) {
    console.error(e);
  }
};

// Actions
// Modal State
const showCobrarModal = ref(false);
const turnoParaCobrar = ref(null);

// Actions
const cobrar = (id) => {
  turnoParaCobrar.value = id;
  showCobrarModal.value = true;
};

const confirmarCobro = async () => {
  if (!turnoParaCobrar.value) return;
  try {
    await api.cobrar(turnoParaCobrar.value);
    showCobrarModal.value = false;
    turnoParaCobrar.value = null;
    cargarDatos(); // Refresh immediately
  } catch (e) {
    alert("Error al cobrar");
  }
};

const registrarCliente = async () => {
  if (!form.value.nombre || !form.value.telefono) return;
  try {
    await api.registrarAdmin(form.value.nombre, form.value.telefono);
    formSuccess.value = `Turno creado para ${form.value.nombre}`;
    form.value = { nombre: "", telefono: "" };
    setTimeout(() => (formSuccess.value = null), 3000);
    // SignalR actualizará la tabla
  } catch (e) {
    alert("Error al registrar");
  }
};

// Barberos CRUD Actions
const abrirModalBarbero = (barbero = null) => {
  if (barbero) {
    barberoForm.value = { ...barbero };
  } else {
    barberoForm.value = { id: null, nombre: "", pin: "" };
  }
  barberoFormError.value = null;
  showBarberoModal.value = true;
};

const guardarBarbero = async () => {
  try {
    barberoFormError.value = null;
    if (barberoForm.value.id) {
      await api.adminEditarBarbero(
        barberoForm.value.id,
        barberoForm.value.nombre,
        barberoForm.value.pin,
      );
    } else {
      await api.adminCrearBarbero(
        barberoForm.value.nombre,
        barberoForm.value.pin,
      );
    }
    showBarberoModal.value = false;
    cargarDatos(); // Refresh list
  } catch (e) {
    barberoFormError.value =
      e.response?.data?.message || "Error al guardar el barbero";
  }
};

const confirmarEliminarBarbero = (id) => {
  barberoToDelete.value = id;
  showDeleteConfirmModal.value = true;
};

const eliminarBarbero = async () => {
  if (!barberoToDelete.value) return;
  try {
    await api.adminEliminarBarbero(barberoToDelete.value);
    showDeleteConfirmModal.value = false;
    barberoToDelete.value = null;
    cargarDatos();
  } catch (e) {
    alert("Error al eliminar el barbero");
  }
};

// SignalR
const handleQueueUpdate = () => {
  cargarDatos();
};

onMounted(() => {
  const stored = localStorage.getItem("admin_user");
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
  <div class="max-w-6xl mx-auto">
    <!-- Dashboard -->
    <div v-if="usuario">
      <div class="flex items-center justify-between mb-8">
        <h1 class="text-2xl font-bold text-white">Panel Admin</h1>
        <button
          @click="logout"
          class="text-sm text-slate-400 hover:text-white underline"
        >
          Salir
        </button>
      </div>

      <!-- Tabs -->
      <div class="flex gap-2 mb-8 overflow-x-auto pb-2">
        <button
          v-for="tab in [
            { id: 'porpagar', label: 'Caja / Por Pagar', icon: CreditCard },
            { id: 'stats', label: 'Estadísticas', icon: BarChart3 },
            { id: 'barberos', label: 'Gestión Personal', icon: Users },
            { id: 'agregar', label: 'Nuevo Cliente', icon: Plus },
            { id: 'qr', label: 'Código QR', icon: QrCode },
            { id: 'cola', label: 'Historial Diario', icon: List },
          ]"
          :key="tab.id"
          @click="activeTab = tab.id"
          class="flex items-center gap-2 px-5 py-3 rounded-xl font-medium transition-all whitespace-nowrap"
          :class="
            activeTab === tab.id
              ? 'bg-slate-800 text-white shadow-lg border border-slate-700'
              : 'text-slate-400 hover:text-white hover:bg-slate-900'
          "
        >
          <component :is="tab.icon" class="w-4 h-4" /> {{ tab.label }}
        </button>
      </div>

      <!-- Content -->
      <div
        class="glass rounded-2xl p-6 border border-slate-800 shadow-xl min-h-[400px]"
      >
        <!-- Por Pagar -->
        <div v-if="activeTab === 'porpagar'">
          <div
            v-if="turnosPorPagar.length === 0"
            class="flex flex-col items-center justify-center py-20 text-slate-500"
          >
            <CheckSquare class="w-16 h-16 mb-4 text-slate-700" />
            <p>Todo al día. No hay cobros pendientes.</p>
          </div>

          <div v-else class="grid gap-4">
            <div
              v-for="turno in turnosPorPagar"
              :key="turno.id"
              class="p-6 bg-slate-900/50 rounded-xl border border-slate-700 flex flex-col md:flex-row md:items-center justify-between gap-4"
            >
              <div>
                <div class="flex items-center gap-3 mb-1">
                  <span class="text-2xl font-black text-white"
                    >#{{ turno.turnoDiario }}</span
                  >
                  <span class="text-lg font-bold text-white">{{
                    turno.clienteNombre
                  }}</span>
                </div>
                <div class="flex items-center gap-4 text-sm text-slate-400">
                  <span class="flex items-center gap-1"
                    ><User class="w-3 h-3" /> {{ turno.barberoNombre }}</span
                  >
                  <span class="flex items-center gap-1"
                    ><Calendar class="w-3 h-3" />
                    {{
                      new Date(turno.fechaCreacion).toLocaleTimeString([], {
                        hour: "2-digit",
                        minute: "2-digit",
                      })
                    }}</span
                  >
                </div>
                <!-- Servicios -->
                <div class="mt-3 flex flex-wrap gap-2">
                  <span
                    v-for="detalle in turno.detalles"
                    :key="detalle.id"
                    class="px-2 py-0.5 rounded text-xs bg-slate-800 text-slate-300 border border-slate-700"
                  >
                    {{ detalle.servicioNombre }}
                  </span>
                </div>
              </div>

              <div class="flex items-center gap-6">
                <div class="text-right">
                  <div class="text-xs text-slate-400 uppercase font-bold">
                    Total
                  </div>
                  <div class="text-3xl font-black text-green-500">
                    ${{
                      turno.detalles.reduce(
                        (sum, d) => sum + d.precioCobrado,
                        0,
                      )
                    }}
                  </div>
                </div>
                <button
                  @click="cobrar(turno.id)"
                  class="bg-green-600 hover:bg-green-700 text-white p-4 rounded-xl shadow-lg shadow-green-900/20 transition-all"
                  title="Marcar como Cobrado"
                >
                  <Check class="w-6 h-6" />
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Estadísticas -->
        <div v-if="activeTab === 'stats'">
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
            <!-- Card Ingresos -->
            <div class="bg-slate-900/50 p-6 rounded-xl border border-slate-700">
              <div class="flex items-center gap-4 mb-2">
                <div class="p-3 bg-green-500/10 rounded-lg text-green-500">
                  <Wallet class="w-6 h-6" />
                </div>
                <div>
                  <div class="text-sm text-slate-400 font-medium">
                    Ingresos Totales
                  </div>
                  <div class="text-3xl font-black text-white">
                    ${{ stats.totalIngresos }}
                  </div>
                </div>
              </div>
            </div>

            <!-- Card Atendidos -->
            <div class="bg-slate-900/50 p-6 rounded-xl border border-slate-700">
              <div class="flex items-center gap-4 mb-2">
                <div class="p-3 bg-blue-500/10 rounded-lg text-blue-500">
                  <Users class="w-6 h-6" />
                </div>
                <div>
                  <div class="text-sm text-slate-400 font-medium">
                    Clientes Atendidos
                  </div>
                  <div class="text-3xl font-black text-white">
                    {{ stats.clientesAtendidos }}
                  </div>
                </div>
              </div>
            </div>

            <!-- Card En Cola -->
            <div class="bg-slate-900/50 p-6 rounded-xl border border-slate-700">
              <div class="flex items-center gap-4 mb-2">
                <div class="p-3 bg-purple-500/10 rounded-lg text-purple-500">
                  <List class="w-6 h-6" />
                </div>
                <div>
                  <div class="text-sm text-slate-400 font-medium">
                    En Espera
                  </div>
                  <div class="text-3xl font-black text-white">
                    {{ stats.clientesEnCola }}
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Tabla Barberos -->
          <h3 class="text-lg font-bold text-white mb-4 flex items-center gap-2">
            <TrendingUp class="w-5 h-5 text-slate-400" /> Rendimiento por
            Barbero
          </h3>
          <div class="overflow-hidden rounded-xl border border-slate-800">
            <table class="w-full text-left text-sm">
              <thead class="bg-slate-900 text-slate-400">
                <tr>
                  <th class="py-3 px-4">Barbero</th>
                  <th class="py-3 px-4 text-center">Clientes</th>
                  <th class="py-3 px-4 text-right">Generado</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-800">
                <tr
                  v-for="(data, nombre) in stats.porBarbero"
                  :key="nombre"
                  class="bg-slate-900/30"
                >
                  <td class="py-3 px-4 font-medium text-white">{{ nombre }}</td>
                  <td class="py-3 px-4 text-center text-slate-300">
                    {{ data.count }}
                  </td>
                  <td class="py-3 px-4 text-right font-bold text-green-400">
                    ${{ data.total }}
                  </td>
                </tr>
                <tr v-if="Object.keys(stats.porBarbero).length === 0">
                  <td colspan="3" class="py-8 text-center text-slate-500">
                    Sin datos registrados hoy
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Gestión de Barberos -->
        <div v-if="activeTab === 'barberos'">
          <div class="flex items-center justify-between mb-6">
            <h3 class="text-xl font-bold text-white">Personal (Barberos)</h3>
            <button
              @click="abrirModalBarbero()"
              class="flex items-center gap-2 bg-slate-700 hover:bg-slate-600 text-white px-4 py-2 rounded-lg font-medium transition-all"
            >
              <Plus class="w-4 h-4" /> Nuevo Barbero
            </button>
          </div>

          <div class="overflow-hidden rounded-xl border border-slate-800">
            <table class="w-full text-left text-sm">
              <thead class="bg-slate-900 text-slate-400">
                <tr>
                  <th class="py-3 px-4">ID</th>
                  <th class="py-3 px-4">Nombre</th>
                  <th class="py-3 px-4">PIN de Acceso</th>
                  <th class="py-3 px-4 text-right">Acciones</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-800">
                <tr
                  v-for="barbero in listaBarberos"
                  :key="barbero.id"
                  class="bg-slate-900/30 hover:bg-slate-800/30 transition-colors"
                >
                  <td class="py-3 px-4 text-slate-500">#{{ barbero.id }}</td>
                  <td class="py-3 px-4 font-medium text-white">
                    {{ barbero.nombre }}
                  </td>
                  <td
                    class="py-3 px-4 font-mono text-slate-300 tracking-widest"
                  >
                    {{ barbero.pin }}
                  </td>
                  <td class="py-3 px-4 text-right space-x-2">
                    <button
                      @click="abrirModalBarbero(barbero)"
                      class="px-3 py-1 bg-blue-500/10 hover:bg-blue-500/20 text-blue-400 rounded transition-colors"
                    >
                      Editar
                    </button>
                    <button
                      @click="confirmarEliminarBarbero(barbero.id)"
                      class="px-3 py-1 bg-red-500/10 hover:bg-red-500/20 text-red-500 rounded transition-colors"
                    >
                      <Trash2 class="w-4 h-4 inline-block" />
                    </button>
                  </td>
                </tr>
                <tr v-if="listaBarberos.length === 0">
                  <td colspan="4" class="py-8 text-center text-slate-500">
                    No hay barberos registrados
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Agregar Manual -->
        <div v-if="activeTab === 'agregar'" class="max-w-md mx-auto">
          <h3 class="text-xl font-bold text-white mb-6 text-center">
            Registrar nuevo cliente
          </h3>

          <form @submit.prevent="registrarCliente" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-slate-400 mb-1"
                >Nombre</label
              >
              <input
                v-model="form.nombre"
                type="text"
                class="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-3 text-white focus:border-slate-500 transition-colors"
                required
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-400 mb-1"
                >Teléfono</label
              >
              <input
                v-model="form.telefono"
                type="text"
                class="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-3 text-white focus:border-slate-500 transition-colors"
                required
              />
            </div>

            <div
              v-if="formSuccess"
              class="p-3 bg-green-500/10 text-green-400 rounded-lg text-sm text-center"
            >
              {{ formSuccess }}
            </div>

            <button
              type="submit"
              class="w-full bg-slate-700 hover:bg-slate-600 text-white font-bold py-3.5 rounded-xl transition-all"
            >
              Agregar a la Cola
            </button>
          </form>
        </div>

        <!-- QR -->
        <div
          v-if="activeTab === 'qr'"
          class="flex flex-col items-center justify-center py-12"
        >
          <div class="p-4 bg-white rounded-2xl shadow-2xl">
            <QrcodeVue :value="qrValue" :size="250" level="H" />
          </div>
          <p class="mt-6 text-slate-400 text-lg">Escanea para registrarte</p>
          <p class="text-slate-600 text-sm font-mono mt-2">{{ qrValue }}</p>
        </div>

        <!-- Cola Completa -->
        <div v-if="activeTab === 'cola'">
          <table class="w-full text-left text-sm">
            <thead class="text-slate-400 border-b border-slate-800">
              <tr>
                <th class="pb-3 pl-4">#</th>
                <th class="pb-3">Cliente</th>
                <th class="pb-3">Estado</th>
                <th class="pb-3">Barbero</th>
                <th class="pb-3 text-right pr-4">Total</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-800/50">
              <tr
                v-for="t in colaCompleta"
                :key="t.id"
                class="hover:bg-slate-800/20 transition-colors"
              >
                <td class="py-3 pl-4 font-bold text-slate-300">
                  {{ t.turnoDiario }}
                </td>
                <td class="py-3 text-white">{{ t.clienteNombre }}</td>
                <td class="py-3">
                  <span
                    class="px-2 py-0.5 rounded-full text-xs font-medium border"
                    :class="{
                      'bg-slate-800 text-slate-400 border-slate-700':
                        t.estado === 'EnCola',
                      'bg-yellow-500/10 text-yellow-400 border-yellow-500/20':
                        t.estado === 'Llamado',
                      'bg-indigo-500/10 text-indigo-400 border-indigo-500/20':
                        t.estado === 'EnSilla',
                      'bg-green-500/10 text-green-400 border-green-500/20':
                        t.estado === 'PorPagar',
                      'bg-slate-900 text-slate-600 border-transparent':
                        t.estado === 'Finalizado',
                    }"
                  >
                    {{ t.estado }}
                  </span>
                </td>
                <td class="py-3 text-slate-400">
                  {{ t.barberoNombre || "-" }}
                </td>
                <td class="py-3 pr-4 text-right text-slate-300">
                  {{ t.total ? "$" + t.total : "-" }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Modal Cobrar -->
    <div
      v-if="showCobrarModal"
      class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/80 backdrop-blur-sm"
    >
      <div
        class="glass w-full max-w-sm rounded-2xl p-6 border border-slate-700 shadow-2xl animate-in zoom-in-95 duration-200 text-center"
      >
        <div
          class="w-16 h-16 bg-green-500/10 rounded-full flex items-center justify-center mx-auto mb-4 text-green-500"
        >
          <CreditCard class="w-8 h-8" />
        </div>
        <h3 class="text-xl font-bold text-white mb-2">Confirmar Pago</h3>
        <p class="text-slate-400 mb-6">
          ¿Marcar el turno como cobrado y finalizado?
        </p>

        <div class="grid grid-cols-2 gap-3">
          <button
            @click="showCobrarModal = false"
            class="px-4 py-3 rounded-xl font-medium text-slate-300 hover:text-white hover:bg-slate-800 transition-colors"
          >
            Cancelar
          </button>
          <button
            @click="confirmarCobro"
            class="px-4 py-3 bg-green-600 hover:bg-green-700 text-white font-bold rounded-xl shadow-lg shadow-green-900/20 transition-all"
          >
            Confirmar
          </button>
        </div>
      </div>
    </div>

    <!-- Modal Formulario Barbero -->
    <div
      v-if="showBarberoModal"
      class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/80 backdrop-blur-sm"
    >
      <div
        class="glass w-full max-w-md rounded-2xl p-6 border border-slate-700 shadow-2xl animate-in zoom-in-95 duration-200"
      >
        <h3 class="text-xl font-bold text-white mb-6">
          {{ barberoForm.id ? "Editar" : "Nuevo" }} Barbero
        </h3>

        <form @submit.prevent="guardarBarbero" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-slate-400 mb-1"
              >Nombre Completo</label
            >
            <input
              v-model="barberoForm.nombre"
              type="text"
              class="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-3 text-white focus:border-slate-500 transition-colors"
              required
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-400 mb-1"
              >PIN de Acceso (4 dígitos)</label
            >
            <input
              v-model="barberoForm.pin"
              type="password"
              maxlength="4"
              pattern="[0-9]{4}"
              class="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-3 text-white text-center font-mono tracking-widest text-2xl focus:border-slate-500 transition-colors"
              required
              placeholder="••••"
            />
            <p class="text-xs text-slate-500 mt-1">
              Debe ser un número de 4 dígitos exactos.
            </p>
          </div>

          <div
            v-if="barberoFormError"
            class="p-3 bg-red-500/10 text-red-400 rounded-lg text-sm text-center"
          >
            {{ barberoFormError }}
          </div>

          <div class="grid grid-cols-2 gap-3 mt-6">
            <button
              type="button"
              @click="showBarberoModal = false"
              class="px-4 py-3 rounded-xl font-medium text-slate-300 hover:text-white hover:bg-slate-800 transition-colors"
            >
              Cancelar
            </button>
            <button
              type="submit"
              class="px-4 py-3 bg-blue-600 hover:bg-blue-700 text-white font-bold rounded-xl shadow-lg shadow-blue-900/20 transition-all"
            >
              Guardar
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Modal Eliminar Barbero -->
    <div
      v-if="showDeleteConfirmModal"
      class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/80 backdrop-blur-sm"
    >
      <div
        class="glass w-full max-w-sm rounded-2xl p-6 border border-slate-700 shadow-2xl animate-in zoom-in-95 duration-200 text-center"
      >
        <div
          class="w-16 h-16 bg-red-500/10 rounded-full flex items-center justify-center mx-auto mb-4 text-red-500"
        >
          <Trash2 class="w-8 h-8" />
        </div>
        <h3 class="text-xl font-bold text-white mb-2">Eliminar Barbero</h3>
        <p class="text-slate-400 mb-6">
          ¿Estás seguro que deseas eliminar este barbero? Sus turnos históricos
          perderán el nombre asignado. Esta acción no se puede deshacer.
        </p>

        <div class="grid grid-cols-2 gap-3">
          <button
            @click="showDeleteConfirmModal = false"
            class="px-4 py-3 rounded-xl font-medium text-slate-300 hover:text-white hover:bg-slate-800 transition-colors"
          >
            Cancelar
          </button>
          <button
            @click="eliminarBarbero"
            class="px-4 py-3 bg-red-600 hover:bg-red-700 text-white font-bold rounded-xl shadow-lg shadow-red-900/20 transition-all"
          >
            Sí, Eliminar
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
