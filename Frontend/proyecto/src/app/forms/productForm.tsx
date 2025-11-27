'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useProducts } from '@/hooks/useProducts';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { CategoryDropdown } from '../components/categoryDropdown';

export function ProductForm() {
  const router = useRouter();
  const { createProduct, loading } = useProducts();

  const [formData, setFormData] = useState({
    name: '',
    categoryId: undefined as string | undefined,
    price: '',
    description: '',
    stock: '',
    image: null as File | null,
  });

  const [error, setError] = useState('');
  const [userId, setUserId] = useState<string | null>(null);

  useEffect(() => {
    const currentUser =
      typeof window !== 'undefined'
        ? JSON.parse(sessionStorage.getItem('currentUser') || '{}')
        : null;

    if (!currentUser?.id) {
      setError('Debes iniciar sesión para crear un producto');
    } else {
      setUserId(currentUser.id);
    }
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;

    // Si es un input de tipo file
    if (e.target instanceof HTMLInputElement && e.target.type === 'file') {
      const file = e.target.files?.[0] || null;
      setFormData({ ...formData, image: file });
    } else {
      setFormData({ ...formData, [name]: value });
    }
  };


  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (!userId) {
      setError('Debes iniciar sesión para crear un producto');
      return;
    }

    if (!formData.name || formData.price === '' || Number(formData.price) < 0) {
      setError('Por favor completa todos los campos correctamente.');
      return;
    }

    if (!formData.categoryId) {
      setError('Debes seleccionar una categoría');
      return;
    }

    console.log("formData al enviar:", formData);

    try {
      await createProduct({
        name: formData.name,
        price: Number(formData.price),
        description: formData.description,
        stock: Number(formData.stock),
        userId: userId,
        categoryId: formData.categoryId,
        image: formData.image || undefined,
      });

      setFormData({
        name: '',
        price: '',
        description: '',
        stock: '',
        image: null,
        categoryId: undefined,
      });
      router.push('/products');
    } catch (err: unknown) {
      console.error(err);

      if (err instanceof Error) {
        // Solo los objetos de tipo Error tienen .message
        alert(err.message);
      } else {
        alert("No se pudo crear el chat");
      }
    }

  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-50 p-4">
      <Card className="w-full max-w-md">
        <CardHeader>
          <CardTitle className="text-center">Crear Producto</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            {error && (
              <div className="bg-destructive/15 text-destructive text-sm p-3 rounded">
                {error}
              </div>
            )}

            <div className="space-y-2">
              <Label htmlFor="name">Nombre del producto</Label>
              <Input
                id="name"
                name="name"
                value={formData.name}
                onChange={handleChange}
                placeholder="Ej: Zapatos deportivos"
                required
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="category">Categoría</Label>
              <CategoryDropdown
                value={formData.categoryId ?? null}
                onSelect={(id) => setFormData({ ...formData, categoryId: id ?? undefined })}
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="price">Precio</Label>
              <Input
                id="price"
                name="price"
                type="number"
                value={formData.price}
                onChange={handleChange}
                placeholder="Ej: 59.99"
                required
                min={0}
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="stock">Stock</Label>
              <Input
                id="stock"
                name="stock"
                type="number"
                value={formData.stock}
                onChange={handleChange}
                placeholder="Ej: 10"
                min={0}
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="description">Descripción</Label>
              <Textarea
                id="description"
                name="description"
                value={formData.description}
                onChange={handleChange}
                placeholder="Describe el producto..."
                rows={3}
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="image">Imagen (opcional)</Label>
              <Input
                id="image"
                name="image"
                type="file"
                accept="image/*"
                onChange={handleChange}
              />
            </div>

            <Button type="submit" className="w-full" disabled={loading}>
              {loading ? 'Creando producto...' : 'Crear producto'}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
