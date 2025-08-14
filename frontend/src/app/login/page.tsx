"use client";
import React, { useState } from "react";
import { api } from "@/lib/api";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [token, setToken] = useState<string>();
  const [error, setError] = useState<string>();

  const submit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError(undefined);
    try {
      const res = await api.post("/identity/login", { email, password });
      const t = res.data?.access_token as string | undefined;
      if (!t) {
        setError("Login failed: no token returned");
        return;
      }
      setToken(t);
      localStorage.setItem("access_token", t);
    } catch (e) {
      const msg =
        e instanceof Error
          ? e.message
          : "Login failed";
      setError(msg);
    }
  };

  return (
    <div className="max-w-md mx-auto p-6">
      <h1 className="text-2xl font-bold mb-4">Login</h1>
      <form onSubmit={submit} className="space-y-3">
        <input
          className="border w-full p-2"
          placeholder="Email"
          value={email}
          onChange={(ev) => setEmail(ev.target.value)}
        />
        <input
          className="border w-full p-2"
          placeholder="Password"
          type="password"
          value={password}
          onChange={(ev) => setPassword(ev.target.value)}
        />
        <button className="bg-black text-white px-4 py-2 rounded">
          Login
        </button>
      </form>
      {error && <div className="text-red-600 mt-2">{error}</div>}
      {token && (
        <div className="mt-4">
          <div className="font-mono break-all">Token: {token}</div>
        </div>
      )}
    </div>
  );
}
