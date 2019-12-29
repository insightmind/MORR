import '@babel/polyfill'
import { BrowserEvent } from './Shared/SharedDeclarations'
import { ICommunicationStrategy, WebSocketInterface } from './ApplicationInterface/';
import ListenerManager from "./ListenerManager"
import * as Mock from './__mock__'

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
    private readonly RETRYDELAYMS = 5000;

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
    public start = () : void => {
        this.listenerManager.startAll();
    }
    /**
     * Stop all listeners and wait for next start signal
     */
    public stop = () : void => {
        this.listenerManager.stopAll();
        this.waitForStart();
    }

    private async run() : Promise<void> {
        await this.establishConnection(true);
        try {
            await this.waitForStart();
        } catch (e) {
            this.run();
            return;
        }
        this.start();
    }

    /**
     * Callback to hand over to the listeners
     */
    public callback = (event : BrowserEvent) : void => {
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
    private waitForStart = () : Promise<void> => {
        return this.appInterface.waitForStart();
    }

    /**
     * @deprecated
     * Request config of the MORR application
     */
    private requestConfig = () : void => {
        throw new Error("Method unused.");
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
     * @deprecated
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