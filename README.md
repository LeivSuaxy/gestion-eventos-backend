# E-Event Horizon Backend Server

## Desarrollado en ASP.NET (C-SHARP)

## Descripcion
E-Event Horizon es una aplicacion web con el proposito de gestionar eventos y administrar
la asistencia de los mismos.

Este repositorio esta construido en ASP.NET (C#) y con fines educativos. De codigo libre bajo
la licencia MIT.

_**Hecho por y para estudiantes**_

Forma parte de la evaluacion de la materia de Programacion Web en la Universidad de Ciencias
Informaticas (La Habana).

## Requisitos
- .NET 8.0
- PostgreSQL 16
- Sistema Opeativo (Linux / Windows)
- RAM Indeterminada (Pronto se especificara) (Probado con 16 GB RAM)
- Procesador Indeterminado (Pronto se especificara) (Probado con Ryzen 5 5500U)

## Instalacion
- Clonar el repositorio
```bash
  git clone https://github.com/LeivSuaxy/gestion-eventos-backend.git
```

- Instalar las dependencias
```bash
  dotnet restore
```

- Crear la base de datos (Posible configuracion previa)
```bash
  dotnet ef database update
```
## Para desarrolladores

### Estructura del proyecto

```
  ├── Properties -> Propiedades del proyecto
  │   └── launcheSettings.json -> Opciones de lanzamiento
  │ 
  ├── wwwroot -> Archivos estaticos 
  │   └── images -> Imagenes estaticas
  │ 
  ├── Migrations -> Migraciones de la base de datos
  │   └──  ...
  │   
  ├── src -> Codigo fuente
  │   ├── Common -> Codigo comun del proyecto (Abstracciones)
  │   ├── Core -> Nucleo del proyecto (Funciones generales del proyecto)
  │   └── Modules -> Modulos del proyecto (Asignacion de responsabilidades)
  │       ├── Authentication
  │       ├── Category
  │       ├── Events
  │       └── Users
  │
  ├── Templates -> Plantillas HTML para complementar servicios (Mensajeria)
  ├── Tests -> Tests integrados del proyecto
  ├── appsettings.json -> Configuracion del proyecto
  ├── appsettings.Development.json -> Configuracion de desarrollo del proyecto
  ├── Builder.cs -> Clase para la construccion del proyecto (Alter Name Startup.cs)
  └── Program.cs -> Clase principal del proyecto
```

### Modulos
#### Estructura de un modulo
```
└── ModuleName
    ├── Controllers -> Endpoints de la API
    ├── DTO -> Estructuras que definen los datos de entrada y salida del modulo
    ├── Mappers -> Mapeadores de objetos (Serializers)
    ├── Models -> Modelos que trabajara el modulo
    ├── Services -> Servicios del modulo 
    └── Tests -> Tests unitarios del modulo
```