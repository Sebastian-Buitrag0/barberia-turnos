import axios from 'axios'

const api = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' }
})

// Interceptor para agregar token
api.interceptors.request.use(config => {
  const stored = localStorage.getItem('barbero_user')
  if (stored) {
    try {
      const user = JSON.parse(stored)
      if (user.token) {
        config.headers.Authorization = `Bearer ${user.token}`
      }
    } catch (e) {
      console.error("Error parsing user from localStorage", e)
    }
  }
  return config
})

export default {
  // Turnos Publicos
  getTurnosHoy: () => api.get('/turnos/hoy').then(res => res.data),
  getCola: () => api.get('/turnos/cola').then(res => res.data),
  miTurno: (telefono) => api.get(`/turnos/mi-turno/${telefono}`).then(res => res.data),
  registrar: (nombre, telefono) => api.post('/turnos/registrar', { nombre, telefono }).then(res => res.data),
  
  // Barbero / Admin (Protected)
  login: (pin) => api.post('/auth/login', { pin }).then(res => res.data),
  
  llamar: (barberoId) => api.post('/turnos/llamar', { barberoId }).then(res => res.data),
  enSilla: (turnoId) => api.post('/turnos/ensilla', { turnoId }),
  finalizar: (turnoId, servicioIds) => api.post('/turnos/finalizar', { turnoId, servicioIds }),
  
  // Admin
  getPorPagar: () => api.get('/turnos/porpagar').then(res => res.data),
  cobrar: (turnoId) => api.post('/turnos/cobrar', { turnoId }),
  registrarAdmin: (nombre, telefono) => api.post('/turnos/registrar-admin', { nombre, telefono }).then(res => res.data),
  
  // Servicios
  getServicios: () => api.get('/servicios').then(res => res.data),

  // Stats
  getStats: () => api.get('/stats').then(res => res.data)
}
