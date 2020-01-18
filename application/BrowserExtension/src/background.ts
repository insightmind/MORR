import '@babel/polyfill'
import { BrowserEvent } from './Shared/SharedDeclarations'
import { ICommunicationStrategy, PostHTTPInterface } from './ApplicationInterface/';
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
     * Helper variables
     */
    private configString : string | undefined;
    private static readonly RETRYDELAYMS = 5000;
    private isRunning : boolean;

    /**
     * Creates an instance of background script and initializes the listeners.
     */
    constructor() {
        this.listenerManager = new ListenerManager(this.callback);
        this.appInterface = new Mock.CommunicationMock();
        this.isRunning = false;
    }

    /**
     * Start all listeners
     */
    private start = () : void => {
        if (!this.isRunning) {
            this.isRunning = true;
            this.listenerManager.startAll();
        }
    }
    /**
     * Stop all listeners and wait for next start signal
     */
    private stop = () : void => {
        if (this.isRunning) {
            this.isRunning = false;
            this.listenerManager.stopAll();
            this.appInterface.waitForStart()
            .then(() => this.start())
            .catch((e) => {
                this.reset();
            });
        }
    }

    //completely reset the connection status
    private reset = () : void => {
        if (this.isRunning) {
            this.isRunning = false;
            this.listenerManager.stopAll();
            this.run();
        }
    }

    public run = () : void => {
        this.establishConnection(true)
        .then(() => this.appInterface.waitForStart())
        .catch((e) => this.run())
        .then(() => this.start());
    }

    /**
     * Callback to hand over to the listeners
     */
    public callback = (event : BrowserEvent) : void => {
        console.log(`${BackgroundScript.timeStampString(event.timeStamp)}: ${event.type} occured in tab ${event.tabID} in window ${event.windowID}`);
        this.appInterface.sendData(JSON.stringify(event)).
        catch((e) => {
            if (e == "Stop")
                this.stop();
            else
                this.reset();
        });
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
                console.error(`Error during initialization: ${err}`);
                setTimeout(() => {
                    this.establishConnection(getConfig)
                    .then(() => resolve());
                }, BackgroundScript.RETRYDELAYMS);
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
main.run();