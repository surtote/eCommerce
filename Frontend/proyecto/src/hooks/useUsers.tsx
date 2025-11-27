// src/hooks/useUsers.ts
import { useState, useEffect } from 'react';
import { User, CreateUserRequest } from '@/types/User';
import { userService } from '@/services/userService';

export function useUsers() {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  // 🔹 Cargar usuarios al montar el hook
  useEffect(() => {
    const fetchUsers = async () => {
      try {
        setLoading(true);
        const data = await userService.getUsers();
        setUsers(data);
        setError('');
      } catch (err) {
        console.error('Error al cargar usuarios:', err);
        setError((err as Error).message);
        setUsers([]);
      } finally {
        setLoading(false);
      }
    };

    fetchUsers();
  }, []);

  // 🔹 Crear usuario
  const createUser = async (userData: CreateUserRequest): Promise<User> => {
    const newUser = await userService.createUser(userData);
    setUsers(prev => [...prev, newUser]);
    return newUser;
  };

  // 🔹 Obtener usuario por ID
  const getUserById = async (id: string): Promise<User | null> => {
    try {
      return await userService.getUserById(id);
    } catch (err) {
      console.error(`Error al obtener usuario ${id}:`, err);
      return null;
    }
  };

  // 🔹 Login 
  // useUsers.ts
  const login = async (credentials: { userName: string; password: string }): Promise<{ user: User; token: string }> => {
    setLoading(true);
    setError('');
    try {
      const { user, token } = await userService.login(credentials); // userService devuelve ambos
      return { user, token }; // <-- devuelve ambos
    } catch (err) {
      setError((err as Error).message);
      throw err;
    } finally {
      setLoading(false);
    }
  };


  return {
    users,
    loading,
    error,
    refetch: () => userService.getUsers().then(setUsers),
    createUser,
    getUserById,
    login,
  };
}
