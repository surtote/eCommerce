'use client';

import { ProductList } from '../lists/productLists';
import { Button } from '@/components/ui/button';
import { useRouter } from 'next/navigation';

export default function ProductPage() {
  const router = useRouter();

  const handleAddProduct = () => {
    router.push('/products/form'); // Ruta donde tienes el formulario
  };

  return (
    <div className="p-8">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold">Productos</h1>
        <Button onClick={handleAddProduct}>Añadir Producto</Button>
      </div>

      <ProductList />
    </div>
  );
}
