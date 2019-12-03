import * as Listeners from './Listeners'
import { IListener } from './Shared/SharedDeclarations';
import { BrowserEvent } from './Shared/SharedDeclarations'
import { IApplicationInterface, WebSocketInterface } from './ApplicationInterface/';

/**
 * The "main" class of the webextension
 */
class BackgroundScript {
    /**
     * Listeners controlled by background script
     */
    listeners: IListener[] = new Array();
    /**
     * App interface to the MORR application
     */
    appInterface : IApplicationInterface;
    /**
     * URI to initialize the appInterface to
     */
    static receiverURI : string = "ws://127.0.0.1:31337/";
    /**
     * Creates an instance of background script and initializes the listeners.
     */
    constructor() {
        this.listeners.push(new Listeners.TabListener((event : BrowserEvent) => { this.callback(event);}));
        this.listeners.push(new Listeners.DOMListener((event : BrowserEvent) => { this.callback(event);}));
        //etc.
        this.appInterface = new WebSocketInterface(BackgroundScript.receiverURI);
        this.appInterface.establishConnection(this.requestConfig, this.retryConnection);
    }
    /**
     * Start all listeners
     */
    public start = () => {
        this.listeners.forEach(listener => listener.start())
    }
    /**
     * Stop all listeners and wait for next start signal
     */
    public stop = () => {
        this.listeners.forEach(listener => listener.stop())
        this.waitForStart();
    }

    /**
     * Callback to hand over to the listeners
     */
    public callback = (event : BrowserEvent) => {
        console.log(`${BackgroundScript.timeStampString(event.timeStamp)}: ${event.type} occured in tab ${event.tabID} in window ${event.windowID}`);
        this.appInterface.sendData(JSON.stringify(event), () => {}, this.stop);
    }

    /**
     * Helper Function. Generates timesstampstring for console/debug output
     * @param date the Date to represent
     * @returns string representing the date
     */
    private static timeStampString(date : Date) : string {
        return `${date.getHours()}:${date.getMinutes()}:${date.getSeconds()}:${date.getMilliseconds()}`;
    }

    /**
     * Wait for start signal of the MORR application
     */
    private waitForStart = () => {
        throw new Error("Method not implemented.");
    }

    /**
     * Request config of the MORR application
     */
    private requestConfig = (response? : string) => {
        throw new Error("Method not implemented.");
    }
    /**
     * Retry setting up a connection to the main application
     */
    private retryConnection = () => {
        throw new Error("Method not implemented.");
    }

    /**
     * Parses configuration string and applies the configuration by setting the respective values in the web storage.
     * @param configuration The configuration string (JSON)
     * @returns true if configuration was valid, false otherwise
     */
    private parseAndApplyConfiguration(configuration : string) : boolean {
        throw new Error("Method not implemented.");
    }
}

//entry point
let main = new BackgroundScript()