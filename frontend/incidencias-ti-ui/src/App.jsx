import { useState } from "react";
import Navigation from "./components/Navigation";
import Incidencias from "./pages/Incidencias2";
import Dashboard from "./components/Dashboard";
import "./styles/global.css";

function App() {
  const [currentPage, setCurrentPage] = useState("incidencias");

  const renderPage = () => {
    switch (currentPage) {
      case "dashboard":
        return <Dashboard />;
      case "incidencias":
      default:
        return <Incidencias />;
    }
  };

  return (
    <div className="app">
      <Navigation onNavigate={setCurrentPage} currentPage={currentPage} />
      <main className="app-main">
        {renderPage()}
      </main>
    </div>
  );
}

export default App;
