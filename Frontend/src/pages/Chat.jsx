import { useEffect, useState } from "react";
import { getMessages, sendMessage } from "../services/api";
import {
    joinConversation,
    leaveConversation,
    onReceiveMessage
} from "../services/signalr";
import { useParams } from "react-router-dom";

export default function Chat() {
    const { id } = useParams();

    const [messages, setMessages] = useState([]);
    const [text, setText] = useState("");

    useEffect(() => {
        load();

        joinConversation(id);

        onReceiveMessage(msg => {
        setMessages(prev => [...prev, msg]);
        });

        return () => {
        leaveConversation(id);
        };
    }, [id]);

    async function load() {
        const data = await getMessages(id);
        setMessages(data);
    }

    async function handleSend() {
        if (!text) return;

        await sendMessage(id, text);
        setText("");
    }

    return (
        <div>
        <h2>Chat {id}</h2>

        <div style={{ height: 300, overflowY: "auto" }}>
            {messages.map(m => (
            <div key={m.id}>
                <b>{m.senderName}:</b> {m.content}
            </div>
            ))}
        </div>

        <input
            value={text}
            onChange={e => setText(e.target.value)}
            placeholder="Digite..."
        />

        <button onClick={handleSend}>
            Enviar
        </button>
        </div>
    );
}