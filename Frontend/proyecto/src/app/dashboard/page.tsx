'use client';

import * as React from 'react';
import { Loader2, MessageCircle, ShoppingCart } from 'lucide-react';
import Image from 'next/image';
import { useRouter } from 'next/navigation';
import { useProducts } from '@/hooks/useProducts';
import { CategoryDropdown } from '../components/categoryDropdown';
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  CardFooter,
} from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Product } from '@/types/Product';
import CartSidebar from '../components/carritoSidebar';

export default function ProductsDashboard() {
  const [isClient, setIsClient] = React.useState(false);
  const router = useRouter();
  const {
    products,
    loading,
    error,
    fetchProductsByCategory,
    selectedCategory,
    setSelectedCategory,
    fetchProducts,
  } = useProducts();

  // Esto asegura que el componente solo se renderiza en el cliente
  React.useEffect(() => {
    setIsClient(true);
  }, []);

  // Si no estamos en el cliente, no renderizamos nada
  if (!isClient) {
    return null;
  }

  const filteredProducts = products;

  const handleCategoryChange = (categoryId: string | null) => {
    if (categoryId) {
      fetchProductsByCategory(categoryId);
    } else {
      setSelectedCategory(null);
      fetchProducts();
    }
  };

  const handleChat = (productId: string, sellerId: string) => {
    // Redirige a la página de chats con el producto
    router.push(`/chats?productId=${productId}&sellerId=${sellerId}`);
  };

  const handleAddToCart = (product: Product) => {
    // Obtén el carrito del localStorage
    const cart = JSON.parse(localStorage.getItem('cart') || '[]');

    // Verifica si el producto ya está en el carrito
    const existingItem = cart.find((item: Product) => item.id === product.id);

    if (existingItem) {
      existingItem.quantity += 1;
    } else {
      cart.push({
        id: product.id,
        name: product.name,
        price: product.price,
        imageData: product.imageData,
        imageContentType: product.imageContentType,
        quantity: 1,
      });
    }

    localStorage.setItem('cart', JSON.stringify(cart));
    // Disparar evento para que CartSidebar se actualice
    window.dispatchEvent(new Event('cartUpdated'));
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="flex flex-col items-center gap-4">
          <Loader2 className="h-8 w-8 animate-spin text-primary" />
          <p className="text-muted-foreground">Cargando productos...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <Card className="w-full max-w-md border-destructive">
          <CardContent className="pt-6">
            <p className="text-destructive">Error: {error}</p>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <>
      <div className="min-h-screen bg-background p-6">
        <div className="max-w-7xl mx-auto">
          {/* Header */}
          <div className="mb-8">
            <h2 className="text-3xl font-bold tracking-tight">Re-Sports</h2>
          </div>

          {/* Filtro por categoría */}
          <div className="mb-6">
            <CategoryDropdown
              value={selectedCategory}
              onSelect={handleCategoryChange}
            />
          </div>

          {/* Grid de productos */}
          {filteredProducts.length === 0 ? (
            <Card className="flex items-center justify-center py-12">
              <CardContent>
                <p className="text-muted-foreground text-center">
                  No hay productos para mostrar.
                </p>
              </CardContent>
            </Card>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {filteredProducts.map((product) => (
                <Card key={product.id} className="overflow-hidden hover:shadow-lg transition-shadow">
                  {/* Imagen */}
                  <div className="relative w-full h-48 bg-muted overflow-hidden">
                    {product.imageData ? (
                      <Image
                        src={`data:${product.imageContentType};base64,${product.imageData}`}
                        alt={product.name}
                        width={400}
                        height={400}
                        className="w-full h-full object-cover"
                      />
                    ) : (
                      <div className="w-full h-full flex items-center justify-center bg-muted">
                        <span className="text-muted-foreground">Sin imagen</span>
                      </div>
                    )}
                    {product.categoryName && (
                      <div className="absolute top-2 right-2 bg-primary text-primary-foreground px-3 py-1 rounded-full text-xs font-semibold">
                        {product.categoryName}
                      </div>
                    )}
                  </div>

                  {/* Contenido */}
                  <CardHeader>
                    <CardTitle className="line-clamp-2">{product.name}</CardTitle>
                    <CardDescription className="line-clamp-2">
                      {product.description || 'Sin descripción'}
                    </CardDescription>
                  </CardHeader>

                  <CardContent>
                    <div className="flex justify-between items-center">
                      <div className="text-2xl font-bold text-primary">
                        ${product.price?.toFixed(2) || '0.00'}
                      </div>
                      {product.stock !== undefined && (
                        <div className="text-sm text-muted-foreground">
                          Stock: {product.stock}
                        </div>
                      )}
                    </div>
                  </CardContent>

                  {/* Acciones */}
                  <CardFooter className="gap-2">
                    <Button
                      variant="outline"
                      size="sm"
                      className="flex-1"
                      onClick={() => handleChat(product.id, product.userId || '')}
                    >
                      <MessageCircle className="h-4 w-4 mr-2" />
                      Chat
                    </Button>
                    <Button
                      size="sm"
                      className="flex-1"
                      disabled={product.stock !== undefined && product.stock <= 0}
                      onClick={() => handleAddToCart(product)}
                    >
                      <ShoppingCart className="h-4 w-4 mr-2" />
                      {product.stock !== undefined && product.stock <= 0
                        ? 'Sin stock'
                        : 'Carrito'}
                    </Button>
                  </CardFooter>
                </Card>
              ))}
            </div>
          )}
        </div>
      </div>

      {/* Carrito importado */}
      <CartSidebar />
    </>
  );
}