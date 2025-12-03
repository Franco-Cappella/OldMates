# Plan de Implementación - Sistema de Amigos

- [ ] 1. Validar y mejorar métodos de base de datos existentes
  - Revisar y validar todos los métodos de BD.cs relacionados con amigos
  - Agregar validaciones de autorización en AceptarSolicitudAmistad y RechazarSolicitudAmistad
  - Asegurar que EnviarSolicitudAmistad previene auto-solicitudes y duplicados
  - Verificar que ObtenerAmigos y ObtenerSolicitudesPendientes retornan datos correctos
  - _Requirements: 2.2, 2.3, 2.4, 4.4, 5.4, 6.1, 3.1_

- [ ] 1.1 Escribir property test para búsqueda de usuarios
  - **Property 1: Resultados de búsqueda contienen el texto buscado**
  - **Validates: Requirements 1.1**

- [ ] 1.2 Escribir property test para exclusión del usuario actual
  - **Property 2: Exclusión del usuario actual en búsquedas**
  - **Validates: Requirements 1.2**

- [ ] 1.3 Escribir property test para estado de amistad en resultados
  - **Property 3: Todos los resultados tienen estado de amistad**
  - **Validates: Requirements 1.3**

- [ ] 1.4 Escribir property test para información completa en resultados
  - **Property 4: Resultados contienen información completa del usuario**
  - **Validates: Requirements 1.5**

- [ ] 1.5 Escribir property test para creación de solicitudes
  - **Property 5: Creación de solicitud con estado pendiente**
  - **Validates: Requirements 2.1**

- [ ] 1.6 Escribir property test para prevención de duplicados
  - **Property 6: Prevención de solicitudes duplicadas pendientes**
  - **Validates: Requirements 2.3**

- [ ] 1.7 Escribir property test para prevención entre amigos
  - **Property 7: Prevención de solicitudes entre amigos**
  - **Validates: Requirements 2.4**

- [ ] 1.8 Escribir property test para fecha de solicitud
  - **Property 8: Registro de fecha de solicitud**
  - **Validates: Requirements 2.5**

- [ ] 2. Mejorar validaciones en el controlador
  - Agregar validación explícita de sesión en todas las acciones de amigos
  - Mejorar mensajes de error usando ViewBag.Error
  - Agregar validación de IDs antes de llamar a métodos de BD
  - Implementar manejo de errores robusto con try-catch donde sea necesario
  - _Requirements: 8.1, 8.2, 8.3_

- [ ] 2.1 Escribir property test para solicitudes pendientes del receptor
  - **Property 9: Solicitudes pendientes solo para el receptor**
  - **Validates: Requirements 3.1**

- [ ] 2.2 Escribir property test para información del solicitante
  - **Property 10: Información completa del solicitante**
  - **Validates: Requirements 3.2**

- [ ] 2.3 Escribir property test para transición al aceptar
  - **Property 11: Transición de estado al aceptar**
  - **Validates: Requirements 4.1**

- [ ] 2.4 Escribir property test para fecha de respuesta al aceptar
  - **Property 12: Registro de fecha de respuesta al aceptar**
  - **Validates: Requirements 4.2**

- [ ] 2.5 Escribir property test para simetría de amistad
  - **Property 13: Simetría de amistad aceptada**
  - **Validates: Requirements 4.3**

- [ ] 3. Agregar índices y constraints a la base de datos
  - Crear índices en IDUsuario1, IDUsuario2 y Estado en tabla Amistad
  - Agregar constraint de unicidad para prevenir solicitudes duplicadas
  - Verificar que el constraint CHECK para estados funciona correctamente
  - Documentar los cambios en un script SQL
  - _Requirements: 2.3, 2.4_

- [ ] 3.1 Escribir property test para autorización al aceptar
  - **Property 14: Autorización para aceptar solicitudes**
  - **Validates: Requirements 4.4**

- [ ] 3.2 Escribir property test para validación de estado al aceptar
  - **Property 15: Validación de estado al aceptar**
  - **Validates: Requirements 4.5**

- [ ] 3.3 Escribir property test para transición al rechazar
  - **Property 16: Transición de estado al rechazar**
  - **Validates: Requirements 5.1**

- [ ] 3.4 Escribir property test para fecha de respuesta al rechazar
  - **Property 17: Registro de fecha de respuesta al rechazar**
  - **Validates: Requirements 5.2**

- [ ] 4. Mejorar las vistas existentes
  - Agregar validación de entrada en el formulario de búsqueda
  - Mejorar mensajes de error y éxito en las vistas
  - Agregar confirmación antes de eliminar amigos (ya existe)
  - Mejorar accesibilidad con atributos ARIA apropiados
  - _Requirements: 1.1, 1.4, 3.3, 6.3_

- [ ] 4.1 Escribir property test para exclusión de rechazadas
  - **Property 18: Exclusión de solicitudes rechazadas de pendientes**
  - **Validates: Requirements 5.3**

- [ ] 4.2 Escribir property test para autorización al rechazar
  - **Property 19: Autorización para rechazar solicitudes**
  - **Validates: Requirements 5.4**

- [ ] 4.3 Escribir property test para validación de estado al rechazar
  - **Property 20: Validación de estado al rechazar**
  - **Validates: Requirements 5.5**

- [ ] 4.4 Escribir property test para lista de amigos aceptados
  - **Property 21: Lista de amigos solo con amistades aceptadas**
  - **Validates: Requirements 6.1**

- [ ] 5. Agregar validación de edge cases
  - Implementar manejo de búsqueda vacía (ya existe)
  - Agregar validación para prevenir auto-solicitudes en el backend
  - Implementar manejo de eliminación de amistad inexistente
  - Agregar validación de estados antes de transiciones
  - _Requirements: 1.4, 2.2, 7.4, 4.5, 5.5_

- [ ] 5.1 Escribir property test para información de amigos
  - **Property 22: Información completa de amigos**
  - **Validates: Requirements 6.2**

- [ ] 5.2 Escribir property test para eliminación completa
  - **Property 23: Eliminación completa de la relación**
  - **Validates: Requirements 7.1**

- [ ] 5.3 Escribir property test para simetría al eliminar
  - **Property 24: Simetría al eliminar amistad**
  - **Validates: Requirements 7.2**

- [ ] 5.4 Escribir property test para reenvío después de eliminar
  - **Property 25: Posibilidad de reenvío después de eliminar**
  - **Validates: Requirements 7.3**

- [ ] 6. Implementar mejoras de seguridad
  - Verificar que todos los formularios POST tienen protección CSRF
  - Sanitizar entrada de búsqueda para prevenir XSS
  - Agregar validación de autorización en todas las acciones
  - Verificar que las consultas SQL usan parámetros (ya usan Dapper)
  - _Requirements: 8.1, 8.2_

- [ ] 6.1 Escribir property test para validación de existencia al eliminar
  - **Property 26: Validación de existencia al eliminar**
  - **Validates: Requirements 7.4**

- [ ] 6.2 Escribir property test para redirección sin autenticación
  - **Property 27: Redirección sin autenticación**
  - **Validates: Requirements 8.1**

- [ ] 6.3 Escribir property test para rechazo sin sesión
  - **Property 28: Rechazo de operaciones sin sesión**
  - **Validates: Requirements 8.2**

- [ ] 6.4 Escribir property test para atomicidad
  - **Property 29: Atomicidad de operaciones**
  - **Validates: Requirements 8.3**

- [ ] 7. Checkpoint - Verificar que todo funciona correctamente
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas

- [ ] 7.1 Escribir tests de integración para flujo completo de amistad
  - Crear test que simula: buscar usuario, enviar solicitud, aceptar, verificar listas
  - _Requirements: 1.1, 2.1, 4.1, 6.1_

- [ ] 7.2 Escribir tests de integración para flujo de rechazo
  - Crear test que simula: enviar solicitud, rechazar, verificar que no aparece en pendientes
  - _Requirements: 2.1, 5.1, 5.3_

- [ ] 7.3 Escribir tests de integración para flujo de eliminación
  - Crear test que simula: amistad existente, eliminar, verificar listas, reenviar solicitud
  - _Requirements: 7.1, 7.2, 7.3_

- [ ] 8. Documentación y limpieza final
  - Agregar comentarios XML a métodos públicos de BD.cs
  - Documentar el flujo de estados de amistad
  - Crear diagrama de estados si es necesario
  - Limpiar código comentado o no utilizado
  - _Requirements: Todos_
