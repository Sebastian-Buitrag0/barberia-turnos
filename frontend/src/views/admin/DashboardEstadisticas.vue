<script setup>
import { ref, onMounted, computed } from "vue";
import {
  Banknote,
  Users,
  TrendingUp,
  History,
  AlertCircle,
  CheckCircle2,
  Lock,
  UserCheck,
} from "lucide-vue-next";
import api from "../../services/api";
import { Line as LineChart, Bar as BarChart } from "vue-chartjs";
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  Title,
  Tooltip,
  Legend,
  Filler,
} from "chart.js";

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  Title,
  Tooltip,
  Legend,
  Filler,
);

const isClosing = ref(false);
const showHistory = ref(false);

const resumenHoy = ref({
  totalRecaudado: 0,
  cantidadTurnos: 0,
  ticketPromedio: 0,
  rendimientoBarberos: [],
  cajaCerrada: false,
  detalleCierre: null,
});

const historico = ref([]);
const todosLosBarberos = ref([]);

const barberosDisponibles = computed(() => {
  return todosLosBarberos.value.filter((b) => b.isAvailable);
});

// Formatters
const formatMoney = (amount) => {
  return new Intl.NumberFormat("es-CO", {
    style: "currency",
    currency: "COP",
    minimumFractionDigits: 0,
  }).format(amount || 0);
};

const formatDate = (dateString) => {
  if (!dateString) return "";
  return new Date(dateString).toLocaleDateString("es-CO", {
    year: "numeric",
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
};

const loadData = async () => {
  try {
    const resResumen = await api.getResumenHoy();
    resumenHoy.value = resResumen;

    const resHistorico = await api.getHistorico();
    historico.value = resHistorico;

    // Obtener lista completa de barberos para ver disponibilidad
    const resBarberos = await api.adminGetBarberos();
    todosLosBarberos.value = resBarberos;
  } catch (error) {
    console.error("Error loading stats:", error);
  }
};

const cerrarCaja = async () => {
  if (
    !confirm(
      "⚠️ ¿Estás seguro de que quieres cerrar la caja de hoy? Esta acción es irreversible y fijará los ingresos actuales en el historial.",
    )
  )
    return;

  isClosing.value = true;
  try {
    await api.cerrarCaja();
    await loadData();
    alert("✅ Caja cerrada exitosamente.");
  } catch (error) {
    console.error("Error cerrado caja:", error);
    alert(
      error.response?.data?.message || "Ocurrió un error al cerrar la caja.",
    );
  } finally {
    isClosing.value = false;
  }
};

onMounted(() => {
  loadData();
});

// Chard.js Computed Data
const barberoChartData = computed(() => {
  return {
    labels: resumenHoy.value.rendimientoBarberos.map((b) => b.nombre),
    datasets: [
      {
        label: "Ingresos del Día",
        backgroundColor: "#ef4444",
        borderRadius: 6,
        data: resumenHoy.value.rendimientoBarberos.map((b) => b.total),
      },
    ],
  };
});

const barberoChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: { legend: { display: false } },
  scales: {
    y: { beginAtZero: true, grid: { color: "rgba(255,255,255,0.05)" } },
    x: { grid: { display: false } },
  },
};

const historyChartData = computed(() => {
  // Take last 7 days from history, reversed so oldest is first
  const last7Days = [...historico.value].slice(0, 7).reverse();

  return {
    labels: last7Days.map((h) =>
      new Date(h.fecha).toLocaleDateString("es-CO", {
        weekday: "short",
        day: "numeric",
      }),
    ),
    datasets: [
      {
        label: "Ingresos Diarios",
        borderColor: "#ef4444",
        backgroundColor: "rgba(239, 68, 68, 0.2)",
        tension: 0.4,
        fill: true,
        pointBackgroundColor: "#ef4444",
        data: last7Days.map((h) => h.totalRecaudado),
      },
    ],
  };
});

const historyChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: { legend: { display: false } },
  scales: {
    y: { beginAtZero: true, grid: { color: "rgba(255,255,255,0.05)" } },
    x: { grid: { display: false } },
  },
};
</script>

<template>
  <div class="space-y-6 animate-in fade-in slide-in-from-bottom-4 duration-500">
    <div class="mb-2">
      <h2
        class="text-2xl font-bold tracking-tight text-white flex items-center gap-3"
      >
        <TrendingUp class="w-8 h-8 text-red-500" />
        Estadísticas y Cierre
      </h2>
      <p class="text-slate-400 mt-1">
        Analiza el rendimiento del negocio y asegura los ingresos al final del
        día.
      </p>
    </div>

    <!-- Header/Warning -->
    <div
      v-if="resumenHoy.cajaCerrada"
      class="flex flex-col md:flex-row items-center justify-between gap-4 p-4 bg-green-500/10 border border-green-500/20 rounded-2xl text-green-400"
    >
      <div class="flex items-center gap-3">
        <CheckCircle2 class="w-6 h-6" />
        <div>
          <h3 class="font-bold">Caja del Día Cerrada</h3>
          <p class="text-sm opacity-80">
            Los datos de hoy ya fueron guardados en el historial. (Cerrado por
            {{ resumenHoy.detalleCierre?.cerradoPorNombre || "Admin" }} a las
            {{ formatDate(resumenHoy.detalleCierre?.fechaCierre) }})
          </p>
        </div>
      </div>
    </div>

    <!-- Barberos Disponibles Banner -->
    <div
      class="glass p-6 rounded-2xl border border-slate-700 shadow-xl flex flex-col md:flex-row items-center gap-6 justify-between"
    >
      <div class="flex items-center gap-4">
        <div class="p-4 bg-emerald-500/10 rounded-xl text-emerald-400">
          <UserCheck class="w-8 h-8" />
        </div>
        <div>
          <h3 class="text-lg font-bold text-white mb-1">
            Barberos Disponibles
          </h3>
          <p class="text-sm text-slate-400">
            <span class="font-bold text-white">{{
              barberosDisponibles.length
            }}</span>
            de {{ todosLosBarberos.length }} barberos han registrado su llegada.
          </p>
        </div>
      </div>
      <div class="flex flex-wrap gap-2 justify-end">
        <div
          v-for="b in barberosDisponibles"
          :key="b.id"
          class="px-3 py-1.5 bg-emerald-500/20 text-emerald-300 rounded-full text-xs font-bold border border-emerald-500/30"
        >
          {{ b.nombre }}
        </div>
        <div
          v-if="barberosDisponibles.length === 0"
          class="text-sm text-slate-500 italic"
        >
          Nadie ha registrado llegada aún.
        </div>
      </div>
    </div>

    <!-- KPI Cards -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div
        class="glass p-6 rounded-2xl border border-slate-800 flex items-center gap-4 relative overflow-hidden"
      >
        <div
          class="absolute -right-4 -top-4 w-24 h-24 bg-red-500/10 rounded-full blur-2xl"
        ></div>
        <div
          class="p-4 bg-slate-900/50 rounded-xl border border-slate-800 text-red-400"
        >
          <Banknote class="w-7 h-7" />
        </div>
        <div>
          <p class="text-sm font-medium text-slate-400 mb-1">Ingresos de Hoy</p>
          <p class="text-3xl font-bold text-white">
            {{ formatMoney(resumenHoy.totalRecaudado) }}
          </p>
        </div>
      </div>

      <div
        class="glass p-6 rounded-2xl border border-slate-800 flex items-center gap-4 relative overflow-hidden"
      >
        <div
          class="absolute -right-4 -top-4 w-24 h-24 bg-blue-500/10 rounded-full blur-2xl"
        ></div>
        <div
          class="p-4 bg-slate-900/50 rounded-xl border border-slate-800 text-blue-400"
        >
          <Users class="w-7 h-7" />
        </div>
        <div>
          <p class="text-sm font-medium text-slate-400 mb-1">
            Turnos Atendidos
          </p>
          <p class="text-3xl font-bold text-white">
            {{ resumenHoy.cantidadTurnos }}
            <span class="text-lg text-slate-500 font-normal">clientes</span>
          </p>
        </div>
      </div>

      <div
        class="glass p-6 rounded-2xl border border-slate-800 flex items-center gap-4 relative overflow-hidden"
      >
        <div
          class="absolute -right-4 -top-4 w-24 h-24 bg-purple-500/10 rounded-full blur-2xl"
        ></div>
        <div
          class="p-4 bg-slate-900/50 rounded-xl border border-slate-800 text-purple-400"
        >
          <TrendingUp class="w-7 h-7" />
        </div>
        <div>
          <p class="text-sm font-medium text-slate-400 mb-1">Ticket Promedio</p>
          <p class="text-3xl font-bold text-white">
            {{ formatMoney(resumenHoy.ticketPromedio) }}
          </p>
        </div>
      </div>
    </div>

    <!-- Main Analytics Area -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Left Column: Trend Chart (Full width on smaller, 2/3 on large) -->
      <div
        class="lg:col-span-2 glass p-6 rounded-2xl border border-slate-800 flex flex-col"
      >
        <div class="flex items-center justify-between mb-6">
          <div class="flex items-center gap-2">
            <TrendingUp class="w-5 h-5 text-red-400" />
            <h3 class="text-lg font-semibold text-white">
              Ingresos Últimos 7 Días
            </h3>
          </div>
        </div>

        <div class="relative flex-1 min-h-[300px]">
          <div
            v-if="historico.length === 0"
            class="absolute inset-0 flex flex-col items-center justify-center text-slate-500"
          >
            <AlertCircle class="w-8 h-8 mb-2 opacity-50" />
            <p>No hay suficientes datos históricos</p>
            <p class="text-xs mt-1">
              Cierra la caja al final del día para generar historial.
            </p>
          </div>
          <LineChart
            v-else
            :data="historyChartData"
            :options="historyChartOptions"
          />
        </div>
      </div>

      <!-- Right Column: Operations Panel & Barber Chart -->
      <div class="flex flex-col gap-6">
        <!-- Control de Caja Card -->
        <div
          class="glass p-6 rounded-2xl border border-slate-800 flex flex-col items-center justify-center text-center"
        >
          <h3 class="text-lg font-semibold text-white mb-4">Control de Caja</h3>

          <div
            v-if="resumenHoy.cajaCerrada"
            class="flex items-center justify-center gap-3 p-4 rounded-xl bg-green-500/10 border border-green-500/20 text-green-400 w-full"
          >
            <Lock class="w-6 h-6" />
            <div class="text-left">
              <p class="font-bold">Caja Cerrada</p>
              <p class="text-xs opacity-80 mt-0.5">
                por {{ resumenHoy.detalleCierre?.admin }} a las
                {{
                  new Date(
                    resumenHoy.detalleCierre?.horaCierre,
                  ).toLocaleTimeString("es-CO")
                }}
              </p>
            </div>
          </div>

          <button
            v-else
            @click="cerrarCaja"
            :disabled="isClosing"
            class="group w-full relative flex items-center justify-center gap-2 px-6 py-4 bg-red-600 hover:bg-red-500 text-white rounded-xl shadow-lg shadow-red-900/20 border border-red-500 transition-all font-bold overflow-hidden disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <div
              class="absolute inset-0 w-full h-full bg-gradient-to-r from-transparent via-white/10 to-transparent -translate-x-full group-hover:translate-x-full transition-transform duration-1000"
            ></div>
            <Banknote class="w-5 h-5 relative z-10" />
            <span class="relative z-10">{{
              isClosing ? "Procesando..." : "Cerrar Caja de Hoy"
            }}</span>
          </button>
          <p v-if="!resumenHoy.cajaCerrada" class="text-xs text-slate-500 mt-3">
            Fija permanentemente los ingresos actuales en el historial.
          </p>
        </div>

        <!-- Rendimiento Barberos Chart -->
        <div
          class="glass p-6 rounded-2xl border border-slate-800 flex flex-col flex-1 min-h-[250px]"
        >
          <div class="flex items-center justify-between mb-6">
            <div class="flex items-center gap-2">
              <Users class="w-5 h-5 text-blue-400" />
              <h3 class="font-semibold text-white">Rendimiento Diario</h3>
            </div>
          </div>

          <div class="relative flex-1">
            <div
              v-if="resumenHoy.rendimientoBarberos.length === 0"
              class="absolute inset-0 flex flex-col items-center justify-center text-slate-500 text-center px-4"
            >
              <AlertCircle class="w-8 h-8 mb-2 opacity-50" />
              <p class="text-sm">Aún no hay ingresos registrados hoy.</p>
            </div>
            <BarChart
              v-else
              :data="barberoChartData"
              :options="barberoChartOptions"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- Historial de Cierres -->
    <div class="glass rounded-2xl border border-slate-800 overflow-hidden">
      <div
        class="p-6 border-b border-slate-800/50 flex justify-between items-center cursor-pointer hover:bg-slate-800/20 transition-colors"
        @click="showHistory = !showHistory"
      >
        <div class="flex items-center gap-3">
          <div class="p-2 bg-slate-900/50 rounded-lg border border-slate-800">
            <History class="w-5 h-5 text-slate-400" />
          </div>
          <div>
            <h3 class="text-lg font-semibold text-white">
              Libro de Cierres Diarios
            </h3>
            <p class="text-sm text-slate-400">
              Auditoría inmutable de ingresos pasados
            </p>
          </div>
        </div>
        <button
          class="text-sm font-medium text-red-400 hover:text-red-300 transition-colors"
        >
          {{ showHistory ? "Ocultar Historial" : "Ver Historial Completo" }}
        </button>
      </div>

      <div v-if="showHistory" class="overflow-x-auto">
        <table class="w-full text-left text-sm text-slate-300">
          <thead class="text-xs uppercase bg-slate-900/50 text-slate-400">
            <tr>
              <th scope="col" class="px-6 py-4 font-medium">Fecha Operativa</th>
              <th scope="col" class="px-6 py-4 font-medium text-right">
                Turnos Cobrados
              </th>
              <th scope="col" class="px-6 py-4 font-medium text-right">
                Total Ingresos
              </th>
              <th scope="col" class="px-6 py-4 font-medium">
                Auditoría (Cerrado Por)
              </th>
              <th scope="col" class="px-6 py-4 font-medium">Hora de Cierre</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-800/50">
            <tr v-if="historico.length === 0">
              <td colspan="5" class="px-6 py-8 text-center text-slate-500">
                <AlertCircle class="w-6 h-6 mx-auto mb-2 opacity-50" />
                No existen registros de cierres de caja anteriores.
              </td>
            </tr>
            <tr
              v-for="cierre in historico"
              :key="cierre.id"
              class="hover:bg-slate-800/20 transition-colors"
            >
              <td
                class="px-6 py-4 font-medium text-white flex items-center gap-2"
              >
                <CheckCircle2 class="w-4 h-4 text-green-500" />
                {{
                  new Date(cierre.fecha).toLocaleDateString("es-CO", {
                    weekday: "long",
                    year: "numeric",
                    month: "long",
                    day: "numeric",
                  })
                }}
              </td>
              <td class="px-6 py-4 text-right font-medium">
                {{ cierre.cantidadTurnos }}
              </td>
              <td class="px-6 py-4 text-right font-bold text-emerald-400">
                {{ formatMoney(cierre.totalRecaudado) }}
              </td>
              <td class="px-6 py-4">
                <div class="flex items-center gap-2">
                  <div
                    class="w-6 h-6 rounded-full bg-slate-700 flex items-center justify-center text-xs font-bold text-white"
                  >
                    {{ cierre.adminNombre.charAt(0).toUpperCase() }}
                  </div>
                  {{ cierre.adminNombre }}
                </div>
              </td>
              <td class="px-6 py-4 text-slate-400">
                {{ formatDate(cierre.fechaCierre) }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
