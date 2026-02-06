import { useEffect, useState } from "react";
import { getConversations } from "../services/api";
import { useNavigate } from "react-router-dom";

export default function Conversations() {
    const [conversations, setConversations] = useState([]);
    const navigate = useNavigate();

    useEffect(() => {
        load();
    }, []);

    async function load() {
        const data = await getConversations();
        setConversations(data);
    }

    return (
        <div>
        <h2>Conversas</h2>

        {conversations.map(c => (
            <div
            key={c.id}
            onClick={() => navigate(`/chat/${c.id}`)}
            style={{ cursor: "pointer" }}
            >
            Conversa #{c.id}
            </div>
        ))}
        </div>
    );
}