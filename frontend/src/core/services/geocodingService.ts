export interface GeocodingResult {
  id: string;
  placeName: string;
  latitude: number;
  longitude: number;
}

export const geocodingService = {
  async searchPlaces(query: string): Promise<GeocodingResult[]> {
    if (!query || query.trim().length < 3) return [];

    const token = import.meta.env.VITE_MAPBOX_TOKEN;
    
    if (!token) {
      console.warn("Falta VITE_MAPBOX_TOKEN. Retornando resultados de prueba en consola.");
      // Fallback amigable si el desarrollador no ha puesto el token aún para que no rompa la aplicación:
      return [
         { id: '1', placeName: `${query} (Stubb/No Token)`, latitude: -34.6037, longitude: -58.3816 }
      ];
    }

    try {
      const url = `https://api.mapbox.com/geocoding/v5/mapbox.places/${encodeURIComponent(query)}.json?types=place,locality,neighborhood&language=es&access_token=${token}`;
      
      const response = await fetch(url);
      if (!response.ok) throw new Error("Error en geocoding API");
      
      const data = await response.json();
      
      return data.features.map((f: any) => ({
        id: f.id,
        placeName: f.place_name,
        longitude: f.center[0],
        latitude: f.center[1]
      }));
      
    } catch (e) {
      console.error("Geocoding fetch failed:", e);
      return [];
    }
  }
};
