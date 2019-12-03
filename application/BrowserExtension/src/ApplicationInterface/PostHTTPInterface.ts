import * as jquery from 'jquery';
import IApplicationInterface from './IApplicationInterface';
/**
 * Application Interface using the HTTP-POST. Expects a HTTPListener on the main application side.
 */
export default class PostHTTPInterface implements IApplicationInterface {
    /**
     * URL of the HTTPListener attached to the main application.
     */
    private listenerURL : string;
    constructor(url: string) {
        this.listenerURL = url;
    }
    establishConnection(onSuccess: (response?: string | undefined) => void, onFail: (response?: string | undefined) => void): void {
        throw new Error("Method not implemented.");
    }
    public requestConfig(onSuccess : (response? : string) => void, onFail : (response? : string) => void) : void {
        throw new Error("Method not implemented.");
    }

    public waitForStart = (onStart : (response? : string) => void, onFail : (response? : string) => void) => {
        throw new Error("Method not implemented.");
    }
    
    public sendData(data : string, onSuccess : (response? : string) => void, onFail : (response? : string) => void) : void{
        throw new Error("Method not implemented.");
    }
    
}
