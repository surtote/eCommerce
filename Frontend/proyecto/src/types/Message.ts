import { User } from "./User";
import { Chat } from "./Chat";

export interface Message {
  id: string;
  chatId: string;
  senderId: string;
  senderName: string;
  content: string;
  sentAt: string;
  sender?: User;   // opcional, si el backend lo incluye
  chat?: Chat;     // opcional
  timestamp?: string;
}
