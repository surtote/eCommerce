import NextAuth from "next-auth";
import CredentialsProvider from "next-auth/providers/credentials";

const handler = NextAuth({
  providers: [
    CredentialsProvider({
      name: "Credentials",
      credentials: {
        userName: { label: "Username", type: "text" },
        password: { label: "Password", type: "password" },
      },

      async authorize(credentials) {
        const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/v1/auth/login`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            userName: credentials?.userName,
            password: credentials?.password,
          }),
        });

        const data = await res.json();

        if (!res.ok) {
          console.log("❌ Login backend falló:", data);
          return null;
        }

        // Backend returns: { user: {...}, token: "jwt..." }
        return {
          id: data.user.id,
          name: data.user.userName,
          token: data.token,     // 👈 TOKEN REAL AQUÍ
        };
      }
    })
  ],

  session: {
    strategy: "jwt", // Usamos JWT
  },

  callbacks: {
    async jwt({ token, user }) {
      if (user) {
        token.id = user.id;
        token.jwt = user.token;  // 👈 Guardamos tu token real
      }
      return token;
    },

    async session({ session, token }) {
      session.user.id = token.id;
      session.user.token = token.jwt; // 👈 Token disponible para el frontend
      return session;
    }
  },

  pages: {
    signIn: "/login",
  }
});

export { handler as GET, handler as POST };
