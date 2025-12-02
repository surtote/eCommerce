import { Message } from "@/types/Message";

const API_URL = `${process.env.NEXT_PUBLIC_API_URL}/api/messages`;
let token: string | null = null;

export type CreateMessageRequest = {
  chatId: string;
  senderId: string;
  content: string;
};

export type UpdateMessageRequest = {
  content?: string;
};

export const messageService = {
  // 🔹 Configurar token
  setToken: (jwt: string) => {
    token = jwt;
  },

  // 🔹 Obtener mensajes de un chat
  async getByChatId(chatId: string): Promise<Message[]> {
    const res = await fetch(`${API_URL}/chat/${chatId}`, {
      headers: { ...(token ? { Authorization: `Bearer ${token}` } : {}) },
    });
    if (!res.ok) {
      const errorText = await res.text();
      throw new Error(`Error al obtener mensajes: ${res.status} - ${errorText}`);
    }
    return res.json();
  },

  // 🔹 Obtener mensaje por ID
  async getById(id: string): Promise<Message> {
    const res = await fetch(`${API_URL}/${id}`, {
      headers: { ...(token ? { Authorization: `Bearer ${token}` } : {}) },
    });
    if (!res.ok) {
      const errorText = await res.text();
      throw new Error(`Error al obtener mensaje: ${res.status} - ${errorText}`);
    }
    return res.json();
  },

  // 🔹 Crear mensaje
  async create(data: CreateMessageRequest): Promise<Message> {
    const res = await fetch(API_URL, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
      body: JSON.stringify(data),
    });
    if (!res.ok) {
      const errorText = await res.text();
      throw new Error(`Error al enviar mensaje: ${res.status} - ${errorText}`);
    }
    return res.json();
  },

  // 🔹 Actualizar mensaje
  async update(id: string, data: UpdateMessageRequest): Promise<Message> {
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
      throw new Error(`Error al actualizar mensaje: ${res.status} - ${errorText}`);
    }
    return res.json();
  },

  // 🔹 Eliminar mensaje
  async delete(id: string): Promise<void> {
    const res = await fetch(`${API_URL}/${id}`, {
      method: 'DELETE',
      headers: { ...(token ? { Authorization: `Bearer ${token}` } : {}) },
    });
    if (!res.ok) {
      const errorText = await res.text();
      throw new Error(`Error al eliminar mensaje: ${res.status} - ${errorText}`);
    }
  },
};
