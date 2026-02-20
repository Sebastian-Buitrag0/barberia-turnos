import { ref } from "vue";
import * as signalR from "@microsoft/signalr";

let connection = null;
export const connectionState = ref("Desconectado");

export function getConnection() {
  if (!connection) {
    const hubUrl = import.meta.env.VITE_API_URL
      ? `${import.meta.env.VITE_API_URL}/hubs/turnos`
      : "/hubs/turnos";

    connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl)
      .withAutomaticReconnect()
      .build();

    connection.onreconnecting(() => {
      connectionState.value = "Reconectando...";
    });

    connection.onreconnected(() => {
      connectionState.value = "Conectado";
    });

    connection.onclose(() => {
      connectionState.value = "Desconectado";
    });
  }
  return connection;
}

export async function startConnection() {
  const conn = getConnection();
  if (conn.state === signalR.HubConnectionState.Disconnected) {
    try {
      await conn.start();
      connectionState.value = "Conectado";
      console.log("SignalR connected");
    } catch (err) {
      console.error("SignalR connection error:", err);
      connectionState.value = "Error Conexión";
      setTimeout(startConnection, 3000);
    }
  } else if (conn.state === signalR.HubConnectionState.Connected) {
    connectionState.value = "Conectado";
  }
  return conn;
}

export function onQueueUpdated(callback) {
  const conn = getConnection();
  conn.on("QueueUpdated", callback);
}

export function offQueueUpdated(callback) {
  const conn = getConnection();
  conn.off("QueueUpdated", callback);
}
