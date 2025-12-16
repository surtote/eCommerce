'use client';

import { ProductList } from '../lists/productLists';
import { Button } from '@/components/ui/button';
import { useRouter } from 'next/navigation';
import { ArrowLeft } from 'lucide-react';

export default function ProductPage() {
  const router = useRouter();

  const handleAddProduct = () => {
    router.push('/products/form'); // Ruta donde tienes el formulario
  };

  const handleBackToDashboard = () => {
    router.push('/dashboard'); 
  };

  return (
    <div className="p-8">
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-4">
          <Button
            variant="outline"
            size="sm"
            onClick={handleBackToDashboard}
            className="flex items-center gap-2"
          >
            <ArrowLeft className="h-4 w-4" />
            Volver
          </Button>
        </div>
        <Button onClick={handleAddProduct}>Añadir Producto</Button>
      </div>

      <ProductList />
    </div>
  );
}