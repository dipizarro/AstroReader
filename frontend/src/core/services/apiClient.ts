const BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

class ApiError extends Error {
  status: number;
  constructor(status: number, message: string) {
    super(message);
    this.status = status;
    this.name = 'ApiError';
  }
}

export const apiClient = {
  async get<T>(endpoint: string): Promise<T> {
    const url = `${BASE_URL}${endpoint}`;

    const headers: HeadersInit = {
      'Accept': 'application/json',
    };

    try {
      const response = await fetch(url, {
        method: 'GET',
        headers,
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        const validationDetails = extractValidationDetails(errorData);
        throw new ApiError(
          response.status,
          validationDetails || errorData.message || errorData.detail || errorData.title || `Error ${response.status}: Ha ocurrido un problema al comunicar con el servidor.`
        );
      }

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
  },

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
        const validationDetails = extractValidationDetails(errorData);
        throw new ApiError(
          response.status, 
          validationDetails || errorData.message || errorData.detail || errorData.title || `Error ${response.status}: Ha ocurrido un problema al comunicar con el servidor.`
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

function extractValidationDetails(errorData: unknown): string | null {
  if (!errorData || typeof errorData !== 'object' || !('errors' in errorData)) {
    return null;
  }

  const errors = (errorData as { errors?: Record<string, string[] | string> }).errors;

  if (!errors || typeof errors !== 'object') {
    return null;
  }

  const messages = Object.values(errors)
    .flatMap((value) => Array.isArray(value) ? value : [value])
    .filter((value): value is string => typeof value === 'string' && value.trim().length > 0);

  return messages.length > 0 ? messages.join(' ') : null;
}
