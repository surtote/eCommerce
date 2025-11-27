'use client';

import { useState, useEffect } from 'react';
import { chatService, CreateChatRequest, UpdateChatRequest } from '@/services/chatService';
import { userService } from '@/services/userService';
import { Chat } from '@/types/Chat';

export const useChats = () => {
  const [chats, setChats] = useState<Chat[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  // 🧩 Cache para no repetir peticiones al mismo usuario
  const userCache = new Map<string, string>();

  // 🔹 Helper para extraer mensaje de error
  const getErrorMessage = (err: unknown): string => {
    if (err instanceof Error) return err.message;
    return 'Ocurrió un error desconocido';
  };

  // 🔹 Obtener el nombre de usuario con cache
  const getUserName = async (id: string): Promise<string> => {
    if (userCache.has(id)) return userCache.get(id)!;
    try {
      const user = await userService.getUserById(id);
      const name = user?.nombre || `Usuario ${id}`;
      userCache.set(id, name);
      return name;
    } catch {
      return `Usuario ${id}`;
    }
  };

  // 🔹 Obtener todos los chats y añadir nombres de buyer y seller
  const fetchChats = async (): Promise<void> => {
    try {
      setLoading(true);
      setError(null);

      const chatsRaw = await chatService.getAll();

      const enrichedChats = await Promise.all(
        chatsRaw.map(async (chat: Chat) => {
          const [sellerName, buyerName] = await Promise.all([
            getUserName(chat.sellerId),
            getUserName(chat.buyerId),
          ]);
          return { ...chat, sellerName, buyerName };
        })
      );

      setChats(enrichedChats);
    } catch (err: unknown) {
      console.error('❌ Error al obtener los chats:', err);
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  // 🔹 Crear un nuevo chat
  const createChat = async (chatData: CreateChatRequest): Promise<Chat> => {
    try {
      setLoading(true);
      setError(null);

      const newChat = await chatService.create(chatData);

      const [sellerName, buyerName] = await Promise.all([
        getUserName(newChat.sellerId),
        getUserName(newChat.buyerId),
      ]);

      const enrichedChat = { ...newChat, sellerName, buyerName };
      setChats((prev) => [...prev, enrichedChat]);
      return enrichedChat;
    } catch (err: unknown) {
      console.error('❌ Error al crear el chat:', err);
      const message = getErrorMessage(err);
      setError(message);
      throw new Error(message);
    } finally {
      setLoading(false);
    }
  };

  // 🔹 Actualizar un chat existente
  const updateChat = async (id: string, chatData: UpdateChatRequest): Promise<Chat> => {
    try {
      setLoading(true);
      setError(null);

      const updatedChat = await chatService.update(id, chatData);

      const [sellerName, buyerName] = await Promise.all([
        getUserName(updatedChat.sellerId),
        getUserName(updatedChat.buyerId),
      ]);

      const enrichedChat = { ...updatedChat, sellerName, buyerName };

      setChats((prev) =>
        prev.map((chat) => (chat.id === id ? enrichedChat : chat))
      );

      return enrichedChat;
    } catch (err: unknown) {
      console.error('❌ Error al actualizar chat:', err);
      const message = getErrorMessage(err);
      setError(message);
      throw new Error(message);
    } finally {
      setLoading(false);
    }
  };

  // 🔹 Eliminar un chat
  const deleteChat = async (id: string): Promise<void> => {
    try {
      setLoading(true);
      setError(null);

      await chatService.delete(id);
      setChats((prev) => prev.filter((chat) => chat.id !== id));
    } catch (err: unknown) {
      console.error('❌ Error al eliminar chat:', err);
      const message = getErrorMessage(err);
      setError(message);
      throw new Error(message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchChats();
  }, []);

  return {
    chats,
    loading,
    error,
    fetchChats,
    createChat,
    updateChat,
    deleteChat,
  };
};
