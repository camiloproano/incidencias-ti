# ⚡ Quick Start - IncidenciasTI

**¡Inicia el proyecto en 5 minutos!**

---

## 🚀 Pre-requisitos Instalados

Verificar que tienes:
```powershell
node --version    # v18+
npm --version
dotnet --version  # 8.0+
psql --version    # 14+
mongod --version  # 6.0+
```

---

## ⚡ 5 Pasos para Empezar

### 1️⃣ PostgreSQL - Crear BD y Cargar Datos (1 min)

```powershell
# Conectar a PostgreSQL
psql -U postgres

# En la consola psql:
CREATE DATABASE incidencias_ti;
\q

# Ejecutar script SQL
psql -U postgres -d incidencias_ti -f "backend/IncidenciasTI.API/Data/SQL_INIT.sql"
```

**Verificar:**
```powershell
psql -U postgres -d incidencias_ti
SELECT COUNT(*) FROM "Incidencias";  # Debe retornar 5
\q
```

---

### 2️⃣ MongoDB - Crear Collections y Datos (1 min)

```powershell
# Abrir MongoDB Shell
mongosh

# Dentro de mongosh:
use IncidenciaLogs
db.createCollection("IncidenciaLogs")
db.createCollection("IncidenciasDirect")
exit
```

**Importar datos:**
```powershell
mongoimport --uri mongodb://localhost:27017/IncidenciaLogs ^
            --collection IncidenciaLogs ^
            --file backend/IncidenciasTI.API/Data/MONGO_SEED.json ^
            --jsonArray

mongoimport --uri mongodb://localhost:27017/IncidenciaLogs ^
            --collection IncidenciasDirect ^
            --file backend/IncidenciasTI.API/Data/MONGO_SEED_DIRECT.json ^
            --jsonArray
```

---

### 3️⃣ Backend - Iniciar API (1 min)

```powershell
cd backend/IncidenciasTI.API

# Compilar y ejecutar
dotnet build
dotnet run
```

**Esperado:**
```
Now listening on: http://localhost:5268
```

✅ API lista en http://localhost:5268

---

### 4️⃣ Frontend - Instalar y Ejecutar (1.5 min)

**En otra terminal:**

```powershell
cd frontend/incidencias-ti-ui

# Instalar dependencias
npm install

# Ejecutar en desarrollo
npm run dev
```

**Esperado:**
```
Local:   http://localhost:5173/
```

✅ Frontend listo en http://localhost:5173

---

### 5️⃣ ¡Usar la Aplicación! (0.5 min)

Abre tu navegador:

🌐 **http://localhost:5173**

---

## 🎮 Primeros Pasos en la UI

1. ✅ Haz click en "Incidencias" en el menú (ya deberías ver los 5 datos de ejemplo)
2. ✅ Haz click en "Dashboard" para ver estadísticas
3. ✅ Prueba crear una nueva incidencia en el formulario
4. ✅ Edita una incidencia clickeando en la tarjeta
5. ✅ Filtra por prioridad o estado

---

## 🔗 URLs de Acceso

| Sistema | URL | Uso |
|---------|-----|-----|
| Frontend | http://localhost:5173 | UI principal |
| Backend API | http://localhost:5268 | Requests REST |
| Swagger Docs | http://localhost:5268/swagger | Documentación API |
| MongoDB Compass | mongodb://localhost:27017 | Inspeccionar BD |
| pgAdmin | localhost:5050 | Inspeccionar PostgreSQL |

---

## 📝 Comandos Útiles

### Ver datos en PostgreSQL
```powershell
psql -U postgres -d incidencias_ti
SELECT * FROM "Incidencias";
\q
```

### Ver datos en MongoDB
```powershell
mongosh
use IncidenciasLogs
db.IncidenciaLogs.find().pretty()
db.IncidenciasDirect.find().pretty()
exit
```

### Detener aplicaciones
```powershell
# Backend: Presionar Ctrl+C
# Frontend: Presionar Ctrl+C
# MongoDB: Presionar Ctrl+C
```

---

## 🧪 Probar API con Swagger

1. Abre http://localhost:5268/swagger
2. Expande cualquier endpoint
3. Click en "Try it out"
4. Click en "Execute"
5. Ver respuesta JSON

---

## 🐛 Si Algo No Funciona

### Error: "Connection refused" en Frontend
```powershell
# Verificar que backend está corriendo
# Verificar que .env tiene VITE_API_URL=http://localhost:5268/api
# Reiniciar frontend: npm run dev
```

### Error: "Cannot connect to PostgreSQL"
```powershell
# Verificar PostgreSQL está running
Get-Service postgres*
# Si no está, iniciar: Start-Service postgresql-x64-15
```

### Error: "Cannot connect to MongoDB"
```powershell
# Verificar MongoDB está running
# En una terminal nueva:
mongod
```

### Error: "Build failed in C#"
```powershell
# Limpiar y reconstruir
cd backend/IncidenciasTI.API
dotnet clean
dotnet build
dotnet run
```

---

## 📚 Documentación Completa

| Documento | Lectura | Propósito |
|-----------|---------|----------|
| [README.md](README.md) | 5 min | Overview |
| [SETUP.md](SETUP.md) | 20 min | Instalación detallada |
| [API.md](API.md) | 15 min | Endpoints |
| [ARCHITECTURE.md](ARCHITECTURE.md) | 10 min | Técnica |
| [CONTRIBUTING.md](CONTRIBUTING.md) | 10 min | Contribuir |

---

## 🎯 Próximos Pasos

Una vez que tengas todo corriendo:

1. **Explorar Swagger** (10 min)
   - http://localhost:5268/swagger
   - Probar todos los endpoints

2. **Crear Datos** (5 min)
   - Crear nueva incidencia en UI
   - Editar una existente
   - Eliminar una

3. **Ver Estadísticas** (5 min)
   - Click en Dashboard
   - Observar métricas
   - Auto-refresh cada 30 seg

4. **Sincronizar** (5 min)
   - En Swagger: POST /api/incidencias/sync
   - Verificar que logs se aplican a SQL

5. **Leer Documentación** (30 min)
   - SETUP.md para detalles
   - ARCHITECTURE.md para entender el sistema
   - API.md para endpoints

---

## 💾 Estructura Mínima Para Empezar

```
Si SOLO quieres que funcione rápido, necesitas:

✅ PostgreSQL con data
✅ MongoDB con data  
✅ Backend corriendo
✅ Frontend corriendo
✅ Navegador abierto en localhost:5173

¡Eso es! El resto es documentación de apoyo.
```

---

## ⏱️ Estimación de Tiempo

| Paso | Tiempo | Nota |
|------|--------|------|
| PostgreSQL | 2 min | Si ya está instalado |
| MongoDB | 2 min | Si ya está instalado |
| Backend | 1 min | `dotnet run` |
| Frontend | 2 min | `npm install` + `npm run dev` |
| **TOTAL** | **7 min** | Sin instalar DB engines |

---

## 🎓 Si es tu Primer Proyecto

Te recomendamos:

1. **Primera ejecución** (5 min)
   - Seguir Quick Start hasta que todo funcione
   - Ver los datos en la UI

2. **Exploración** (15 min)
   - Hacer clicks en UI
   - Crear/editar/eliminar incidencias
   - Ver Dashboard
   - Probar filtros

3. **Lectura** (20 min)
   - Leer README.md
   - Leer SETUP.md (secciones de troubleshooting)
   - Ver ARCHITECTURE.md para entender flujos

4. **Profundización** (1-2 horas)
   - Leer código en Controllers/
   - Ver Services/
   - Analizar Models/
   - Inspeccionar componentes React

---

## 🔑 Puntos Clave

- 🎯 **3 formas de sincronizar**: SQL, Logs, Directo
- 📊 **6 tipos de reportes**: Resumen, Críticas, Recientes, etc
- 🎨 **UI responsive**: Funciona en mobile
- 📚 **Bien documentado**: 3000+ líneas de docs
- ✅ **Listo para producción**: Error handling completo

---

## 🆘 Soporte Rápido

```
❌ NO funciona → Revisar SETUP.md sección Troubleshooting
❌ Preguntas API → Ver API.md con ejemplos cURL
❌ Entender código → Leer ARCHITECTURE.md
❌ Contribuir → Ver CONTRIBUTING.md
```

---

## ✅ Checklist Final

Antes de trabajar, verifica:

- [ ] PostgreSQL corriendo (verify: psql -U postgres)
- [ ] MongoDB corriendo (verify: mongosh)
- [ ] Backend corriendo (verify: http://localhost:5268/swagger)
- [ ] Frontend corriendo (verify: http://localhost:5173)
- [ ] Datos cargados (verify: al menos 5 incidencias en lista)

---

**¡Listo para empezar! 🚀**

---

*Para detalles, ver:*
- **SETUP.md** - Instalación completa
- **API.md** - Todos los endpoints
- **ARCHITECTURE.md** - Cómo funciona el sistema

---

**Última actualización:** 17 de Enero de 2025
