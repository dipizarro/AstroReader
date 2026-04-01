/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        background: '#0a0a0b', // Muy oscuro
        surface: '#121216',    // Un pelín más claro
        surfaceHighlight: '#1a1a20',
        primary: {
          DEFAULT: '#d4af37',   // Dorado tenue
          hover: '#e5c158',
          dark: '#b39126',
        },
        secondary: {
          DEFAULT: '#6b4c9a',   // Morado astrológico, oscuro
          hover: '#8265a7',
        },
        text: {
          DEFAULT: '#e5e7eb',   // Gris claro para legibilidad
          muted: '#9ca3af',
        }
      },
      fontFamily: {
        sans: ['Inter', 'sans-serif'],
        display: ['Playfair Display', 'serif'],
      },
      backgroundImage: {
        'gradient-radial': 'radial-gradient(var(--tw-gradient-stops))',
      }
    },
  },
  plugins: [],
}
