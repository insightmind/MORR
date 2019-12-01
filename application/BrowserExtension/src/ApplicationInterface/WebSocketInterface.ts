import IApplicationInterface from './IApplicationInterface'

/**
 * Application interface using  a web socket. Expects a websocket-server on the main application side.
 */
export default class WebSocketInterface implements IApplicationInterface {
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

    requestConfig(onSuccess: (response?: string | undefined) => void, onFail: (response?: string | undefined) => void): void {
        throw new Error("Method not implemented.");
    }
    waitForStart(onStart: (response?: string | undefined) => void, onFail: (response?: string | undefined) => void): void {
        throw new Error("Method not implemented.");
    }
    sendData(data: string, onSuccess: (response?: string | undefined) => void, onFail: (response?: string | undefined) => void): void {
        throw new Error("Method not implemented.");
    }
}