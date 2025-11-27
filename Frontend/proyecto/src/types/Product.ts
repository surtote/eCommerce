export interface Product {
  id: string;
  name: string;
  categoryId: string,
  price: number;
  description?: string;
  stock?: number;
  userId: string;
  imageData?: string; // Base64 si decides enviar así
  imageContentType?: string;
  createdAt: string;
  categoryName?: string; //
}

export interface CreateProductRequest {
  name: string;
  categoryId: string,
  price: number;
  description?: string;
  stock?: number;
  userId: string;
  image?: File; // Imagen opcional
}
