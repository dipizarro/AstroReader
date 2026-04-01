import { Link } from 'react-router-dom';
import { Sparkles, Star } from 'lucide-react';

export const HomePage = () => {
  return (
    <div className="flex min-h-full flex-col items-center justify-center p-6 text-center lg:p-12">
      <div className="relative mb-12 flex max-w-4xl flex-col items-center">
        {/* Subtle background glow effect behind text */}
        <div className="absolute inset-x-0 -top-40 -z-10 transform-gpu overflow-hidden blur-3xl sm:-top-80">
          <div
            className="relative left-[calc(50%-11rem)] aspect-[1155/678] w-[36.125rem] -translate-x-1/2 rotate-[30deg] bg-gradient-to-tr from-[#6b4c9a] to-[#d4af37] opacity-20 sm:left-[calc(50%-30rem)] sm:w-[72.1875rem]"
            style={{
              clipPath:
                'polygon(74.1% 44.1%, 100% 61.6%, 97.5% 26.9%, 85.5% 0.1%, 80.7% 2%, 72.5% 32.5%, 60.2% 62.4%, 52.4% 68.1%, 47.5% 58.3%, 45.2% 34.5%, 27.5% 76.7%, 0.1% 64.9%, 17.9% 100%, 27.6% 76.8%, 76.1% 97.7%, 74.1% 44.1%)',
            }}
          />
        </div>

        <h1 className="mb-6 max-w-2xl font-display text-4xl font-semibold leading-tight tracking-tight sm:text-6xl text-glow">
          Descubre el cosmos <br /> en tu interior
        </h1>
        <p className="mb-10 max-w-xl text-lg text-text-muted sm:text-xl">
          AstroReader te ofrece una lectura precisa, profunda y elegante de tu carta natal. 
          Desentraña los misterios de tu diseño cósmico.
        </p>

        <div className="flex flex-col gap-4 sm:flex-row">
          <Link
            to="/chart/calculate"
            className="group relative inline-flex items-center justify-center gap-2 overflow-hidden rounded-full bg-primary px-8 py-3 text-sm font-semibold text-[#0a0a0b] transition-all hover:bg-primary-hover shadow-[0_0_20px_rgba(212,175,55,0.4)] hover:shadow-[0_0_30px_rgba(212,175,55,0.6)]"
          >
            <Sparkles className="h-4 w-4" />
            <span>Calcular mi Carta Natal</span>
          </Link>
          <button className="inline-flex items-center justify-center gap-2 rounded-full border border-white/10 bg-white/5 px-8 py-3 text-sm font-medium text-white transition-all hover:bg-white/10 glass-panel">
            <Star className="h-4 w-4 text-primary" />
            <span>Conocer más</span>
          </button>
        </div>
      </div>

      <div className="grid w-full max-w-5xl grid-cols-1 gap-8 md:grid-cols-3">
        {[
          {
            title: 'Precisión Astronómica',
            description: 'Efemérides exactas suizas para cálculos detallados punto por punto.',
          },
          {
            title: 'Interpretación Profunda',
            description: 'Textos elaborados que conectan símbolos celestes con el crecimiento personal.',
          },
          {
            title: 'Privacidad Absoluta',
            description: 'Tus datos nunca se comparten. Lo que pasa en el universo, se queda en el universo.',
          },
        ].map((feature, i) => (
          <div
            key={i}
            className="flex flex-col items-center rounded-2xl glass-panel p-6 text-center transition-transform hover:-translate-y-1"
          >
            <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-full bg-primary/10">
              <Star className="h-6 w-6 text-primary" strokeWidth={1.5} />
            </div>
            <h3 className="mb-2 text-lg font-medium text-white">{feature.title}</h3>
            <p className="text-sm text-text-muted">{feature.description}</p>
          </div>
        ))}
      </div>
    </div>
  );
};
