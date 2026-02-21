import axios from "axios";

const baseURL = import.meta.env.VITE_API_URL
  ? `${import.meta.env.VITE_API_URL}/api`
  : "/api";

const api = axios.create({
  baseURL,
  headers: { "Content-Type": "application/json" },
});

// Interceptor para agregar token
api.interceptors.request.use((config) => {
  const stored = localStorage.getItem("barbero_user");
  if (stored) {
    try {
      const user = JSON.parse(stored);
      if (user.token) {
        config.headers.Authorization = `Bearer ${user.token}`;
      }
    } catch (e) {
      console.error("Error parsing user from localStorage", e);
    }
  }
  return config;
});

export default {
  // Turnos Publicos
  getBarberos: () => api.get("/turnos/barberos").then((res) => res.data),
  getTurnosHoy: () => api.get("/turnos/hoy").then((res) => res.data),
  getCola: () => api.get("/turnos/cola").then((res) => res.data),
  miTurno: (telefono) =>
    api.get(`/turnos/mi-turno/${telefono}`).then((res) => res.data),
  registrar: (nombre, telefono, barberoId) =>
    api
      .post("/turnos/registrar", { nombre, telefono, barberoId })
      .then((res) => res.data),
  cancelar: (telefono) =>
    api.post("/turnos/cancelar", { telefono }).then((res) => res.data),

  // Barbero / Admin (Protected)
  login: (pin) => api.post("/auth/login", { pin }).then((res) => res.data),
  toggleDisponibilidad: () =>
    api.post("/usuarios/me/disponibilidad").then((res) => res.data),

  llamar: (barberoId) =>
    api.post("/turnos/llamar", { barberoId }).then((res) => res.data),
  enSilla: (turnoId) => api.post("/turnos/ensilla", { turnoId }),
  finalizar: (turnoId, servicioIds) =>
    api.post("/turnos/finalizar", { turnoId, servicioIds }),

  // Admin
  getPorPagar: () => api.get("/turnos/porpagar").then((res) => res.data),
  cobrar: (turnoId) => api.post("/turnos/cobrar", { turnoId }),
  registrarAdmin: (nombre, telefono, barberoId) =>
    api
      .post("/turnos/registrar-admin", { nombre, telefono, barberoId })
      .then((res) => res.data),

  adminGetBarberos: () => api.get("/usuarios/barberos").then((res) => res.data),
  adminCrearBarbero: (nombre, pin) =>
    api.post("/usuarios/barberos", { nombre, pin }).then((res) => res.data),
  adminEditarBarbero: (id, nombre, pin) =>
    api
      .put(`/usuarios/barberos/${id}`, { nombre, pin })
      .then((res) => res.data),
  adminEliminarBarbero: (id) =>
    api.delete(`/usuarios/barberos/${id}`).then((res) => res.data),

  // Estadisticas & Cierre
  getResumenHoy: () =>
    api.get("/estadisticas/resumen-hoy").then((res) => res.data),
  cerrarCaja: () =>
    api.post("/estadisticas/cerrar-caja").then((res) => res.data),
  getHistorico: () =>
    api.get("/estadisticas/historico").then((res) => res.data),

  // Servicios
  getServicios: () => api.get("/servicios").then((res) => res.data),

  // Stats
  getStats: () => api.get("/stats").then((res) => res.data),
};
