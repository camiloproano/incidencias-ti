import { useEffect, useState } from "react";
import { obtenerIncidencias } from "../api/incidenciasApi";

export default function Incidencias() {
  const [mensaje, setMensaje] = useState("");

  useEffect(() => {
    obtenerIncidencias()
      .then(res => setMensaje(res.data))
      .catch(() => setMensaje("Error al conectar con el backend"));
  }, []);

  return (
    <div>
      <h1>Incidencias TI</h1>
      <p>{mensaje}</p>
    </div>
  );
}
