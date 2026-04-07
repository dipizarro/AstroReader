import { useState } from 'react';
import { MapPin, Calendar, Clock, Loader2, MousePointerClick, Settings2 } from 'lucide-react';
import type { CalculateChartRequest } from '../types/chart.types';

interface CalculateChartFormProps {
  onSubmit: (data: CalculateChartRequest) => Promise<void>;
  isLoading: boolean;
}

interface LocationData {
  latitude: number;
  longitude: number;
  timezoneOffsetMinutes: number;
  placeName: string;
}

export const CalculateChartForm = ({ onSubmit, isLoading }: CalculateChartFormProps) => {
  const [birthDate, setBirthDate] = useState('');
  const [birthTime, setBirthTime] = useState('');
  const [placeSearch, setPlaceSearch] = useState('');
  
  // Almacena internamente las coordenadas técnicas
  const [selectedLocation, setSelectedLocation] = useState<LocationData | null>(null);
  
  const [showAdvanced, setShowAdvanced] = useState(false);
  const [errors, setErrors] = useState<{ [key: string]: string }>({});

  const validate = (): boolean => {
    const newErrors: { [key: string]: string } = {};
    if (!birthDate) newErrors.birthDate = 'Requerido';
    if (!birthTime) newErrors.birthTime = 'Requerido';
    if (!selectedLocation && !showAdvanced) {
       newErrors.placeSearch = 'Debes seleccionar una ubicación válida del listado.';
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (validate()) {
      onSubmit({
        birthDate,
        birthTime,
        latitude: selectedLocation?.latitude ?? 0,
        longitude: selectedLocation?.longitude ?? 0,
        timezoneOffsetMinutes: selectedLocation?.timezoneOffsetMinutes ?? 0,
        placeName: selectedLocation?.placeName || placeSearch
      });
    }
  };

  // Simulación temporal de un click de "Autocompletado de Google/Mapbox"
  const handleSelectMockPlace = () => {
    setPlaceSearch('Buenos Aires, Argentina (Demo)');
    setSelectedLocation({
      latitude: -34.6037,
      longitude: -58.3816,
      timezoneOffsetMinutes: -180, // UTC-3
      placeName: 'Buenos Aires, Argentina'
    });
    setErrors(prev => ({ ...prev, placeSearch: '' }));
  };

  // Modo fallback manual para desarrolladores o lugares no listados
  const handleManualLocationUpdate = (field: keyof LocationData, value: string) => {
    const numValue = field === 'placeName' ? value : parseFloat(value) || 0;
    setSelectedLocation(prev => ({
      ...prev,
      [field]: numValue
    }) as LocationData);
  };

  return (
    <div className="w-full glass-panel rounded-3xl p-8 shadow-2xl relative overflow-hidden">
      {isLoading && (
        <div className="absolute inset-0 bg-background/50 backdrop-blur-sm z-10 flex items-center justify-center">
          <Loader2 className="w-10 h-10 text-primary animate-spin" />
        </div>
      )}

      <form className="flex flex-col gap-6 relative z-0" onSubmit={handleSubmit} noValidate>
        
        {/* Date of Birth */}
        <div className="flex flex-col gap-2 relative">
          <label htmlFor="birthDate" className="text-sm font-medium text-text">Fecha de nacimiento</label>
          <div className="relative">
            <Calendar className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-text-muted" />
            <input 
              type="date" 
              id="birthDate"
              value={birthDate}
              onChange={(e) => { setBirthDate(e.target.value); setErrors(prev => ({...prev, birthDate: ''})); }}
              disabled={isLoading}
              className={`w-full rounded-lg border ${errors.birthDate ? 'border-red-500/50 focus:ring-red-500/50' : 'border-white/10 focus:border-primary/50 focus:ring-primary/50'} bg-surface pl-10 pr-4 py-3 text-white focus:outline-none focus:ring-1 transition-all [color-scheme:dark]`}
            />
          </div>
          {errors.birthDate && <span className="text-xs text-red-400">{errors.birthDate}</span>}
        </div>

        {/* Time of Birth */}
        <div className="flex flex-col gap-2 relative">
          <label htmlFor="birthTime" className="text-sm font-medium text-text">Hora local exacta</label>
          <div className="relative">
            <Clock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-text-muted" />
            <input 
              type="time" 
              id="birthTime"
              value={birthTime}
              onChange={(e) => { setBirthTime(e.target.value); setErrors(prev => ({...prev, birthTime: ''})); }}
              disabled={isLoading}
              className={`w-full rounded-lg border ${errors.birthTime ? 'border-red-500/50 focus:ring-red-500/50' : 'border-white/10 focus:border-primary/50 focus:ring-primary/50'} bg-surface pl-10 pr-4 py-3 text-white focus:outline-none focus:ring-1 transition-all [color-scheme:dark]`}
            />
          </div>
          {errors.birthTime && <span className="text-xs text-red-400">{errors.birthTime}</span>}
        </div>

        {/* Location (Autocomplete Proxy) */}
        <div className="flex flex-col gap-2 relative">
          <label htmlFor="placeSearch" className="text-sm font-medium text-text flex items-center justify-between">
            Lugar de nacimiento
            <button 
              type="button" 
              onClick={() => setShowAdvanced(!showAdvanced)}
              className="text-text-muted hover:text-primary transition-colors flex items-center gap-1 text-xs"
            >
              <Settings2 className="w-3 h-3" />
              Ingreso Manual
            </button>
          </label>
          <div className="relative">
            <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-text-muted" />
            <input 
              type="text" 
              id="placeSearch"
              value={placeSearch}
              onChange={(e) => { 
                setPlaceSearch(e.target.value);
                setSelectedLocation(null); // Reseteamos la coordenada si cambian el texto
                setErrors(prev => ({...prev, placeSearch: ''})); 
              }}
              disabled={isLoading || showAdvanced}
              placeholder="Ej: Buenos Aires, Argentina"
              className={`w-full rounded-lg border ${errors.placeSearch ? 'border-red-500/50 focus:ring-red-500/50' : selectedLocation ? 'border-primary/50 ring-1 ring-primary/30' : 'border-white/10 focus:border-primary/50 focus:ring-primary/50'} bg-surface pl-10 pr-4 py-3 text-white placeholder-text-muted/50 focus:outline-none focus:ring-1 transition-all`}
            />
            {/* TO-DO: Aquí se inyectará el dropdown de Resultados Creado con la API de Places */}
            {!selectedLocation && placeSearch.length > 2 && !showAdvanced && (
              <div className="absolute top-[110%] left-0 right-0 z-20 bg-background border border-white/10 rounded-lg shadow-xl overflow-hidden animate-in fade-in zoom-in-95 duration-200">
                 <button 
                   type="button"
                   onClick={handleSelectMockPlace} 
                   className="w-full text-left px-4 py-3 flex items-center gap-3 hover:bg-white/5 transition-colors text-sm text-white"
                 >
                   <MapPin className="w-4 h-4 text-primary" />
                   Elegir Buenos Aires (Stub temporal)
                 </button>
              </div>
            )}
            
            {/* Status Indicador */}
            {selectedLocation && !showAdvanced && (
               <div className="absolute right-3 top-1/2 -translate-y-1/2">
                 <div className="bg-primary/20 text-primary text-[10px] px-2 py-1 rounded-full font-semibold uppercase tracking-widest flex items-center gap-1">
                   Coordenadas Fijadas
                 </div>
               </div>
            )}
          </div>
          {errors.placeSearch && <span className="text-xs text-red-400">{errors.placeSearch}</span>}
        </div>

        {/* Fallback Técnico (Modo Avanzado) */}
        {showAdvanced && (
           <div className="bg-[#121216] border border-white/5 rounded-xl p-5 space-y-4 animate-in slide-in-from-top-4 fade-in duration-300">
              <div className="text-xs text-primary bg-primary/10 border border-primary/20 p-2 rounded-lg font-medium text-center">
                Modo Técnico Activado: Ingresa coordenadas astronómicas crudas.
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-1">
                  <label className="text-[11px] text-text-muted uppercase">Latitud (Decimal)</label>
                  <input type="number" step="any" onChange={e => handleManualLocationUpdate('latitude', e.target.value)} className="w-full bg-background border border-white/10 rounded p-2 text-sm text-white" placeholder="-34.6037" />
                </div>
                <div className="space-y-1">
                  <label className="text-[11px] text-text-muted uppercase">Longitud (Decimal)</label>
                  <input type="number" step="any" onChange={e => handleManualLocationUpdate('longitude', e.target.value)} className="w-full bg-background border border-white/10 rounded p-2 text-sm text-white" placeholder="-58.3816" />
                </div>
              </div>
              <div className="space-y-1">
                <label className="text-[11px] text-text-muted uppercase">Offset Zona Horaria (Minutos)</label>
                <input type="number" onChange={e => handleManualLocationUpdate('timezoneOffsetMinutes', e.target.value)} className="w-full bg-background border border-white/10 rounded p-2 text-sm text-white" placeholder="-180" />
                <span className="text-[10px] text-text-muted/60">Ej: GMT-3 = -180 minutos. Usado para calcular UTC exacto.</span>
              </div>
           </div>
        )}

        {/* Submit */}
        <button 
          type="submit" 
          disabled={isLoading || (!selectedLocation && !showAdvanced)}
          className="mt-6 flex w-full items-center justify-center gap-2 rounded-xl bg-primary py-3.5 text-sm font-semibold text-[#0a0a0b] shadow-[0_0_15px_rgba(212,175,55,0.3)] transition-all hover:bg-primary-hover hover:shadow-[0_0_25px_rgba(212,175,55,0.5)] disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {isLoading ? (
             <>Calculando Cúspides...</>
          ) : (
             <>
               <MousePointerClick className="w-4 h-4 opacity-50" />
               Revelar Carta Astral
             </>
          )}
        </button>
      </form>
    </div>
  );
};
