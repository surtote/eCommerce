// src/services/productService.ts
import { Product, CreateProductRequest } from '../types/Product';

const API_URL = `${process.env.NEXT_PUBLIC_API_URL}/api/Products`;
let token: string | null = null; 

export const productService = {
  setToken: (jwt: string) => {
    token = jwt;
  },

  // 🔹 Obtener todos los productos
  async getProducts(): Promise<Product[]> {
    const res = await fetch(API_URL, {
      headers: { ...(token ? { Authorization: `Bearer ${token}` } : {}) },
    });
    if (!res.ok) throw new Error('Error al obtener productos');
    return res.json();
  },

  // 🔹 Obtener producto por ID
  async getProductById(id: string): Promise<Product> {
    const res = await fetch(`${API_URL}/${id}`, {
      headers: { ...(token ? { Authorization: `Bearer ${token}` } : {}) },
    });
    if (!res.ok) throw new Error('Error al obtener producto');
    return res.json();
  },

  // 🔹 Obtener productos por usuario
  async getProductsByUserId(userId: string): Promise<Product[]> {
    const res = await fetch(`${API_URL}/user/${userId}`, {
      headers: { ...(token ? { Authorization: `Bearer ${token}` } : {}) },
    });
    if (!res.ok) throw new Error('Error al obtener productos del usuario');
    return res.json();
  },

  // 🔹 Obtener productos por categoría
  async getProductsByCategory(categoryId: string): Promise<Product[]> {
    const res = await fetch(`${API_URL}/category/${categoryId}`, {
      headers: { ...(token ? { Authorization: `Bearer ${token}` } : {}) },
    });
    if (!res.ok) throw new Error('Error al obtener productos por categoría');
    return res.json();
  },

  // 🔹 Crear producto (soporta imagen y campos opcionales)
  async createProduct(data: CreateProductRequest): Promise<Product> {
    const formData = new FormData();
    formData.append('name', data.name);
    formData.append('price', data.price.toString());
    formData.append('userId', data.userId.toString());

    if (data.categoryId !== undefined && data.categoryId !== null)
      formData.append('categoryId', data.categoryId.toString());

    if (data.description) formData.append('description', data.description);
    if (data.stock !== undefined && data.stock !== null)
      formData.append('stock', data.stock.toString());
    if (data.image) formData.append('image', data.image);

    const res = await fetch(API_URL, {
      method: 'POST',
      headers: { ...(token ? { Authorization: `Bearer ${token}` } : {}) },
      body: formData,
    });

    if (!res.ok) {
      let message = 'Error al crear producto';
      try {
        const error = await res.json();
        message = error.message || message;
      } catch {}
      throw new Error(message);
    }

    return res.json();
  },

  // 🔹 Actualizar producto
  async updateProduct(id: string, data: Partial<Product>): Promise<Product> {
    const res = await fetch(`${API_URL}/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
      body: JSON.stringify(data),
    });

    if (!res.ok) {
      let message = 'Error al actualizar producto';
      try {
        const error = await res.json();
        message = error.message || message;
      } catch {}
      throw new Error(message);
    }

    return res.json();
  },

  // 🔹 Eliminar producto
  async deleteProduct(id: string): Promise<void> {
    const res = await fetch(`${API_URL}/${id}`, {
      method: 'DELETE',
      headers: { ...(token ? { Authorization: `Bearer ${token}` } : {}) },
    });
    if (!res.ok) throw new Error('Error al eliminar producto');
  },
};

// 🔹 Función auxiliar opcional
export async function fetchProducts(): Promise<Product[]> {
  return productService.getProducts();
}
