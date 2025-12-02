import { Message } from "./Message";
import { Product } from "./Product";

export interface Chat {
  id: string;
  productId: string;
  buyerId: string;
  sellerId: string;
  createdAt: string;
  product?: Product;      
  sellerName?: string;
  buyerName?: string;    
  messages?: Message[];   // mensajes asociados
}
export interface CreateChatRequest {
  buyerId: string;
  sellerId: string;
  productId: string;
}
export interface UpdateChatRequest{
  buyerId: string;
  sellerId: string;
  productId: string;
}