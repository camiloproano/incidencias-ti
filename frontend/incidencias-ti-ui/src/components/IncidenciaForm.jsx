import "../styles/IncidenciaForm.css";

export default function IncidenciaForm({ titulo, setTitulo, descripcion, setDescripcion, prioridad, setPrioridad, onSubmit, loading }) {
  const handleSubmit = (e) => {
    e.preventDefault();
    if (!titulo.trim()) {
      alert("Por favor ingresa un título");
      return;
    }
    onSubmit();
  };

  return (
    <form className="form-container" onSubmit={handleSubmit}>
      <div className="form-group">
        <label htmlFor="titulo">Título *</label>
        <input
          id="titulo"
          type="text"
          placeholder="Ej: Servidor SQL caído"
          value={titulo}
          onChange={(e) => setTitulo(e.target.value)}
          maxLength={100}
        />
      </div>

      <div className="form-group">
        <label htmlFor="descripcion">Descripción</label>
        <textarea
          id="descripcion"
          placeholder="Describe el problema en detalle..."
          value={descripcion}
          onChange={(e) => setDescripcion(e.target.value)}
          rows={3}
          maxLength={500}
        />
        <small>{descripcion.length}/500 caracteres</small>
      </div>

      <div className="form-group">
        <label htmlFor="prioridad">Prioridad</label>
        <select
          id="prioridad"
          value={prioridad}
          onChange={(e) => setPrioridad(e.target.value)}
        >
          <option value="Baja">🟢 Baja</option>
          <option value="Media">🟡 Media</option>
          <option value="Alta">🔴 Alta</option>
          <option value="Crítica">⚠️ Crítica</option>
        </select>
      </div>

      <button type="submit" className="btn-primary" disabled={loading}>
        {loading ? "Creando..." : "+ Crear Incidencia"}
      </button>
    </form>
  );
}
