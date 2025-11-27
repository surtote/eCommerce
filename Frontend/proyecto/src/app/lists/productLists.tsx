"use client";
import * as React from "react";
import { GenericTable } from "../components/genericTable";
import { useProducts } from "@/hooks/useProducts";
import type { ColumnDef } from "@tanstack/react-table";
import { Product } from "@/types/Product";
import { CategoryDropdown } from "../components/categoryDropdown";
import Image from 'next/image';
export const ProductList = () => {
  const {
    products,
    loading,
    error,
    fetchProductsByCategory,
    selectedCategory,
    setSelectedCategory,
    fetchProducts,
  } = useProducts();

  // 🔹 Obtener el usuario actual del sessionStorage
  const currentUser =
    typeof window !== "undefined"
      ? JSON.parse(sessionStorage.getItem("currentUser") || "{}")
      : null;
  const currentUserId = currentUser?.id || null;

  // 🔹 Filtrar productos para que solo muestre los del usuario logueado
  const filteredProducts = React.useMemo(() => {
    if (!currentUserId) return products;
    return products.filter((p) => p.userId === currentUserId);
  }, [products, currentUserId]);

  // 🔹 Manejar cambio de categoría
  const handleCategoryChange = (categoryId: string | null) => {
    if (categoryId) {
      fetchProductsByCategory(categoryId);
    } else {
      setSelectedCategory(null);
      fetchProducts(); // Mostrar todos si se elige "Todas las categorías"
    }
  };

  const columns: ColumnDef<Product>[] = [
    { accessorKey: "name", header: "Nombre" },
    {
      accessorKey: "categoryName",
      header: "Categoría",
      cell: (info) => info.getValue() || "-",
    },
    {
      accessorKey: "description",
      header: "Descripción",
      cell: (info) => info.getValue() || "-",
    },
    {
      accessorKey: "price",
      header: "Precio",
      cell: (info) => `$${info.getValue()}`,
    },
    {
      accessorKey: "stock",
      header: "Stock",
      cell: (info) => info.getValue() ?? "-",
    },
    {
      accessorKey: "imageData",
      header: "Imagen",
      cell: (info) => {
        const value = info.getValue<string | undefined>();
        if (!value) return <span>-</span>;
        return (
          <Image
            src={`data:${info.row.original.imageContentType};base64,${value}`}
            alt={info.row.original.name}
            width={64}   // equivalente a w-16
            height={64}  // equivalente a h-16
            className="object-cover rounded"
          />
        );
      },
    },
  ];

  if (loading) return <p>Cargando productos...</p>;
  if (error) return <p className="text-red-500">Error: {error}</p>;

  return (
    <div className="p-4">
      <h2 className="mb-4 text-lg font-semibold">Mis Productos</h2>

      {/* 🔹 Dropdown para filtrar por categoría */}
      <div className="mb-4">
        <CategoryDropdown
          value={selectedCategory}
          onSelect={handleCategoryChange}
          showAllOption={true}
        />
      </div>

      {filteredProducts.length === 0 ? (
        <p>No hay productos para mostrar.</p>
      ) : (
        <GenericTable
          columns={columns}
          data={filteredProducts}
          filterBy="name"
          pageSize={5}
        />
      )}
    </div>
  );
};
