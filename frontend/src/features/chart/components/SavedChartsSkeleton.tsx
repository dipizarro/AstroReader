export const SavedChartsSkeleton = () => {
  return (
    <div className="grid grid-cols-1 gap-6 xl:grid-cols-2">
      {Array.from({ length: 4 }).map((_, index) => (
        <div
          key={index}
          className="overflow-hidden rounded-3xl border border-white/8 bg-white/[0.03] p-6"
        >
          <div className="mb-5 h-6 w-36 animate-pulse rounded-full bg-white/8" />
          <div className="mb-3 h-8 w-3/4 animate-pulse rounded-xl bg-white/10" />
          <div className="mb-6 h-4 w-40 animate-pulse rounded bg-white/8" />

          <div className="grid gap-3 sm:grid-cols-2">
            <div className="h-24 animate-pulse rounded-2xl bg-white/8" />
            <div className="h-24 animate-pulse rounded-2xl bg-white/8" />
          </div>

          <div className="mt-6 h-10 animate-pulse rounded-2xl bg-white/8" />
        </div>
      ))}
    </div>
  );
};
