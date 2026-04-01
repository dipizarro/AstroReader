const BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

class ApiError extends Error {
  status: number;
  constructor(status: number, message: string) {
    super(message);
    this.status = status;
    this.name = 'ApiError';
  }
}

export const apiClient = {
  async post<T, U = unknown>(endpoint: string, body: U): Promise<T> {
    const url = `${BASE_URL}${endpoint}`;
    
    // Configuración por defecto de los headers
    const headers: HeadersInit = {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
    };

    try {
      const response = await fetch(url, {
        method: 'POST',
        headers,
        body: JSON.stringify(body),
      });

      if (!response.ok) {
        // Tratar de obtener el mensaje de error del backend
        const errorData = await response.json().catch(() => ({}));
        throw new ApiError(
          response.status, 
          errorData.message || errorData.title || `Error ${response.status}: Ha ocurrido un problema al comunicar con el servidor.`
        );
      }

      // Si la respuesta no tiene body (ej. 204), retornamos as T vacío
      if (response.status === 204) {
        return {} as T;
      }

      return await response.json();
    } catch (error) {
      if (error instanceof ApiError) {
        throw error;
      }
      throw new Error(error instanceof Error ? error.message : 'Error desconocido de conexión');
    }
  }
  
  // get<T>... put<T>... delete<T>... pueden añadirse aquí luego.
};
