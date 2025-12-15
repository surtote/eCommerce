'use client';

import { useState, useEffect, useCallback, useRef } from 'react';
import { chatService } from '@/services/chatService';
import { userService } from '@/services/userService';
import { Chat, CreateChatRequest, UpdateChatRequest } from '@/types/Chat';

export const useChats = () => {
  const [chats, setChats] = useState<Chat[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [currentUserId, setCurrentUserId] = useState<string | null>(null);

  const userCache = useRef<Map<string, string>>(new Map());

  const getErrorMessage = (err: unknown): string =>
    err instanceof Error ? err.message : 'Ocurrió un error desconocido';

  useEffect(() => {
    if (typeof window === 'undefined') return;

    const stored = localStorage.getItem('currentUser');
    if (!stored) return;

    const currentUser = JSON.parse(stored);
    const id = currentUser?.user?.id;
    const token = currentUser?.token;

    if (id) setCurrentUserId(id);
    if (token) chatService.setToken(token);
  }, []);

  const getUserName = useCallback(async (id: string): Promise<string> => {
    if (userCache.current.has(id)) return userCache.current.get(id)!;

    try {
      const user = await userService.getUserById(id);
      const name = user?.nombre || `Usuario ${id}`;
      userCache.current.set(id, name);
      return name;
    } catch {
      return `Usuario ${id}`;
    }
  }, []);

  const fetchChats = useCallback(async () => {
    if (!currentUserId) return;

    try {
      setLoading(true);
      setError(null);

      const chatsRaw = await chatService.getAll();

      const enriched = await Promise.all(
        chatsRaw.map(async (chat: Chat) => {
          const sellerName = chat.sellerId === currentUserId ? 'Tú' : await getUserName(chat.sellerId);
          const buyerName = chat.buyerId === currentUserId ? 'Tú' : await getUserName(chat.buyerId);
          return { ...chat, sellerName, buyerName };
        })
      );

      setChats(enriched);
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  }, [currentUserId, getUserName]);

  const createChat = async (chatData: CreateChatRequest): Promise<Chat> => {
    if (!currentUserId) throw new Error('Usuario no autenticado');

    try {
      setLoading(true);
      const newChat = await chatService.createChat(chatData);

      const sellerName = newChat.sellerId === currentUserId ? 'Tú' : await getUserName(newChat.sellerId);
      const buyerName = newChat.buyerId === currentUserId ? 'Tú' : await getUserName(newChat.buyerId);

      const enrichedChat = { ...newChat, sellerName, buyerName };
      setChats(prev => [...prev, enrichedChat]);
      return enrichedChat;
    } catch (err) {
      const msg = getErrorMessage(err);
      setError(msg);
      throw new Error(msg);
    } finally {
      setLoading(false);
    }
  };

  const updateChat = async (id: string, data: UpdateChatRequest): Promise<Chat> => {
    try {
      setLoading(true);
      const updated = await chatService.update(id, data as Chat);

      const sellerName = updated.sellerId === currentUserId ? 'Tú' : await getUserName(updated.sellerId);
      const buyerName = updated.buyerId === currentUserId ? 'Tú' : await getUserName(updated.buyerId);

      const enriched = { ...updated, sellerName, buyerName };
      setChats(prev => prev.map(c => (c.id === id ? enriched : c)));
      return enriched;
    } catch (err) {
      const msg = getErrorMessage(err);
      setError(msg);
      throw new Error(msg);
    } finally {
      setLoading(false);
    }
  };

  const deleteChat = async (id: string): Promise<void> => {
    try {
      setLoading(true);
      await chatService.delete(id);
      setChats(prev => prev.filter(c => c.id !== id));
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchChats();
  }, [currentUserId, fetchChats]);

  return { chats, loading, error, fetchChats, createChat, updateChat, deleteChat, currentUserId };
};