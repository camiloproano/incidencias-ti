# 🤝 Guía de Contribución - IncidenciasTI

## Introducción

¡Gracias por tu interés en contribuir al proyecto **IncidenciasTI**! Esta guía te ayudará a entender cómo contribuir de manera efectiva.

---

## Tabla de Contenidos

1. [Código de Conducta](#código-de-conducta)
2. [Cómo Reportar Bugs](#cómo-reportar-bugs)
3. [Cómo Sugerir Mejoras](#cómo-sugerir-mejoras)
4. [Proceso de Desarrollo](#proceso-de-desarrollo)
5. [Estándares de Código](#estándares-de-código)
6. [Testing](#testing)
7. [Documentación](#documentación)
8. [Pull Requests](#pull-requests)

---

## Código de Conducta

### Nuestro Compromiso

Nos comprometemos a proporcionar un entorno acogedor y respetuoso para todos los colaboradores, independientemente de su experiencia, edad, origen étnico, identidad de género, o cualquier otra característica.

### Comportamiento Esperado

- Ser respetuoso y considerado en todas las interacciones
- Aceptar críticas constructivas
- Enfocarse en lo que es mejor para la comunidad
- Mostrar empatía hacia otros miembros

### Comportamiento Inaceptable

- Lenguaje ofensivo o discriminatorio
- Acoso o intimidación
- Ataques personales
- Cualquier conducta que viola el respeto mutuo

---

## Cómo Reportar Bugs

### Antes de Reportar

1. Verificar en la [documentación](README.md) si tu problema ya está documentado
2. Buscar en [GitHub Issues](https://github.com/tu-repo/incidencias-ti/issues) si ya existe un reporte
3. Intentar reproducir el bug en un entorno fresco

### Formato de Reporte

Cuando reportes un bug, incluye:

**Título clara y descriptivo:**
```
[BUG] Error en sincronización cuando hay registros duplicados
```

**Descripción:**
```
Descripción clara de lo que sucede

Pasos para reproducir:
1. Crear una incidencia con titulo "Test"
2. Modificarla en MongoDB
3. Ejecutar POST /api/incidencias/sync

Comportamiento esperado:
Los cambios se aplican a PostgreSQL

Comportamiento actual:
Se genera una excepción: "Duplicate key"

Capturas de pantalla (si aplica):
[adjuntar]

Información del sistema:
- Windows 10
- .NET 8.0
- MongoDB 6.0
- PostgreSQL 14
```

---

## Cómo Sugerir Mejoras

### Antes de Sugerir

1. Verificar que la mejora no existe ya
2. Revisar la [roadmap](ROADMAP.md) (si existe)
3. Asegurar que la mejora se alinea con la visión del proyecto

### Formato de Sugerencia

**Título:**
```
[FEATURE] Agregar autenticación con JWT
```

**Descripción:**
```
Descripción clara de la mejora

Motivación:
- Por qué es necesaria
- Qué problemas resuelve
- Ejemplos de uso

Implementación Propuesta:
- Cambios backend
- Cambios frontend
- Bases de datos

Beneficios:
- Seguridad mejorada
- Mejor UX
- etc.

Alternativas Consideradas:
- OAuth2
- SAML
- etc.
```

---

## Proceso de Desarrollo

### Setup Local

```bash
# 1. Fork el repositorio
# 2. Clonar tu fork
git clone https://github.com/tu-usuario/incidencias-ti.git
cd incidencias-ti

# 3. Crear rama de desarrollo
git checkout -b develop

# 4. Crear rama para tu feature
git checkout -b feature/nombre-del-feature

# 5. Instalar dependencias
cd backend/IncidenciasTI.API && dotnet restore
cd ../../frontend/incidencias-ti-ui && npm install
```

### Ramas (Git Flow)

```
main (releases solamente)
 ├── develop (integración)
 │   └── feature/* (desarrollo)
 │   └── bugfix/* (correcciones)
 │   └── hotfix/* (urgentes)
```

**Nombramiento de Ramas:**

```
feature/add-authentication
bugfix/sync-timeout-issue
hotfix/critical-security-fix
chore/update-dependencies
docs/api-documentation
test/unit-tests-controllers
```

---

## Estándares de Código

### Backend (C#)

#### Convenciones de Nombre

```csharp
// Clases: PascalCase
public class IncidenciaService { }

// Métodos públicos: PascalCase
public async Task<IncidenciaDto> GetIncidenciaAsync(int id) { }

// Variables locales y parámetros: camelCase
var incidenciaCount = 0;
async Task ProcessIncidenciaAsync(string titulo) { }

// Constantes: UPPER_SNAKE_CASE
const string DEFAULT_STATUS = "Abierta";

// Propiedades privadas: _camelCase
private string _connectionString = "";
```

#### Estilo de Código

```csharp
// ✅ Correcto
public class IncidenciasController : ControllerBase
{
    private readonly ILogger<IncidenciasController> _logger;
    
    public IncidenciasController(ILogger<IncidenciasController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IncidenciaDto>> GetIncidencia(int id)
    {
        try
        {
            var incidencia = await _context.Incidencias.FindAsync(id);
            
            if (incidencia == null)
            {
                _logger.LogWarning($"Incidencia {id} no encontrada");
                return NotFound();
            }
            
            return Ok(incidencia);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener incidencia: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

// ❌ Incorrecto
public class IncidenciasController:ControllerBase{
    public ActionResult GetIncidencia(int id){
        try{
            var inc=_context.Incidencias.Find(id);
            if(inc==null)return NotFound();
            return Ok(inc);
        }catch(Exception ex){
            return StatusCode(500,ex.Message);
        }
    }
}
```

#### Comentarios

```csharp
/// <summary>
/// Obtiene una incidencia por su ID.
/// </summary>
/// <param name="id">El ID de la incidencia a obtener.</param>
/// <returns>La incidencia encontrada.</returns>
/// <exception cref="ArgumentException">Si el ID es menor a 0.</exception>
public async Task<IncidenciaDto> GetIncidenciaAsync(int id)
{
    if (id <= 0)
        throw new ArgumentException("El ID debe ser mayor a 0", nameof(id));
    
    // Buscar incidencia en base de datos
    var incidencia = await _context.Incidencias.FindAsync(id);
    
    // Mapear a DTO
    return _mapper.Map<IncidenciaDto>(incidencia);
}
```

### Frontend (JavaScript/React)

#### Convenciones de Nombre

```javascript
// Componentes: PascalCase
function IncidenciaCard({ incidencia }) { }

// Funciones utilitarias: camelCase
const formatDate = (date) => { }

// Constantes: UPPER_SNAKE_CASE
const API_TIMEOUT = 30000;

// Variables locales: camelCase
const [isLoading, setIsLoading] = useState(false);
```

#### Estructura de Componentes

```jsx
// ✅ Correcto
import React, { useState, useEffect } from 'react';
import '../styles/IncidenciaCard.css';

/**
 * Componente que muestra una tarjeta de incidencia
 * @param {Object} props - Props del componente
 * @param {Object} props.incidencia - Datos de la incidencia
 * @param {Function} props.onUpdate - Callback para actualización
 * @returns {JSX.Element}
 */
export default function IncidenciaCard({ incidencia, onUpdate }) {
  const [isEditing, setIsEditing] = useState(false);
  
  useEffect(() => {
    // Inicialización
    console.log('Incidencia cargada:', incidencia.id);
    
    return () => {
      // Cleanup
    };
  }, [incidencia.id]);

  const handleEdit = () => {
    setIsEditing(!isEditing);
  };

  return (
    <div className="incidencia-card">
      <h3>{incidencia.titulo}</h3>
      <p>{incidencia.descripcion}</p>
      <button onClick={handleEdit}>
        {isEditing ? 'Guardar' : 'Editar'}
      </button>
    </div>
  );
}

// ❌ Incorrecto
function IncidenciaCard(props) {
  const [isEditing, setIsEditing] = React.useState(false);
  
  return (
    <div>
      <h3>{props.incidencia.titulo}</h3>
      <p>{props.incidencia.descripcion}</p>
      <button onClick={() => setIsEditing(!isEditing)}>Edit</button>
    </div>
  );
}
```

---

## Testing

### Backend - Unit Tests

```bash
# Crear proyecto de tests
dotnet new xunit -n IncidenciasTI.API.Tests
cd IncidenciasTI.API.Tests
dotnet add reference ../IncidenciasTI.API/IncidenciasTI.API.csproj

# Ejecutar tests
dotnet test
```

**Ejemplo de Test:**

```csharp
public class IncidenciasControllerTests
{
    [Fact]
    public async Task GetIncidencia_WithValidId_ReturnsOk()
    {
        // Arrange
        var mockService = new Mock<IIncidenciaService>();
        var incidencia = new IncidenciaDto { Id = 1, Titulo = "Test" };
        mockService.Setup(s => s.GetIncidenciaAsync(1))
            .ReturnsAsync(incidencia);
        
        var controller = new IncidenciasController(mockService.Object);

        // Act
        var result = await controller.GetIncidencia(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(incidencia, okResult.Value);
    }
}
```

### Frontend - Component Tests

```bash
# Instalar testing library
npm install --save-dev @testing-library/react @testing-library/jest-dom vitest

# Ejecutar tests
npm run test
```

**Ejemplo de Test:**

```javascript
import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import IncidenciaCard from '../components/IncidenciaCard';

describe('IncidenciaCard', () => {
  it('renders incidencia title correctly', () => {
    const incidencia = {
      id: 1,
      titulo: 'Test Issue',
      descripcion: 'Test Description'
    };

    render(<IncidenciaCard incidencia={incidencia} />);
    
    expect(screen.getByText('Test Issue')).toBeInTheDocument();
  });
});
```

---

## Documentación

### Comentarios en Código

- Explicar el **por qué**, no el **qué**
- Mantener comentarios actualizados
- Usar comentarios XML en métodos públicos (C#)
- JSDoc en funciones importantes (JavaScript)

### Archivos de Documentación

- **README.md**: Descripción general y setup rápido
- **SETUP.md**: Instalación detallada
- **API.md**: Documentación de endpoints
- **ARCHITECTURE.md**: Diagrama y decisiones
- **CONTRIBUTING.md**: Este archivo

---

## Pull Requests

### Antes de Hacer un PR

1. Asegurar que tu código pasa todos los tests
2. Actualizar documentación si es necesario
3. Hacer rebase contra develop: `git rebase develop`
4. Hacer push a tu fork

### Crear un Pull Request

**Título:**
```
[FEATURE] Agregar endpoint de estadísticas
```

**Descripción:**
```markdown
## Descripción
Agrega nuevo endpoint `/api/estadisticas/resumen` para obtener un resumen general de incidencias.

## Cambios
- Nuevo controlador: `EstadisticasController.cs`
- Nuevos endpoints: 6 métodos públicos
- Actualizado: `Program.cs` (DI)
- Agregado: Documentación en README

## Testing
- [x] Testeado localmente
- [x] Todos los tests pasan
- [x] Tested en navegador (Chrome 120)

## Checklist
- [x] Código sigue estándares del proyecto
- [x] He actualizado la documentación
- [x] He agregado tests
- [x] No hay cambios sin necesidad
- [x] Este PR tiene un propósito único

## Closes
Cierra #123
```

### Revisión de PR

- Espera revisión de al menos 1 mantenedor
- Responde los comentarios constructivamente
- Haz cambios solicitados en commits separados
- Rebase y force push una vez que los cambios estén aprobados

---

## Mejores Prácticas

### Commits

```bash
# ✅ Correcto
git commit -m "feat: agregar endpoint de estadísticas"
git commit -m "fix: resolver timeout en sincronización"
git commit -m "docs: actualizar API.md con nuevos endpoints"
git commit -m "refactor: simplificar SyncService"
git commit -m "test: agregar tests para IncidenciasController"

# ❌ Incorrecto
git commit -m "cambios"
git commit -m "fix bug"
git commit -m "actualizado todo"
```

### Semantic Commit Messages

```
type(scope): subject

feat: nueva feature
fix: corrección de bug
docs: cambios en documentación
style: cambios de formato (no afectan código)
refactor: refactorización sin cambiar funcionalidad
perf: mejoras de performance
test: agregar o actualizar tests
chore: cambios en build, deps, etc.
```

---

## Roadmap

### v2.0 (Próximo)
- [ ] Autenticación JWT
- [ ] Real-time notifications (SignalR)
- [ ] Búsqueda avanzada
- [ ] Export a PDF

### v3.0
- [ ] Dockerización
- [ ] Kubernetes ready
- [ ] Redis cache
- [ ] Message Queue

---

## Licencia

Al contribuir al proyecto, aceptas que tus contribuciones se licencian bajo la [MIT License](LICENSE).

---

## Contacto

- **Reportar bugs**: [GitHub Issues](https://github.com/tu-repo/incidencias-ti/issues)
- **Preguntas**: [GitHub Discussions](https://github.com/tu-repo/incidencias-ti/discussions)
- **Email**: tu-email@ejemplo.com

---

## Reconocimientos

Gracias a todos los contribuyentes que ayudan a hacer este proyecto mejor! 🎉

---

**Última actualización:** Enero 2025  
**Versión:** 1.0
