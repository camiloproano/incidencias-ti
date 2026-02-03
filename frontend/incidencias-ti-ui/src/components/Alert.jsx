import "../styles/Alert.css";

export default function Alert({ type = "info", message, onClose }) {
  if (!message) return null;

  return (
    <div className={`alert alert-${type}`}>
      <div className="alert-content">
        <span className="alert-message">{message}</span>
        <button className="alert-close" onClick={onClose}>✕</button>
      </div>
    </div>
  );
}
