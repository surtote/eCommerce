// app/page.tsx
import { LoginForm } from "./forms/loginForm"


export default function Home() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 p-4">
      <LoginForm />
    </div>
  )
}
