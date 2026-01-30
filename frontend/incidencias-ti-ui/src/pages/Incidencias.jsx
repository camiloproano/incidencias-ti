import { useEffect, useState } from "react";
import { obtenerIncidencias, crearIncidencia, eliminarIncidencia } from "../api/incidenciasApi";

export default function Incidencias() {
  const [incidencias, setIncidencias] = useState([]);
  const [titulo, setTitulo] = useState("");

  const cargarIncidencias = () => {
    obtenerIncidencias().then(res => setIncidencias(res.data));
  };

  useEffect(() => {
    cargarIncidencias();
  }, []);

  const agregarIncidencia = () => {
    crearIncidencia({
      titulo,
      descripcion: "Incidencia creada desde React",
      prioridad: "Media",
      estado: "Abierta"
    }).then(() => {
      setTitulo("");
      cargarIncidencias();
    });
  };

  const borrarIncidencia = (id) => {
    eliminarIncidencia(id).then(cargarIncidencias);
  };

  return (
    <div style={{ padding: "20px" }}>
      <h1>Incidencias TI</h1>

      <input
        placeholder="Título de la incidencia"
        value={titulo}
        onChange={e => setTitulo(e.target.value)}
      />
      <button onClick={agregarIncidencia}>Agregar</button>

      <ul>
        {incidencias.map(i => (
          <li key={i.id}>
            <strong>{i.titulo}</strong> ({i.prioridad})
            <button onClick={() => borrarIncidencia(i.id)}>Eliminar</button>
          </li>
        ))}
      </ul>
    </div>
  );
}
