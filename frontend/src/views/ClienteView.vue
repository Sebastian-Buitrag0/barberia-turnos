<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import api from '../services/api'
import * as signalRService from '../services/signalr'
import { Ticket, User, Phone, Loader2, PlayCircle, Clock, BellRing, Armchair, AlertCircle } from 'lucide-vue-next'

const nombre = ref('')
const telefono = ref('')
// Turno State
const miTurno = ref(null)
const personasDelante = ref(0)
const loading = ref(false)
const error = ref(null)
const registering = ref(false)

// Cargar estado inicial
const cargarEstado = async () => {
  const storedPhone = localStorage.getItem('cliente_telefono')
  if (storedPhone) {
    loading.value = true
    try {
    const data = await api.miTurno(storedPhone)
      if (data) {
        miTurno.value = data // The endpoint returns the turn object directly with extras
        personasDelante.value = data.personasDelante
        telefono.value = storedPhone
      }
    } catch (e) {
      // Si no hay turno 404, limpiamos storage
      if (e.response && e.response.status === 404) {
        localStorage.removeItem('cliente_telefono')
        miTurno.value = null
      }
    } finally {
      loading.value = false
    }
  }
}

// Registrar Turno
const sacarTurno = async () => {
  if (!nombre.value || !telefono.value) {
    error.value = "Por favor completa todos los campos"
    return
  }
  
  registering.value = true
  error.value = null
  
  try {
    await api.registrar(nombre.value, telefono.value)
    
    localStorage.setItem('cliente_telefono', telefono.value)
    await cargarEstado()
    
  } catch (e) {
    error.value = e.response?.data?.message || "Error al sacar turno"
  } finally {
    registering.value = false
  }
}

// Registrar otro turno (reset)
const reset = () => {
  localStorage.removeItem('cliente_telefono')
  miTurno.value = null
  nombre.value = ''
  telefono.value = ''
  error.value = null
}

// SignalR Events
const handleQueueUpdate = async () => {
  if (telefono.value) {
    const data = await api.miTurno(telefono.value)
    if (data) {
      miTurno.value = data
      personasDelante.value = data.personasDelante
    }
  }
}

onMounted(() => {
  cargarEstado()
  signalRService.onQueueUpdated(handleQueueUpdate)
})

onUnmounted(() => {
  signalRService.offQueueUpdated(handleQueueUpdate)
})
</script>

<template>
  <div class="max-w-md mx-auto">
    
    <!-- Header -->
    <div class="text-center mb-10">
      <div class="inline-flex items-center justify-center p-4 bg-slate-900 rounded-2xl mb-4 border border-slate-800 shadow-xl shadow-black/40">
        <Armchair class="w-12 h-12 text-red-500" />
      </div>
      <h1 class="text-3xl font-bold mb-2">Bienvenido a BarberQ</h1>
      <p class="text-slate-400">Tu turno, sin esperas innecesarias.</p>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="flex flex-col items-center justify-center py-12 text-slate-400">
      <Loader2 class="w-8 h-8 animate-spin mb-4 text-red-500" />
      <p>Cargando tu información...</p>
    </div>

    <!-- Waiting Screen -->
    <div v-else-if="miTurno" class="glass rounded-2xl p-8 border border-slate-700 shadow-2xl relative overflow-hidden">
      <!-- Background Accents -->
      <div class="absolute top-0 right-0 p-32 bg-red-600/5 blur-[80px] rounded-full pointer-events-none"></div>
      
      <div class="relative z-10 text-center">
        <!-- Status Indicator -->
        <div class="mb-8">
           <span v-if="miTurno.estado === 'EnCola'" 
             class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-blue-500/10 text-blue-400 border border-blue-500/20 text-sm font-medium">
             <Clock class="w-4 h-4" /> En Espera
           </span>
           <span v-else-if="miTurno.estado === 'Llamado'" 
             class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-yellow-500/10 text-yellow-400 border border-yellow-500/20 text-sm font-medium animate-pulse">
             <BellRing class="w-4 h-4" /> ¡Es tu Turno!
           </span>
           <span v-else
             class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-green-500/10 text-green-400 border border-green-500/20 text-sm font-medium">
             <Scissors class="w-4 h-4" /> Siendo Atendido
           </span>
        </div>

        <h2 class="text-slate-400 text-sm uppercase tracking-wider font-semibold mb-2">Tu Número</h2>
        <div class="text-7xl font-black text-white mb-8 tracking-tighter">
          #{{ miTurno.turnoDiario }}
        </div>

        <!-- Waiting Info -->
        <div class="grid grid-cols-1 gap-4 mb-8">
          <div class="bg-slate-900/50 rounded-xl p-4 border border-slate-800">
            <div class="text-slate-400 text-xs uppercase font-bold mb-1">Personas delante</div>
            <div class="text-2xl font-bold text-white">{{ personasDelante }}</div>
          </div>
        </div>

        <p class="text-slate-400 text-sm mb-6 flex items-center justify-center gap-2">
          Te avisaremos cuando sea tu momento <br>
          <span class="text-xs text-slate-500">(Mantén esta pantalla abierta)</span>
        </p>
        
        <button @click="reset" class="text-slate-500 hover:text-white text-sm underline decoration-slate-700 hover:decoration-white transition-all">
          Registrar otro turno
        </button>
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
          <label class="block text-sm font-medium text-slate-300">Nombre Completo</label>
          <div class="relative group">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <User class="h-5 w-5 text-slate-500 group-focus-within:text-red-500 transition-colors" />
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
          <label class="block text-sm font-medium text-slate-300">Teléfono (WhatsApp)</label>
          <div class="relative group">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Phone class="h-5 w-5 text-slate-500 group-focus-within:text-red-500 transition-colors" />
            </div>
            <input 
              v-model="telefono" 
              type="tel" 
              class="block w-full pl-10 pr-3 py-3 bg-slate-900/50 border border-slate-700 rounded-xl text-white placeholder-slate-500 focus:ring-2 focus:ring-red-500/20 focus:border-red-500 transition-all" 
              placeholder="Ej. 3001234567"
              required 
            />
          </div>
        </div>

        <!-- Error Message -->
        <div v-if="error" class="flex items-center gap-2 p-3 bg-red-500/10 border border-red-500/20 rounded-lg text-red-400 text-sm">
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

  </div>
</template>
