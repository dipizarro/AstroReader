import type { ChartInterpretation, ChartSummary } from '../types/chart.types';

interface ChartInterpretationSectionProps {
  interpretation: ChartInterpretation;
  summary: ChartSummary;
}

export const ChartInterpretationSection = ({
  interpretation,
  summary,
}: ChartInterpretationSectionProps) => {
  if (!interpretation.core?.sun && !interpretation.core?.moon && !interpretation.core?.ascendant) {
    return null;
  }

  return (
    <section className="max-w-4xl mx-auto space-y-8">
      <div className="text-center mb-10">
        <h3 className="text-xs uppercase tracking-[0.3em] text-primary mb-6">Interpretación Inicial</h3>
        {interpretation.headline && (
          <p className="text-2xl md:text-3xl font-display text-white/90 leading-tight">
            "{interpretation.headline}"
          </p>
        )}
        {interpretation.summary && (
          <p className="mx-auto mt-5 max-w-3xl text-sm leading-7 text-text-muted">
            {interpretation.summary}
          </p>
        )}
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-8 text-sm leading-relaxed text-text">
        {interpretation.core.sun && (
          <div className="glass-panel p-6 rounded-2xl border-t border-t-primary/30">
            <span className="block text-white font-medium mb-3">Tu Sol en {summary.sun}</span>
            <p className="opacity-80">{interpretation.core.sun}</p>
          </div>
        )}
        {interpretation.core.moon && (
          <div className="glass-panel p-6 rounded-2xl border-t border-t-slate-400/30">
            <span className="block text-white font-medium mb-3">Tu Luna en {summary.moon}</span>
            <p className="opacity-80">{interpretation.core.moon}</p>
          </div>
        )}
        {interpretation.core.ascendant && (
          <div className="glass-panel p-6 rounded-2xl border-t border-t-primary/30">
            <span className="block text-white font-medium mb-3">Tu Asc. en {summary.ascendant}</span>
            <p className="opacity-80">{interpretation.core.ascendant}</p>
          </div>
        )}
      </div>
    </section>
  );
};
