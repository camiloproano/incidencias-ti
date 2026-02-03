import { useEffect, useState } from "react";
import { obtenerIncidencias, crearIncidencia, eliminarIncidencia, actualizarIncidencia } from "../api/incidenciasApi";
import IncidenciaCard from "../components/IncidenciaCard";
import "./Incidencias.css";

export default function Incidencias() {
  const [incidencias, setIncidencias] = useState([]);
  const [titulo, setTitulo] = useState("");
  const [mensaje, setMensaje] = useState("");
  const [loading, setLoading] = useState(true);
  const [filtroPrioridad, setFiltroPrioridad] = useState("");
  const [filtroEstado, setFiltroEstado] = useState("");

  const cargarIncidencias = async () => {
    setLoading(true);
    const res = await obtenerIncidencias();
    setIncidencias(res.data);
    setLoading(false);
  };

  useEffect(() => {
    cargarIncidencias();
  }, []);

  const agregarIncidencia = async () => {
    if (!titulo.trim()) return;

    await crearIncidencia({
      titulo,
      descripcion: "Incidencia creada desde React",
      prioridad: "Media",
      estado: "Abierta"
    });

    setTitulo("");
    setMensaje("Incidencia creada correctamente ✔️");
    cargarIncidencias();
    setTimeout(() => setMensaje(""), 3000);
  };

  const borrarIncidencia = async (id) => {
    if (!confirm("¿Eliminar incidencia?")) return;
    await eliminarIncidencia(id);
    cargarIncidencias();
  };

  const actualizar = async (id, data) => {
    await actualizarIncidencia(id, data);
    setMensaje("Incidencia actualizada ✔️");
    cargarIncidencias();
    setTimeout(() => setMensaje(""), 3000);
  };

  const incidenciasFiltradas = incidencias.filter(i =>
    (filtroPrioridad ? i.prioridad === filtroPrioridad : true) &&
    (filtroEstado ? i.estado === filtroEstado : true)
  );

  return (
    <div className="container">
      <h1 className="title">📋 Incidencias TI</h1>

      {mensaje && <p className="mensaje-ok">{mensaje}</p>}

      <div className="form">
        <input
          className="input"
          placeholder="Título de la incidencia"
          value={titulo}
          onChange={e => setTitulo(e.target.value)}
        />
        <button className="btn-primary" onClick={agregarIncidencia}>
          + Agregar
        </button>
      </div>

      <div className="filters">
        <select onChange={e => setFiltroPrioridad(e.target.value)}>
          <option value="">Todas las prioridades</option>
          <option>Alta</option>
          <option>Media</option>
          <option>Baja</option>
        </select>

        <select onChange={e => setFiltroEstado(e.target.value)}>
          <option value="">Todos los estados</option>
          <option>Abierta</option>
          <option>Cerrada</option>
        </select>
      </div>

      {loading ? (
        <p className="loading">Cargando incidencias...</p>
      ) : incidenciasFiltradas.length === 0 ? (
        <p className="empty">No hay incidencias para mostrar 📭</p>
      ) : (
        <div className="grid">
          {incidenciasFiltradas.map(i => (
            <IncidenciaCard
              key={i.id}
              incidencia={i}
              onDelete={borrarIncidencia}
              onUpdate={actualizar}
            />
          ))}
        </div>
      )}
    </div>
  );
}

