import NextAuth, { DefaultSession } from "next-auth";

declare module "next-auth" {
  interface User {
    id: string;
    token: string; // 👈 AQUI agregamos tu token real
    name?: string;
  }

  interface Session {
    user: {
      id: string;
      token: string;
    } & DefaultSession["user"];
  }
}

declare module "next-auth/jwt" {
  interface JWT {
    id: string;
    jwt: string; // 👈 nombre del token que guardamos en callbacks.jwt
  }
}
