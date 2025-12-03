# Documento de Requerimientos - Sistema de Amigos

## Introducción

El sistema de amigos de OldMates permite a los usuarios conectarse entre sí mediante solicitudes de amistad, buscar otros usuarios por nombre, y gestionar sus relaciones sociales dentro de la plataforma. Este sistema facilita la interacción social y la creación de comunidades entre adultos mayores.

## Glosario

- **Sistema**: La aplicación web OldMates
- **Usuario**: Persona registrada en la plataforma OldMates
- **Solicitud de Amistad**: Petición enviada por un Usuario a otro Usuario para establecer una conexión de amistad
- **Amistad**: Relación bidireccional aceptada entre dos Usuarios
- **Estado de Amistad**: Condición actual de una relación entre dos Usuarios (pendiente, aceptada, rechazada, ninguna)
- **Lista de Amigos**: Colección de todos los Usuarios con los que un Usuario tiene una Amistad aceptada
- **Búsqueda de Usuarios**: Funcionalidad que permite localizar otros Usuarios mediante criterios de texto

## Requerimientos

### Requerimiento 1

**User Story:** Como usuario, quiero buscar otros usuarios por su nombre, apellido o username, para poder encontrar personas que conozco o con las que quiero conectar.

#### Criterios de Aceptación

1. WHEN un Usuario ingresa texto en el campo de búsqueda, THEN el Sistema SHALL mostrar todos los Usuarios cuyo nombre, apellido o username contengan el texto ingresado
2. WHEN un Usuario realiza una búsqueda, THEN el Sistema SHALL excluir al Usuario actual de los resultados
3. WHEN los resultados de búsqueda se muestran, THEN el Sistema SHALL indicar el Estado de Amistad actual con cada Usuario encontrado
4. WHEN un Usuario ingresa una búsqueda vacía, THEN el Sistema SHALL mostrar una lista vacía de resultados
5. WHEN los resultados incluyen Usuarios, THEN el Sistema SHALL mostrar el nombre completo, foto de perfil, localidad e intereses de cada Usuario

### Requerimiento 2

**User Story:** Como usuario, quiero enviar solicitudes de amistad a otros usuarios, para poder establecer conexiones sociales en la plataforma.

#### Criterios de Aceptación

1. WHEN un Usuario hace clic en enviar solicitud a otro Usuario, THEN el Sistema SHALL crear una Solicitud de Amistad con estado pendiente
2. WHEN un Usuario intenta enviar una Solicitud de Amistad a sí mismo, THEN el Sistema SHALL rechazar la operación
3. WHEN ya existe una Solicitud de Amistad pendiente entre dos Usuarios, THEN el Sistema SHALL prevenir el envío de una nueva solicitud
4. WHEN ya existe una Amistad aceptada entre dos Usuarios, THEN el Sistema SHALL prevenir el envío de una nueva solicitud
5. WHEN se crea una Solicitud de Amistad, THEN el Sistema SHALL registrar la fecha y hora de creación

### Requerimiento 3

**User Story:** Como usuario, quiero ver las solicitudes de amistad que he recibido, para poder decidir si acepto o rechazo cada conexión.

#### Criterios de Aceptación

1. WHEN un Usuario accede a su lista de amigos, THEN el Sistema SHALL mostrar todas las Solicitudes de Amistad pendientes dirigidas a ese Usuario
2. WHEN se muestra una Solicitud de Amistad pendiente, THEN el Sistema SHALL incluir el nombre, foto, localidad e intereses del Usuario solicitante
3. WHEN se muestra una Solicitud de Amistad pendiente, THEN el Sistema SHALL proporcionar opciones para aceptar o rechazar
4. WHEN un Usuario no tiene Solicitudes de Amistad pendientes, THEN el Sistema SHALL mostrar una lista vacía

### Requerimiento 4

**User Story:** Como usuario, quiero aceptar solicitudes de amistad, para confirmar conexiones con personas que deseo tener en mi red social.

#### Criterios de Aceptación

1. WHEN un Usuario acepta una Solicitud de Amistad pendiente, THEN el Sistema SHALL cambiar el Estado de Amistad a aceptada
2. WHEN una Solicitud de Amistad es aceptada, THEN el Sistema SHALL registrar la fecha y hora de respuesta
3. WHEN una Amistad es aceptada, THEN el Sistema SHALL mostrar ambos Usuarios en las respectivas Listas de Amigos
4. WHEN un Usuario intenta aceptar una solicitud que no le fue dirigida, THEN el Sistema SHALL rechazar la operación
5. WHEN un Usuario intenta aceptar una solicitud que no está en estado pendiente, THEN el Sistema SHALL rechazar la operación

### Requerimiento 5

**User Story:** Como usuario, quiero rechazar solicitudes de amistad, para mantener control sobre mis conexiones sociales.

#### Criterios de Aceptación

1. WHEN un Usuario rechaza una Solicitud de Amistad pendiente, THEN el Sistema SHALL cambiar el Estado de Amistad a rechazada
2. WHEN una Solicitud de Amistad es rechazada, THEN el Sistema SHALL registrar la fecha y hora de respuesta
3. WHEN una Solicitud de Amistad es rechazada, THEN el Sistema SHALL remover la solicitud de la lista de solicitudes pendientes
4. WHEN un Usuario intenta rechazar una solicitud que no le fue dirigida, THEN el Sistema SHALL rechazar la operación
5. WHEN un Usuario intenta rechazar una solicitud que no está en estado pendiente, THEN el Sistema SHALL rechazar la operación

### Requerimiento 6

**User Story:** Como usuario, quiero ver mi lista de amigos, para acceder rápidamente a las personas con las que tengo una conexión establecida.

#### Criterios de Aceptación

1. WHEN un Usuario accede a su Lista de Amigos, THEN el Sistema SHALL mostrar todos los Usuarios con los que tiene una Amistad aceptada
2. WHEN se muestra un amigo en la lista, THEN el Sistema SHALL incluir el nombre, foto, localidad e intereses del Usuario
3. WHEN se muestra un amigo en la lista, THEN el Sistema SHALL proporcionar una opción para eliminar la Amistad
4. WHEN un Usuario no tiene amigos, THEN el Sistema SHALL mostrar una lista vacía

### Requerimiento 7

**User Story:** Como usuario, quiero eliminar amigos de mi lista, para gestionar mis conexiones sociales cuando una relación ya no es relevante.

#### Criterios de Aceptación

1. WHEN un Usuario elimina una Amistad, THEN el Sistema SHALL remover la relación de la base de datos
2. WHEN una Amistad es eliminada, THEN el Sistema SHALL remover ambos Usuarios de sus respectivas Listas de Amigos
3. WHEN una Amistad es eliminada, THEN el Sistema SHALL permitir que cualquiera de los dos Usuarios envíe una nueva Solicitud de Amistad
4. WHEN un Usuario intenta eliminar una Amistad que no existe, THEN el Sistema SHALL rechazar la operación

### Requerimiento 8

**User Story:** Como usuario, quiero que el sistema valide mis acciones de amistad, para prevenir errores y mantener la integridad de mis conexiones sociales.

#### Criterios de Aceptación

1. WHEN un Usuario no está autenticado, THEN el Sistema SHALL redirigir al Usuario a la página de inicio de sesión
2. WHEN un Usuario intenta realizar cualquier acción de amistad sin sesión activa, THEN el Sistema SHALL rechazar la operación
3. WHEN se produce un error en una operación de amistad, THEN el Sistema SHALL mantener el estado anterior sin cambios
4. WHEN un Usuario realiza una acción de amistad exitosa, THEN el Sistema SHALL actualizar la interfaz para reflejar el nuevo estado
