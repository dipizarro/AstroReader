import { Link, Outlet } from 'react-router-dom';
import { Moon, Menu } from 'lucide-react';

export const MainLayout = () => {
  return (
    <div className="flex min-h-screen flex-col bg-background">
      {/* Navbar with subtle glassmorphism */}
      <header className="sticky top-0 z-50 w-full border-b border-white/5 bg-background/80 backdrop-blur-md">
        <div className="container mx-auto flex h-16 items-center justify-between px-4 sm:px-6 lg:px-8">
          <Link to="/" className="flex items-center gap-2 transition-opacity hover:opacity-80">
            <Moon className="h-6 w-6 text-primary" strokeWidth={1.5} />
            <span className="font-display text-xl font-semibold tracking-wide text-white">
              AstroReader
            </span>
          </Link>
          
          <nav className="hidden items-center gap-8 md:flex">
            <Link to="/" className="text-sm font-medium text-text-muted transition-colors hover:text-primary">
              Inicio
            </Link>
            <Link to="/chart/calculate" className="text-sm font-medium text-text-muted transition-colors hover:text-primary">
              Carta Natal
            </Link>
          </nav>

          <div className="flex items-center gap-4">
            <Link 
              to="/chart/calculate"
              className="hidden rounded-full border border-primary/30 bg-primary/10 px-5 py-2 text-sm font-medium text-primary shadow-[0_0_15px_rgba(212,175,55,0.15)] transition-all hover:bg-primary/20 hover:shadow-[0_0_20px_rgba(212,175,55,0.3)] md:inline-flex"
            >
              Comenzar
            </Link>
            <button className="inline-flex items-center justify-center rounded-md p-2 text-text-muted hover:bg-white/5 md:hidden">
              <Menu className="h-5 w-5" />
            </button>
          </div>
        </div>
      </header>

      <main className="flex-1">
        {/* Render child pages */}
        <Outlet />
      </main>

      <footer className="border-t border-white/5 bg-surfaceHighlight py-8">
        <div className="container mx-auto px-4 text-center text-sm text-text-muted">
          &copy; {new Date().getFullYear()} AstroReader. Todos los derechos reservados.
        </div>
      </footer>
    </div>
  );
};
