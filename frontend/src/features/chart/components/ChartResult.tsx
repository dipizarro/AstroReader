import { useEffect, useMemo, useState } from 'react';
import type { CalculateChartRequest, CalculateChartResponse } from '../types/chart.types';
import { chartService } from '../services/chartService';
import { Sparkles, Moon, Sun, ArrowUpCircle, Bookmark, CheckCircle2, Loader2, AlertCircle } from 'lucide-react';

interface ChartResultProps {
  data: CalculateChartResponse;
  request: CalculateChartRequest | null;
  showSavePanel?: boolean;
}

type SaveStatus = 'idle' | 'loading' | 'success' | 'error';

export const ChartResult = ({ data, request, showSavePanel = true }: ChartResultProps) => {
  const { metadata, summary, planets = [], houses = [], interpretation } = data || {};
  const [chartLabel, setChartLabel] = useState('');
  const [saveStatus, setSaveStatus] = useState<SaveStatus>('idle');
  const [feedbackMessage, setFeedbackMessage] = useState<string | null>(null);
  const [savedChartId, setSavedChartId] = useState<string | null>(null);

  const defaultChartLabel = useMemo(() => {
    const placeName = request?.placeName || metadata?.placeName;
    if (placeName) {
      return `Carta natal · ${placeName}`;
    }

    if (summary?.sun && summary?.moon && summary?.ascendant) {
      return `Carta ${summary.sun} · ${summary.moon} · ${summary.ascendant}`;
    }

    return 'Mi carta natal';
  }, [metadata?.placeName, request?.placeName, summary?.ascendant, summary?.moon, summary?.sun]);

  useEffect(() => {
    setChartLabel(defaultChartLabel);
    setSaveStatus('idle');
    setFeedbackMessage(null);
    setSavedChartId(null);
  }, [defaultChartLabel, data, request]);

  const renderIcon = (name: string, className = "w-6 h-6") => {
    switch (name.toLowerCase()) {
      case 'sun': return <Sun className={`${className} text-[#D4AF37]`} />;
      case 'moon': return <Moon className={`${className} text-slate-300`} />;
      case 'ascendant': return <ArrowUpCircle className={`${className} text-[#D4AF37]`} />;
      default: return <div className={`w-2 h-2 rounded-full bg-primary/40`} />;
    }
  };

  const handleSaveChart = async () => {
    const trimmedLabel = chartLabel.trim();

    if (!request) {
      setSaveStatus('error');
      setFeedbackMessage('No encontramos los datos natales originales para guardar esta carta.');
      return;
    }

    if (!trimmedLabel) {
      setSaveStatus('error');
      setFeedbackMessage('Asigna un nombre breve a tu carta antes de guardarla.');
      return;
    }

    setSaveStatus('loading');
    setFeedbackMessage(null);

    try {
      const savedChart = await chartService.saveChart({
        profileName: trimmedLabel,
        placeName: request.placeName,
        birthDate: request.birthDate,
        birthTime: request.birthTime,
        latitude: request.latitude,
        longitude: request.longitude,
        timezoneOffsetMinutes: request.timezoneOffsetMinutes,
      });

      setSaveStatus('success');
      setSavedChartId(savedChart.id);
      setFeedbackMessage(`Carta guardada correctamente como "${savedChart.profileName}".`);
    } catch (error) {
      setSaveStatus('error');
      setFeedbackMessage(
        error instanceof Error
          ? error.message
          : 'No pudimos guardar la carta en este momento.'
      );
    }
  };

  const statusToneClassName = saveStatus === 'success'
    ? 'border-emerald-500/20 bg-emerald-500/10 text-emerald-200'
    : saveStatus === 'error'
      ? 'border-red-500/20 bg-red-500/10 text-red-200'
      : 'border-white/10 bg-white/[0.03] text-text-muted';

  const buttonClassName = saveStatus === 'success'
    ? 'save-success-glow bg-gradient-to-r from-emerald-300 via-emerald-200 to-emerald-300 text-[#08110d] shadow-[0_0_28px_rgba(52,211,153,0.28)]'
    : 'bg-primary text-[#0a0a0b] shadow-[0_0_18px_rgba(212,175,55,0.28)] hover:bg-primary-hover hover:shadow-[0_0_26px_rgba(212,175,55,0.4)]';

  // Si no hay datos mínimos para renderizar siquiera el header
  if (!summary) return null;

  return (
    <div className="w-full animate-in fade-in slide-in-from-bottom-4 duration-1000 mt-12 space-y-12 pb-20">

      {showSavePanel && (
        <section className="relative overflow-hidden rounded-3xl border border-white/10 bg-gradient-to-br from-[#131318] via-[#101014] to-[#0a0a0c] p-6 shadow-[0_0_40px_rgba(0,0,0,0.45)] md:p-8">
          <div className="absolute inset-x-0 top-0 h-px bg-gradient-to-r from-transparent via-primary/40 to-transparent" />
          <div className="absolute -right-20 top-8 h-40 w-40 rounded-full bg-primary/10 blur-3xl" />

          <div className="relative z-10 flex flex-col gap-6 lg:flex-row lg:items-end lg:justify-between">
            <div className="max-w-xl space-y-3">
              <div className="inline-flex items-center gap-2 rounded-full border border-primary/15 bg-primary/10 px-3 py-1 text-[11px] font-semibold uppercase tracking-[0.28em] text-primary">
                <Bookmark className="h-3.5 w-3.5" />
                Guardar carta
              </div>
              <div className="space-y-2">
                <h3 className="text-2xl font-display font-medium text-white md:text-3xl">
                  Conserva esta lectura para volver a ella cuando quieras
                </h3>
                <p className="text-sm leading-6 text-text-muted">
                  Asigna un nombre claro a esta carta y guárdala en tu colección sin salir de la lectura actual.
                </p>
              </div>
            </div>

            <div className="w-full max-w-xl space-y-4">
              <div className="space-y-2">
                <label htmlFor="chartLabel" className="text-sm font-medium text-text">
                  Nombre o etiqueta
                </label>
                <input
                  id="chartLabel"
                  type="text"
                  value={chartLabel}
                  maxLength={120}
                  disabled={saveStatus === 'loading'}
                  onChange={(event) => {
                    setChartLabel(event.target.value);
                    if (saveStatus !== 'idle') {
                      setSaveStatus('idle');
                      setFeedbackMessage(null);
                    }
                  }}
                  placeholder="Ej. Carta natal de Antonia"
                  className="w-full rounded-2xl border border-white/10 bg-white/[0.04] px-4 py-3 text-sm text-white outline-none transition focus:border-primary/50 focus:ring-1 focus:ring-primary/40"
                />
              </div>

              <div className="flex flex-col gap-3 sm:flex-row sm:items-center">
                <button
                  type="button"
                  onClick={handleSaveChart}
                  disabled={saveStatus === 'loading'}
                  className={`relative inline-flex min-w-[180px] items-center justify-center gap-2 overflow-hidden rounded-2xl px-5 py-3 text-sm font-semibold transition-all disabled:cursor-not-allowed disabled:opacity-60 ${buttonClassName}`}
                >
                  {saveStatus === 'success' && (
                    <span className="save-success-sheen pointer-events-none absolute inset-0" aria-hidden="true" />
                  )}
                  {saveStatus === 'loading' ? (
                    <>
                      <Loader2 className="h-4 w-4 animate-spin" />
                      Guardando...
                    </>
                  ) : saveStatus === 'success' ? (
                    <>
                      <CheckCircle2 className="save-success-badge h-4 w-4" />
                      Carta guardada
                    </>
                  ) : (
                    <>
                      <Bookmark className="h-4 w-4" />
                      Guardar carta
                    </>
                  )}
                </button>

                <div className={`relative inline-flex min-h-12 items-center gap-2 overflow-hidden rounded-2xl border px-4 py-3 text-sm ${statusToneClassName} ${saveStatus === 'success' ? 'save-success-panel' : ''}`}>
                  {saveStatus === 'success' && (
                    <span className="save-success-sheen pointer-events-none absolute inset-0 opacity-60" aria-hidden="true" />
                  )}
                  {saveStatus === 'success' && <CheckCircle2 className="h-4 w-4 shrink-0" />}
                  {saveStatus === 'error' && <AlertCircle className="h-4 w-4 shrink-0" />}
                  {saveStatus === 'loading' && <Loader2 className="h-4 w-4 shrink-0 animate-spin" />}
                  <span className="relative z-10">
                    {feedbackMessage || 'Podrás renombrarla más adelante cuando exista la biblioteca de cartas.'}
                  </span>
                </div>
              </div>

              {savedChartId && (
                <p className="text-xs tracking-wide text-text-muted/80">
                  ID de guardado: <span className="font-mono text-text-muted">{savedChartId}</span>
                </p>
              )}
            </div>
          </div>
        </section>
      )}
      
      {/* --- HEADER --- */}
      <div className="text-center space-y-3">
        <div className="inline-flex items-center justify-center gap-2 px-3 py-1 bg-primary/10 border border-primary/20 rounded-full mb-2">
          <Sparkles className="w-4 h-4 text-primary" />
          <span className="text-xs font-semibold tracking-widest text-primary uppercase">Calculado Exitosamente</span>
        </div>
        <h2 className="text-4xl md:text-5xl font-display font-medium text-white tracking-wide">
          AstroReader
        </h2>
        <p className="text-text-muted font-light tracking-wide">
          Tu Mapa Cósmico Personal
        </p>
      </div>

      {/* --- BIG 3 --- */}
      <div className="relative rounded-3xl overflow-hidden shadow-[0_0_40px_rgba(0,0,0,0.5)] border border-white/10 bg-gradient-to-b from-[#121216] to-[#0a0a0b]">
        {/* Glow de fondo tenue */}
        <div className="absolute top-0 inset-x-0 h-px bg-gradient-to-r from-transparent via-primary/30 to-transparent" />
        <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[80%] h-[80%] bg-primary/5 blur-[100px] pointer-events-none rounded-full" />
        
        <div className="grid grid-cols-1 md:grid-cols-3 relative z-10">
          {/* Sol */}
          <div className="flex flex-col items-center p-10 relative">
            <div className="absolute bottom-0 inset-x-8 h-px bg-white/5 md:hidden" />
            <div className="mb-4 drop-shadow-[0_0_15px_rgba(212,175,55,0.4)]">
              {renderIcon('sun', 'w-10 h-10')}
            </div>
            <h4 className="text-text-muted text-xs uppercase tracking-[0.2em] mb-2">Sol</h4>
            <span className="text-white font-display text-2xl font-medium tracking-wide">{summary.sun || 'N/A'}</span>
          </div>

          {/* Luna */}
          <div className="flex flex-col items-center p-10 md:border-x md:border-white/5 relative">
            <div className="absolute bottom-0 inset-x-8 h-px bg-white/5 md:hidden" />
            <div className="mb-4 drop-shadow-[0_0_15px_rgba(203,213,225,0.4)]">
              {renderIcon('moon', 'w-10 h-10')}
            </div>
            <h4 className="text-text-muted text-xs uppercase tracking-[0.2em] mb-2">Luna</h4>
            <span className="text-white font-display text-2xl font-medium tracking-wide">{summary.moon || 'N/A'}</span>
          </div>

          {/* Ascendente */}
          <div className="flex flex-col items-center p-10">
            <div className="mb-4 drop-shadow-[0_0_15px_rgba(212,175,55,0.4)]">
              {renderIcon('ascendant', 'w-10 h-10')}
            </div>
            <h4 className="text-text-muted text-xs uppercase tracking-[0.2em] mb-2">Ascendente</h4>
            <span className="text-white font-display text-2xl font-medium tracking-wide">{summary.ascendant || 'N/A'}</span>
          </div>
        </div>
      </div>

      {/* --- INTERPRETATION --- */}
      {interpretation && (interpretation.core?.sun || interpretation.core?.moon || interpretation.core?.ascendant) && (
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
            {interpretation.core?.sun && (
              <div className="glass-panel p-6 rounded-2xl border-t border-t-primary/30">
                <span className="block text-white font-medium mb-3">Tu Sol en {summary.sun}</span>
                <p className="opacity-80">{interpretation.core.sun}</p>
              </div>
            )}
            {interpretation.core?.moon && (
              <div className="glass-panel p-6 rounded-2xl border-t border-t-slate-400/30">
                <span className="block text-white font-medium mb-3">Tu Luna en {summary.moon}</span>
                <p className="opacity-80">{interpretation.core.moon}</p>
              </div>
            )}
            {interpretation.core?.ascendant && (
              <div className="glass-panel p-6 rounded-2xl border-t border-t-primary/30">
                <span className="block text-white font-medium mb-3">Tu Asc. en {summary.ascendant}</span>
                <p className="opacity-80">{interpretation.core.ascendant}</p>
              </div>
            )}
          </div>
        </section>
      )}

      {/* --- DETAILED DATA (PLANETS & HOUSES) --- */}
      {(planets.length > 0 || houses.length > 0) && (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-12 max-w-5xl mx-auto pt-8 border-t border-white/5">
          
          {/* Planets List */}
          {planets.length > 0 && (
            <div>
              <div className="flex items-center gap-3 mb-6">
                <div className="w-1 h-4 bg-primary rounded-full" />
                <h3 className="text-lg font-display text-white tracking-wide">Posiciones Planetarias</h3>
              </div>
              
              <div className="flex flex-col">
                {planets.map((pt, idx) => (
                  <div 
                    key={idx} 
                    className={`flex items-center justify-between py-4 ${idx !== planets.length - 1 ? 'border-b border-white/5' : ''} hover:bg-white/[0.02] transition-colors rounded-lg px-2 -mx-2`}
                  >
                    <div className="flex items-center gap-4">
                      <div className="w-6 h-6 flex items-center justify-center opacity-80">
                        {renderIcon(pt.name, 'w-5 h-5')}
                      </div>
                      <div>
                        <span className="text-white text-sm font-medium">{pt.name}</span>
                      </div>
                    </div>
                    <div className="text-right">
                      <span className="text-text-muted text-sm capitalize block">{pt.sign}</span>
                      <span className="text-primary/60 font-mono text-xs">{pt.signDegree?.toFixed(2)}°</span>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* Houses List */}
          {houses.length > 0 && (
            <div>
              <div className="flex items-center gap-3 mb-6">
                <div className="w-1 h-4 bg-slate-500 rounded-full" />
                <h3 className="text-lg font-display text-white tracking-wide">Las Doce Casas</h3>
              </div>
              
              <div className="grid grid-cols-2 gap-4">
                {houses.map((house, idx) => (
                  <div 
                    key={idx} 
                    className="flex items-center justify-between p-3 rounded-xl border border-white/5 bg-[#121216]/50 hover:border-white/10 transition-colors"
                  >
                    <span className="text-xs text-text-muted uppercase tracking-wider font-semibold">
                      <span className="opacity-50 mr-1">H</span>
                      {house.number}
                    </span>
                    <span className="text-sm font-medium text-white capitalize">{house.sign}</span>
                  </div>
                ))}
              </div>
            </div>
          )}

        </div>
      )}

    </div>
  );
};
