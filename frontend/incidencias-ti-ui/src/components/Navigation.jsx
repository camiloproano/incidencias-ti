import React, { useState } from 'react';
import '../styles/Navigation.css';

export default function Navigation({ onNavigate, currentPage }) {
  const menuItems = [
    { id: 'incidencias', label: 'Incidencias', icon: '📋' },
    { id: 'dashboard', label: 'Dashboard', icon: '📊' }
  ];

  return (
    <nav className="navigation">
      <div className="nav-container">
        <div className="nav-logo">
          <span className="nav-icon">⚙️</span>
          <span className="nav-title">IncidenciasTI</span>
        </div>

        <ul className="nav-menu">
          {menuItems.map(item => (
            <li key={item.id}>
              <button
                className={`nav-link ${currentPage === item.id ? 'active' : ''}`}
                onClick={() => onNavigate(item.id)}
              >
                <span className="nav-item-icon">{item.icon}</span>
                <span>{item.label}</span>
              </button>
            </li>
          ))}
        </ul>

        <div className="nav-info">
          <span className="timestamp">
            {new Date().toLocaleTimeString('es-ES')}
          </span>
        </div>
      </div>
    </nav>
  );
}
