import { useEffect, useMemo, useState } from 'react';
import type { CalculateChartRequest, CalculateChartResponse } from '../types/chart.types';
import { Moon, Sparkles, Sun, ArrowUpCircle } from 'lucide-react';
import { chartService } from '../services/chartService';
import { ChartInterpretationSection } from './ChartInterpretationSection';
import { SaveChartPanel, type SaveStatus } from './SaveChartPanel';

interface ChartResultProps {
  data: CalculateChartResponse;
  request: CalculateChartRequest | null;
  showSavePanel?: boolean;
}

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

  // Si no hay datos mínimos para renderizar siquiera el header
  if (!summary) return null;

  return (
    <div className="w-full animate-in fade-in slide-in-from-bottom-4 duration-1000 mt-12 space-y-12 pb-20">

      {showSavePanel && (
        <SaveChartPanel
          chartLabel={chartLabel}
          saveStatus={saveStatus}
          feedbackMessage={feedbackMessage}
          savedChartId={savedChartId}
          onChartLabelChange={(value) => {
            setChartLabel(value);
            if (saveStatus !== 'idle') {
              setSaveStatus('idle');
              setFeedbackMessage(null);
            }
          }}
          onSave={() => void handleSaveChart()}
        />
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
      {interpretation && (
        <ChartInterpretationSection interpretation={interpretation} summary={summary} />
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
