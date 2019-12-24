import * as jquery from 'jquery';
import ICommunicationStrategy from './ICommunicationStrategy';
/**
 * Application Interface using the HTTP-POST. Expects a HTTPListener on the main application side.
 */
export default class PostHTTPInterface implements ICommunicationStrategy {
    /**
     * URL of the HTTPListener attached to the main application.
     */
    private listenerURL : string;
    constructor(url: string) {
        this.listenerURL = url;
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
