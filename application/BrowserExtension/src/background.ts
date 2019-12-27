import '@babel/polyfill'
import * as Listeners from './Listeners'
import { IListener } from './Shared/SharedDeclarations';
import { BrowserEvent } from './Shared/SharedDeclarations'
import { ICommunicationStrategy, WebSocketInterface } from './ApplicationInterface/';
import ListenerManager from "./ListenerManager"
import * as Mock from './__mock'

/**
 * The "main" class of the webextension
 */
class BackgroundScript {
    /**
     * ListenerManager controlled by background script
     */
    listenerManager : ListenerManager;
    /**
     * App interface to the MORR application
     */
    appInterface : ICommunicationStrategy;
    /**
     * URI to initialize the appInterface to
     */
    static receiverURI : string = "ws://127.0.0.1:31337/";


    /**
     * Helper variables
     */
    private configString : string | undefined;
    private readonly retryDelayMS = 5000;

    /**
     * Creates an instance of background script and initializes the listeners.
     */
    constructor() {
        this.listenerManager = new ListenerManager(this.callback);
        this.appInterface = new Mock.CommunicationMock();
        this.run();
    }

    /**
     * Start all listeners
     */
    public start = () => {
        this.listenerManager.startAll();
    }
    /**
     * Stop all listeners and wait for next start signal
     */
    public stop = () => {
        this.listenerManager.stopAll();
        this.waitForStart();
    }

    private async run() {
        await this.establishConnection(true);
        await this.waitForStart();
        this.start();
    }

    /**
     * Callback to hand over to the listeners
     */
    public callback = (event : BrowserEvent) => {
        console.log(`${BackgroundScript.timeStampString(event.timeStamp)}: ${event.type} occured in tab ${event.tabID} in window ${event.windowID}`);
        this.appInterface.sendData(JSON.stringify(event)).catch(this.stop);
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
    private waitForStart = async () => {
        await this.appInterface.waitForStart();
    }

    /**
     * Request config of the MORR application
     */
    private requestConfig = () => {
        throw new Error("Method not implemented.");
    }
    /**
     * Set up a connection to the main application using the appInterface.
     * @param getConfig set to true if a (new) configuration string should be requested, false otherwise
     */
    private establishConnection = (getConfig : boolean) : Promise<void> => {
        return new Promise((resolve, reject) => {
            this.appInterface.establishConnection()
            .then(() => {
                if (getConfig) {
                    return this.appInterface.requestConfig()
            } else {
                return Promise.resolve(undefined);
            }})
            .then((configString : string | undefined) => {
                if (configString)
                    this.configString = configString;
                resolve();
            })
            .catch((err : string) => {
                console.log(`Error during initialization: ${err}`);
                reject(`Error during initialization: ${err}`);
            });
        });
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