import type { CalculateChartResponse } from '../types/chart.types';
import { Sparkles, Moon, Sun, CircleDot, ArrowUpCircle } from 'lucide-react';

interface ChartResultProps {
  data: CalculateChartResponse;
}

export const ChartResult = ({ data }: ChartResultProps) => {
  const { summary, planets, houses, interpretation } = data;

  const renderIcon = (name: string) => {
    switch (name.toLowerCase()) {
      case 'sun': return <Sun className="w-6 h-6 text-yellow-500" />;
      case 'moon': return <Moon className="w-6 h-6 text-slate-300" />;
      case 'ascendant': return <ArrowUpCircle className="w-6 h-6 text-primary" />;
      default: return <CircleDot className="w-5 h-5 text-primary" />;
    }
  };

  return (
    <div className="w-full animate-in fade-in slide-in-from-bottom-4 duration-700 mt-8 space-y-8">
      
      {/* 1. Header / Big 3 Summary */}
      <div className="glass-panel p-8 rounded-3xl shadow-2xl relative overflow-hidden">
        <div className="absolute -top-10 -right-10 w-60 h-60 bg-primary/10 blur-[60px] rounded-full pointer-events-none" />
        
        <div className="flex items-center gap-3 mb-8 border-b border-white/10 pb-6">
          <Sparkles className="w-6 h-6 text-primary" />
          <h2 className="text-2xl font-display font-medium text-white tracking-wide">
            Tus Tres Pilares (Big 3)
          </h2>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div className="flex flex-col items-center bg-surfaceHighlight/50 p-6 rounded-2xl border border-white/5 hover:border-primary/20 transition-all text-center">
            <div className="mb-3 p-3 bg-white/5 rounded-full">{renderIcon('sun')}</div>
            <h4 className="text-white font-medium mb-1">Sol</h4>
            <span className="text-primary font-bold tracking-wide uppercase text-sm">{summary.sunSign}</span>
          </div>
          <div className="flex flex-col items-center bg-surfaceHighlight/50 p-6 rounded-2xl border border-white/5 hover:border-primary/20 transition-all text-center">
            <div className="mb-3 p-3 bg-white/5 rounded-full">{renderIcon('moon')}</div>
            <h4 className="text-white font-medium mb-1">Luna</h4>
            <span className="text-primary font-bold tracking-wide uppercase text-sm">{summary.moonSign}</span>
          </div>
          <div className="flex flex-col items-center bg-surfaceHighlight/50 p-6 rounded-2xl border border-white/5 hover:border-primary/20 transition-all text-center">
             <div className="mb-3 p-3 bg-white/5 rounded-full">{renderIcon('ascendant')}</div>
            <h4 className="text-white font-medium mb-1">Ascendente</h4>
            <span className="text-primary font-bold tracking-wide uppercase text-sm">{summary.ascendantSign}</span>
          </div>
        </div>
      </div>

      {/* 2. Interpretation Text */}
      {interpretation && (
        <div className="glass-panel p-8 rounded-3xl shadow-xl border-l-[3px] border-l-primary relative">
           <h3 className="text-xl font-display text-white mb-6">Interpretación Inicial</h3>
           <div className="space-y-6 text-text-muted leading-relaxed">
             <p><strong className="text-white/90 block mb-1">El Sol en {summary.sunSign}:</strong> {interpretation.sunSignInterpretation}</p>
             <p><strong className="text-white/90 block mb-1">La Luna en {summary.moonSign}:</strong> {interpretation.moonSignInterpretation}</p>
             <p><strong className="text-white/90 block mb-1">El Ascendente en {summary.ascendantSign}:</strong> {interpretation.ascendantInterpretation}</p>
           </div>
        </div>
      )}

      {/* 3. Detailed Data (Planets & Houses) */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        
        {/* Planets List */}
        <div className="glass-panel p-6 rounded-3xl">
          <h3 className="text-sm uppercase tracking-[0.2em] text-text-muted mb-6 font-semibold border-b border-white/10 pb-4">
            Posiciones Planetarias
          </h3>
          <ul className="space-y-3">
            {planets.map((pt, idx) => (
              <li key={idx} className="flex items-center justify-between p-3 rounded-xl bg-surface/30 hover:bg-surface/60 transition-colors">
                <div className="flex items-center gap-3">
                  <div className="text-primary/70">{renderIcon(pt.name)}</div>
                  <div>
                    <span className="font-medium text-white text-sm block">{pt.name}</span>
                    <span className="text-xs text-text-muted capitalize">{pt.sign}</span>
                  </div>
                </div>
                <span className="text-sm font-mono text-primary/80">{pt.degree.toFixed(2)}°</span>
              </li>
            ))}
          </ul>
        </div>

        {/* Houses List */}
        <div className="glass-panel p-6 rounded-3xl">
          <h3 className="text-sm uppercase tracking-[0.2em] text-text-muted mb-6 font-semibold border-b border-white/10 pb-4">
            Casas Astrológicas
          </h3>
          <div className="grid grid-cols-2 gap-3">
            {houses.map((house, idx) => (
              <div key={idx} className="flex items-center justify-between p-3 rounded-xl bg-surface/30">
                <span className="text-sm text-text-muted font-medium">Casa {house.houseNumber}</span>
                <div className="text-right">
                  <span className="block text-sm text-white capitalize">{house.sign}</span>
                  <span className="text-xs font-mono text-primary/60">{house.degree.toFixed(2)}°</span>
                </div>
              </div>
            ))}
          </div>
        </div>

      </div>

    </div>
  );
};
