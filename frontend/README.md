# AstroReader - Web Client

Bienvenido al cliente web de AstroReader. Este proyecto es una Single Page Application (SPA) construida con tecnologías modernas y enfocada en proveer una experiencia de usuario premium, fluida y con un diseño estético atractivo (orientado a la astrología con tonos oscuros y glassmorphism).

## 🚀 Tecnologías

- **React** 18+
- **TypeScript** para seguridad de tipos
- **Vite** para construir y levantar el servidor de desarrollo rápido
- **Tailwind CSS v3** como motor de utilidades CSS
- **React Router v6** para el enrutamiento
- **Lucide React** para iconos minimalistas

## 📂 Arquitectura del Proyecto (Módulos por Funcionalidad)

Este proyecto evita la sobrearquitectura ("Clean Architecture" estricta) en favor de una arquitectura orientada a "*Features*". Esto significa que agrupamos el código por su dominio en la aplicación y no por su tipo.

```
src/
├── core/            # Servicios transversales (clientes HTTP), Tipos globales y Utils.
├── features/        # Módulos específicos. Aquí ocurre la mayoría del desarrollo.
│   ├── home/        # Ej. código específico de la landing
│   └── chart/       # Ej. lógica e UI de la calculadora natal
├── shared/          # Componentes tontos (UI) o layouts que cruzan toda la app.
├── App.tsx          # Router principal
└── main.tsx         # Punto de entrada de React
```

## 📝 Convenciones Generales

Para mantener el código ordenado y conciso:

1. **Nombres de Archivos**:
   - Componentes y Páginas: `PascalCase.tsx` (ej. `HomePage.tsx`, `HeroCTA.tsx`).
   - Hooks y Utilidades: `camelCase.ts` (ej. `useAstronomyData.ts`, `formatter.ts`).
   - Archivos de Tipos: `camelCase.types.ts` si son de un dominio específico (ej. `chart.types.ts`).

2. **Tipado**:
   - Usa `interface` para objetos de dominio y `type` para utilidades de TypeScript o uniones.
   - Evita el uso de `any` a toda costa.

3. **Routing**:
   - Definimos un esquema centralizado en `App.tsx` con React Router V6.

4. **Estética Front-End**:
   - Evitar colores estándar. Todo componente visual debe construirse utilizando los tokens del archivo `tailwind.config.js` (`bg-background`, `text-primary`, etc).
   - Evitar usar estado global (Redux/Zustand) hasta que sea enteramente justificable. Usa Contextos agrupados en `core/providers` solo si la prop-drilling excede 3 capas complejas.

## ⚙️ Cómo iniciar en Local

Asegúrate de tener Node.js instalado.

```bash
# 1. Instalar dependencias
npm install

# 2. Iniciar el servidor de desarrollo en local
npm run dev

# 3. Compilar el proyecto para producción
npm run build
```
