import { IListener, BrowserEvent } from "./Shared/SharedDeclarations"
import * as Listeners from './Listeners'

export default class ListenerManager {
    /**
     * Indicates whether the listeners are currently running.
     */
    private running : boolean = false;
    /**
     * Array of managed listeners.
     */
    private listeners: IListener[];

    /**
     * Creates an instance of ListenerManager and of its managed listeners.
     * @param callback the callback function to pass onto all managed listeners
     * @param [configurationString] the string to parse a configuration from
     */
    constructor(callback: (event : BrowserEvent) => void, configurationString?: string) {
        this.listeners = new Array();
        this.listeners.push(new Listeners.TabListener((event : BrowserEvent) => { callback(event);}));
        this.listeners.push(new Listeners.DOMListener((event : BrowserEvent) => { callback(event);}));
        this.listeners.push(new Listeners.DownloadListener((event : BrowserEvent) => { callback(event);}));
    }

    /**
     * Starts all listeners.
     */
    public startAll() : void {
        if (this.running)
            return;
        this.listeners.forEach((listener) => {
            listener.start();
        });
        this.running = true;
    }

    /**
     * Stops all
     */
    public stopAll() : void {
        if (!this.running)
            return;
        this.listeners.forEach((listener) => {
            listener.stop();
        });
        this.running = false;
    }
}