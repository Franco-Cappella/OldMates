# Documento de Diseño - Sistema de Amigos

## Overview

El sistema de amigos de OldMates es un módulo que permite a los usuarios establecer y gestionar conexiones sociales dentro de la plataforma. El sistema implementa un modelo de solicitud-aceptación bidireccional donde los usuarios pueden buscar otros usuarios, enviar solicitudes de amistad, aceptar o rechazar solicitudes recibidas, y gestionar su lista de amigos.

La arquitectura actual ya incluye la mayoría de la funcionalidad backend implementada en el modelo `BD.cs` y controladores en `HomeController.cs`. Este diseño se enfoca en completar, validar y mejorar la implementación existente para asegurar que cumple con todos los requerimientos.

## Architecture

### Arquitectura Actual

El sistema sigue el patrón MVC (Model-View-Controller) de ASP.NET Core:

- **Models**: `Usuario`, `Amistad` - representan las entidades del dominio
- **Views**: `BuscarAmigos.cshtml`, `ListaDeAmigos.cshtml` - interfaces de usuario
- **Controllers**: `HomeController.cs` - maneja las peticiones HTTP y coordina la lógica
- **Data Access**: `BD.cs` - capa de acceso a datos usando Dapper

### Flujo de Datos

1. Usuario realiza acción en la vista (buscar, enviar solicitud, aceptar, etc.)
2. La vista envía petición HTTP (GET/POST) al controlador
3. El controlador valida la sesión del usuario
4. El controlador invoca métodos de `BD.cs` para operaciones de base de datos
5. `BD.cs` ejecuta consultas SQL usando Dapper
6. El controlador actualiza ViewBag y retorna la vista correspondiente
7. La vista renderiza los datos actualizados

## Components and Interfaces

### Modelo de Datos

#### Usuario
```csharp
public class Usuario
{
    public int ID { get; set; }
    public string Username { get; set; }
    public string Contraseña { get; set; }
    public string Localidad { get; set; }
    public string Intereses { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public bool Admin { get; set; }
    public string Foto { get; set; }
}
```

#### Amistad
```csharp
public class Amistad
{
    public int ID { get; set; }
    public int IDUsuario1 { get; set; }  // Usuario que envía la solicitud
    public int IDUsuario2 { get; set; }  // Usuario que recibe la solicitud
    public string Estado { get; set; }   // "pendiente", "aceptada", "rechazada"
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaRespuesta { get; set; }
    public Usuario Usuario1 { get; set; }  // Navegación
    public Usuario Usuario2 { get; set; }  // Navegación
}
```

### Capa de Acceso a Datos (BD.cs)

Métodos existentes que necesitan validación:

```csharp
// Búsqueda
List<Usuario> BuscarUsuarios(string busqueda, int IDUsuarioActual)
string ObtenerEstadoAmistad(int IDUsuario1, int IDUsuario2)

// Solicitudes
bool EnviarSolicitudAmistad(int IDUsuario1, int IDUsuario2)
List<Amistad> ObtenerSolicitudesPendientes(int IDUsuario)

// Gestión de amistades
bool AceptarSolicitudAmistad(int IDSolicitud, int IDUsuario)
bool RechazarSolicitudAmistad(int IDSolicitud, int IDUsuario)
List<Usuario> ObtenerAmigos(int IDUsuario)
bool EliminarAmigo(int IDUsuario, int IDAmigo)

// Utilidades
int ContarSolicitudesPendientes(int IDUsuario)
```

### Controlador (HomeController.cs)

Acciones existentes que necesitan validación:

```csharp
// GET: Vistas
IActionResult BuscarAmigos(string busqueda)
IActionResult ListaDeAmigos()

// POST: Acciones
IActionResult EnviarSolicitudAmistad(int IDAmigo)
IActionResult AceptarSolicitud(int IDSolicitud)
IActionResult RechazarSolicitud(int IDSolicitud)
IActionResult EliminarAmigo(int IDAmigo)
```

### Vistas

#### BuscarAmigos.cshtml
- Formulario de búsqueda por texto
- Lista de resultados con información del usuario
- Indicadores de estado de amistad
- Botones de acción según el estado

#### ListaDeAmigos.cshtml
- Sección de solicitudes pendientes
- Lista de amigos actuales
- Acciones para aceptar/rechazar solicitudes
- Acciones para eliminar amigos

## Data Models

### Esquema de Base de Datos

#### Tabla Usuario
```sql
CREATE TABLE Usuario (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Contraseña NVARCHAR(255) NOT NULL,
    Localidad NVARCHAR(100),
    Intereses NVARCHAR(MAX),
    Nombre NVARCHAR(100),
    Apellido NVARCHAR(100),
    Admin BIT DEFAULT 0,
    Foto NVARCHAR(255)
)
```

#### Tabla Amistad
```sql
CREATE TABLE Amistad (
    ID INT PRIMARY KEY IDENTITY(1,1),
    IDUsuario1 INT NOT NULL,
    IDUsuario2 INT NOT NULL,
    Estado NVARCHAR(20) NOT NULL CHECK (Estado IN ('pendiente', 'aceptada', 'rechazada')),
    FechaSolicitud DATETIME NOT NULL DEFAULT GETDATE(),
    FechaRespuesta DATETIME NULL,
    FOREIGN KEY (IDUsuario1) REFERENCES Usuario(ID),
    FOREIGN KEY (IDUsuario2) REFERENCES Usuario(ID),
    CONSTRAINT CK_DiferentesUsuarios CHECK (IDUsuario1 != IDUsuario2)
)
```

### Estados de Amistad

- **ninguno**: No existe relación entre los usuarios
- **pendiente**: Solicitud enviada pero no respondida
- **aceptada**: Amistad confirmada por ambas partes
- **rechazada**: Solicitud rechazada (permite reenvío futuro)

## Correctness Properties


*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Búsqueda de Usuarios

**Property 1: Resultados de búsqueda contienen el texto buscado**
*For any* texto de búsqueda no vacío y cualquier usuario en los resultados, el nombre, apellido o username del usuario debe contener el texto buscado (ignorando mayúsculas/minúsculas)
**Validates: Requirements 1.1**

**Property 2: Exclusión del usuario actual en búsquedas**
*For any* usuario y cualquier texto de búsqueda, el usuario actual no debe aparecer en sus propios resultados de búsqueda
**Validates: Requirements 1.2**

**Property 3: Todos los resultados tienen estado de amistad**
*For any* resultado de búsqueda, cada usuario debe tener un estado de amistad asociado (pendiente, aceptada, rechazada, o ninguno)
**Validates: Requirements 1.3**

**Property 4: Resultados contienen información completa del usuario**
*For any* usuario en los resultados de búsqueda, debe incluir nombre, apellido, foto, localidad e intereses
**Validates: Requirements 1.5**

### Envío de Solicitudes

**Property 5: Creación de solicitud con estado pendiente**
*For any* par de usuarios distintos sin relación previa, enviar una solicitud debe crear una amistad con estado "pendiente"
**Validates: Requirements 2.1**

**Property 6: Prevención de solicitudes duplicadas pendientes**
*For any* par de usuarios con una solicitud pendiente existente, intentar enviar otra solicitud debe ser rechazado
**Validates: Requirements 2.3**

**Property 7: Prevención de solicitudes entre amigos**
*For any* par de usuarios con amistad aceptada, intentar enviar una solicitud debe ser rechazado
**Validates: Requirements 2.4**

**Property 8: Registro de fecha de solicitud**
*For any* solicitud de amistad creada, debe tener una fecha de solicitud válida (no nula y no en el futuro)
**Validates: Requirements 2.5**

### Gestión de Solicitudes Pendientes

**Property 9: Solicitudes pendientes solo para el receptor**
*For any* usuario, su lista de solicitudes pendientes debe contener solo solicitudes donde él es IDUsuario2 y el estado es "pendiente"
**Validates: Requirements 3.1**

**Property 10: Información completa del solicitante**
*For any* solicitud pendiente mostrada, debe incluir nombre, apellido, foto, localidad e intereses del usuario solicitante (IDUsuario1)
**Validates: Requirements 3.2**

### Aceptación de Solicitudes

**Property 11: Transición de estado al aceptar**
*For any* solicitud pendiente válida, cuando el receptor la acepta, el estado debe cambiar a "aceptada"
**Validates: Requirements 4.1**

**Property 12: Registro de fecha de respuesta al aceptar**
*For any* solicitud aceptada, debe tener una fecha de respuesta válida (no nula y posterior o igual a la fecha de solicitud)
**Validates: Requirements 4.2**

**Property 13: Simetría de amistad aceptada**
*For any* amistad aceptada entre usuario A y usuario B, A debe aparecer en la lista de amigos de B y B debe aparecer en la lista de amigos de A
**Validates: Requirements 4.3**

**Property 14: Autorización para aceptar solicitudes**
*For any* solicitud de amistad, solo el usuario receptor (IDUsuario2) debe poder aceptarla
**Validates: Requirements 4.4**

**Property 15: Validación de estado al aceptar**
*For any* solicitud que no está en estado "pendiente", intentar aceptarla debe ser rechazado
**Validates: Requirements 4.5**

### Rechazo de Solicitudes

**Property 16: Transición de estado al rechazar**
*For any* solicitud pendiente válida, cuando el receptor la rechaza, el estado debe cambiar a "rechazada"
**Validates: Requirements 5.1**

**Property 17: Registro de fecha de respuesta al rechazar**
*For any* solicitud rechazada, debe tener una fecha de respuesta válida (no nula y posterior o igual a la fecha de solicitud)
**Validates: Requirements 5.2**

**Property 18: Exclusión de solicitudes rechazadas de pendientes**
*For any* usuario, su lista de solicitudes pendientes no debe contener solicitudes con estado "rechazada"
**Validates: Requirements 5.3**

**Property 19: Autorización para rechazar solicitudes**
*For any* solicitud de amistad, solo el usuario receptor (IDUsuario2) debe poder rechazarla
**Validates: Requirements 5.4**

**Property 20: Validación de estado al rechazar**
*For any* solicitud que no está en estado "pendiente", intentar rechazarla debe ser rechazado
**Validates: Requirements 5.5**

### Lista de Amigos

**Property 21: Lista de amigos solo con amistades aceptadas**
*For any* usuario, su lista de amigos debe contener solo usuarios con los que tiene una amistad en estado "aceptada"
**Validates: Requirements 6.1**

**Property 22: Información completa de amigos**
*For any* amigo en la lista, debe incluir nombre, apellido, foto, localidad e intereses
**Validates: Requirements 6.2**

### Eliminación de Amistades

**Property 23: Eliminación completa de la relación**
*For any* amistad aceptada, después de eliminarla, no debe existir ningún registro de amistad entre esos dos usuarios
**Validates: Requirements 7.1**

**Property 24: Simetría al eliminar amistad**
*For any* amistad eliminada entre usuario A y usuario B, A no debe aparecer en la lista de amigos de B y B no debe aparecer en la lista de amigos de A
**Validates: Requirements 7.2**

**Property 25: Posibilidad de reenvío después de eliminar**
*For any* par de usuarios que eliminaron su amistad, cualquiera de los dos debe poder enviar una nueva solicitud de amistad
**Validates: Requirements 7.3**

**Property 26: Validación de existencia al eliminar**
*For any* intento de eliminar una amistad que no existe, la operación debe ser rechazada o no tener efecto
**Validates: Requirements 7.4**

### Seguridad y Validación

**Property 27: Redirección sin autenticación**
*For any* acción del sistema de amigos, si el usuario no está autenticado, debe ser redirigido a la página de inicio de sesión
**Validates: Requirements 8.1**

**Property 28: Rechazo de operaciones sin sesión**
*For any* acción de amistad, si no hay sesión activa, la operación debe ser rechazada
**Validates: Requirements 8.2**

**Property 29: Atomicidad de operaciones**
*For any* operación de amistad que falla, el estado del sistema debe permanecer sin cambios (no debe haber cambios parciales)
**Validates: Requirements 8.3**

## Error Handling

### Validación de Entrada

1. **Validación de Sesión**: Todas las acciones deben verificar que existe un usuario en sesión usando `ObtenerIntegranteDesdeSession()`
2. **Validación de IDs**: Verificar que los IDs de usuario y solicitud existen antes de realizar operaciones
3. **Validación de Estados**: Verificar que las transiciones de estado son válidas (ej: solo aceptar solicitudes pendientes)
4. **Validación de Autorización**: Verificar que el usuario tiene permiso para realizar la acción (ej: solo el receptor puede aceptar)

### Manejo de Errores

1. **Errores de Base de Datos**: Los métodos de `BD.cs` retornan `bool` o listas vacías en caso de error
2. **Errores de Validación**: El controlador debe verificar condiciones antes de llamar a `BD.cs`
3. **Redirecciones**: En caso de error de autenticación, redirigir a `Index` de `AccountController`
4. **Mensajes de Error**: Usar `ViewBag.Error` para comunicar errores al usuario

### Casos Edge

1. **Búsqueda vacía**: Retornar lista vacía
2. **Auto-solicitud**: Prevenir que un usuario se envíe solicitud a sí mismo
3. **Solicitudes duplicadas**: Prevenir múltiples solicitudes entre los mismos usuarios
4. **Eliminación de amistad inexistente**: Operación debe fallar silenciosamente o retornar false
5. **Usuarios sin amigos**: Mostrar estado vacío apropiado en la UI

## Testing Strategy

### Unit Testing

El sistema de amigos requiere unit tests para validar:

1. **Métodos de BD.cs**:
   - `BuscarUsuarios`: Verificar que retorna usuarios correctos y excluye al usuario actual
   - `EnviarSolicitudAmistad`: Verificar que crea solicitudes correctamente y previene duplicados
   - `AceptarSolicitudAmistad`: Verificar que actualiza el estado correctamente
   - `RechazarSolicitudAmistad`: Verificar que actualiza el estado correctamente
   - `EliminarAmigo`: Verificar que elimina la relación correctamente
   - `ObtenerAmigos`: Verificar que retorna solo amistades aceptadas
   - `ObtenerSolicitudesPendientes`: Verificar que retorna solo solicitudes pendientes para el usuario

2. **Validaciones del Controlador**:
   - Verificar que las acciones validan la sesión correctamente
   - Verificar que las redirecciones funcionan cuando no hay sesión
   - Verificar que los ViewBag se populan correctamente

### Property-Based Testing

El sistema utilizará **xUnit** con **FsCheck** para property-based testing en C#.

**Configuración**:
- Cada property test debe ejecutar un mínimo de 100 iteraciones
- Cada test debe estar etiquetado con el formato: `**Feature: sistema-amigos, Property {number}: {property_text}**`
- Los tests deben generar datos aleatorios válidos (usuarios, solicitudes, estados)

**Generadores necesarios**:
- Generador de usuarios con datos válidos
- Generador de pares de usuarios distintos
- Generador de solicitudes de amistad con diferentes estados
- Generador de textos de búsqueda

**Properties a implementar**:
- Todas las 29 properties listadas en la sección "Correctness Properties"
- Cada property debe ser implementada como un test independiente
- Los tests deben usar datos generados aleatoriamente para validar las propiedades universales

### Integration Testing

Tests de integración para validar:

1. **Flujo completo de amistad**:
   - Usuario A busca a Usuario B
   - Usuario A envía solicitud a Usuario B
   - Usuario B ve la solicitud pendiente
   - Usuario B acepta la solicitud
   - Ambos usuarios se ven en sus listas de amigos

2. **Flujo de rechazo**:
   - Usuario A envía solicitud a Usuario B
   - Usuario B rechaza la solicitud
   - La solicitud no aparece en pendientes
   - Usuario A puede reenviar la solicitud

3. **Flujo de eliminación**:
   - Usuarios A y B son amigos
   - Usuario A elimina la amistad
   - Ninguno aparece en la lista del otro
   - Pueden volver a enviarse solicitudes

## Implementation Notes

### Mejoras Necesarias

1. **Validación de Autorización**: Agregar validación explícita en `AceptarSolicitudAmistad` y `RechazarSolicitudAmistad` para verificar que el usuario que ejecuta la acción es el receptor (IDUsuario2)

2. **Manejo de Errores**: Mejorar el manejo de errores en el controlador para proporcionar mensajes más específicos

3. **Índices de Base de Datos**: Agregar índices en la tabla Amistad para mejorar el rendimiento de las consultas:
   ```sql
   CREATE INDEX IX_Amistad_Usuario1 ON Amistad(IDUsuario1)
   CREATE INDEX IX_Amistad_Usuario2 ON Amistad(IDUsuario2)
   CREATE INDEX IX_Amistad_Estado ON Amistad(Estado)
   ```

4. **Constraint de Unicidad**: Agregar constraint para prevenir solicitudes duplicadas:
   ```sql
   CREATE UNIQUE INDEX IX_Amistad_Unique 
   ON Amistad(IDUsuario1, IDUsuario2) 
   WHERE Estado != 'rechazada'
   ```

### Consideraciones de Rendimiento

1. **Paginación**: Para usuarios con muchos amigos, considerar implementar paginación
2. **Caché**: Considerar cachear la lista de amigos para reducir consultas a la base de datos
3. **Consultas Optimizadas**: Las consultas actuales usan JOINs apropiados, pero verificar planes de ejecución

### Seguridad

1. **SQL Injection**: Dapper con parámetros previene SQL injection
2. **CSRF**: ASP.NET Core incluye protección CSRF automática para formularios
3. **Autorización**: Verificar que los usuarios solo pueden modificar sus propias relaciones
4. **Validación de Entrada**: Sanitizar texto de búsqueda para prevenir ataques XSS
