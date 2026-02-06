import * as signalR from "@microsoft/signalr";

let connection = null;

/* Cria conexão */
export function createConnection() {
    const userId = localStorage.getItem("userId");

    connection = new signalR.HubConnectionBuilder()
        .withUrl(`http://localhost:5006/chatHub`, {
            accessTokenFactory: () => userId || ""
        })
        .withAutomaticReconnect()
        .build();

    return connection;
}

/* Conecta */
export async function startConnection() {
    if (!connection) return;
    if (connection.state === "Disconnected") {
        await connection.start();
        console.log("SignalR conectado");
    }
}

/* Entrar numa conversa */
export async function joinConversation(conversationId) {
    if (!connection) return;
    await connection.invoke("JoinConversation", conversationId);
}

/* Escutar mensagens */
export function onReceiveMessage(callback) {
    if (!connection) return;
    connection.on("ReceiveMessage", callback);
}

/* Sair da conversa */
export async function leaveConversation(conversationId) {
    if (!connection) return;
    await connection.invoke("LeaveConversation", conversationId);
}