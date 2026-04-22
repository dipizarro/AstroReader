import { useState } from 'react';
import { Calendar, Check, ChevronLeft, ChevronRight, Clock, Loader2, MapPin, Sparkles, Stars, UserRound } from 'lucide-react';
import tzlookup from 'tz-lookup';
import { DateTime } from 'luxon';
import { LocationAutocomplete } from '../../chart/components/LocationAutocomplete';
import { GuidedQuestionCard } from './GuidedQuestionCard';
import { personalProfileService } from '../services/personalProfileService';
import { personalProfileStorage } from '../services/personalProfileStorage';
import type { CreatePersonalProfileRequest, PersonalProfileDetail } from '../types/profile.types';

interface LocationData {
  latitude: number;
  longitude: number;
  placeName: string;
}

interface ManualLocationForm {
  latitude: string;
  longitude: string;
  timezoneOffsetMinutes: string;
}

type StepId = 'birth' | 'guided' | 'voice';

interface FormState {
  fullName: string;
  birthDate: string;
  birthTime: string;
  birthPlace: string;
  selfPerceptionFocus: string;
  currentChallenge: string;
  desiredInsight: string;
  selfDescription: string;
}

type FormErrors = Partial<Record<
  | 'fullName'
  | 'birthDate'
  | 'birthTime'
  | 'birthPlace'
  | 'placeSearch'
  | 'manualLatitude'
  | 'manualLongitude'
  | 'manualTimezoneOffsetMinutes'
  | 'selfPerceptionFocus'
  | 'currentChallenge'
  | 'desiredInsight'
  | 'selfDescription',
  string
>>;

const STEP_ORDER: StepId[] = ['birth', 'guided', 'voice'];

const STEP_META: Record<StepId, { eyebrow: string; title: string; description: string }> = {
  birth: {
    eyebrow: 'Paso 1',
    title: 'Tu base astral',
    description: 'Tomamos tus datos natales con precisión, pero con una experiencia amable y clara.',
  },
  guided: {
    eyebrow: 'Paso 2',
    title: 'Tu momento actual',
    description: 'Estas preguntas no buscan etiquetarte. Solo darnos una mejor puerta de entrada para leerte.',
  },
  voice: {
    eyebrow: 'Paso 3',
    title: 'Tu propia voz',
    description: 'Si quieres, puedes agregar una pequeña descripción libre. Corta, íntima y sin presión.',
  },
};

const QUESTION_SUGGESTIONS = {
  selfPerceptionFocus: [
    'La sensibilidad con la que percibo todo',
    'Mi necesidad de crear y expresar lo que siento',
    'La intensidad con la que vivo mis vínculos',
  ],
  currentChallenge: [
    'Sostener claridad en medio de tantos cambios',
    'Poner límites sin sentir culpa',
    'Confiar más en mí y menos en la aprobación externa',
  ],
  desiredInsight: [
    'Entender por qué repito ciertos patrones',
    'Ver con más claridad qué necesito hoy',
    'Reconocer mejor mis talentos y cómo encarnarlos',
  ],
};

const SELF_DESCRIPTION_MAX_LENGTH = 420;

const SELF_DESCRIPTION_STARTERS = [
  {
    label: 'Cómo me siento',
    text: 'Últimamente me siento una persona que ',
  },
  {
    label: 'Lo que me mueve',
    text: 'En este momento me mueve mucho la necesidad de ',
  },
  {
    label: 'Cómo me vivo',
    text: 'Si tuviera que resumirme hoy, diría que soy alguien que ',
  },
];

const SELF_DESCRIPTION_PROMPTS = [
  'qué estás intentando cuidar más en esta etapa',
  'qué parte de ti se siente más viva hoy',
  'qué contradicción sientes que te acompaña',
];

const initialFormState: FormState = {
  fullName: '',
  birthDate: '',
  birthTime: '',
  birthPlace: '',
  selfPerceptionFocus: '',
  currentChallenge: '',
  desiredInsight: '',
  selfDescription: '',
};

export const PersonalProfileOnboardingForm = () => {
  const [currentStep, setCurrentStep] = useState<StepId>('birth');
  const [form, setForm] = useState<FormState>(initialFormState);
  const [selectedLocation, setSelectedLocation] = useState<LocationData | null>(null);
  const [manualLocation, setManualLocation] = useState<ManualLocationForm>({
    latitude: '',
    longitude: '',
    timezoneOffsetMinutes: '',
  });
  const [showManualLocation, setShowManualLocation] = useState(false);
  const [errors, setErrors] = useState<FormErrors>({});
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [createdProfile, setCreatedProfile] = useState<PersonalProfileDetail | null>(null);

  const currentStepIndex = STEP_ORDER.indexOf(currentStep);
  const currentMeta = STEP_META[currentStep];

  const setField = (field: keyof FormState, value: string) => {
    setForm((previous) => ({
      ...previous,
      [field]: value,
    }));

    setErrors((previous) => ({
      ...previous,
      [field]: '',
    }));
  };

  const clearLocationErrors = () => {
    setErrors((previous) => ({
      ...previous,
      birthPlace: '',
      placeSearch: '',
      manualLatitude: '',
      manualLongitude: '',
      manualTimezoneOffsetMinutes: '',
    }));
  };

  const validateBirthStep = (): boolean => {
    const nextErrors: FormErrors = {};

    if (!form.fullName.trim()) {
      nextErrors.fullName = 'Necesitamos tu nombre para personalizar esta experiencia.';
    }

    if (!form.birthDate) {
      nextErrors.birthDate = 'Elige tu fecha de nacimiento.';
    }

    if (!form.birthTime) {
      nextErrors.birthTime = 'Indica tu hora local de nacimiento.';
    }

    if (!showManualLocation) {
      if (!selectedLocation) {
        nextErrors.placeSearch = 'Busca y selecciona tu lugar de nacimiento.';
      }
    } else {
      const latitude = Number(manualLocation.latitude.trim());
      const longitude = Number(manualLocation.longitude.trim());
      const offset = Number(manualLocation.timezoneOffsetMinutes.trim());

      if (!manualLocation.latitude.trim()) {
        nextErrors.manualLatitude = 'Ingresa la latitud.';
      } else if (!Number.isFinite(latitude) || latitude < -90 || latitude > 90) {
        nextErrors.manualLatitude = 'La latitud debe estar entre -90 y 90.';
      }

      if (!manualLocation.longitude.trim()) {
        nextErrors.manualLongitude = 'Ingresa la longitud.';
      } else if (!Number.isFinite(longitude) || longitude < -180 || longitude > 180) {
        nextErrors.manualLongitude = 'La longitud debe estar entre -180 y 180.';
      }

      if (!manualLocation.timezoneOffsetMinutes.trim()) {
        nextErrors.manualTimezoneOffsetMinutes = 'Ingresa el offset horario en minutos.';
      } else if (!Number.isInteger(offset) || offset < -720 || offset > 840) {
        nextErrors.manualTimezoneOffsetMinutes = 'El offset debe estar entre -720 y +840.';
      }

      if (!form.birthPlace.trim()) {
        nextErrors.birthPlace = 'Dale un nombre humano a este lugar.';
      }
    }

    setErrors((previous) => ({ ...previous, ...nextErrors }));
    return Object.keys(nextErrors).length === 0;
  };

  const validateGuidedStep = (): boolean => {
    const nextErrors: FormErrors = {};

    if (!form.selfPerceptionFocus.trim()) {
      nextErrors.selfPerceptionFocus = 'Cuéntanos qué sientes que más te define hoy.';
    }

    if (!form.currentChallenge.trim()) {
      nextErrors.currentChallenge = 'Cuéntanos qué se siente más difícil hoy.';
    }

    if (!form.desiredInsight.trim()) {
      nextErrors.desiredInsight = 'Cuéntanos qué te gustaría comprender mejor.';
    }

    setErrors((previous) => ({ ...previous, ...nextErrors }));
    return Object.keys(nextErrors).length === 0;
  };

  const goToNextStep = () => {
    const isValid = currentStep === 'birth' ? validateBirthStep() : validateGuidedStep();

    if (!isValid) {
      return;
    }

    setCurrentStep(STEP_ORDER[currentStepIndex + 1]);
  };

  const goToPreviousStep = () => {
    if (currentStepIndex === 0) {
      return;
    }

    setCurrentStep(STEP_ORDER[currentStepIndex - 1]);
  };

  const buildRequest = (): CreatePersonalProfileRequest | null => {
    if (!validateBirthStep() || !validateGuidedStep()) {
      return null;
    }

    let latitude = selectedLocation?.latitude ?? 0;
    let longitude = selectedLocation?.longitude ?? 0;
    let timezoneOffsetMinutes = 0;
    let birthPlace = form.birthPlace.trim();

    if (!showManualLocation && selectedLocation) {
      birthPlace = selectedLocation.placeName;

      try {
        const timezoneIana = tzlookup(latitude, longitude);
        const zonedDate = DateTime.fromISO(`${form.birthDate}T${form.birthTime}`, { zone: timezoneIana });

        timezoneOffsetMinutes = zonedDate.isValid ? zonedDate.offset : 0;
      } catch {
        timezoneOffsetMinutes = 0;
      }
    } else {
      latitude = Number(manualLocation.latitude.trim());
      longitude = Number(manualLocation.longitude.trim());
      timezoneOffsetMinutes = Number(manualLocation.timezoneOffsetMinutes.trim());
    }

    return {
      fullName: form.fullName.trim(),
      birthDate: form.birthDate,
      birthTime: form.birthTime,
      birthPlace,
      latitude,
      longitude,
      timezoneOffsetMinutes,
      selfPerceptionFocus: form.selfPerceptionFocus.trim(),
      currentChallenge: form.currentChallenge.trim(),
      desiredInsight: form.desiredInsight.trim(),
      selfDescription: form.selfDescription.trim() || undefined,
    };
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setSubmitError(null);

    const birthStepValid = validateBirthStep();
    const guidedStepValid = validateGuidedStep();

    if (!birthStepValid) {
      setCurrentStep('birth');
      return;
    }

    if (!guidedStepValid) {
      setCurrentStep('guided');
      return;
    }

    const request = buildRequest();

    if (!request) {
      return;
    }

    setSubmitting(true);

    try {
      const profile = await personalProfileService.createProfile(request);
      personalProfileStorage.saveLastProfile(profile);
      setCreatedProfile(profile);
    } catch (error) {
      setSubmitError(error instanceof Error ? error.message : 'No pudimos guardar tu perfil ahora mismo.');
    } finally {
      setSubmitting(false);
    }
  };

  const resetFlow = () => {
    setForm(initialFormState);
    setSelectedLocation(null);
    setManualLocation({
      latitude: '',
      longitude: '',
      timezoneOffsetMinutes: '',
    });
    setShowManualLocation(false);
    setErrors({});
    setSubmitError(null);
    setCreatedProfile(null);
    setCurrentStep('birth');
  };

  const applySelfDescriptionStarter = (starter: string) => {
    setForm((previous) => {
      if (previous.selfDescription.trim().length > 0) {
        return previous;
      }

      return {
        ...previous,
        selfDescription: starter,
      };
    });
  };

  const insertSelfDescriptionPrompt = (prompt: string) => {
    setForm((previous) => {
      const base = previous.selfDescription.trim();
      const addition = base.length > 0
        ? ` Me doy cuenta de ${prompt}.`
        : `Podrías empezar por contar ${prompt}.`;

      const nextValue = `${previous.selfDescription}${addition}`.slice(0, SELF_DESCRIPTION_MAX_LENGTH);

      return {
        ...previous,
        selfDescription: nextValue,
      };
    });
  };

  const renderBirthStep = () => (
    <div className="space-y-6">
      <div className="grid gap-5 md:grid-cols-2">
        <div className="space-y-2">
          <label htmlFor="fullName" className="text-sm font-medium text-text">
            Nombre
          </label>
          <div className="relative">
            <UserRound className="pointer-events-none absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-text-muted" />
            <input
              id="fullName"
              type="text"
              value={form.fullName}
              maxLength={120}
              onChange={(event) => setField('fullName', event.target.value)}
              placeholder="Como te gustaría que te nombremos"
              className={`w-full rounded-2xl border bg-background/70 py-3.5 pl-11 pr-4 text-sm text-white outline-none transition placeholder:text-text-muted/50 ${
                errors.fullName ? 'border-red-500/50' : 'border-white/10 focus:border-primary/40'
              }`}
            />
          </div>
          {errors.fullName && <p className="text-xs text-red-400">{errors.fullName}</p>}
        </div>

        <div className="space-y-2">
          <label htmlFor="birthDate" className="text-sm font-medium text-text">
            Fecha de nacimiento
          </label>
          <div className="relative">
            <Calendar className="pointer-events-none absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-text-muted" />
            <input
              id="birthDate"
              type="date"
              value={form.birthDate}
              onChange={(event) => setField('birthDate', event.target.value)}
              className={`w-full rounded-2xl border bg-background/70 py-3.5 pl-11 pr-4 text-sm text-white outline-none transition [color-scheme:dark] ${
                errors.birthDate ? 'border-red-500/50' : 'border-white/10 focus:border-primary/40'
              }`}
            />
          </div>
          {errors.birthDate && <p className="text-xs text-red-400">{errors.birthDate}</p>}
        </div>
      </div>

      <div className="grid gap-5 md:grid-cols-2">
        <div className="space-y-2">
          <label htmlFor="birthTime" className="text-sm font-medium text-text">
            Hora local exacta
          </label>
          <div className="relative">
            <Clock className="pointer-events-none absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-text-muted" />
            <input
              id="birthTime"
              type="time"
              value={form.birthTime}
              onChange={(event) => setField('birthTime', event.target.value)}
              className={`w-full rounded-2xl border bg-background/70 py-3.5 pl-11 pr-4 text-sm text-white outline-none transition [color-scheme:dark] ${
                errors.birthTime ? 'border-red-500/50' : 'border-white/10 focus:border-primary/40'
              }`}
            />
          </div>
          {errors.birthTime && <p className="text-xs text-red-400">{errors.birthTime}</p>}
        </div>

        <div className="rounded-2xl border border-white/8 bg-white/[0.03] p-4 text-sm leading-6 text-text-muted">
          <p className="text-white">Un dato importante</p>
          <p className="mt-2">
            La fecha, hora y lugar nos permiten anclar tu carta con precisión. Luego agregaremos una capa más humana:
            cómo te sientes hoy y qué quieres comprender mejor.
          </p>
        </div>
      </div>

      {!showManualLocation ? (
        <div className="space-y-2">
          <div className="flex items-center justify-between gap-3">
            <label className="text-sm font-medium text-text">Lugar de nacimiento</label>
            <button
              type="button"
              onClick={() => {
                setShowManualLocation(true);
                setSelectedLocation(null);
                clearLocationErrors();
              }}
              className="text-xs font-medium text-primary transition hover:text-primary-hover"
            >
              No encuentro mi lugar
            </button>
          </div>

          <LocationAutocomplete
            onSelect={(location) => {
              setSelectedLocation(location);
              setField('birthPlace', location.placeName);
              clearLocationErrors();
            }}
            error={errors.placeSearch}
            onClearError={clearLocationErrors}
            disabled={submitting}
          />

          {errors.placeSearch && <p className="text-xs text-red-400">{errors.placeSearch}</p>}

          {selectedLocation && (
            <div className="rounded-2xl border border-primary/20 bg-primary/10 px-4 py-3 text-sm text-primary">
              Seleccionaste {selectedLocation.placeName}. Ajustaremos la zona horaria según la fecha y hora exactas.
            </div>
          )}
        </div>
      ) : (
        <div className="space-y-5 rounded-[1.75rem] border border-white/8 bg-surface/70 p-5">
          <div className="flex items-center justify-between gap-4">
            <div>
              <p className="text-sm font-medium text-white">Ingreso manual</p>
              <p className="mt-1 text-xs text-text-muted">
                Pensado para casos donde la búsqueda no encuentra tu ciudad o necesitas máxima precisión técnica.
              </p>
            </div>
            <button
              type="button"
              onClick={() => {
                setShowManualLocation(false);
                setManualLocation({
                  latitude: '',
                  longitude: '',
                  timezoneOffsetMinutes: '',
                });
                setField('birthPlace', '');
                clearLocationErrors();
              }}
              className="text-xs font-medium text-primary transition hover:text-primary-hover"
            >
              Volver a buscador
            </button>
          </div>

          <div className="space-y-2">
            <label htmlFor="birthPlace" className="text-sm font-medium text-text">
              Nombre del lugar
            </label>
            <div className="relative">
              <MapPin className="pointer-events-none absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-text-muted" />
              <input
                id="birthPlace"
                type="text"
                value={form.birthPlace}
                maxLength={200}
                onChange={(event) => setField('birthPlace', event.target.value)}
                placeholder="Ej: Valparaiso, Chile"
                className={`w-full rounded-2xl border bg-background/70 py-3.5 pl-11 pr-4 text-sm text-white outline-none transition placeholder:text-text-muted/50 ${
                  errors.birthPlace ? 'border-red-500/50' : 'border-white/10 focus:border-primary/40'
                }`}
              />
            </div>
            {errors.birthPlace && <p className="text-xs text-red-400">{errors.birthPlace}</p>}
          </div>

          <div className="grid gap-4 md:grid-cols-3">
            <div className="space-y-2">
              <label htmlFor="manualLatitude" className="text-sm font-medium text-text">
                Latitud
              </label>
              <input
                id="manualLatitude"
                type="number"
                step="any"
                value={manualLocation.latitude}
                onChange={(event) => {
                  setManualLocation((previous) => ({ ...previous, latitude: event.target.value }));
                  setErrors((previous) => ({ ...previous, manualLatitude: '' }));
                }}
                placeholder="-33.0472"
                className={`w-full rounded-2xl border bg-background/70 px-4 py-3.5 text-sm text-white outline-none transition ${
                  errors.manualLatitude ? 'border-red-500/50' : 'border-white/10 focus:border-primary/40'
                }`}
              />
              {errors.manualLatitude && <p className="text-xs text-red-400">{errors.manualLatitude}</p>}
            </div>

            <div className="space-y-2">
              <label htmlFor="manualLongitude" className="text-sm font-medium text-text">
                Longitud
              </label>
              <input
                id="manualLongitude"
                type="number"
                step="any"
                value={manualLocation.longitude}
                onChange={(event) => {
                  setManualLocation((previous) => ({ ...previous, longitude: event.target.value }));
                  setErrors((previous) => ({ ...previous, manualLongitude: '' }));
                }}
                placeholder="-71.6127"
                className={`w-full rounded-2xl border bg-background/70 px-4 py-3.5 text-sm text-white outline-none transition ${
                  errors.manualLongitude ? 'border-red-500/50' : 'border-white/10 focus:border-primary/40'
                }`}
              />
              {errors.manualLongitude && <p className="text-xs text-red-400">{errors.manualLongitude}</p>}
            </div>

            <div className="space-y-2">
              <label htmlFor="manualOffset" className="text-sm font-medium text-text">
                Offset horario
              </label>
              <input
                id="manualOffset"
                type="number"
                step="1"
                value={manualLocation.timezoneOffsetMinutes}
                onChange={(event) => {
                  setManualLocation((previous) => ({
                    ...previous,
                    timezoneOffsetMinutes: event.target.value,
                  }));
                  setErrors((previous) => ({ ...previous, manualTimezoneOffsetMinutes: '' }));
                }}
                placeholder="-180"
                className={`w-full rounded-2xl border bg-background/70 px-4 py-3.5 text-sm text-white outline-none transition ${
                  errors.manualTimezoneOffsetMinutes ? 'border-red-500/50' : 'border-white/10 focus:border-primary/40'
                }`}
              />
              {errors.manualTimezoneOffsetMinutes && (
                <p className="text-xs text-red-400">{errors.manualTimezoneOffsetMinutes}</p>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );

  const renderGuidedStep = () => (
    <div className="space-y-5">
      <GuidedQuestionCard
        id="selfPerceptionFocus"
        title="¿Qué sientes que más te define hoy?"
        description="Puede ser una fuerza, un rasgo, una emoción dominante o una forma de estar en el mundo."
        placeholder="Ej: Siento que hoy me define mucho mi sensibilidad y la necesidad de encontrar algo más auténtico."
        value={form.selfPerceptionFocus}
        maxLength={280}
        suggestions={QUESTION_SUGGESTIONS.selfPerceptionFocus}
        error={errors.selfPerceptionFocus}
        disabled={submitting}
        onChange={(value) => setField('selfPerceptionFocus', value)}
      />

      <GuidedQuestionCard
        id="currentChallenge"
        title="¿Qué te cuesta más hoy?"
        description="No hace falta explicarlo perfecto. Basta con señalar lo que hoy se siente más tenso o más confuso."
        placeholder="Ej: Me cuesta sostener claridad emocional cuando siento presión externa."
        value={form.currentChallenge}
        maxLength={280}
        suggestions={QUESTION_SUGGESTIONS.currentChallenge}
        error={errors.currentChallenge}
        disabled={submitting}
        onChange={(value) => setField('currentChallenge', value)}
      />

      <GuidedQuestionCard
        id="desiredInsight"
        title="¿Qué te gustaría entender mejor de ti?"
        description="Esta respuesta nos ayuda a orientar la lectura hacia lo que hoy más sentido tiene para ti."
        placeholder="Ej: Quiero entender por qué me cuesta confiar en mi intuición cuando más la necesito."
        value={form.desiredInsight}
        maxLength={280}
        suggestions={QUESTION_SUGGESTIONS.desiredInsight}
        error={errors.desiredInsight}
        disabled={submitting}
        onChange={(value) => setField('desiredInsight', value)}
      />
    </div>
  );

  const renderVoiceStep = () => (
    <div className="grid gap-6 lg:grid-cols-[1.15fr_0.85fr]">
      <section className="rounded-[1.75rem] border border-white/8 bg-surface/80 p-6 shadow-[0_20px_70px_rgba(0,0,0,0.28)]">
        <div className="mb-4">
          <p className="text-sm font-medium text-white">Si tuvieras que describirte en pocas líneas...</p>
          <p className="mt-1 text-sm leading-6 text-text-muted">
            Esta parte es opcional. Piensa en ella como una pequeña nota tuya para enriquecer la lectura, no como una definición perfecta de quién eres.
          </p>
        </div>

        <div className="mb-5 rounded-2xl border border-primary/12 bg-primary/10 p-4">
          <p className="text-xs uppercase tracking-[0.22em] text-primary/80">Ayuda suave</p>
          <p className="mt-2 text-sm leading-6 text-text-muted">
            Puedes escribir algo breve y simple. Una o dos frases son suficientes. Si te bloqueas, usa uno de estos comienzos y sigue desde ahí.
          </p>

          <div className="mt-4 flex flex-wrap gap-2">
            {SELF_DESCRIPTION_STARTERS.map((starter) => (
              <button
                key={starter.label}
                type="button"
                disabled={submitting || form.selfDescription.trim().length > 0}
                onClick={() => applySelfDescriptionStarter(starter.text)}
                className="rounded-full border border-white/10 bg-white/5 px-3 py-1.5 text-xs text-text-muted transition hover:border-primary/25 hover:bg-primary/10 hover:text-primary disabled:cursor-not-allowed disabled:opacity-50"
              >
                {starter.label}
              </button>
            ))}
          </div>
        </div>

        <textarea
          value={form.selfDescription}
          maxLength={SELF_DESCRIPTION_MAX_LENGTH}
          disabled={submitting}
          onChange={(event) => setField('selfDescription', event.target.value)}
          placeholder="Ej: Me considero alguien sensible, observador y muy movedizo por dentro. Estoy aprendiendo a confiar más en lo que siento sin exigirme tener todo resuelto."
          className="min-h-[220px] w-full rounded-2xl border border-white/10 bg-background/70 px-4 py-4 text-sm leading-6 text-white outline-none transition placeholder:text-text-muted/50 focus:border-primary/40"
        />

        <div className="mt-4 flex flex-wrap gap-2">
          {SELF_DESCRIPTION_PROMPTS.map((prompt) => (
            <button
              key={prompt}
              type="button"
              disabled={submitting}
              onClick={() => insertSelfDescriptionPrompt(prompt)}
              className="rounded-full border border-white/10 bg-white/[0.04] px-3 py-1.5 text-xs text-text-muted transition hover:border-primary/25 hover:bg-primary/10 hover:text-primary"
            >
              {prompt}
            </button>
          ))}
        </div>

        <div className="mt-3 flex items-center justify-between">
          <span className="text-xs text-text-muted/70">
            Una voz propia suele decir más que una definición perfecta.
          </span>
          <span className="text-xs text-text-muted/70">
            {form.selfDescription.trim().length}/{SELF_DESCRIPTION_MAX_LENGTH}
          </span>
        </div>
      </section>

      <aside className="rounded-[1.75rem] border border-primary/12 bg-gradient-to-br from-primary/12 via-white/[0.03] to-transparent p-6 shadow-[0_24px_80px_rgba(0,0,0,0.24)]">
        <div className="mb-5 flex items-center gap-3">
          <div className="flex h-10 w-10 items-center justify-center rounded-full border border-primary/20 bg-primary/10 text-primary">
            <Stars className="h-5 w-5" />
          </div>
          <div>
            <p className="text-sm font-medium text-white">Lo que estamos construyendo</p>
            <p className="text-xs text-text-muted">Una base clara para lecturas futuras más personales.</p>
          </div>
        </div>

        <div className="space-y-4 text-sm">
          <div className="rounded-2xl border border-white/8 bg-black/10 p-4">
            <p className="text-xs uppercase tracking-[0.22em] text-primary/80">Nombre</p>
            <p className="mt-2 text-white">{form.fullName || 'Tu nombre aparecerá aquí'}</p>
          </div>

          <div className="rounded-2xl border border-white/8 bg-black/10 p-4">
            <p className="text-xs uppercase tracking-[0.22em] text-primary/80">Enfoque actual</p>
            <p className="mt-2 leading-6 text-text-muted">
              {form.selfPerceptionFocus || 'Tu respuesta del paso anterior aparecerá aquí.'}
            </p>
          </div>

          <div className="rounded-2xl border border-white/8 bg-black/10 p-4">
            <p className="text-xs uppercase tracking-[0.22em] text-primary/80">Qué quieres comprender</p>
            <p className="mt-2 leading-6 text-text-muted">
              {form.desiredInsight || 'Tu intención de lectura aparecerá aquí.'}
            </p>
          </div>
        </div>
      </aside>
    </div>
  );

  if (createdProfile) {
    return (
      <div className="rounded-[2rem] border border-primary/20 bg-gradient-to-br from-primary/12 via-surfaceHighlight/80 to-surface/90 p-8 shadow-[0_25px_100px_rgba(0,0,0,0.35)]">
        <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
          <div className="max-w-2xl">
            <div className="mb-4 inline-flex items-center gap-2 rounded-full border border-primary/20 bg-primary/10 px-4 py-1.5 text-xs font-medium uppercase tracking-[0.22em] text-primary">
              <Check className="h-3.5 w-3.5" />
              Perfil creado
            </div>
            <h2 className="text-3xl font-semibold text-white sm:text-4xl">Tu perfil base ya está listo</h2>
            <p className="mt-3 text-base leading-7 text-text-muted">
              Guardamos tus datos natales y el contexto personal que más importa hoy. Esta base ya puede acompañar lecturas futuras con una capa más íntima y humana.
            </p>
          </div>
          <button
            type="button"
            onClick={resetFlow}
            className="inline-flex items-center justify-center rounded-full border border-white/10 bg-white/5 px-5 py-2.5 text-sm font-medium text-white transition hover:bg-white/10"
          >
            Crear otro perfil
          </button>
        </div>

        <div className="mt-8 grid gap-4 md:grid-cols-2 xl:grid-cols-4">
          <div className="rounded-2xl border border-white/8 bg-black/10 p-4">
            <p className="text-xs uppercase tracking-[0.22em] text-primary/80">Nombre</p>
            <p className="mt-2 text-white">{createdProfile.fullName}</p>
          </div>
          <div className="rounded-2xl border border-white/8 bg-black/10 p-4">
            <p className="text-xs uppercase tracking-[0.22em] text-primary/80">Nacimiento</p>
            <p className="mt-2 text-white">
              {createdProfile.birthDate} · {createdProfile.birthTime}
            </p>
            <p className="mt-1 text-sm text-text-muted">{createdProfile.birthPlace}</p>
          </div>
          <div className="rounded-2xl border border-white/8 bg-black/10 p-4">
            <p className="text-xs uppercase tracking-[0.22em] text-primary/80">Lo que hoy te define</p>
            <p className="mt-2 text-sm leading-6 text-text-muted">{createdProfile.selfPerceptionFocus}</p>
          </div>
          <div className="rounded-2xl border border-white/8 bg-black/10 p-4">
            <p className="text-xs uppercase tracking-[0.22em] text-primary/80">Lo que quieres entender</p>
            <p className="mt-2 text-sm leading-6 text-text-muted">{createdProfile.desiredInsight}</p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <form
      onSubmit={handleSubmit}
      className="overflow-hidden rounded-[2rem] border border-white/8 bg-[radial-gradient(circle_at_top,rgba(212,175,55,0.14),transparent_35%),linear-gradient(180deg,rgba(26,26,32,0.96),rgba(10,10,11,0.98))] shadow-[0_28px_120px_rgba(0,0,0,0.42)]"
      noValidate
    >
      <div className="grid gap-0 xl:grid-cols-[0.95fr_1.35fr]">
        <aside className="border-b border-white/8 p-6 xl:border-b-0 xl:border-r xl:p-8">
          <div className="mb-8">
            <div className="inline-flex items-center gap-2 rounded-full border border-primary/20 bg-primary/10 px-4 py-1.5 text-xs font-medium uppercase tracking-[0.22em] text-primary">
              <Sparkles className="h-3.5 w-3.5" />
              Onboarding asistido
            </div>
            <h2 className="mt-5 text-3xl font-semibold text-white sm:text-[2.25rem]">
              Una lectura más cercana empieza con contexto, no con fricción.
            </h2>
            <p className="mt-4 text-sm leading-7 text-text-muted">
              En tres pasos breves reunimos tu base natal y un poco de tu presente. Sin preguntas eternas. Sin tono burocrático.
            </p>
          </div>

          <div className="space-y-4">
            {STEP_ORDER.map((step, index) => {
              const isActive = step === currentStep;
              const isComplete = index < currentStepIndex;
              const stepMeta = STEP_META[step];

              return (
                <button
                  key={step}
                  type="button"
                  onClick={() => setCurrentStep(step)}
                  className={`flex w-full items-start gap-4 rounded-[1.5rem] border px-4 py-4 text-left transition ${
                    isActive
                      ? 'border-primary/30 bg-primary/10'
                      : 'border-white/8 bg-white/[0.03] hover:bg-white/[0.05]'
                  }`}
                >
                  <div
                    className={`flex h-10 w-10 shrink-0 items-center justify-center rounded-full border text-sm font-semibold ${
                      isComplete
                        ? 'border-primary/30 bg-primary text-background'
                        : isActive
                          ? 'border-primary/30 bg-primary/15 text-primary'
                          : 'border-white/10 bg-background/70 text-text-muted'
                    }`}
                  >
                    {isComplete ? <Check className="h-4 w-4" /> : index + 1}
                  </div>
                  <div>
                    <p className="text-xs uppercase tracking-[0.22em] text-primary/80">{stepMeta.eyebrow}</p>
                    <p className="mt-1 text-base font-medium text-white">{stepMeta.title}</p>
                    <p className="mt-1 text-sm leading-6 text-text-muted">{stepMeta.description}</p>
                  </div>
                </button>
              );
            })}
          </div>
        </aside>

        <div className="p-6 sm:p-8 xl:p-10">
          <div className="mb-8 flex flex-col gap-4 border-b border-white/8 pb-6 md:flex-row md:items-end md:justify-between">
            <div>
              <p className="text-xs uppercase tracking-[0.22em] text-primary/80">{currentMeta.eyebrow}</p>
              <h3 className="mt-2 text-2xl font-semibold text-white sm:text-3xl">{currentMeta.title}</h3>
              <p className="mt-3 max-w-2xl text-sm leading-7 text-text-muted">{currentMeta.description}</p>
            </div>
            <div className="text-sm text-text-muted">
              {currentStepIndex + 1} de {STEP_ORDER.length}
            </div>
          </div>

          {currentStep === 'birth' && renderBirthStep()}
          {currentStep === 'guided' && renderGuidedStep()}
          {currentStep === 'voice' && renderVoiceStep()}

          {submitError && (
            <div className="mt-6 rounded-2xl border border-red-500/25 bg-red-500/10 px-4 py-3 text-sm text-red-300">
              {submitError}
            </div>
          )}

          <div className="mt-8 flex flex-col gap-3 border-t border-white/8 pt-6 sm:flex-row sm:items-center sm:justify-between">
            <button
              type="button"
              onClick={goToPreviousStep}
              disabled={currentStepIndex === 0 || submitting}
              className="inline-flex items-center justify-center gap-2 rounded-full border border-white/10 bg-white/5 px-5 py-3 text-sm font-medium text-white transition hover:bg-white/10 disabled:cursor-not-allowed disabled:opacity-50"
            >
              <ChevronLeft className="h-4 w-4" />
              Volver
            </button>

            {currentStep !== 'voice' ? (
              <button
                type="button"
                onClick={goToNextStep}
                disabled={submitting}
                className="inline-flex items-center justify-center gap-2 rounded-full bg-primary px-6 py-3 text-sm font-semibold text-background shadow-[0_0_20px_rgba(212,175,55,0.25)] transition hover:bg-primary-hover disabled:cursor-not-allowed disabled:opacity-60"
              >
                Continuar
                <ChevronRight className="h-4 w-4" />
              </button>
            ) : (
              <button
                type="submit"
                disabled={submitting}
                className="inline-flex items-center justify-center gap-2 rounded-full bg-primary px-6 py-3 text-sm font-semibold text-background shadow-[0_0_24px_rgba(212,175,55,0.28)] transition hover:bg-primary-hover disabled:cursor-not-allowed disabled:opacity-60"
              >
                {submitting ? (
                  <>
                    <Loader2 className="h-4 w-4 animate-spin" />
                    Guardando tu perfil...
                  </>
                ) : (
                  <>
                    Crear mi perfil
                    <Sparkles className="h-4 w-4" />
                  </>
                )}
              </button>
            )}
          </div>
        </div>
      </div>
    </form>
  );
};
