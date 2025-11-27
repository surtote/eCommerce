import { Message } from "./Message";
import { Product } from "./Product";
import { User } from "./User";

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
