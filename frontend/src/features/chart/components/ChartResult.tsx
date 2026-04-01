import type { CalculateChartResponse } from '../types/chart.types';
import { Sparkles, Moon, Sun, ArrowRight, CircleDot } from 'lucide-react';

interface ChartResultProps {
  data: CalculateChartResponse;
}

export const ChartResult = ({ data }: ChartResultProps) => {
  // Helpers para hacer los mocks vistosos
  const renderIcon = (pointName: string) => {
    switch (pointName.toLowerCase()) {
      case 'sun': return <Sun className="w-5 h-5 text-yellow-400" />;
      case 'moon': return <Moon className="w-5 h-5 text-gray-200 cursor-none" />;
      default: return <CircleDot className="w-5 h-5 text-primary" />;
    }
  };

  return (
    <div className="w-full animate-in fade-in slide-in-from-bottom-4 duration-700 mt-8">
      <div className="glass-panel p-8 rounded-3xl shadow-2xl relative overflow-hidden">
        {/* Decorative corner glow */}
        <div className="absolute -top-10 -right-10 w-40 h-40 bg-primary/10 blur-[50px] rounded-full pointer-events-none" />

        <div className="flex items-center gap-3 mb-8 border-b border-white/10 pb-6">
          <Sparkles className="w-6 h-6 text-primary" />
          <h2 className="text-2xl font-display font-medium text-white tracking-wide">
            Interpretación Celestial
          </h2>
        </div>

        <div className="grid md:grid-cols-2 gap-8">
          {/* Posiciones Planetarias */}
          <div>
            <h3 className="text-sm uppercase tracking-[0.2em] text-text-muted mb-4 font-semibold">
              Posiciones Planetarias
            </h3>
            <ul className="space-y-4">
              {data.points.map((pt, idx) => (
                <li key={idx} className="flex items-center justify-between p-3 rounded-xl bg-surface/50 border border-white/5 hover:border-primary/20 transition-colors">
                  <div className="flex items-center gap-3">
                    <div className="p-2 bg-surfaceHighlight rounded-lg text-primary">
                      {renderIcon(pt.pointName)}
                    </div>
                    <div>
                      <p className="font-medium text-white text-sm">{pt.pointName}</p>
                      <p className="text-xs text-text-muted capitalize">{pt.zodiacSign}</p>
                    </div>
                  </div>
                  <div className="text-right">
                    <span className="text-sm font-mono text-primary/80">{pt.degrees}°</span>
                  </div>
                </li>
              ))}
            </ul>
          </div>

          {/* Aspectos */}
          <div>
            <h3 className="text-sm uppercase tracking-[0.2em] text-text-muted mb-4 font-semibold">
              Aspectos Mayores
            </h3>
            <div className="space-y-4">
              {data.aspects.map((asp, idx) => (
                <div key={idx} className="p-4 rounded-xl glass-panel relative overflow-hidden group">
                  <div className="absolute inset-0 bg-gradient-to-r from-primary/0 via-primary/5 to-primary/0 translate-x-[-100%] group-hover:translate-x-[100%] transition-transform duration-1000" />
                  <div className="flex items-center justify-between mb-2 z-10 relative">
                    <span className="text-sm font-medium text-white">{asp.point1}</span>
                    <ArrowRight className="w-4 h-4 text-text-muted/50" />
                    <span className="text-sm font-medium text-white">{asp.point2}</span>
                  </div>
                  <div className="flex items-center justify-between mt-2 z-10 relative">
                    <span className="text-xs px-2 py-1 rounded-md bg-secondary/20 text-secondary border border-secondary/20">
                      {asp.aspectType}
                    </span>
                    <span className="text-xs text-text-muted font-mono tracking-tight">
                      Orbe: {asp.orb}°
                    </span>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>

        <div className="mt-8 pt-6 border-t border-white/10 text-center">
          <p className="text-sm text-text-muted max-w-lg mx-auto leading-relaxed">
            Esta es una vista preliminar de tus posiciones. AstroReader cruzará estos datos
            para brindarte un análisis en profundidad sobre tus ciclos actuales y tu propósito.
          </p>
        </div>
      </div>
    </div>
  );
};
