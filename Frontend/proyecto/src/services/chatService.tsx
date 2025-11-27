// src/services/chatService.ts
import { Chat } from '@/types/Chat';

const API_URL = `${process.env.NEXT_PUBLIC_API_URL}/api/chats`;

export type CreateChatRequest = {
  sellerId: string;
  buyerId: string;
  productId: string;
};

export type UpdateChatRequest = {
  sellerId?: string;
  buyerId?: string;
  productId?: string;
};

export const chatService = {
  // 🔹 Obtener todos los chats
  async getAll(): Promise<Chat[]> {
    const res = await fetch(API_URL, { credentials: 'include' });
    if (!res.ok) {
      const errorText = await res.text();
      throw new Error(`Error al obtener chats: ${res.status} - ${errorText}`);
    }
    return res.json();
  },

  // 🔹 Obtener chat por ID
  async getById(id: string): Promise<Chat> {
    const res = await fetch(`${API_URL}/${id}`, { credentials: 'include' });
    if (!res.ok) {
      const errorText = await res.text();
      throw new Error(`Error al obtener chat: ${res.status} - ${errorText}`);
    }
    return res.json();
  },

  // 🔹 Crear chat
  async create(data: CreateChatRequest): Promise<Chat> {
    const res = await fetch(API_URL, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify(data),
    });
    if (!res.ok) {
      const errorText = await res.text();
      throw new Error(`Error al crear chat: ${res.status} - ${errorText}`);
    }
    return res.json();
  },

  // 🔹 Actualizar chat
  async update(id: string, data: UpdateChatRequest): Promise<Chat> {
    const res = await fetch(`${API_URL}/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
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
      credentials: 'include',
    });
    if (!res.ok) {
      const errorText = await res.text();
      throw new Error(`Error al eliminar chat: ${res.status} - ${errorText}`);
    }
  },
};
