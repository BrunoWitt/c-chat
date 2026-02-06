import axios from "axios";

const api = axios.create({
    baseURL: "http://localhost:5006"
});

// Pegamos o userId do localStorage automaticamente em todas as requisições
api.interceptors.request.use(config => {
    const userId = localStorage.getItem("userId");
    if (userId) {
        config.headers["x-user-id"] = userId;
    }
    return config;
});

/* -------- AUTH -------- */
export async function login(username) {
    const res = await api.post("/auth/login", { username });
    // salva userId no localStorage
    localStorage.setItem("userId", res.data.user.id);
    return res.data;
}

/* -------- CONVERSAS -------- */
export async function getConversations() {
    const res = await api.get("/auth/conversations");
    return res.data.conversations;
}

export async function createConversation(otherUserId) {
    const res = await api.post("/auth/conversations", { otherUserId });
    return res.data.conversation;
}

/* -------- MENSAGENS -------- */
export async function getMessages(conversationId) {
    const res = await api.get(`/conversations/${conversationId}/messages`);
    return res.data.messages;
}

export async function sendMessage(conversationId, content) {
    const res = await api.post(`/conversations/${conversationId}/messages`, { content });
    return res.data.message;
}

export default api;