export interface User {
  id: string;
  userName: string;
  nombre: string;
  apellido: string;
  dni: string;
  email: string;
  telefono?: number;
  direccion?: string;
  passwordHash: string; // Ahora sabemos que existe
  createdAt: string;
  updatedAt?: string;
}

export interface CreateUserRequest {
  userName: string;
  nombre: string;
  apellido: string;
  dni: string;
  email: string;
  telefono?: number;
  direccion?: string;
  password: string; // En el frontend usamos "password", se transforma a "passwordHash"
}

export interface LoginRequest {
  userName: string;
  password: string;
}

// Para actualizar usuario
export interface UpdateUserRequest {
  userName: string;
  nombre: string;
  apellido: string;
  dni: string;
  email: string;
  telefono?: number;
  direccion?: string;
  password?: string; // Opcional para actualización
}