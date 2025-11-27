"use client"

import * as React from "react"
import type { ColumnDef, SortingState } from "@tanstack/react-table"
import {
  useReactTable,
  getCoreRowModel,
  getSortedRowModel,
  getPaginationRowModel,
  flexRender,
} from "@tanstack/react-table"

import { Table, TableHeader, TableBody, TableRow, TableHead, TableCell } from "@/components/ui/table"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"

interface GenericTableProps<T> {
  columns: ColumnDef<T>[]
  data: T[]
  filterBy?: keyof T
  pageSize?: number
}

export function GenericTable<T>({
  columns,
  data,
  filterBy,
  pageSize = 5,
}: GenericTableProps<T>) {
  const [sorting, setSorting] = React.useState<SortingState>([])
  const [filterValue, setFilterValue] = React.useState("")
  const [pageIndex, setPageIndex] = React.useState(0)

  const filteredData = React.useMemo(() => {
    if (!filterBy) return data
    return data.filter(item =>
      String(item[filterBy]).toLowerCase().includes(filterValue.toLowerCase())
    )
  }, [data, filterValue, filterBy])

  const table = useReactTable({
    data: filteredData,
    columns,
    state: { sorting, pagination: { pageIndex, pageSize } },
    onSortingChange: setSorting,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    pageCount: Math.ceil(filteredData.length / pageSize),
  })

  return (
    <div>
      {filterBy && (
        <Input
          placeholder={`Filtrar por ${String(filterBy)}...`}
          value={filterValue}
          onChange={e => setFilterValue(e.target.value)}
          className="w-full border rounded-md p-2"
        />
      )}

      <Table>
        <TableHeader>
          {table.getHeaderGroups().map(headerGroup => (
            <TableRow key={headerGroup.id}>
              {headerGroup.headers.map(header => (
                <TableHead key={header.id}>
                  {header.isPlaceholder
                    ? null
                    : flexRender(header.column.columnDef.header, header.getContext())}
                </TableHead>
              ))}
            </TableRow>
          ))}
        </TableHeader>

        <TableBody>
          {table.getRowModel().rows.length ? (
            table.getRowModel().rows.map(row => (
              <TableRow key={row.id}>
                {row.getVisibleCells().map(cell => (
                  <TableCell key={cell.id}>
                    {flexRender(cell.column.columnDef.cell, cell.getContext())}
                  </TableCell>
                ))}
              </TableRow>
            ))
          ) : (
            <TableRow>
              <TableCell colSpan={columns.length} className="text-center">
                No se encontraron resultados
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>

      <div className="flex justify-end space-x-2 mt-2">
        <Button
          size="sm"
          variant="outline"
          onClick={() => setPageIndex(prev => Math.max(prev - 1, 0))}
          disabled={pageIndex === 0}
        >
          Prev
        </Button>
        <Button
          size="sm"
          variant="outline"
          onClick={() => setPageIndex(prev => Math.min(prev + 1, table.getPageCount() - 1))}
          disabled={pageIndex >= table.getPageCount() - 1}
        >
          Next
        </Button>
      </div>
    </div>
  )
}
