// src/types/Order.ts

export interface OrderItem {
  productId: string;
  quantity: number;
}

export interface CreateOrderRequest {
  items: OrderItem[];
  shippingAddress: string;
  notes?: string;
}

export interface OrderItemResponse {
  id: string;
  productId: string;
  productName: string;
  unitPrice: number;
  quantity: number;
  subtotal: number;
}

export interface OrderResponse {
  id: string;
  userId: string;
  status: 'Pending' | 'Confirmed' | 'Processing' | 'Shipped' | 'Delivered' | 'Cancelled';
  totalAmount: number;
  shippingAddress: string;
  notes?: string;
  createdAt: string;
  updatedAt: string;
  orderProducts: OrderItemResponse[];
}

export interface OrderListResponse {
  id: string;
  status: 'Pending' | 'Confirmed' | 'Processing' | 'Shipped' | 'Delivered' | 'Cancelled';
  totalAmount: number;
  itemCount: number;
  createdAt: string;
}

export interface UpdateOrderStatusRequest {
  status: 'Pending' | 'Confirmed' | 'Processing' | 'Shipped' | 'Delivered' | 'Cancelled';
}

export type Order = OrderResponse;