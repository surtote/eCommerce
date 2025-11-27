import { Message } from '@/types/Message';
import { useState, useEffect, useRef } from 'react';

interface ChatWindowProps {
  chatId: string; // 👈 Guid ahora es string
  currentUserId: string;
  buyerId: string;
  sellerId: string;
  messages: Message[];
  onSendMessage: ({ senderId, content }: { senderId: string; content: string }) => Promise<Message>;
  refreshMessages: () => Promise<void>;
  buyerName: string;
  sellerName: string;
}

export default function ChatWindow({
  chatId,
  currentUserId,
  buyerId,
  sellerId,
  messages,
  onSendMessage,
  refreshMessages,
  buyerName,
  sellerName,
}: ChatWindowProps) {
  const [content, setContent] = useState('');
  const bottomRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  const handleSend = async () => {
    if (!content.trim()) return;
    await onSendMessage({ senderId: currentUserId, content });
    setContent('');
    await refreshMessages();
  };

  return (
    <div className="flex flex-col w-2/3 h-full">
      {/* 💬 Encabezado */}
      <div className="border-b p-4 bg-gray-50 text-sm text-gray-600 flex justify-center">
        💬 Chat entre{" "}
        <span className="font-semibold text-gray-800 mx-1">{sellerName}</span> (Vendedor)
        {" "}y{" "}
        <span className="font-semibold text-gray-800 mx-1">{buyerName}</span> (Comprador)
      </div>

      {/* 📩 Mensajes */}
      <div className="flex-1 overflow-y-auto p-4 space-y-2 bg-white">
        {messages.map((msg) => {
          const isMine = msg.senderId === currentUserId;
          const senderName =
            msg.senderId === buyerId
              ? buyerName
              : msg.senderId === sellerId
                ? sellerName
                : 'Usuario desconocido';

          return (
            <div
              key={msg.id}
              className={`flex ${isMine ? 'justify-end' : 'justify-start'}`}
            >
              <div
                className={`max-w-xs px-3 py-2 rounded-2xl text-sm shadow-sm ${
                  isMine ? 'bg-blue-500 text-white' : 'bg-gray-200 text-gray-800'
                }`}
              >
                {!isMine && (
                  <div className="text-xs text-gray-500 mb-1">{senderName}</div>
                )}
                <div>{msg.content}</div>
                <div className="text-[10px] text-gray-500 mt-1 text-right">
                  {(() => {
                    const dateString = msg.sentAt || msg.sentAt || msg.timestamp;
                    if (!dateString) return '';
                    const date = new Date(dateString);
                    if (isNaN(date.getTime())) return '';
                    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
                  })()}
                </div>
              </div>
            </div>
          );
        })}
        <div ref={bottomRef} />
      </div>

      {/* ✍️ Input */}
      <div className="p-4 border-t flex items-center bg-white">
        <input
          type="text"
          value={content}
          onChange={(e) => setContent(e.target.value)}
          placeholder="Escribe un mensaje..."
          className="flex-1 border rounded-full px-4 py-2 mr-2 focus:outline-none focus:ring focus:ring-blue-200"
          onKeyDown={(e) => e.key === 'Enter' && handleSend()}
        />
        <button
          onClick={handleSend}
          className="bg-blue-500 hover:bg-blue-600 text-white rounded-full px-4 py-2"
        >
          Enviar
        </button>
      </div>
    </div>
  );
}
