// src/services/chatService.ts
import { Chat, CreateChatRequest } from '@/types/Chat';

const API_URL = `${process.env.NEXT_PUBLIC_API_URL}/api/chats`;
let token: string | null = null;

// Permite guardar el JWT desde login
export const chatService = {
  setToken: (jwt: string) => {
    token = jwt;
  },

  // 🔹 Obtener todos los chats
  async getAll(): Promise<Chat[]> {
    const res = await fetch(API_URL, {
      headers: {
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
    });

    if (!res.ok) {
      const errorText = await res.text();
      throw new Error(`Error al obtener chats: ${res.status} - ${errorText}`);
    }
    return res.json();
  },

  // 🔹 Obtener chat por ID
  async getById(id: string): Promise<Chat> {
    const res = await fetch(`${API_URL}/${id}`, {
      headers: {
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
    });

    if (!res.ok) {
      const errorText = await res.text();
      throw new Error(`Error al obtener chat: ${res.status} - ${errorText}`);
    }
    return res.json();
  },

  // 🔹 Crear chat (incluye userId si lo necesitas)
async createChat(chatData: CreateChatRequest): Promise<Chat> {
  const res = await fetch(API_URL, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    body: JSON.stringify(chatData), // AHORA SOLO TIENE 3 CAMPOS
  });

  if (!res.ok) {
    throw new Error(`Error al crear chat`);
  }

  return res.json();
},


  // 🔹 Actualizar chat
  async update(id: string, data: any): Promise<Chat> {
    const res = await fetch(`${API_URL}/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
      body: JSON.stringify(data),
    });

    if (!res.ok) {
      const errorText = await res.text();
      throw new Error(`Error al actualizar chat: ${res.status} - ${errorText}`);
    }
    return res.json();
  },

  // 🔹 Eliminar chat
  async delete(id: string): Promise<void> {
    const res = await fetch(`${API_URL}/${id}`, {
      method: 'DELETE',
      headers: {
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
    });

    if (!res.ok) {
      const errorText = await res.text();
      throw new Error(`Error al eliminar chat: ${res.status} - ${errorText}`);
    }
  },
};
