'use client';

import React, { useState, useEffect, useCallback } from 'react';
import { useRouter } from 'next/navigation';
import { Loader2, ArrowLeft, MapPin, CreditCard } from 'lucide-react';
import Image from 'next/image';
import { Button } from '@/components/ui/button';
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardDescription,
} from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Separator } from '@/components/ui/separator';
import { orderService } from '@/services/orderService';

interface CartItem {
  id: string;
  name: string;
  price: number;
  imageData?: string;
  imageContentType?: string;
  quantity: number;
}

interface ShippingData {
  fullName: string;
  email: string;
  phone: string;
  address: string;
  city: string;
  postalCode: string;
  country: string;
  notes: string;
}

export default function CheckoutPage() {
  const [isClient, setIsClient] = useState(false);
  const [cartItems, setCartItems] = useState<CartItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [step, setStep] = useState<'shipping' | 'payment' | 'review'>('shipping');
  const router = useRouter();

  const [shippingData, setShippingData] = useState<ShippingData>({
    fullName: '',
    email: '',
    phone: '',
    address: '',
    city: '',
    postalCode: '',
    country: '',
    notes: '',
  });

  const [paymentData, setPaymentData] = useState({
    cardName: '',
    cardNumber: '',
    expiryDate: '',
    cvv: '',
  });

  const loadCart = useCallback(() => {
    if (typeof window !== 'undefined') {
      const cart = JSON.parse(localStorage.getItem('cart') || '[]');
      if (cart.length === 0) {
        router.push('/dashboard');
        return;
      }
      setCartItems(cart);
    }
  }, [router]);

  useEffect(() => {
    setIsClient(true);
    loadCart();
    
    // Obtener token del usuario
    const stored = localStorage.getItem('currentUser');
    if (stored) {
      const user = JSON.parse(stored);
      if (user?.token) {
        orderService.setToken(user.token);
      }
    }
  }, [loadCart]);

  const subtotal = cartItems.reduce((acc, item) => acc + item.price * item.quantity, 0);
  const tax = subtotal * 0.1;
  const shippingCost = subtotal > 100 ? 0 : 10;
  const total = subtotal + tax + shippingCost;

  const handleShippingChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setShippingData(prev => ({
      ...prev,
      [name]: value,
    }));
  };

  const validateShipping = () => {
    const required = ['fullName', 'email', 'phone', 'address', 'city', 'postalCode', 'country'];
    for (const field of required) {
      if (!shippingData[field as keyof ShippingData]) {
        alert(`Por favor completa el campo: ${field}`);
        return false;
      }
    }
    return true;
  };

  const handlePaymentChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name } = e.target;
    let { value } = e.target;

    // Formatear número de tarjeta
    if (name === 'cardNumber') {
      value = value.replace(/\s/g, '').replace(/(\d{4})/g, '$1 ').trim();
    }

    // Formatear fecha de expiración
    if (name === 'expiryDate') {
      value = value.replace(/\D/g, '');
      if (value.length >= 2) {
        value = value.slice(0, 2) + '/' + value.slice(2, 4);
      }
    }

    // Limitar CVV a 3-4 dígitos
    if (name === 'cvv') {
      value = value.replace(/\D/g, '').slice(0, 4);
    }

    setPaymentData(prev => ({
      ...prev,
      [name]: value,
    }));
  };

  const validatePayment = () => {
    if (!paymentData.cardName || !paymentData.cardNumber || !paymentData.expiryDate || !paymentData.cvv) {
      alert('Por favor completa todos los datos de pago');
      return false;
    }
    if (paymentData.cardNumber.replace(/\s/g, '').length !== 16) {
      alert('El número de tarjeta debe tener 16 dígitos');
      return false;
    }
    if (paymentData.cvv.length < 3) {
      alert('El CVV debe tener al menos 3 dígitos');
      return false;
    }
    return true;
  };

  const handlePlaceOrder = async () => {
    setLoading(true);
    try {
      // Construir la orden según tu endpoint
      const orderData = {
        items: cartItems.map(item => ({
          productId: item.id,
          quantity: item.quantity,
        })),
        shippingAddress: `${shippingData.address}, ${shippingData.city}, ${shippingData.postalCode}, ${shippingData.country}`,
        notes: shippingData.notes,
      };

      // Crear la orden usando el servicio
      await orderService.createOrder(orderData);

      // Limpiar carrito
      localStorage.removeItem('cart');
      
      // Mostrar mensaje de éxito
      alert('¡Orden creada exitosamente!');
      
      // Redirigir al dashboard
      router.push('/dashboard');
    } catch (error) {
      alert('Error al procesar la orden: ' + (error instanceof Error ? error.message : 'Error desconocido'));
    } finally {
      setLoading(false);
    }
  };

  if (!isClient || cartItems.length === 0) {
    return null;
  }

  return (
    <div className="min-h-screen bg-background p-6">
      <div className="max-w-6xl mx-auto">
        {/* Header */}
        <div className="mb-8">
          <Button
            variant="ghost"
            onClick={() => router.push('/dashboard')}
            className="mb-4"
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Volver al carrito
          </Button>
          <h1 className="text-3xl font-bold tracking-tight">Checkout</h1>
          <p className="text-muted-foreground mt-2">Completa tu compra</p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 auto-rows-max">
          {/* Formulario */}
          <div className="lg:col-span-2 space-y-6">
            {/* Pasos */}
            <div className="flex gap-4 mb-8">
              <div className={`flex items-center gap-2 pb-2 border-b-2 transition ${step === 'shipping' ? 'border-primary text-primary' : 'border-gray-200'}`}>
                <MapPin className="h-5 w-5" />
                <span className="font-semibold">Envío</span>
              </div>
              <div className={`flex items-center gap-2 pb-2 border-b-2 transition ${step === 'payment' ? 'border-primary text-primary' : 'border-gray-200'}`}>
                <CreditCard className="h-5 w-5" />
                <span className="font-semibold">Pago</span>
              </div>
              <div className={`flex items-center gap-2 pb-2 border-b-2 transition ${step === 'review' ? 'border-primary text-primary' : 'border-gray-200'}`}>
                <span className="font-semibold">Revisión</span>
              </div>
            </div>

            {/* Paso 1: Envío */}
            {step === 'shipping' && (
              <Card className="flex flex-col">
                <CardHeader>
                  <CardTitle>Información de Envío</CardTitle>
                  <CardDescription>Proporciona tu dirección de entrega</CardDescription>
                </CardHeader>
                <CardContent className="space-y-4 flex-1">
                  <div className="grid grid-cols-2 gap-4">
                    <Input
                      placeholder="Nombre completo"
                      name="fullName"
                      value={shippingData.fullName}
                      onChange={handleShippingChange}
                    />
                    <Input
                      placeholder="Email"
                      name="email"
                      type="email"
                      value={shippingData.email}
                      onChange={handleShippingChange}
                    />
                  </div>

                  <Input
                    placeholder="Teléfono"
                    name="phone"
                    value={shippingData.phone}
                    onChange={handleShippingChange}
                  />

                  <Input
                    placeholder="Dirección"
                    name="address"
                    value={shippingData.address}
                    onChange={handleShippingChange}
                  />

                  <div className="grid grid-cols-2 gap-4">
                    <Input
                      placeholder="Ciudad"
                      name="city"
                      value={shippingData.city}
                      onChange={handleShippingChange}
                    />
                    <Input
                      placeholder="Código postal"
                      name="postalCode"
                      value={shippingData.postalCode}
                      onChange={handleShippingChange}
                    />
                  </div>

                  <Input
                    placeholder="País"
                    name="country"
                    value={shippingData.country}
                    onChange={handleShippingChange}
                  />

                  <Textarea
                    placeholder="Notas adicionales (opcional)"
                    name="notes"
                    value={shippingData.notes}
                    onChange={handleShippingChange}
                    rows={4}
                  />

                  <Button
                    onClick={() => {
                      if (validateShipping()) {
                        setStep('payment');
                      }
                    }}
                    className="w-full"
                    size="lg"
                  >
                    Continuar al Pago
                  </Button>
                </CardContent>
              </Card>
            )}

            {/* Paso 2: Pago */}
            {step === 'payment' && (
              <Card className="flex flex-col">
                <CardHeader>
                  <CardTitle>Información de Pago</CardTitle>
                  <CardDescription>Ingresa los detalles de tu tarjeta</CardDescription>
                </CardHeader>
                <CardContent className="space-y-4 flex-1">
                  <Input
                    placeholder="Nombre en la tarjeta"
                    name="cardName"
                    value={paymentData.cardName}
                    onChange={handlePaymentChange}
                  />

                  <Input
                    placeholder="Número de tarjeta (16 dígitos)"
                    name="cardNumber"
                    value={paymentData.cardNumber}
                    onChange={handlePaymentChange}
                    maxLength={19}
                  />

                  <div className="grid grid-cols-2 gap-4">
                    <Input
                      placeholder="MM/YY"
                      name="expiryDate"
                      value={paymentData.expiryDate}
                      onChange={handlePaymentChange}
                      maxLength={5}
                    />
                    <Input
                      placeholder="CVV"
                      name="cvv"
                      value={paymentData.cvv}
                      onChange={handlePaymentChange}
                      maxLength={4}
                      type="password"
                    />
                  </div>

                  <div className="flex gap-4">
                    <Button
                      onClick={() => setStep('shipping')}
                      variant="outline"
                      className="flex-1"
                      size="lg"
                    >
                      Atrás
                    </Button>
                    <Button
                      onClick={() => {
                        if (validatePayment()) {
                          setStep('review');
                        }
                      }}
                      className="flex-1"
                      size="lg"
                    >
                      Revisar Orden
                    </Button>
                  </div>
                </CardContent>
              </Card>
            )}

            {/* Paso 3: Revisión */}
            {step === 'review' && (
              <Card className="flex flex-col">
                <CardHeader>
                  <CardTitle>Revisión de la Orden</CardTitle>
                  <CardDescription>Verifica todos los detalles antes de confirmar</CardDescription>
                </CardHeader>
                <CardContent className="space-y-6 flex-1">
                  {/* Datos de envío */}
                  <div>
                    <h3 className="font-semibold mb-3">Dirección de Envío</h3>
                    <div className="bg-muted p-4 rounded-lg text-sm space-y-1">
                      <p>{shippingData.fullName}</p>
                      <p>{shippingData.address}</p>
                      <p>{shippingData.city}, {shippingData.postalCode}</p>
                      <p>{shippingData.country}</p>
                      <p className="text-muted-foreground">Email: {shippingData.email}</p>
                      <p className="text-muted-foreground">Teléfono: {shippingData.phone}</p>
                    </div>
                  </div>

                  <Separator />

                  {/* Datos de pago */}
                  <div>
                    <h3 className="font-semibold mb-3">Método de Pago</h3>
                    <div className="bg-muted p-4 rounded-lg text-sm">
                      <p>{paymentData.cardName}</p>
                      <p>•••• •••• •••• {paymentData.cardNumber.slice(-4)}</p>
                    </div>
                  </div>

                  <Separator />

                  {/* Productos */}
                  <div>
                    <h3 className="font-semibold mb-3">Productos</h3>
                    <div className="space-y-3">
                      {cartItems.map(item => (
                        <div key={item.id} className="flex gap-3 bg-muted p-3 rounded-lg">
                          {item.imageData && (
                            <div className="relative w-16 h-16 rounded overflow-hidden shrink-0">
                              <Image
                                src={`data:${item.imageContentType};base64,${item.imageData}`}
                                alt={item.name}
                                width={64}
                                height={64}
                                className="w-full h-full object-cover"
                              />
                            </div>
                          )}
                          <div className="flex-1">
                            <p className="font-semibold text-sm">{item.name}</p>
                            <p className="text-xs text-muted-foreground">
                              {item.quantity} x ${item.price.toFixed(2)}
                            </p>
                            <p className="text-sm font-semibold text-primary mt-1">
                              ${(item.price * item.quantity).toFixed(2)}
                            </p>
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  <div className="flex gap-4">
                    <Button
                      onClick={() => setStep('payment')}
                      variant="outline"
                      className="flex-1"
                      size="lg"
                    >
                      Atrás
                    </Button>
                    <Button
                      onClick={handlePlaceOrder}
                      disabled={loading}
                      className="flex-1"
                      size="lg"
                    >
                      {loading ? (
                        <>
                          <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                          Procesando...
                        </>
                      ) : (
                        'Confirmar Orden'
                      )}
                    </Button>
                  </div>
                </CardContent>
              </Card>
            )}
          </div>

          {/* Resumen lateral */}
          <div className="lg:col-span-1 mt-16">
            <Card className="sticky top-6 h-fit">
              <CardHeader>
                <CardTitle>Resumen</CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                {/* Productos */}
                <div className="space-y-3 max-h-64 overflow-y-auto">
                  {cartItems.map(item => (
                    <div key={item.id} className="flex justify-between text-sm">
                      <span className="text-muted-foreground">
                        {item.name} x{item.quantity}
                      </span>
                      <span className="font-semibold">
                        ${(item.price * item.quantity).toFixed(2)}
                      </span>
                    </div>
                  ))}
                </div>

                <Separator />

                {/* Totales */}
                <div className="space-y-2 text-sm">
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Subtotal:</span>
                    <span>${subtotal.toFixed(2)}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Impuesto (10%):</span>
                    <span>${tax.toFixed(2)}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Envío:</span>
                    <span>
                      {shippingCost === 0 ? (
                        <span className="text-green-600 font-semibold">Gratis</span>
                      ) : (
                        `$${shippingCost.toFixed(2)}`
                      )}
                    </span>
                  </div>
                </div>

                <Separator />

                <div className="flex justify-between text-lg font-bold">
                  <span>Total:</span>
                  <span className="text-primary">${total.toFixed(2)}</span>
                </div>

                {subtotal <= 100 && (
                  <div className="bg-blue-50 border border-blue-200 rounded-lg p-3 text-xs text-blue-800">
                    💡 ¡Envío gratis en órdenes mayores a $100!
                  </div>
                )}
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    </div>
  );
}