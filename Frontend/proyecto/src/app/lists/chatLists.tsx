'use client';

import { Trash2 } from "lucide-react";
import { useChats } from "@/hooks/useChats";
import { Chat } from "@/types/Chat";

interface ChatListProps {
  chats: Chat[];
  selectedChatId: string | null; // ✅ ahora string
  onSelectChat: (id: string) => void; // ✅ ahora string
}

export default function ChatList({ selectedChatId, onSelectChat }: ChatListProps) {
  const { chats, loading, deleteChat } = useChats();

  if (loading) {
    return <div className="p-4 text-gray-400 text-sm">Cargando chats...</div>;
  }

  if (!chats || chats.length === 0) {
    return <div className="p-4 text-gray-400 text-sm">No tienes conversaciones aún.</div>;
  }

  const handleDelete = async (id: string) => { // ✅ tipo string
    if (confirm("¿Seguro que quieres eliminar este chat?")) {
      await deleteChat(id);
    }
  };

  return (
    <div className="overflow-y-auto flex-1">
      {chats.map((chat) => (
        <div
          key={chat.id}
          className={`p-4 border-b flex items-center justify-between hover:bg-gray-100 transition ${
            chat.id === selectedChatId ? "bg-blue-100" : ""
          }`}
        >
          <div
            className="flex-1 cursor-pointer"
            onClick={() => onSelectChat(chat.id)} // ✅ string
          >
            <div className="font-semibold text-gray-800">
              🧑 {chat.sellerName} &nbsp;💬&nbsp; 👤 {chat.buyerName}
            </div>
            <div className="text-xs text-gray-500">
              Creado: {new Date(chat.createdAt).toLocaleString()}
            </div>
          </div>

          {/* 🗑️ Botón eliminar */}
          <button
            onClick={(e) => {
              e.stopPropagation();
              handleDelete(chat.id);
            }}
            className="ml-3 text-gray-400 hover:text-red-500 transition-colors"
          >
            <Trash2 size={18} />
          </button>
        </div>
      ))}
    </div>
  );
}
