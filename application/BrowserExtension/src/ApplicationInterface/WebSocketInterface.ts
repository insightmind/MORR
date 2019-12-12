import ICommunicationStrategy from './IApplicationInterface'

/**
 * Application interface using  a web socket. Expects a websocket-server on the main application side.
 */
export default class WebSocketInterface implements ICommunicationStrategy {
    /**
     * URI of the websocket server of the main application.
     */
    wsUri : string;
    /**
     * Web socket of web socket interface
     */
    webSocket : WebSocket;
    constructor(uri : string) {
        this.wsUri = uri;
        this.webSocket = new WebSocket(this.wsUri);
        throw new Error("Method not implemented.");
        this.webSocket.onmessage = function (e : MessageEvent) {
            throw new Error("Method not implemented.");
        };

        this.webSocket.onerror = function (e : any) {
            throw new Error("Method not implemented.");
        };

        this.webSocket.onclose = function (e) {
            throw new Error("Method not implemented.");
        };
    }
    establishConnection(): Promise<void> {
        throw new Error("Method not implemented.");
    }
    requestConfig(): Promise<string> {
        throw new Error("Method not implemented.");
    }
    waitForStart(): Promise<void> {
        throw new Error("Method not implemented.");
    }
    sendData(data: string): Promise<void> {
        throw new Error("Method not implemented.");
    }
}