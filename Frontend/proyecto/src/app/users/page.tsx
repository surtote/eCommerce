"use client";

import { UserList } from '../lists/userLists';

export default function Page() {
  return (
    <div className="min-h-screen w-full bg-gray-50 p-8 flex flex-col items-center justify-start">
      <h1 className="text-3xl font-bold mb-6">Usuarios</h1>
      <div className="w-full max-w-6xl">
        <UserList />
      </div>
    </div>
  );
}
