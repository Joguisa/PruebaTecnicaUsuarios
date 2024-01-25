# Prueba Técnica
Proyecto para realizar un CRUD a la tabla de usuarios la cual esta relacionada con una tabla de perfil y cargo.

## Instrucciones de Clonación y Ejecución
Siga estos pasos para clonar y ejecutar el proyecto en su máquina local.

## Prerrequisitos
Asegúrese de tener instalados en su sistema:

- .NET SDK (v6.0.0)

## Pasos
1. Clonar el Repositorio:
```
git clone https://github.com/joguisa/PruebaTecnicaUsuarios.git
```
2. Crear la base de datos y los respecticos procedimientos que están separados por archivos.

3. **El script se encuentra en la solución**
   - Localiza el archivo de configuración appsettings.json en el proyecto. Actualiza la cadena de conexión en la sección ConnectionStrings según tu entorno y preferencias.
  
5. Migrar la Base de Datos:

En el directorio del proyecto, ejecute el siguiente comando para aplicar las migraciones y crear la base de datos:
```
dotnet ef database update
```
4. Ejecutar la aplicación

## Dependencias
### Paquetes NuGet
- Microsoft.EntityFrameworkCore (v6.0.0)
- Microsoft.EntityFrameworkCore.Design (v6.0.0)
- Microsoft.EntityFrameworkCore.SqlServer (v6.0.0)
- Microsoft.EntityFrameworkCore.Tools (v6.0.0)
- Microsoft.VisualStudio.Web.CodeGeneration.Design (v6.0.0)

## Licencia
Este proyecto está bajo la Licencia MIT. Consulta el archivo LICENSE para obtener más detalles.
