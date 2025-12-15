'use client';
import { useState, useEffect, useCallback } from 'react';
import { messageService, CreateMessageRequest, UpdateMessageRequest } from '@/services/messageService';
import { Message } from '@/types/Message';

export const useMessages = (chatId: string | null) => {
  const [messages, setMessages] = useState<Message[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const getErrorMessage = (err: unknown): string =>
    err instanceof Error ? err.message : 'Ocurrió un error desconocido';

  useEffect(() => {
    if (typeof window === 'undefined') return;
    const stored = localStorage.getItem('currentUser');
    if (!stored) return;

    const currentUser = JSON.parse(stored);
    const token = currentUser?.token;
    if (token) messageService.setToken(token);
  }, []);

  const fetchMessages = useCallback(async (id: string) => {
    setLoading(true);
    setError(null);
    try {
      const data = await messageService.getByChatId(id);
      setMessages(data);
    } catch (err: unknown) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  }, []);

  const createMessage = useCallback(
    async (messageData: Omit<CreateMessageRequest, 'chatId'>) => {
      if (!chatId) throw new Error('Chat no seleccionado');
      setLoading(true);
      setError(null);
      try {
        const newMessage = await messageService.create({ ...messageData, chatId });
        setMessages((prev) => [...prev, newMessage]);
        return newMessage;
      } catch (err: unknown) {
        const message = getErrorMessage(err);
        setError(message);
        throw new Error(message);
      } finally {
        setLoading(false);
      }
    },
    [chatId]
  );

  const updateMessage = useCallback(async (id: string, messageData: UpdateMessageRequest) => {
    setLoading(true);
    setError(null);
    try {
      const updatedMessage = await messageService.update(id, messageData);
      setMessages((prev) => prev.map((m) => (m.id === id ? { ...m, ...updatedMessage } : m)));
      return updatedMessage;
    } catch (err: unknown) {
      const message = getErrorMessage(err);
      setError(message);
      throw new Error(message);
    } finally {
      setLoading(false);
    }
  }, []);

  const deleteMessage = useCallback(async (id: string) => {
    setLoading(true);
    setError(null);
    try {
      await messageService.delete(id);
      setMessages((prev) => prev.filter((m) => m.id !== id));
    } catch (err: unknown) {
      const message = getErrorMessage(err);
      setError(message);
      throw new Error(message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    if (chatId) fetchMessages(chatId);
  }, [chatId, fetchMessages]);

  return {
    messages,
    loading,
    error,
    fetchMessages,
    createMessage,
    updateMessage,
    deleteMessage,
  };
};
