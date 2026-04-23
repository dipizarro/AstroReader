import { useState, useEffect, useRef } from 'react';
import { MapPin, Loader2, Search } from 'lucide-react';
import { geocodingService, type GeocodingResult } from '../../../core/services/geocodingService';

interface LocationAutocompleteProps {
  onSelect: (location: { latitude: number; longitude: number; placeName: string }) => void;
  disabled?: boolean;
  error?: string;
  onClearError?: () => void;
  initialPlaceName?: string;
}

export const LocationAutocomplete = ({ onSelect, disabled, error, onClearError, initialPlaceName }: LocationAutocompleteProps) => {
  const [query, setQuery] = useState('');
  const [results, setResults] = useState<GeocodingResult[]>([]);
  const [isSearching, setIsSearching] = useState(false);
  const [isOpen, setIsOpen] = useState(false);
  const [selectedPlace, setSelectedPlace] = useState<string | null>(null);
  
  const dropdownRef = useRef<HTMLDivElement>(null);

  // Cerrar el dropdown al clickear afuera
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  useEffect(() => {
    if (!initialPlaceName) {
      return;
    }

    setQuery(initialPlaceName);
    setSelectedPlace(initialPlaceName);
  }, [initialPlaceName]);

  // Debounce para llamadas a la API
  useEffect(() => {
    if (query.length < 3 || selectedPlace === query) {
      setResults([]);
      setIsOpen(false);
      return;
    }

    const searchTimer = setTimeout(async () => {
      setIsSearching(true);
      const data = await geocodingService.searchPlaces(query);
      setResults(data);
      setIsOpen(true);
      setIsSearching(false);
    }, 500); // 500ms debounce

    return () => clearTimeout(searchTimer);
  }, [query, selectedPlace]);

  const handleSelect = (result: GeocodingResult) => {
    setQuery(result.placeName);
    setSelectedPlace(result.placeName);
    setIsOpen(false);
    
    if (onClearError) onClearError();
    
    onSelect({
      latitude: result.latitude,
      longitude: result.longitude,
      placeName: result.placeName
    });
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setQuery(e.target.value);
    setSelectedPlace(null); // Invalida la selección actual si el usuario edita de nuevo
    if (onClearError) onClearError();
  };

  return (
    <div className="relative" ref={dropdownRef}>
      <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-text-muted" />
      <input 
        type="text" 
        value={query}
        onChange={handleInputChange}
        onFocus={() => { if (results.length > 0) setIsOpen(true); }}
        disabled={disabled}
        placeholder="Ej: Madrid, España"
        className={`w-full rounded-lg border ${error ? 'border-red-500/50 focus:ring-red-500/50' : selectedPlace ? 'border-primary/50 ring-1 ring-primary/30' : 'border-white/10 focus:border-primary/50 focus:ring-primary/50'} bg-surface pl-10 pr-10 py-3 text-white placeholder-text-muted/50 focus:outline-none focus:ring-1 transition-all`}
      />
      
      {/* Icono de Lupa o Spin Derecho */}
      <div className="absolute right-3 top-1/2 -translate-y-1/2 flex items-center justify-center">
         {isSearching ? <Loader2 className="w-4 h-4 text-primary animate-spin" /> : 
          selectedPlace ? <div className="bg-primary/20 text-primary text-[10px] px-2 py-[2px] rounded-full font-semibold uppercase tracking-widest hidden sm:block">OK</div> : 
          <Search className="w-4 h-4 text-text-muted/50" />}
      </div>

      {/* Dropdown de Resultados */}
      {isOpen && results.length > 0 && !disabled && (
        <div className="absolute top-[110%] left-0 right-0 z-50 bg-background border border-white/10 rounded-xl shadow-2xl overflow-hidden animate-in fade-in zoom-in-95 duration-200">
           {results.map((res) => (
             <button 
               key={res.id}
               type="button"
               onClick={() => handleSelect(res)} 
               className="w-full text-left px-4 py-3 flex items-center gap-3 hover:bg-white/5 transition-colors text-sm text-white border-b border-white/5 last:border-b-0"
             >
               <MapPin className="w-4 h-4 text-primary shrink-0" />
               <span className="truncate">{res.placeName}</span>
             </button>
           ))}
        </div>
      )}
    </div>
  );
};
