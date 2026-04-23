import { useEffect, useMemo, useState } from 'react';
import { Calendar, Clock, Loader2, MousePointerClick, Settings2 } from 'lucide-react';
import type { CalculateChartRequest } from '../types/chart.types';
import { LocationAutocomplete } from './LocationAutocomplete';
import tzlookup from 'tz-lookup';
import { DateTime } from 'luxon';
import type { PersonalProfileChartContext } from '../../profile/types/profile.types';

interface CalculateChartFormProps {
  onSubmit: (data: CalculateChartRequest) => Promise<void>;
  isLoading: boolean;
  profileContext?: PersonalProfileChartContext | null;
  onClearProfileContext?: () => void;
}

interface LocationData {
  latitude: number;
  longitude: number;
  timezoneOffsetMinutes: number;
  placeName: string;
}

interface ManualLocationForm {
  placeName: string;
  latitude: string;
  longitude: string;
  timezoneOffsetMinutes: string;
}

const COORDINATE_PRECISION = 6;

const coordinatesMatch = (left: number, right: number) =>
  Number(left.toFixed(COORDINATE_PRECISION)) === Number(right.toFixed(COORDINATE_PRECISION));

export const CalculateChartForm = ({
  onSubmit,
  isLoading,
  profileContext = null,
  onClearProfileContext,
}: CalculateChartFormProps) => {
  const [birthDate, setBirthDate] = useState('');
  const [birthTime, setBirthTime] = useState('');
  
  // Almacena internamente las coordenadas del autocomplete o manuales
  const [selectedLocation, setSelectedLocation] = useState<LocationData | null>(null);
  const [manualLocation, setManualLocation] = useState<ManualLocationForm>({
    placeName: '',
    latitude: '',
    longitude: '',
    timezoneOffsetMinutes: '',
  });
  
  const [showAdvanced, setShowAdvanced] = useState(false);
  const [errors, setErrors] = useState<{ [key: string]: string }>({});
  const [profileContextApplied, setProfileContextApplied] = useState(false);

  useEffect(() => {
    if (!profileContext) {
      setProfileContextApplied(false);
      return;
    }

    setBirthDate(profileContext.birthDate);
    setBirthTime(profileContext.birthTime);
    setSelectedLocation({
      latitude: profileContext.latitude,
      longitude: profileContext.longitude,
      timezoneOffsetMinutes: profileContext.timezoneOffsetMinutes,
      placeName: profileContext.birthPlace,
    });
    setShowAdvanced(false);
    setManualLocation({
      placeName: profileContext.birthPlace,
      latitude: '',
      longitude: '',
      timezoneOffsetMinutes: '',
    });
    setErrors({});
    setProfileContextApplied(true);
  }, [profileContext]);

  const hasManualLocationInput =
    manualLocation.latitude.trim().length > 0 &&
    manualLocation.longitude.trim().length > 0 &&
    manualLocation.timezoneOffsetMinutes.trim().length > 0;

  const resolvedDraftLocation = useMemo(() => {
    if (showAdvanced) {
      const latitude = Number(manualLocation.latitude.trim());
      const longitude = Number(manualLocation.longitude.trim());
      const timezoneOffsetMinutes = Number(manualLocation.timezoneOffsetMinutes.trim());

      if (
        !Number.isFinite(latitude) ||
        !Number.isFinite(longitude) ||
        !Number.isInteger(timezoneOffsetMinutes)
      ) {
        return null;
      }

      return {
        latitude,
        longitude,
        timezoneOffsetMinutes,
      };
    }

    if (!selectedLocation) {
      return null;
    }

    let timezoneOffsetMinutes = selectedLocation.timezoneOffsetMinutes;

    if (birthDate && birthTime) {
      try {
        const ianaZone = tzlookup(selectedLocation.latitude, selectedLocation.longitude);
        const dt = DateTime.fromISO(`${birthDate}T${birthTime}`, { zone: ianaZone });

        if (dt.isValid) {
          timezoneOffsetMinutes = dt.offset;
        }
      } catch (error) {
        console.error('No pudimos resolver el offset horario para la vista previa del perfil.', error);
      }
    }

    return {
      latitude: selectedLocation.latitude,
      longitude: selectedLocation.longitude,
      timezoneOffsetMinutes,
    };
  }, [
    birthDate,
    birthTime,
    manualLocation.latitude,
    manualLocation.longitude,
    manualLocation.timezoneOffsetMinutes,
    selectedLocation,
    showAdvanced,
  ]);

  const isProfileContextApplicable = useMemo(() => {
    if (!profileContext || !profileContextApplied) {
      return false;
    }

    if (profileContext.birthDate !== birthDate || profileContext.birthTime !== birthTime) {
      return false;
    }

    if (!resolvedDraftLocation) {
      return false;
    }

    return (
      coordinatesMatch(profileContext.latitude, resolvedDraftLocation.latitude) &&
      coordinatesMatch(profileContext.longitude, resolvedDraftLocation.longitude) &&
      profileContext.timezoneOffsetMinutes === resolvedDraftLocation.timezoneOffsetMinutes
    );
  }, [birthDate, birthTime, profileContext, profileContextApplied, resolvedDraftLocation]);

  const shouldShowProfileMismatchNotice = !!profileContext && profileContextApplied && !isProfileContextApplicable;

  const validate = (): boolean => {
    const newErrors: { [key: string]: string } = {};
    if (!birthDate) newErrors.birthDate = 'Requerido';
    if (!birthTime) newErrors.birthTime = 'Requerido';

    if (!showAdvanced) {
      if (!selectedLocation) {
        newErrors.placeSearch = 'Debes buscar y seleccionar un lugar de nacimiento válido.';
      }
    } else {
      const latitudeRaw = manualLocation.latitude.trim();
      const longitudeRaw = manualLocation.longitude.trim();
      const offsetRaw = manualLocation.timezoneOffsetMinutes.trim();

      if (!latitudeRaw) {
        newErrors.manualLatitude = 'Ingresa una latitud decimal.';
      } else {
        const latitude = Number(latitudeRaw);
        if (!Number.isFinite(latitude)) {
          newErrors.manualLatitude = 'La latitud debe ser un número válido.';
        } else if (latitude < -90 || latitude > 90) {
          newErrors.manualLatitude = 'La latitud debe estar entre -90 y 90.';
        }
      }

      if (!longitudeRaw) {
        newErrors.manualLongitude = 'Ingresa una longitud decimal.';
      } else {
        const longitude = Number(longitudeRaw);
        if (!Number.isFinite(longitude)) {
          newErrors.manualLongitude = 'La longitud debe ser un número válido.';
        } else if (longitude < -180 || longitude > 180) {
          newErrors.manualLongitude = 'La longitud debe estar entre -180 y 180.';
        }
      }

      if (!offsetRaw) {
        newErrors.manualTimezoneOffsetMinutes = 'Ingresa el offset horario en minutos.';
      } else {
        const offset = Number(offsetRaw);
        if (!Number.isInteger(offset)) {
          newErrors.manualTimezoneOffsetMinutes = 'El offset debe ser un número entero en minutos.';
        } else if (offset < -720 || offset > 840) {
          newErrors.manualTimezoneOffsetMinutes = 'El offset debe estar entre -720 y +840 minutos.';
        }
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (validate()) {
      let finalLat = selectedLocation?.latitude ?? 0;
      let finalLng = selectedLocation?.longitude ?? 0;
      let finalOffset = selectedLocation?.timezoneOffsetMinutes ?? 0;
      let finalPlaceName = selectedLocation?.placeName || 'Ubicación geocodificada';
      let finalTimezoneIana: string | undefined;

      // ✅ RESOLUCIÓN PRECISA DE TIEMPO:
      // Si el usuario seleccionó un lugar visualmente (no manual con offset harcodeado),
      // calculamos su offset de huso horario *exacto* para la fecha ingresada 
      // contemplando los complejos casos de horario de verano históricos.
      if (!showAdvanced && selectedLocation) {
        try {
          // 1. Buscamos el String global IANA del huso horario (ej: "America/Argentina/Buenos_Aires")
          const ianaZone = tzlookup(finalLat, finalLng);
          finalTimezoneIana = ianaZone;
          
          // 2. Construimos una fecha anclada puramente a esa zona local
          const dt = DateTime.fromISO(`${birthDate}T${birthTime}`, { zone: ianaZone });
          
          if (dt.isValid) {
            // 3. Obtenemos la desviación literal que existía en ESE mismísimo instante (Luxon entrega minutos)
            finalOffset = dt.offset; 
          }
        } catch (error) {
          console.error("Error resolviendo Timezone dinámico. Usando fallback estático o cero.", error);
        }
      } else {
        finalLat = Number(manualLocation.latitude.trim());
        finalLng = Number(manualLocation.longitude.trim());
        finalOffset = Number(manualLocation.timezoneOffsetMinutes.trim());

        try {
          finalTimezoneIana = tzlookup(finalLat, finalLng);
        } catch (error) {
          console.warn('No pudimos resolver una zona IANA para la ubicación manual.', error);
        }

        const manualPlaceName = manualLocation.placeName.trim();
        finalPlaceName = manualPlaceName.length > 0
          ? manualPlaceName
          : `Coordenadas manuales (${finalLat.toFixed(4)}, ${finalLng.toFixed(4)})`;
      }

      if (!Number.isFinite(finalLat) || finalLat < -90 || finalLat > 90) {
        setErrors(prev => ({
          ...prev,
          manualLatitude: showAdvanced ? 'La latitud ingresada no es válida.' : '',
          placeSearch: !showAdvanced ? 'No pudimos resolver una latitud válida para ese lugar.' : prev.placeSearch,
        }));
        return;
      }

      if (!Number.isFinite(finalLng) || finalLng < -180 || finalLng > 180) {
        setErrors(prev => ({
          ...prev,
          manualLongitude: showAdvanced ? 'La longitud ingresada no es válida.' : '',
          placeSearch: !showAdvanced ? 'No pudimos resolver una longitud válida para ese lugar.' : prev.placeSearch,
        }));
        return;
      }

      if (!Number.isFinite(finalOffset) || !Number.isInteger(finalOffset) || finalOffset < -720 || finalOffset > 840) {
        if (showAdvanced) {
          setErrors(prev => ({
            ...prev,
            manualTimezoneOffsetMinutes: 'El offset horario ingresado no es válido.'
          }));
          return;
        }

        // En modo geocodificado preferimos degradar con fallback a UTC
        // antes que bloquear el flujo por una resolución fina del huso horario.
        finalOffset = 0;
      }

      const personalProfileId =
        profileContext &&
        profileContext.birthDate === birthDate &&
        profileContext.birthTime === birthTime &&
        coordinatesMatch(profileContext.latitude, finalLat) &&
        coordinatesMatch(profileContext.longitude, finalLng) &&
        profileContext.timezoneOffsetMinutes === finalOffset
          ? profileContext.profileId
          : null;

      onSubmit({
        birthDate,
        birthTime,
        latitude: finalLat,
        longitude: finalLng,
        timezoneOffsetMinutes: finalOffset,
        placeName: finalPlaceName,
        timezoneIana: finalTimezoneIana,
        personalProfileId
      });
    }
  };

  const handleManualLocationUpdate = (field: keyof ManualLocationForm, value: string) => {
    setManualLocation(prev => ({
      ...prev,
      [field]: value
    }));
    const errorKey = `manual${field.charAt(0).toUpperCase()}${field.slice(1)}`;
    setErrors(prev => ({ ...prev, [errorKey]: '' }));
  };

  const handleLocationSelected = (loc: { latitude: number; longitude: number; placeName: string }) => {
    // Cuando el autocomplete responde, almacenamos lat/lng puros.
    // El offset se calcula rigurosamente en el handleSubmit dependiendo de la fecha que escribió.
    setSelectedLocation({
      latitude: loc.latitude,
      longitude: loc.longitude,
      timezoneOffsetMinutes: 0, 
      placeName: loc.placeName
    });
  };

  return (
    <div className="w-full glass-panel rounded-3xl p-8 shadow-2xl relative overflow-hidden">
      {isLoading && (
        <div className="absolute inset-0 bg-background/50 backdrop-blur-sm z-10 flex items-center justify-center">
          <Loader2 className="w-10 h-10 text-primary animate-spin" />
        </div>
      )}

      <form className="flex flex-col gap-6 relative z-0" onSubmit={handleSubmit} noValidate>
        {profileContext && (
          <div className="rounded-2xl border border-primary/15 bg-primary/10 px-4 py-4 text-sm">
            <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
              <div>
                <p className="text-xs uppercase tracking-[0.22em] text-primary/80">
                  {shouldShowProfileMismatchNotice ? 'Perfil enriquecido en pausa' : 'Perfil enriquecido activo'}
                </p>
                <p className="mt-2 text-white">
                  Prepararemos esta carta con los datos de <span className="font-medium">{profileContext.fullName}</span>.
                </p>
                <p className="mt-2 leading-6 text-text-muted">
                  Si mantienes fecha, hora y ubicación, la lectura se calculará con ese perfil contextual. Si cambias esos datos o desvinculas el perfil, calcularemos la carta sin adjuntarlo.
                </p>
                {shouldShowProfileMismatchNotice && (
                  <div className="mt-3 rounded-2xl border border-white/10 bg-white/[0.05] px-4 py-3 text-sm leading-6 text-white/84">
                    Esta lectura ya no coincide del todo con ese perfil. Seguiremos con la carta actual, solo que sin usar ese contexto personal hasta que los datos vuelvan a alinearse.
                  </div>
                )}
              </div>

              {onClearProfileContext && (
                <button
                  type="button"
                  onClick={onClearProfileContext}
                  className="inline-flex items-center justify-center rounded-full border border-white/10 bg-white/5 px-4 py-2 text-xs font-medium text-white transition hover:bg-white/10"
                >
                  Desvincular perfil
                </button>
              )}
            </div>
          </div>
        )}
        
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

        {/* Location (Autocomplete Component) */}
        {!showAdvanced && (
           <div className="flex flex-col gap-2 relative">
             <label className="text-sm font-medium text-text flex items-center justify-between">
               Lugar de nacimiento
               <button 
                 type="button" 
                 onClick={() => {
                   setShowAdvanced(true);
                   setSelectedLocation(null);
                   setManualLocation(prev => ({
                     ...prev,
                     placeName: prev.placeName || selectedLocation?.placeName || profileContext?.birthPlace || '',
                   }));
                   setErrors(prev => ({
                     ...prev,
                     placeSearch: '',
                     manualLatitude: '',
                     manualLongitude: '',
                     manualTimezoneOffsetMinutes: '',
                   }));
                 }}
                 className="text-text-muted hover:text-primary transition-colors flex items-center gap-1 text-xs"
               >
                 <Settings2 className="w-3 h-3" />
                 Ingreso Manual
               </button>
             </label>
             <LocationAutocomplete 
               onSelect={handleLocationSelected}
               disabled={isLoading}
               error={errors.placeSearch}
               onClearError={() => setErrors(prev => ({...prev, placeSearch: ''}))}
               initialPlaceName={selectedLocation?.placeName}
             />
             {errors.placeSearch && <span className="text-xs text-red-400">{errors.placeSearch}</span>}

           </div>
        )}

        {/* Fallback Técnico (Modo Avanzado) */}
        {showAdvanced && (
           <div className="bg-[#121216] border border-white/5 rounded-xl p-5 space-y-4 animate-in slide-in-from-top-4 fade-in duration-300">
              <div className="flex items-center justify-between">
                 <div className="text-xs text-primary bg-primary/10 border border-primary/20 px-3 py-1 rounded-lg font-medium">
                   Modo Técnico Activado
                 </div>
                 <button
                   type="button"
                   onClick={() => {
                     setShowAdvanced(false);
                     setManualLocation({
                       placeName: '',
                       latitude: '',
                       longitude: '',
                       timezoneOffsetMinutes: '',
                     });
                     setErrors(prev => ({
                       ...prev,
                       manualLatitude: '',
                       manualLongitude: '',
                       manualTimezoneOffsetMinutes: '',
                     }));
                   }}
                   className="text-xs text-text-muted underline hover:text-white"
                 >
                   Volver a Buscador
                 </button>
              </div>
              <div className="space-y-1">
                <label className="text-[11px] text-text-muted uppercase">Referencia del Lugar</label>
                <input
                  type="text"
                  value={manualLocation.placeName}
                  onChange={e => handleManualLocationUpdate('placeName', e.target.value)}
                  className="w-full bg-background border border-white/10 rounded p-2 text-sm text-white"
                  placeholder="Ej: Valparaiso, Chile"
                />
                <span className="text-[10px] text-text-muted/60">
                  Opcional. Si no lo completas, guardaremos una referencia legible usando las coordenadas.
                </span>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-1">
                  <label className="text-[11px] text-text-muted uppercase">Latitud (Decimal)</label>
                  <input
                    type="number"
                    step="any"
                    value={manualLocation.latitude}
                    onChange={e => handleManualLocationUpdate('latitude', e.target.value)}
                    className={`w-full bg-background border rounded p-2 text-sm text-white ${errors.manualLatitude ? 'border-red-500/50' : 'border-white/10'}`}
                    placeholder="-34.6037"
                  />
                  {errors.manualLatitude && <span className="text-xs text-red-400">{errors.manualLatitude}</span>}
                </div>
                <div className="space-y-1">
                  <label className="text-[11px] text-text-muted uppercase">Longitud (Decimal)</label>
                  <input
                    type="number"
                    step="any"
                    value={manualLocation.longitude}
                    onChange={e => handleManualLocationUpdate('longitude', e.target.value)}
                    className={`w-full bg-background border rounded p-2 text-sm text-white ${errors.manualLongitude ? 'border-red-500/50' : 'border-white/10'}`}
                    placeholder="-58.3816"
                  />
                  {errors.manualLongitude && <span className="text-xs text-red-400">{errors.manualLongitude}</span>}
                </div>
              </div>
              <div className="space-y-1">
                <label className="text-[11px] text-text-muted uppercase">Offset Zona Horaria (Minutos)</label>
                <input
                  type="number"
                  step="1"
                  value={manualLocation.timezoneOffsetMinutes}
                  onChange={e => handleManualLocationUpdate('timezoneOffsetMinutes', e.target.value)}
                  className={`w-full bg-background border rounded p-2 text-sm text-white ${errors.manualTimezoneOffsetMinutes ? 'border-red-500/50' : 'border-white/10'}`}
                  placeholder="-180"
                />
                {errors.manualTimezoneOffsetMinutes && <span className="text-xs text-red-400">{errors.manualTimezoneOffsetMinutes}</span>}
                <span className="text-[10px] text-text-muted/60">Ej: GMT-3 = -180 minutos. Completa los tres campos antes de calcular.</span>
              </div>
           </div>
        )}

        {/* Submit */}
        <button 
          type="submit" 
          disabled={isLoading || (!showAdvanced && !selectedLocation) || (showAdvanced && !hasManualLocationInput)}
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
