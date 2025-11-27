'use client';

import { useEffect, useState } from "react";
import {
  Select,
  SelectTrigger,
  SelectValue,
  SelectContent,
  SelectItem,
} from "@/components/ui/select";
import { categoryService } from "../../services/categoriesService";
import { Category } from "../../types/Category";

interface CategoryDropdownProps {
  value?: string | null; // ✅ ahora los ID son string (GUID)
  onSelect: (id: string | null) => void; // ✅ acepta string o null
  showAllOption?: boolean;
}

export function CategoryDropdown({
  value = null,
  onSelect,
  showAllOption = false,
}: CategoryDropdownProps) {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadCategories = async () => {
      try {
        const data = await categoryService.getAll();
        setCategories(data);
      } catch (error) {
        console.error("Error cargando categorías:", error);
      } finally {
        setLoading(false);
      }
    };

    loadCategories();
  }, []);

  if (loading) return <div>Cargando categorías...</div>;

  return (
    <Select
      value={value ?? "all"} // ✅ usa string o "all"
      onValueChange={(val) => {
        const id = val === "all" ? null : val; // ✅ sin conversión numérica
        onSelect(id);
      }}
    >
      <SelectTrigger className="w-full">
        <SelectValue placeholder="Selecciona una categoría" />
      </SelectTrigger>

      <SelectContent className="w-full">
        {showAllOption && (
          <SelectItem value="all">Todas las categorías</SelectItem>
        )}

        {categories.map((c) => (
          <SelectItem key={c.id} value={c.id}>
            {c.name}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
}
