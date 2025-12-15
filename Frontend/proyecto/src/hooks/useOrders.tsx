'use client';

import { useState, useEffect, useCallback } from 'react';
import { orderService } from '@/services/orderService';
import { 
  CreateOrderRequest, 
  OrderListResponse,
  OrderResponse,
} from '@/types/Order';

export const useOrders = () => {
  const [orders, setOrders] = useState<OrderListResponse[]>([]);
  const [currentOrder, setCurrentOrder] = useState<OrderResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [userId, setUserId] = useState<string | null>(null);
  const [isAdmin, setIsAdmin] = useState(false);

  const getErrorMessage = (err: unknown): string => {
    if (err instanceof Error) return err.message;
    return 'Ocurrió un error desconocido';
  };

  useEffect(() => {
    if (typeof window === 'undefined') return;

    const stored = localStorage.getItem('currentUser');
    if (!stored) return;

    const currentUser = JSON.parse(stored);
    const id = currentUser?.user?.id;
    const token = currentUser?.token;
    const roles = currentUser?.user?.roles || [];

    if (id) setUserId(id);
    if (token) orderService.setToken(token);
    if (roles.includes('Admin')) setIsAdmin(true);
  }, []);

  const fetchMyOrders = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await orderService.getMyOrders();
      setOrders(data);
    } catch (err: unknown) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  }, []);

  const fetchAllOrders = useCallback(async (status?: string, userId?: string) => {
    if (!isAdmin) {
      setError('No tienes permisos para ver todas las órdenes');
      return;
    }

    setLoading(true);
    setError(null);
    try {
      const data = await orderService.getAllOrders(status, userId);
      setOrders(data);
    } catch (err: unknown) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  }, [isAdmin]);

  const getOrderById = useCallback(async (id: string) => {
    setLoading(true);
    setError(null);
    try {
      const data = await orderService.getOrderById(id);
      setCurrentOrder(data);
      return data;
    } catch (err: unknown) {
      setError(getErrorMessage(err));
      return null;
    } finally {
      setLoading(false);
    }
  }, []);

  const getOrderByIdAdmin = useCallback(async (id: string) => {
    if (!isAdmin) {
      setError('No tienes permisos para ver esta orden');
      return null;
    }

    setLoading(true);
    setError(null);
    try {
      const data = await orderService.getOrderByIdAdmin(id);
      setCurrentOrder(data);
      return data;
    } catch (err: unknown) {
      setError(getErrorMessage(err));
      return null;
    } finally {
      setLoading(false);
    }
  }, [isAdmin]);

  const createOrder = useCallback(async (request: CreateOrderRequest) => {
    if (!userId) {
      setError('Debes iniciar sesión para crear una orden');
      return null;
    }

    setLoading(true);
    setError(null);
    try {
      const newOrder = await orderService.createOrder(request);
      setCurrentOrder(newOrder);
      await fetchMyOrders();
      
      return newOrder;
    } catch (err: unknown) {
      setError(getErrorMessage(err));
      return null;
    } finally {
      setLoading(false);
    }
  }, [userId, fetchMyOrders]);

  const cancelOrder = useCallback(async (id: string) => {
    setLoading(true);
    setError(null);
    try {
      await orderService.cancelOrder(id);
      
      setOrders((prev) =>
        prev.map((order) =>
          order.id === id ? { ...order, status: 'Cancelled' } : order
        )
      );

      if (currentOrder?.id === id) {
        setCurrentOrder({ ...currentOrder, status: 'Cancelled' });
      }

      return true;
    } catch (err: unknown) {
      setError(getErrorMessage(err));
      return false;
    } finally {
      setLoading(false);
    }
  }, [currentOrder]);

  const updateOrderStatus = useCallback(
    async (id: string, status: string) => {
      if (!isAdmin) {
        setError('No tienes permisos para actualizar órdenes');
        return null;
      }

      setLoading(true);
      setError(null);
      try {
        const updatedOrder = await orderService.updateOrderStatus(id, status);
        
        setOrders((prev) =>
          prev.map((order) =>
            order.id === id ? { ...order, status: updatedOrder.status } : order
          )
        );

        if (currentOrder?.id === id) {
          setCurrentOrder(updatedOrder);
        }

        return updatedOrder;
      } catch (err: unknown) {
        setError(getErrorMessage(err));
        return null;
      } finally {
        setLoading(false);
      }
    },
    [isAdmin, currentOrder]
  );

  const clearError = useCallback(() => {
    setError(null);
  }, []);

  const clearCurrentOrder = useCallback(() => {
    setCurrentOrder(null);
  }, []);

  return {
    orders,
    currentOrder,
    loading,
    error,
    userId,
    isAdmin,

    fetchMyOrders,
    fetchAllOrders,
    getOrderById,
    getOrderByIdAdmin,
    createOrder,
    cancelOrder,
    updateOrderStatus,
    clearError,
    clearCurrentOrder,
  };
};