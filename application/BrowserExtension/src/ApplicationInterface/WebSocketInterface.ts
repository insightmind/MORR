import IApplicationInterface from './IApplicationInterface'

export default class WebSocketInterface implements IApplicationInterface {
    wsUri : string;
    webSocket : WebSocket;
    constructor(uri : string) {
        this.wsUri = uri;
        this.webSocket = new WebSocket(this.wsUri);

        this.webSocket.onmessage = function (e : MessageEvent) {
            console.log("OnMessage: " + e.data);
        };

        this.webSocket.onerror = function (e : any) {
            console.log("OnError: " + e.data);
        };

        this.webSocket.onclose = function (e) {
            console.log("Websocket disconnected");
        };
    }

    requestConfig(onSuccess: (response?: string | undefined) => void, onFail: (response?: string | undefined) => void): void {
        onSuccess();
    }
    waitForStart(onStart: (response?: string | undefined) => void, onFail: (response?: string | undefined) => void): void {
        onStart();
    }
    sendData(data: string, onSuccess: (response?: string | undefined) => void, onFail: (response?: string | undefined) => void): void {
        console.log("Calling Websocket.send");
        this.webSocket.send(data);
    }
}