import type { MouseEvent } from 'react';
import { Sparkles } from 'lucide-react';

interface GuidedQuestionCardProps {
  id: string;
  title: string;
  description: string;
  placeholder: string;
  value: string;
  maxLength: number;
  suggestions: string[];
  error?: string;
  disabled?: boolean;
  onChange: (value: string) => void;
}

export const GuidedQuestionCard = ({
  id,
  title,
  description,
  placeholder,
  value,
  maxLength,
  suggestions,
  error,
  disabled,
  onChange,
}: GuidedQuestionCardProps) => {
  const handleSuggestionClick = (event: MouseEvent<HTMLButtonElement>, suggestion: string) => {
    event.preventDefault();
    onChange(suggestion);
  };

  return (
    <section className="rounded-[1.75rem] border border-white/8 bg-surface/80 p-5 shadow-[0_20px_70px_rgba(0,0,0,0.28)]">
      <div className="mb-4 flex items-start gap-3">
        <div className="mt-1 flex h-9 w-9 shrink-0 items-center justify-center rounded-full border border-primary/20 bg-primary/10 text-primary">
          <Sparkles className="h-4 w-4" />
        </div>
        <div>
          <h3 className="text-lg font-medium text-white">{title}</h3>
          <p className="mt-1 text-sm leading-6 text-text-muted">{description}</p>
        </div>
      </div>

      <div className="mb-4 flex flex-wrap gap-2">
        {suggestions.map((suggestion) => (
          <button
            key={suggestion}
            type="button"
            disabled={disabled}
            onClick={(event) => handleSuggestionClick(event, suggestion)}
            className="rounded-full border border-white/10 bg-white/5 px-3 py-1.5 text-left text-xs text-text-muted transition hover:border-primary/25 hover:bg-primary/10 hover:text-primary disabled:cursor-not-allowed disabled:opacity-60"
          >
            {suggestion}
          </button>
        ))}
      </div>

      <label htmlFor={id} className="sr-only">
        {title}
      </label>
      <textarea
        id={id}
        value={value}
        maxLength={maxLength}
        disabled={disabled}
        onChange={(event) => onChange(event.target.value)}
        placeholder={placeholder}
        className={`min-h-[124px] w-full rounded-2xl border bg-background/70 px-4 py-4 text-sm leading-6 text-white outline-none transition placeholder:text-text-muted/50 ${
          error
            ? 'border-red-500/50 focus:border-red-500/60'
            : 'border-white/10 focus:border-primary/40'
        }`}
      />

      <div className="mt-2 flex items-center justify-between gap-4">
        <span className={`text-xs ${error ? 'text-red-400' : 'text-text-muted/70'}`}>
          {error || 'Puedes escribir con tus palabras. Una o dos frases bastan.'}
        </span>
        <span className="text-xs text-text-muted/70">
          {value.trim().length}/{maxLength}
        </span>
      </div>
    </section>
  );
};
