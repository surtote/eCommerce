import { Category } from "../types/Category";

// Toma la URL del backend desde tu .env.local
const API_URL = `${process.env.NEXT_PUBLIC_API_URL}/api/categories`;

export const categoryService = {
  getAll: async (): Promise<Category[]> => {
    const res = await fetch(API_URL);
    if (!res.ok) throw new Error("Error al obtener categorías");
    return res.json();
  },

  getById: async (id: string): Promise<Category> => {
    const res = await fetch(`${API_URL}/${id}`);
    if (!res.ok) throw new Error("Categoría no encontrada");
    return res.json();
  },

  create: async (data: { name: string }): Promise<Category> => {
    const res = await fetch(API_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });
    if (!res.ok) throw new Error("Error al crear categoría");
    return res.json();
  },

  delete: async (id: string): Promise<void> => {
    const res = await fetch(`${API_URL}/${id}`, {
      method: "DELETE",
    });
    if (!res.ok) throw new Error("Error al eliminar categoría");
  },
};
