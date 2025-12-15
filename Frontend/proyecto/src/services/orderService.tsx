// src/services/orderService.ts
import { 
  CreateOrderRequest, 
  OrderResponse,
  OrderListResponse,
} from '../types/Order';

const API_URL = `${process.env.NEXT_PUBLIC_API_URL}/api/v1/orders`;
let token: string | null = null;

export const orderService = {
  setToken: (jwt: string) => {
    token = jwt;
  },

  // 🔹 Obtener mis órdenes
  async getMyOrders(): Promise<OrderListResponse[]> {
    const res = await fetch(`${API_URL}/my`, {
      headers: { ...(token ? { Authorization: `Bearer ${token}` } : {}) },
    });
    if (!res.ok) throw new Error('Error al obtener tus órdenes');
    const data = await res.json();
    return data.data || [];
  },

  // 🔹 Obtener orden por ID
  async getOrderById(id: string): Promise<OrderResponse> {
    const res = await fetch(`${API_URL}/${id}`, {
      headers: { ...(token ? { Authorization: `Bearer ${token}` } : {}) },
    });
    if (!res.ok) throw new Error('Error al obtener orden');
    const data = await res.json();
    return data.data;
  },

  // 🔹 Crear orden
  async createOrder(request: CreateOrderRequest): Promise<OrderResponse> {
    const res = await fetch(API_URL, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
      body: JSON.stringify(request),
    });

    if (!res.ok) {
      let message = 'Error al crear orden';
      try {
        const error = await res.json();
        message = error.message || error.errors?.[0] || message;
      } catch {}
      throw new Error(message);
    }

    const data = await res.json();
    return data.data;
  },

  // 🔹 Cancelar orden
  async cancelOrder(id: string): Promise<void> {
    const res = await fetch(`${API_URL}/${id}/cancel`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
    });

    if (!res.ok) {
      let message = 'Error al cancelar orden';
      try {
        const error = await res.json();
        message = error.message || message;
      } catch {}
      throw new Error(message);
    }
  },

  // 🔹 ADMIN: Obtener todas las órdenes
  async getAllOrders(
    status?: string,
    userId?: string
  ): Promise<OrderListResponse[]> {
    const params = new URLSearchParams();
    if (status) params.append('status', status);
    if (userId) params.append('userId', userId);

    const res = await fetch(`${API_URL}/admin?${params.toString()}`, {
      headers: { ...(token ? { Authorization: `Bearer ${token}` } : {}) },
    });

    if (!res.ok) throw new Error('Error al obtener órdenes');
    const data = await res.json();
    return data.data || [];
  },

  // 🔹 ADMIN: Obtener orden por ID (admin)
  async getOrderByIdAdmin(id: string): Promise<OrderResponse> {
    const res = await fetch(`${API_URL}/admin/${id}`, {
      headers: { ...(token ? { Authorization: `Bearer ${token}` } : {}) },
    });

    if (!res.ok) throw new Error('Error al obtener orden');
    const data = await res.json();
    return data.data;
  },

  // 🔹 ADMIN: Actualizar estado de orden
  async updateOrderStatus(
    id: string,
    status: string
  ): Promise<OrderResponse> {
    const res = await fetch(`${API_URL}/admin/${id}/status`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
      body: JSON.stringify({ status }),
    });

    if (!res.ok) {
      let message = 'Error al actualizar estado de orden';
      try {
        const error = await res.json();
        message = error.message || message;
      } catch {}
      throw new Error(message);
    }

    const data = await res.json();
    return data.data;
  },
};

// 🔹 Función auxiliar opcional
export async function fetchMyOrders(): Promise<OrderListResponse[]> {
  return orderService.getMyOrders();
}