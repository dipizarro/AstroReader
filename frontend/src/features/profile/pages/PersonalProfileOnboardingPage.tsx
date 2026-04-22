import { Link } from 'react-router-dom';
import { ArrowRight, Orbit } from 'lucide-react';
import { PersonalProfileOnboardingForm } from '../components/PersonalProfileOnboardingForm';

export const PersonalProfileOnboardingPage = () => {
  return (
    <div className="min-h-screen px-6 py-10 lg:px-10 lg:py-14">
      <div className="mx-auto max-w-7xl">
        <div className="mb-10 grid gap-6 lg:grid-cols-[1.1fr_0.9fr] lg:items-end">
          <div>
            <div className="inline-flex items-center gap-2 rounded-full border border-primary/20 bg-primary/10 px-4 py-1.5 text-xs font-medium uppercase tracking-[0.22em] text-primary">
              <Orbit className="h-3.5 w-3.5" />
              Sprint 2
            </div>
            <h1 className="mt-5 max-w-4xl font-display text-4xl font-semibold leading-tight text-glow sm:text-5xl lg:text-6xl">
              Un onboarding breve para entender tu carta y también tu momento.
            </h1>
            <p className="mt-5 max-w-3xl text-base leading-8 text-text-muted sm:text-lg">
              AstroReader ya puede leer tu diseño astral. Este paso agrega una capa más humana: cómo te sientes hoy,
              qué se te hace difícil y qué te gustaría comprender mejor de ti.
            </p>
          </div>

          <div className="rounded-[1.75rem] border border-white/8 bg-white/[0.03] p-6 shadow-[0_16px_60px_rgba(0,0,0,0.22)]">
            <p className="text-xs uppercase tracking-[0.22em] text-primary/80">Qué incluye</p>
            <ul className="mt-4 space-y-3 text-sm leading-6 text-text-muted">
              <li>Nombre, fecha, hora y lugar para anclar tu carta con precisión.</li>
              <li>Tres preguntas guiadas para dar contexto emocional y vital.</li>
              <li>Una nota libre opcional para sumar matiz sin alargar el proceso.</li>
            </ul>
            <Link
              to="/chart/calculate"
              className="mt-6 inline-flex items-center gap-2 text-sm font-medium text-primary transition hover:text-primary-hover"
            >
              Prefiero ir directo al cálculo clásico
              <ArrowRight className="h-4 w-4" />
            </Link>
          </div>
        </div>

        <PersonalProfileOnboardingForm />
      </div>
    </div>
  );
};
