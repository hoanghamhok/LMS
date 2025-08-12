"use client";
import { useState } from "react";
// Update the import path if necessary, for example:
import { api } from "@/lib/api";
// Or, if the file does not exist, create 'frontend/src/lib/api.ts' with an api export.
export default function LoginPage(){
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [token, setToken] = useState<string | undefined>();
    const [error, setError] = useState<string | undefined>();

    const submit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(undefined);
        try {
            const res = await api.post("/identity/login", { email, password });
            setToken(res.data.access_token);
            localStorage.setItem("access_token", res.data.access_token);
        } catch (err: any) {
            setError("Login failed");
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
                    onChange={e => setEmail(e.target.value)}
                />
                <input
                    className="border w-full p-2"
                    placeholder="Password"
                    type="password"
                    value={password}
                    onChange={e => setPassword(e.target.value)}
                />
                <button className="bg-black text-white px-4 py-2 rounded">Login</button>
            </form>
            {error && <div className="text-red-600 mt-2">{error}</div>}
            {token && (
                <div className="mt-4">
                    <div className="font-mono break-all">Token: {token}</div>
                    <button
                        className="mt-2 underline"
                        onClick={async () => {
                            const t = localStorage.getItem("access_token");
                            const res = await api.post(
                                "/identity/register",
                                {
                                    email: `user+${Math.floor(Math.random() * 9999)}@lms.dev`,
                                    fullName: "New User",
                                    password: "123456",
                                },
                                { headers: { Authorization: `Bearer ${t}` } }
                            );
                            alert("Test call OK: " + res.status);
                        }}
                    >
                        Test call through Gateway
                    </button>
                </div>
            )}
        </div>
    );
}
