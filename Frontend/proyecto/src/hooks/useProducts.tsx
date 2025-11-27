'use client';

import { useState, useEffect } from 'react';
import { productService } from '@/services/productService';
import { categoryService } from '../services/categoriesService';
import { Product, CreateProductRequest } from '@/types/Product';

export const useProducts = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [userId, setUserId] = useState<string | null>(null);
  const [selectedCategory, setSelectedCategory] = useState<string | null>(null);

  // 🔹 Helper para extraer mensaje de error
  const getErrorMessage = (err: unknown): string => {
    if (err instanceof Error) return err.message;
    return 'Ocurrió un error desconocido';
  };

  // 🔹 Obtener el usuario actual del sessionStorage
  useEffect(() => {
    const currentUser = typeof window !== 'undefined'
      ? JSON.parse(sessionStorage.getItem('currentUser') || '{}')
      : null;

    if (currentUser?.id) setUserId(currentUser.id);
  }, []);

  // 🔹 Cargar productos por categoría
  const fetchProductsByCategory = async (categoryId: string) => {
    setSelectedCategory(categoryId);
    setLoading(true);
    setError(null);
    try {
      const data = await productService.getProductsByCategory(categoryId);
      setProducts(data);
    } catch (err: unknown) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  // 🔹 Cargar todos los productos (sin filtrar por usuario)
  const fetchProducts = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await productService.getProducts();

      const enrichedProducts = await Promise.all(
        data.map(async (product) => {
          if (product.categoryId) {
            try {
              const category = await categoryService.getById(product.categoryId);
              return { ...product, categoryName: category.name };
            } catch {
              return { ...product, categoryName: 'Desconocida' };
            }
          }
          return { ...product, categoryName: 'Sin categoría' };
        })
      );

      setProducts(enrichedProducts);
    } catch (err: unknown) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  // 🔹 Crear producto
  const createProduct = async (productData: CreateProductRequest) => {
    if (!userId) {
      setError('Debes iniciar sesión para crear un producto');
      return;
    }

    setLoading(true);
    setError(null);
    try {
      const newProduct = await productService.createProduct({
        ...productData,
        userId,
      });

      if (newProduct.categoryId) {
        try {
          const category = await categoryService.getById(newProduct.categoryId);
          newProduct.categoryName = category.name;
        } catch {
          newProduct.categoryName = 'Desconocida';
        }
      }

      setProducts((prev) => [...prev, newProduct]);
    } catch (err: unknown) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  // 🔹 Actualizar producto
  const updateProduct = async (id: string, data: Partial<Product>) => {
    setLoading(true);
    setError(null);
    try {
      const updatedProduct = await productService.updateProduct(id, data);

      if (updatedProduct.categoryId) {
        try {
          const category = await categoryService.getById(updatedProduct.categoryId);
          updatedProduct.categoryName = category.name;
        } catch {
          updatedProduct.categoryName = 'Desconocida';
        }
      }

      setProducts((prev) =>
        prev.map((p) => (p.id === id ? updatedProduct : p))
      );
    } catch (err: unknown) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  // 🔹 Eliminar producto
  const deleteProduct = async (id: string) => {
    setLoading(true);
    setError(null);
    try {
      await productService.deleteProduct(id);
      setProducts((prev) => prev.filter((p) => p.id !== id));
    } catch (err: unknown) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  // 🔹 Ejecutar al cargar
  useEffect(() => {
    fetchProducts();
  }, [userId]);

  return {
    products,
    loading,
    error,
    selectedCategory,
    setSelectedCategory,
    fetchProducts,
    fetchProductsByCategory,
    createProduct,
    updateProduct,
    deleteProduct,
  };
};
