import axios from "axios";

const API_URL = import.meta.env.VITE_API_URL || "http://localhost:5268/api";

const api = axios.create({
  baseURL: `${API_URL}/incidencias`,
  headers: {
    "Content-Type": "application/json",
  },
});

// Error handling interceptor
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.code === "ERR_NETWORK") {
      console.error("⚠️ Error de conexión: No se puede conectar a la API. Asegúrate de que el servidor está corriendo en", API_URL);
    }
    return Promise.reject(error);
  }
);

export const obtenerIncidencias = () => api.get("/");
export const crearIncidencia = (data) => api.post("/", data);
export const eliminarIncidencia = (id) => api.delete(`/${id}`);
export const actualizarIncidencia = (id, data) => api.put(`/${id}`, data);
export const sincronizarDesdeLogs = () => axios.post(`${API_URL}/incidencias/sync`);
export const sincronizarDesdeMongoDirecto = () => axios.post(`${API_URL}/mongo/direct/incidencias/sync`);
export const sincronizarDesdeSqlAMongo = () => axios.post(`${API_URL}/incidencias/sync`);
export default api;

