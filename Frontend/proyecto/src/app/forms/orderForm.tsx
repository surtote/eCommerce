'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useOrders } from '@/hooks/useOrders';
import { useProducts } from '@/hooks/useProducts';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { CreateOrderRequest, OrderItem } from '@/types/Order';
import { Product } from '@/types/Product';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { X } from 'lucide-react';

export function OrderForm() {
  const router = useRouter();
  const { createOrder, loading, error: orderError } = useOrders();
  const { products } = useProducts();

  const [formData, setFormData] = useState({
    shippingAddress: '',
    notes: '',
  });

  const [orderItems, setOrderItems] = useState<OrderItem[]>([]);
  const [selectedProductId, setSelectedProductId] = useState<string>('');
  const [selectedQuantity, setSelectedQuantity] = useState('1');
  const [error, setError] = useState('');
  const [userId, setUserId] = useState<string | null>(null);

  // 🔹 Leer currentUser y configurar token
  useEffect(() => {
    if (typeof window === 'undefined') return;

    const stored = localStorage.getItem('currentUser');
    if (!stored) {
      setError('Debes iniciar sesión para crear una orden');
      return;
    }

    const currentUser = JSON.parse(stored);
    const token = currentUser?.token;
    const id = currentUser?.user?.id;

    if (!token || !id) {
      setError('Debes iniciar sesión para crear una orden');
      return;
    }

    setUserId(id);

    // Pasar token al servicio
    import('@/services/orderService').then((module) =>
      module.orderService.setToken(token)
    );
  }, []);

  const handleInputChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  // 🔹 Agregar producto a la orden
  const handleAddProduct = () => {
    if (!selectedProductId || !selectedQuantity) {
      setError('Selecciona un producto y cantidad');
      return;
    }

    const quantity = Number(selectedQuantity);
    if (quantity <= 0) {
      setError('La cantidad debe ser mayor a 0');
      return;
    }

    // Verificar si el producto ya existe en la orden
    const existingItem = orderItems.find(
      (item) => item.productId === selectedProductId
    );

    if (existingItem) {
      // Actualizar cantidad
      setOrderItems((prev) =>
        prev.map((item) =>
          item.productId === selectedProductId
            ? { ...item, quantity: item.quantity + quantity }
            : item
        )
      );
    } else {
      // Agregar nuevo item
      setOrderItems((prev) => [
        ...prev,
        {
          productId: selectedProductId,
          quantity,
        },
      ]);
    }

    setSelectedProductId('');
    setSelectedQuantity('1');
    setError('');
  };

  // 🔹 Eliminar producto de la orden
  const handleRemoveProduct = (productId: string) => {
    setOrderItems((prev) => prev.filter((item) => item.productId !== productId));
  };

  // 🔹 Obtener nombre del producto
  const getProductName = (productId: string): string => {
    return products.find((p) => p.id === productId)?.name || 'Producto desconocido';
  };

  // 🔹 Obtener precio del producto
  const getProductPrice = (productId: string): number => {
    return products.find((p) => p.id === productId)?.price || 0;
  };

  // 🔹 Calcular total
  const calculateTotal = (): number => {
    return orderItems.reduce((total, item) => {
      return total + getProductPrice(item.productId) * item.quantity;
    }, 0);
  };

  // 🔹 Enviar formulario
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (!userId) {
      setError('Debes iniciar sesión para crear una orden');
      return;
    }

    if (orderItems.length === 0) {
      setError('Debes agregar al menos un producto a la orden');
      return;
    }

    if (!formData.shippingAddress.trim()) {
      setError('La dirección de envío es requerida');
      return;
    }

    try {
      const request: CreateOrderRequest = {
        items: orderItems,
        shippingAddress: formData.shippingAddress,
        notes: formData.notes || undefined,
      };

      await createOrder(request);

      // Limpiar formulario
      setFormData({
        shippingAddress: '',
        notes: '',
      });
      setOrderItems([]);

      router.push('/orders');
    } catch (err: unknown) {
      console.error(err);
      setError(
        err instanceof Error ? err.message : 'No se pudo crear la orden'
      );
    }
  };

  const total = calculateTotal();

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-50 p-4">
      <Card className="w-full max-w-2xl">
        <CardHeader>
          <CardTitle className="text-center">Crear Orden</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-6">
            {error && (
              <div className="bg-destructive/15 text-destructive text-sm p-3 rounded">
                {error}
              </div>
            )}

            {orderError && (
              <div className="bg-destructive/15 text-destructive text-sm p-3 rounded">
                {orderError}
              </div>
            )}

            {/* 🔹 Sección de Productos */}
            <div className="border rounded-lg p-4 bg-gray-50">
              <h3 className="font-semibold mb-4">Agregar Productos</h3>

              <div className="space-y-3">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                  <div className="space-y-2">
                    <Label htmlFor="product">Producto</Label>
                    <Select value={selectedProductId} onValueChange={setSelectedProductId}>
                      <SelectTrigger id="product">
                        <SelectValue placeholder="Selecciona un producto" />
                      </SelectTrigger>
                      <SelectContent>
                        {products.map((product) => (
                          <SelectItem key={product.id} value={product.id}>
                            {product.name} - ${product.price}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>

                  <div className="space-y-2">
                    <Label htmlFor="quantity">Cantidad</Label>
                    <Input
                      id="quantity"
                      type="number"
                      value={selectedQuantity}
                      onChange={(e) => setSelectedQuantity(e.target.value)}
                      placeholder="1"
                      min={1}
                    />
                  </div>
                </div>

                <Button
                  type="button"
                  onClick={handleAddProduct}
                  variant="outline"
                  className="w-full"
                >
                  Agregar Producto
                </Button>
              </div>

              {/* 🔹 Lista de Productos Agregados */}
              {orderItems.length > 0 && (
                <div className="mt-4 space-y-2">
                  <h4 className="font-medium text-sm">Productos en la orden:</h4>
                  <div className="space-y-2">
                    {orderItems.map((item) => (
                      <div
                        key={item.productId}
                        className="flex items-center justify-between bg-white p-3 rounded border"
                      >
                        <div className="flex-1">
                          <p className="font-sm">
                            {getProductName(item.productId)} x{item.quantity}
                          </p>
                          <p className="text-sm text-gray-600">
                            ${(getProductPrice(item.productId) * item.quantity).toFixed(2)}
                          </p>
                        </div>
                        <Button
                          type="button"
                          variant="ghost"
                          size="sm"
                          onClick={() => handleRemoveProduct(item.productId)}
                        >
                          <X className="w-4 h-4" />
                        </Button>
                      </div>
                    ))}
                  </div>

                  {/* 🔹 Total */}
                  <div className="bg-blue-50 p-3 rounded border border-blue-200 mt-4">
                    <div className="flex justify-between items-center">
                      <span className="font-semibold">Total:</span>
                      <span className="text-lg font-bold text-blue-600">
                        ${total.toFixed(2)}
                      </span>
                    </div>
                  </div>
                </div>
              )}
            </div>

            {/* 🔹 Dirección de Envío */}
            <div className="space-y-2">
              <Label htmlFor="shippingAddress">Dirección de Envío *</Label>
              <Input
                id="shippingAddress"
                name="shippingAddress"
                value={formData.shippingAddress}
                onChange={handleInputChange}
                placeholder="Calle, número, ciudad, código postal"
                required
              />
            </div>

            {/* 🔹 Notas */}
            <div className="space-y-2">
              <Label htmlFor="notes">Notas (opcional)</Label>
              <Textarea
                id="notes"
                name="notes"
                value={formData.notes}
                onChange={handleInputChange}
                placeholder="Instrucciones especiales de entrega..."
                rows={3}
              />
            </div>

            {/* 🔹 Botón Enviar */}
            <Button
              type="submit"
              className="w-full"
              disabled={loading || orderItems.length === 0}
            >
              {loading ? 'Creando orden...' : 'Crear Orden'}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}