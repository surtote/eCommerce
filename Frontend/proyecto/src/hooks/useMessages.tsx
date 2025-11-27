'use client';
import { useState, useEffect } from 'react';
import { messageService, CreateMessageRequest, UpdateMessageRequest } from '@/services/messageService';
import { Message } from '@/types/Message';

export const useMessages = (chatId: string | null) => {
  const [messages, setMessages] = useState<Message[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // 🔹 Helper para extraer mensaje de error
  const getErrorMessage = (err: unknown): string => {
    if (err instanceof Error) return err.message;
    return 'Ocurrió un error desconocido';
  };

  // 🔹 Cargar mensajes del chat
  const fetchMessages = async (id: string) => {
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
  };

  // 🔹 Crear mensaje
  const createMessage = async (messageData: Omit<CreateMessageRequest, 'chatId'>) => {
    if (!chatId) throw new Error('Chat no seleccionado');

    setLoading(true);
    setError(null);
    try {
      const newMessage = await messageService.create({
        ...messageData,
        chatId,
      });

      setMessages((prev) => [...prev, newMessage]);
      return newMessage;
    } catch (err: unknown) {
      const message = getErrorMessage(err);
      setError(message);
      throw new Error(message);
    } finally {
      setLoading(false);
    }
  };

  // 🔹 Actualizar mensaje
  const updateMessage = async (id: string, messageData: UpdateMessageRequest) => {
    setLoading(true);
    setError(null);
    try {
      const updatedMessage = await messageService.update(id, messageData);
      setMessages((prev) =>
        prev.map((m) => (m.id === id ? { ...m, ...updatedMessage } : m))
      );
      return updatedMessage;
    } catch (err: unknown) {
      const message = getErrorMessage(err);
      setError(message);
      throw new Error(message);
    } finally {
      setLoading(false);
    }
  };

  // 🔹 Eliminar mensaje
  const deleteMessage = async (id: string) => {
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
  };

  useEffect(() => {
    if (chatId) fetchMessages(chatId);
  }, [chatId]);

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
