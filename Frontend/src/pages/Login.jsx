import { useState } from "react";
import { login } from "../services/api";
import { createConnection, startConnection } from "../services/signalr";
import { useNavigate } from "react-router-dom";

export default function Login() {
    const [username, setUsername] = useState("");
    const navigate = useNavigate();

    async function handleLogin() {
        const res = await login(username);

        // cria a conexão SignalR após salvar o userId
        createConnection();
        await startConnection();

        navigate("/conversations");
    }

    return (
        <div>
            <h2>Login</h2>
            <input
                value={username}
                onChange={e => setUsername(e.target.value)}
                placeholder="username"
            />
            <button onClick={handleLogin}>Entrar</button>
        </div>
    );
}