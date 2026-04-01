import { MapPin, Calendar, Clock } from 'lucide-react';

export const CalculateChartPage = () => {
  return (
    <div className="flex flex-col items-center justify-center p-6 lg:p-12">
      <div className="mb-12 text-center">
        <h1 className="mb-4 font-display text-3xl font-semibold tracking-tight text-glow sm:text-5xl">
          Tu configuración astral
        </h1>
        <p className="max-w-2xl text-text-muted">
          Ingresa tus datos exactos de nacimiento para calcular el mapa del cielo en el instante en que llegaste al mundo.
        </p>
      </div>

      <div className="w-full max-w-lg glass-panel rounded-3xl p-8 shadow-2xl">
        <form className="flex flex-col gap-6" onSubmit={(e) => e.preventDefault()}>
          
          <div className="flex flex-col gap-2">
            <label htmlFor="name" className="text-sm font-medium text-text">Nombre</label>
            <input 
              type="text" 
              id="name"
              placeholder="¿Cómo te llamas?" 
              className="w-full rounded-lg border border-white/10 bg-surface px-4 py-3 text-white placeholder-text-muted/50 focus:border-primary/50 focus:outline-none focus:ring-1 focus:ring-primary/50"
            />
          </div>

          <div className="flex flex-col gap-2 relative">
            <label htmlFor="date" className="text-sm font-medium text-text">Fecha de nacimiento</label>
            <div className="relative">
              <Calendar className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-text-muted" />
              <input 
                type="date" 
                id="date"
                className="w-full rounded-lg border border-white/10 bg-surface pl-10 pr-4 py-3 text-white focus:border-primary/50 focus:outline-none focus:ring-1 focus:ring-primary/50 [color-scheme:dark]"
              />
            </div>
          </div>

          <div className="flex flex-col gap-2">
            <label htmlFor="time" className="text-sm font-medium text-text">Hora de nacimiento</label>
            <div className="relative">
              <Clock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-text-muted" />
              <input 
                type="time" 
                id="time"
                className="w-full rounded-lg border border-white/10 bg-surface pl-10 pr-4 py-3 text-white focus:border-primary/50 focus:outline-none focus:ring-1 focus:ring-primary/50 [color-scheme:dark]"
              />
            </div>
          </div>

          <div className="flex flex-col gap-2">
            <label htmlFor="location" className="text-sm font-medium text-text">Lugar de nacimiento</label>
             <div className="relative">
              <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-text-muted" />
              <input 
                type="text" 
                id="location"
                placeholder="Ciudad, País"
                className="w-full rounded-lg border border-white/10 bg-surface pl-10 pr-4 py-3 text-white placeholder-text-muted/50 focus:border-primary/50 focus:outline-none focus:ring-1 focus:ring-primary/50"
              />
            </div>
          </div>

          <button 
            type="submit" 
            className="mt-4 flex w-full items-center justify-center rounded-xl bg-primary py-3.5 text-sm font-semibold text-[#0a0a0b] shadow-[0_0_15px_rgba(212,175,55,0.3)] transition-all hover:bg-primary-hover hover:shadow-[0_0_25px_rgba(212,175,55,0.5)]"
          >
            Descubrir mi Carta
          </button>

        </form>
      </div>
    </div>
  );
};
