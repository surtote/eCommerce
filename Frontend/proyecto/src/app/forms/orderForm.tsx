'use client';

import { useState, useEffect } from 'react';
import { GenericTable } from '../components/genericTable';
import { Button } from '@/components/ui/button';
import { useRouter } from 'next/navigation';
import type { ColumnDef } from '@tanstack/react-table';
import { Trash2, ArrowLeft } from 'lucide-react';
import { orderService } from '@/services/orderService';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';

interface OrderItem {
  id: string;
  productId: string;
  productName: string;
  unitPrice: number;
  quantity: number;
  subtotal: number;
}

interface OrderResponse {
  id: string;
  userId: string;
  status: string;
  totalAmount: number;
  shippingAddress: string;
  notes?: string;
  createdAt: string;
  updatedAt: string;
  orderProducts: OrderItem[];
}

export function OrdersForm() {
  const router = useRouter();
  const [orders, setOrders] = useState<OrderResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [token, setToken] = useState<string | null>(null);
  const [statusFilter, setStatusFilter] = useState<string>('');
  const [userIdFilter, setUserIdFilter] = useState<string>('');
  const [selectedOrder, setSelectedOrder] = useState<OrderResponse | null>(null);

  // Obtener token del usuario actual
  useEffect(() => {
    if (typeof window === 'undefined') return;

    const stored = localStorage.getItem('currentUser');
    if (!stored) {
      setError('Debes iniciar sesión para ver las órdenes');
      setLoading(false);
      return;
    }

    const currentUser = JSON.parse(stored);
    const userToken = currentUser?.token;

    if (!userToken) {
      setError('Debes iniciar sesión para ver las órdenes');
      setLoading(false);
      return;
    }

    setToken(userToken);
  }, []);

  // Cargar todas las órdenes
  useEffect(() => {
    if (!token) return;

    const fetchOrders = async () => {
      try {
        setLoading(true);
        orderService.setToken(token);
        const data = await orderService.getAllOrdersDetailed(statusFilter || undefined, userIdFilter || undefined);
        setOrders(data);
        setError('');
      } catch (err) {
        console.error(err);
        setError(err instanceof Error ? err.message : 'Error al cargar las órdenes');
      } finally {
        setLoading(false);
      }
    };

    fetchOrders();
  }, [token, statusFilter, userIdFilter]);

  // Manejar eliminación/cancelación de orden
  const handleCancelOrder = async (id: string) => {
    if (!confirm('¿Estás seguro de que deseas cancelar esta orden?')) {
      return;
    }

    try {
      orderService.setToken(token!);
      await orderService.cancelOrder(id);

      // Actualizar la lista de órdenes
      setOrders((prev) =>
        prev.map((order) =>
          order.id === id ? { ...order, status: 'Cancelled' } : order
        )
      );
    } catch (err) {
      console.error(err);
      setError(err instanceof Error ? err.message : 'Error al cancelar la orden');
    }
  };

  // Definir columnas de la tabla
  const columns: ColumnDef<OrderResponse>[] = [
    {
      accessorKey: 'id',
      header: 'ID Orden',
      cell: (info) => {
        const id = info.getValue<string>();
        return <span className="font-mono text-xs">{id.substring(0, 8)}...</span>;
      },
    },
    {
      accessorKey: 'status',
      header: 'Estado',
      cell: (info) => {
        const status = info.getValue<string>();
        const statusColors: Record<string, string> = {
          Pending: 'bg-yellow-100 text-yellow-800',
          Confirmed: 'bg-blue-100 text-blue-800',
          Processing: 'bg-purple-100 text-purple-800',
          Shipped: 'bg-cyan-100 text-cyan-800',
          Delivered: 'bg-green-100 text-green-800',
          Cancelled: 'bg-red-100 text-red-800',
        };
        return (
          <span
            className={`px-3 py-1 rounded-full text-xs font-semibold ${
              statusColors[status] || 'bg-gray-100 text-gray-800'
            }`}
          >
            {status}
          </span>
        );
      },
    },
    {
      accessorKey: 'totalAmount',
      header: 'Total',
      cell: (info) => `$${(info.getValue<number>()).toFixed(2)}`,
    },
    {
      id: 'itemCount',
      header: 'Items',
      cell: (info) => {
        const products = info.row.original.orderProducts;
        return products?.length || 0;
      },
    },
    {
      accessorKey: 'createdAt',
      header: 'Fecha',
      cell: (info) => {
        const date = new Date(info.getValue<string>());
        return date.toLocaleDateString('es-ES');
      },
    },
    {
      id: 'actions',
      header: 'Acciones',
      cell: (info) => {
        const order = info.row.original;
        const canCancel =
          order.status === 'Pending' || order.status === 'Confirmed';

        return (
          <div className="flex gap-2">
            {canCancel && (
              <Button
                variant="destructive"
                size="sm"
                onClick={() => handleCancelOrder(order.id)}
                className="flex items-center gap-1"
              >
                <Trash2 className="h-4 w-4" />
                Cancelar
              </Button>
            )}
          </div>
        );
      },
    },
  ];

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <p className="text-muted-foreground">Cargando órdenes...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="bg-destructive/15 text-destructive p-4 rounded">
          {error}
        </div>
      </div>
    );
  }

  // Si hay una orden seleccionada, mostrar detalles
  if (selectedOrder) {
    return (
      <div className="p-4">
        <div className="flex items-center gap-4 mb-6">
          <Button
            variant="outline"
            onClick={() => setSelectedOrder(null)}
          >
            ← Volver
          </Button>
          <h2 className="text-2xl font-bold">Detalles de la Orden</h2>
        </div>

        <div className="space-y-6">
          {/* Información General */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="border rounded-lg p-4">
              <p className="text-sm font-medium text-muted-foreground">ID Orden</p>
              <p className="font-mono text-lg mt-1">{selectedOrder.id.substring(0, 12)}...</p>
            </div>
            <div className="border rounded-lg p-4">
              <p className="text-sm font-medium text-muted-foreground">Estado</p>
              <div className="mt-2">
                <span className={`px-3 py-1 rounded-full text-xs font-semibold ${
                  {
                    Pending: 'bg-yellow-100 text-yellow-800',
                    Confirmed: 'bg-blue-100 text-blue-800',
                    Processing: 'bg-purple-100 text-purple-800',
                    Shipped: 'bg-cyan-100 text-cyan-800',
                    Delivered: 'bg-green-100 text-green-800',
                    Cancelled: 'bg-red-100 text-red-800',
                  }[selectedOrder.status] || 'bg-gray-100 text-gray-800'
                }`}>
                  {selectedOrder.status}
                </span>
              </div>
            </div>
            <div className="border rounded-lg p-4">
              <p className="text-sm font-medium text-muted-foreground">Total</p>
              <p className="text-lg font-bold mt-1 text-primary">${selectedOrder.totalAmount.toFixed(2)}</p>
            </div>
          </div>

          {/* Información del Usuario */}
          <div className="border rounded-lg p-4">
            <p className="text-sm font-medium text-muted-foreground mb-2">Usuario ID</p>
            <p className="font-mono">{selectedOrder.userId}</p>
          </div>

          {/* Dirección de Envío */}
          <div className="border rounded-lg p-4">
            <p className="text-sm font-medium text-muted-foreground mb-2">Dirección de Envío</p>
            <p className="text-base">{selectedOrder.shippingAddress}</p>
          </div>

          {/* Notas */}
          {selectedOrder.notes && (
            <div className="border rounded-lg p-4">
              <p className="text-sm font-medium text-muted-foreground mb-2">Notas</p>
              <p className="text-base">{selectedOrder.notes}</p>
            </div>
          )}

          {/* Productos */}
          <div className="border rounded-lg p-4">
            <p className="text-sm font-medium text-muted-foreground mb-4">Productos ({selectedOrder.orderProducts.length})</p>
            <div className="space-y-3">
              {selectedOrder.orderProducts.map((product) => (
                <div key={product.id} className="flex justify-between items-center p-3 bg-muted rounded">
                  <div>
                    <p className="font-medium">{product.productName}</p>
                    <p className="text-xs text-muted-foreground mt-1">
                      ${product.unitPrice.toFixed(2)} × {product.quantity}
                    </p>
                  </div>
                  <p className="font-bold text-lg">${product.subtotal.toFixed(2)}</p>
                </div>
              ))}
            </div>
          </div>

          {/* Fechas */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="border rounded-lg p-4">
              <p className="text-sm font-medium text-muted-foreground">Fecha de Creación</p>
              <p className="text-base mt-1">{new Date(selectedOrder.createdAt).toLocaleDateString('es-ES')}</p>
            </div>
            <div className="border rounded-lg p-4">
              <p className="text-sm font-medium text-muted-foreground">Última Actualización</p>
              <p className="text-base mt-1">{new Date(selectedOrder.updatedAt).toLocaleDateString('es-ES')}</p>
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="p-4">
      <div className="flex items-center gap-4 mb-6">
        <Button
          variant="outline"
          onClick={() => router.push('/dashboard')}
          className="flex items-center gap-2"
        >
          <ArrowLeft className="h-4 w-4" />
          Volver al Dashboard
        </Button>
        <h2 className="text-2xl font-bold">Órdenes</h2>
      </div>

      {/* Filtros */}
      <div className="mb-6 grid grid-cols-1 md:grid-cols-2 gap-4">
        <div className="space-y-2">
          <label className="text-sm font-medium">Filtrar por Estado</label>
          <Select value={statusFilter || "all"} onValueChange={(value) => setStatusFilter(value === "all" ? "" : value)}>
            <SelectTrigger>
              <SelectValue placeholder="Todos los estados" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">Todos los estados</SelectItem>
              <SelectItem value="Pending">Pendiente</SelectItem>
              <SelectItem value="Confirmed">Confirmada</SelectItem>
              <SelectItem value="Processing">Procesando</SelectItem>
              <SelectItem value="Shipped">Enviada</SelectItem>
              <SelectItem value="Delivered">Entregada</SelectItem>
              <SelectItem value="Cancelled">Cancelada</SelectItem>
            </SelectContent>
          </Select>
        </div>

        <div className="space-y-2">
          <label className="text-sm font-medium">Filtrar por Usuario</label>
          <div className="flex gap-2">
            <input
              type="text"
              placeholder="ID del usuario"
              value={userIdFilter}
              onChange={(e) => setUserIdFilter(e.target.value)}
              className="flex-1 px-3 py-2 border rounded-md text-sm"
            />
            {userIdFilter && (
              <Button
                variant="outline"
                size="sm"
                onClick={() => setUserIdFilter("")}
                className="px-3"
              >
                Limpiar
              </Button>
            )}
          </div>
        </div>
      </div>

      {orders.length === 0 ? (
        <div className="text-center py-12">
          <p className="text-muted-foreground">No hay órdenes para mostrar</p>
        </div>
      ) : (
        <GenericTable
          columns={columns}
          data={orders}
          filterBy="status"
          pageSize={10}
        />
      )}
    </div>
  );
}