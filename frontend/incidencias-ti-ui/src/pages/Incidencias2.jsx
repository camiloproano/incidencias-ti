import { useEffect, useState } from "react";
import { obtenerIncidencias, crearIncidencia, eliminarIncidencia, actualizarIncidencia } from "../api/incidenciasApi";
import Header from "../components/Header";
import IncidenciaForm from "../components/IncidenciaForm";
import IncidenciaCard from "../components/IncidenciaCard";
import FilterBar from "../components/FilterBar";
import Alert from "../components/Alert";
import "../styles/Incidencias.css";

export default function Incidencias() {
  const [incidencias, setIncidencias] = useState([]);
  const [titulo, setTitulo] = useState("");
  const [descripcion, setDescripcion] = useState("");
  const [prioridad, setPrioridad] = useState("Media");
  const [filtroPrioridad, setFiltroPrioridad] = useState("");
  const [filtroEstado, setFiltroEstado] = useState("");
  const [loading, setLoading] = useState(true);
  const [alert, setAlert] = useState({ type: "", message: "" });

  const cargarIncidencias = async () => {
    try {
      setLoading(true);
      const res = await obtenerIncidencias();
      setIncidencias(res.data || []);
    } catch (error) {
      setAlert({
        type: "danger",
        message: "❌ Error al cargar incidencias. Verifica que la API esté corriendo en http://localhost:5268"
      });
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    cargarIncidencias();
  }, []);

  const agregarIncidencia = async () => {
    if (!titulo.trim()) {
      setAlert({ type: "warning", message: "⚠️ Por favor ingresa un título" });
      return;
    }

    try {
      await crearIncidencia({
        titulo,
        descripcion,
        prioridad,
        usuario: "Usuario",
        estado: "Abierta"
      });

      setTitulo("");
      setDescripcion("");
      setPrioridad("Media");
      setAlert({ type: "success", message: "✅ Incidencia creada correctamente" });
      await cargarIncidencias();
    } catch (error) {
      setAlert({ type: "danger", message: "❌ Error al crear incidencia" });
      console.error(error);
    }
  };

  const borrarIncidencia = async (id) => {
    try {
      await eliminarIncidencia(id);
      setAlert({ type: "success", message: "✅ Incidencia eliminada correctamente" });
      await cargarIncidencias();
    } catch (error) {
      setAlert({ type: "danger", message: "❌ Error al eliminar incidencia" });
      console.error(error);
    }
  };

  const actualizar = async (id, data) => {
    try {
      await actualizarIncidencia(id, { ...data, usuario: "Usuario" });
      setAlert({ type: "success", message: "✅ Incidencia actualizada correctamente" });
      await cargarIncidencias();
    } catch (error) {
      setAlert({ type: "danger", message: "❌ Error al actualizar incidencia" });
      console.error(error);
    }
  };

  const incidenciasFiltradas = incidencias.filter((i) =>
    (filtroPrioridad ? i.prioridad === filtroPrioridad : true) &&
    (filtroEstado ? i.estado === filtroEstado : true)
  );

  return (
    <div className="app">
      <Header />
      
      <div className="container">
        <IncidenciaForm
          titulo={titulo}
          setTitulo={setTitulo}
          descripcion={descripcion}
          setDescripcion={setDescripcion}
          prioridad={prioridad}
          setPrioridad={setPrioridad}
          onSubmit={agregarIncidencia}
          loading={loading}
        />

        <FilterBar
          prioridad={filtroPrioridad}
          setPrioridad={setFiltroPrioridad}
          estado={filtroEstado}
          setEstado={setFiltroEstado}
          count={incidenciasFiltradas.length}
        />

        {loading ? (
          <div className="loading-container">
            <div className="spinner"></div>
            <p>Cargando incidencias...</p>
          </div>
        ) : incidenciasFiltradas.length === 0 ? (
          <div className="empty-container">
            <p className="empty-text">📭 No hay incidencias para mostrar</p>
          </div>
        ) : (
          <div className="grid">
            {incidenciasFiltradas.map((incidencia) => (
              <IncidenciaCard
                key={incidencia.id}
                incidencia={incidencia}
                onDelete={borrarIncidencia}
                onUpdate={actualizar}
              />
            ))}
          </div>
        )}
      </div>

      {alert.message && (
        <Alert
          type={alert.type}
          message={alert.message}
          onClose={() => setAlert({ type: "", message: "" })}
        />
      )}
    </div>
  );
}
