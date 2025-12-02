"use client";
import * as React from "react";
import { GenericTable } from "../components/genericTable";
import { useProducts } from "@/hooks/useProducts";
import type { ColumnDef } from "@tanstack/react-table";
import { Product } from "@/types/Product";
import { CategoryDropdown } from "../components/categoryDropdown";
import Image from 'next/image';
import { Trash2 } from "lucide-react"; // icono de papelera

export const ProductList = () => {
  const {
    products,
    loading,
    error,
    fetchProductsByCategory,
    selectedCategory,
    setSelectedCategory,
    fetchProducts,
    deleteProduct,
  } = useProducts();

  const currentUser =
    typeof window !== "undefined"
      ? JSON.parse(sessionStorage.getItem("currentUser") || "{}")
      : null;
  const currentUserId = currentUser?.id || null;

  const filteredProducts = React.useMemo(() => {
    if (!currentUserId) return products;
    return products.filter((p) => p.userId === currentUserId);
  }, [products, currentUserId]);

  const handleCategoryChange = (categoryId: string | null) => {
    if (categoryId) {
      fetchProductsByCategory(categoryId);
    } else {
      setSelectedCategory(null);
      fetchProducts();
    }
  };

  const handleDelete = async (id: string) => {
    if (confirm("¿Seguro que deseas eliminar este producto?")) {
      await deleteProduct(id);
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
            width={64}
            height={64}
            className="object-cover rounded"
          />
        );
      },
    },
    {
      id: "actions",
      header: "Acciones",
      cell: (info) => (
        <button
          onClick={() => handleDelete(info.row.original.id)}
          className="text-red-500 hover:text-red-700"
        >
          <Trash2 size={18} />
        </button>
      ),
    },
  ];

  if (loading) return <p>Cargando productos...</p>;
  if (error) return <p className="text-red-500">Error: {error}</p>;

  return (
    <div className="p-4">
      <h2 className="mb-4 text-lg font-semibold">Mis Productos</h2>

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
