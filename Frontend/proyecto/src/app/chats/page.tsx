'use client';

import { useState } from "react";
import { useRouter } from "next/navigation";
import { ArrowLeft } from "lucide-react";
import { useChats } from "@/hooks/useChats";
import { useMessages } from "@/hooks/useMessages";
import { useProducts } from "@/hooks/useProducts";
import ChatList from "../lists/chatLists";
import ChatWindow from "./chatWindow";
import { Product } from "@/types/Product";

export default function ChatPage() {
  const router = useRouter();
  const { products, loading, error } = useProducts();
  const [selectedChatId, setSelectedChatId] = useState<string | null>(null);
  const [showProductModal, setShowProductModal] = useState(false);
  const [selectedProductId, setSelectedProductId] = useState<string | null>(null);

  const { chats, createChat, fetchChats, currentUserId } = useChats();
  const { messages, fetchMessages, createMessage } = useMessages(selectedChatId);

  const selectedChat = chats.find(chat => chat.id === selectedChatId);

  const handleCreateChat = async () => {
    if (!selectedProductId || !currentUserId) {
      alert("Debes seleccionar un producto e iniciar sesión");
      return;
    }

    const product = products.find((p: Product) => p.id === selectedProductId);
    if (!product) {
      alert("Producto no encontrado");
      return;
    }

    try {
      const newChat = await createChat({
        buyerId: currentUserId,
        sellerId: product.userId,
        productId: product.id
      });

      await fetchChats();
      setSelectedChatId(newChat.id);
      setShowProductModal(false);
      setSelectedProductId(null);
    } catch (err) {
      console.error(err);
      alert(err instanceof Error ? err.message : "No se pudo crear el chat");
    }
  };

  if (loading) return <p className="p-4">Cargando ...</p>;
  if (error) return <p className="p-4 text-red-500">Error: {error}</p>;

  return (
    <div className="flex w-full h-screen">
      {/* Panel izquierdo: lista de chats */}
      <div className="w-1/3 border-r flex flex-col">
        <div className="flex justify-between items-center p-4 border-b">
          <div className="flex items-center gap-2">
            <button
              onClick={() => router.push("/dashboard")}
              className="text-gray-600 hover:text-gray-900 transition-colors"
            >
              <ArrowLeft size={20} />
            </button>
            <h2 className="font-semibold">Conversaciones</h2>
          </div>
          <button
            onClick={() => setShowProductModal(true)}
            className="bg-blue-500 hover:bg-blue-600 text-white px-3 py-1 rounded-lg text-sm"
          >
            Nuevo chat
          </button>
        </div>

        <ChatList
          chats={chats}
          selectedChatId={selectedChatId}
          onSelectChat={setSelectedChatId}
        />
      </div>

      {/* Panel derecho: ventana de chat */}
      {selectedChat && selectedChatId ? (
        <ChatWindow
          chatId={selectedChatId}
          currentUserId={currentUserId!}
          messages={messages}
          buyerId={selectedChat.buyerId}
          sellerId={selectedChat.sellerId}
          buyerName={selectedChat.buyerName || "Comprador"}
          sellerName={selectedChat.sellerName || "Vendedor"}
          onSendMessage={async ({ senderId, content }) => {
            const newMessage = await createMessage({ senderId, content });
            return newMessage;
          }}
          refreshMessages={async () => {
            if (selectedChatId) await fetchMessages(selectedChatId);
          }}
        />
      ) : (
        <div className="flex items-center justify-center w-2/3 text-gray-400">
          Selecciona una conversación para empezar
        </div>
      )}

      {/* Modal de selección de producto */}
      {showProductModal && (
        <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl shadow-lg w-96 p-6">
            <h3 className="text-lg font-semibold mb-4">Selecciona un producto</h3>
            <div className="max-h-64 overflow-y-auto border rounded-md mb-4">
              {products
                .filter(p => p.userId !== currentUserId)
                .map(product => (
                  <div
                    key={product.id}
                    className={`p-3 cursor-pointer border-b hover:bg-blue-50 ${selectedProductId === product.id ? "bg-blue-100" : ""}`}
                    onClick={() => setSelectedProductId(product.id)}
                  >
                    <div className="font-medium text-gray-800">{product.name}</div>
                    <div className="text-sm text-gray-500">
                      Vendedor: {product.userId}
                    </div>
                  </div>
                ))}
            </div>

            <div className="flex justify-end gap-2">
              <button
                onClick={() => setShowProductModal(false)}
                className="px-3 py-1 rounded-md bg-gray-300 hover:bg-gray-400"
              >
                Cancelar
              </button>
              <button
                onClick={handleCreateChat}
                className="px-3 py-1 rounded-md bg-blue-500 text-white hover:bg-blue-600"
              >
                Crear chat
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}