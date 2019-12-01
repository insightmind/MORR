import * as Listeners from './Listeners'
import { IListener } from './Shared/SharedDeclarations';
import { BrowserEvent } from './Shared/SharedDeclarations'
import { IApplicationInterface, WebSocketInterface } from './ApplicationInterface/';

/**
 * The "main" class of the webextension
 */
class BackgroundScript {
    listeners: IListener[] = new Array();
    appInterface: IApplicationInterface | null = null;
    static receiverURL: string;
    private configured : boolean = false;
    constructor() {

    }
    /**
     * Start all listeners
     */
    public start = () => {

    }
    /**
     * Stop all listeners
     */
    public stop = () => {

    }

    public stopAndRetry = () => {
        this.stop();
        this.requestStart();
    }

    public callback = (event : BrowserEvent) => {

    }

    private static timeStampString(date : Date) {
        return `${date.getHours()}:${date.getMinutes()}:${date.getSeconds()}:${date.getMilliseconds()}`;
    }

    private requestStart = () => {

    }

    private requestConfig = () => {

    }
}

//entry point
let main = new BackgroundScript()