"use client";
import * as React from "react";
import { GenericTable } from "../components/genericTable";
import { useUsers } from "@/hooks/useUsers";
import type { ColumnDef } from "@tanstack/react-table";
import { User } from "@/types/User";

export const UserList = () => {
  const { users, loading, error, refetch } = useUsers();

  // 🔹 Columnas de la tabla
  const columns: ColumnDef<User>[] = [
    { accessorKey: "nombre", header: "Nombre" },
    { accessorKey: "apellido", header: "Apellido" },
    { accessorKey: "dni", header: "DNI" },
    { accessorKey: "email", header: "Email" },
    { accessorKey: "telefono", header: "Telefono" },
    { accessorKey: "direccion", header: "Dirección" },
  ];

  // 🔹 Refetch manual si quieres un botón, opcional
  // <button onClick={refetch}>Actualizar</button>

  if (loading) return <p>Cargando usuarios...</p>;
  if (error) return <p className="text-red-500">Error: {error}</p>;
  if (!users.length) return <p>No hay usuarios.</p>;

  return (
    <div className="p-4">
      <h2 className="mb-4 text-lg font-semibold">Lista de Usuarios</h2>
      <GenericTable columns={columns} data={users} filterBy="apellido" pageSize={5} />
    </div>
  );
};
