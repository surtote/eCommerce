// src/services/userService.ts
import { User, CreateUserRequest } from '@/types/User';

const API_URL = `${process.env.NEXT_PUBLIC_API_URL}/api/user`;
const API_URL_AUTH = `${process.env.NEXT_PUBLIC_API_URL}/api/v1/auth`;

let token: string | null = null; // Token JWT global
let userId: string | null = null; // 🔹 UserID global (nuevo)

export const userService = {
  // 🔹 Configurar token JWT (después del login)
  setToken: (jwt: string) => {
    token = jwt;
  },

  // 🔹 Configurar userId (nuevo)
  setUserId: (id: string) => {
    userId = id;
    console.log("📌 [userService] userId seteado:", userId);
  },

  // 🔹 Obtener el userId cuando lo necesites
  getUserId: () => userId,

  // 🔹 Obtener todos los usuarios
  async getUsers(): Promise<User[]> {
    const res = await fetch(API_URL, {
      headers: {
        'Content-Type': 'application/json',
      },
    });
    if (!res.ok) throw new Error('Error al obtener usuarios');
    return res.json();
  },

  // 🔹 Obtener usuario por ID
  async getUserById(id: string): Promise<User> {
    const res = await fetch(`${API_URL}/${id}`, {
      headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
    });
    if (!res.ok) throw new Error('Error al obtener usuario');
    return res.json();
  },

  // 🔹 Crear usuario
  async createUser(userData: CreateUserRequest): Promise<User> {
    const requestData = {
      userName: userData.userName,
      nombre: userData.nombre,
      apellido: userData.apellido,
      dni: userData.dni,
      email: userData.email,
      direccion: userData.direccion || null,
      telefono: userData.telefono || null,
      password: userData.password,
    };

    const res = await fetch(`${API_URL_AUTH}/register`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
      body: JSON.stringify(requestData),
    });

    if (!res.ok) {
      const error = await res.json();
      throw new Error(error.message || 'Error al crear usuario');
    }

    return res.json();
  },

  // 🔹 Login
  async login(credentials: { userName: string; password: string }): Promise<{ user: User; token: string }> {
    const res = await fetch(`${API_URL_AUTH}/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(credentials),
    });

    if (!res.ok) {
      const errorText = await res.text();
      throw new Error(errorText || 'Error al iniciar sesión');
    }

    const data = await res.json();

    // Log para ver qué viene del backend
    console.log("📌 [userService] login -> data:", data);
    console.log("📌 [userService] login -> user.id recibido:", data?.user?.id);

    if (!data.token) throw new Error('No se recibió token del servidor');

    // Guardar token y userId igual que token
    userService.setToken(data.token);
    userService.setUserId(data.user.id);

    return { user: data.user, token: data.token };
  },

  // 🔹 Actualizar usuario
  async updateUser(id: string, userData: Partial<User>): Promise<User> {
    const res = await fetch(`${API_URL}/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
      body: JSON.stringify(userData),
    });

    if (!res.ok) {
      const error = await res.json();
      throw new Error(error.message || 'Error al actualizar usuario');
    }

    return res.json();
  },

  // 🔹 Eliminar usuario
  async deleteUser(id: string): Promise<void> {
    const res = await fetch(`${API_URL}/${id}`, {
      method: 'DELETE',
      headers: {
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
    });

    if (!res.ok) throw new Error('Error al eliminar usuario');
  },
};
