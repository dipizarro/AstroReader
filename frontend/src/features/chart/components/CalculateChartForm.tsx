import { useState } from 'react';
import { MapPin, Calendar, Clock, Loader2 } from 'lucide-react';
import type { CalculateChartRequest } from '../types/chart.types';

interface CalculateChartFormProps {
  onSubmit: (data: CalculateChartRequest) => Promise<void>;
  isLoading: boolean;
}

export const CalculateChartForm = ({ onSubmit, isLoading }: CalculateChartFormProps) => {
  const [formData, setFormData] = useState<CalculateChartRequest>({
    birthDate: '',
    birthTime: '',
    birthPlace: '',
  });

  const [errors, setErrors] = useState<Partial<CalculateChartRequest>>({});

  const validate = (): boolean => {
    const newErrors: Partial<CalculateChartRequest> = {};
    if (!formData.birthDate) newErrors.birthDate = 'Requerido';
    if (!formData.birthTime) newErrors.birthTime = 'Requerido';
    if (!formData.birthPlace.trim()) newErrors.birthPlace = 'Requerido';
    
    // Add simple date validation sanity checks if necessary...
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (validate()) {
      onSubmit(formData);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { id, value } = e.target;
    setFormData(prev => ({ ...prev, [id]: value }));
    // Clear error on interact
    if (errors[id as keyof CalculateChartRequest]) {
      setErrors(prev => ({ ...prev, [id]: undefined }));
    }
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
              value={formData.birthDate}
              onChange={handleChange}
              disabled={isLoading}
              className={`w-full rounded-lg border ${errors.birthDate ? 'border-red-500/50 focus:ring-red-500/50' : 'border-white/10 focus:border-primary/50 focus:ring-primary/50'} bg-surface pl-10 pr-4 py-3 text-white focus:outline-none focus:ring-1 transition-all [color-scheme:dark]`}
            />
          </div>
          {errors.birthDate && <span className="text-xs text-red-400">{errors.birthDate}</span>}
        </div>

        {/* Time of Birth */}
        <div className="flex flex-col gap-2 relative">
          <label htmlFor="birthTime" className="text-sm font-medium text-text">Hora de nacimiento</label>
          <div className="relative">
            <Clock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-text-muted" />
            <input 
              type="time" 
              id="birthTime"
              value={formData.birthTime}
              onChange={handleChange}
              disabled={isLoading}
              className={`w-full rounded-lg border ${errors.birthTime ? 'border-red-500/50 focus:ring-red-500/50' : 'border-white/10 focus:border-primary/50 focus:ring-primary/50'} bg-surface pl-10 pr-4 py-3 text-white focus:outline-none focus:ring-1 transition-all [color-scheme:dark]`}
            />
          </div>
          {errors.birthTime && <span className="text-xs text-red-400">{errors.birthTime}</span>}
        </div>

        {/* Location */}
        <div className="flex flex-col gap-2 relative">
          <label htmlFor="birthPlace" className="text-sm font-medium text-text">Lugar de nacimiento</label>
          <div className="relative">
            <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-text-muted" />
            <input 
              type="text" 
              id="birthPlace"
              value={formData.birthPlace}
              onChange={handleChange}
              disabled={isLoading}
              placeholder="Ej: Buenos Aires, Argentina"
              className={`w-full rounded-lg border ${errors.birthPlace ? 'border-red-500/50 focus:ring-red-500/50' : 'border-white/10 focus:border-primary/50 focus:ring-primary/50'} bg-surface pl-10 pr-4 py-3 text-white placeholder-text-muted/50 focus:outline-none focus:ring-1 transition-all`}
            />
          </div>
          {errors.birthPlace && <span className="text-xs text-red-400">{errors.birthPlace}</span>}
        </div>

        <button 
          type="submit" 
          disabled={isLoading}
          className="mt-4 flex w-full items-center justify-center rounded-xl bg-primary py-3.5 text-sm font-semibold text-[#0a0a0b] shadow-[0_0_15px_rgba(212,175,55,0.3)] transition-all hover:bg-primary-hover hover:shadow-[0_0_25px_rgba(212,175,55,0.5)] disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {isLoading ? 'Calculando el cosmos...' : 'Calcular mi Carta'}
        </button>
      </form>
    </div>
  );
};
