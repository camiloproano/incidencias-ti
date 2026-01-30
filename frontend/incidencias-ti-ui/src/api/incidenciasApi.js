import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5268/api/incidencias"
});

export const obtenerIncidencias = () => api.get("/");
export const crearIncidencia = (data) => api.post("/", data);
export const eliminarIncidencia = (id) => api.delete(`/${id}`);

