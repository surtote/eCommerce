'use client';

import { ProductForm } from '@/app/forms/productForm';
import { Button } from '@/components/ui/button';
import { useRouter } from 'next/navigation';

export default function ProductPage() {
  const router = useRouter();

  return (
    <div className="p-8">
      <ProductForm />
    </div>
  );
}
