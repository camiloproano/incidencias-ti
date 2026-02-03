import "../styles/FilterBar.css";

export default function FilterBar({ prioridad, setPrioridad, estado, setEstado, count }) {
  return (
    <div className="filter-bar">
      <div className="filter-info">
        <span className="count-badge">{count} incidencia{count !== 1 ? "s" : ""}</span>
      </div>

      <div className="filter-controls">
        <div className="filter-group">
          <label htmlFor="filter-prioridad">Prioridad</label>
          <select
            id="filter-prioridad"
            value={prioridad}
            onChange={(e) => setPrioridad(e.target.value)}
          >
            <option value="">Todas</option>
            <option value="Baja">🟢 Baja</option>
            <option value="Media">🟡 Media</option>
            <option value="Alta">🔴 Alta</option>
            <option value="Crítica">⚠️ Crítica</option>
          </select>
        </div>

        <div className="filter-group">
          <label htmlFor="filter-estado">Estado</label>
          <select
            id="filter-estado"
            value={estado}
            onChange={(e) => setEstado(e.target.value)}
          >
            <option value="">Todos</option>
            <option value="Abierta">Abierta</option>
            <option value="En Proceso">En Proceso</option>
            <option value="Cerrada">Cerrada</option>
          </select>
        </div>

        <button 
          className="btn-reset"
          onClick={() => {
            setPrioridad("");
            setEstado("");
          }}
        >
          🔄 Limpiar filtros
        </button>
      </div>
    </div>
  );
}
