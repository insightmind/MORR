import { BrowserEvent, EventType } from '../../Shared/SharedDeclarations'

const UNKOWN_URL = "url:unknown";

/**
 * Open tab event
 */
export class OpenTabEvent extends BrowserEvent {
    /**
     * Creates an instance of open tab event.
     * @param tabID the ID of the newly created tab
     * @param windowID the ID of the window the tab was created in
     * @param url empty, as the new tab was just created
     */
    constructor(tabID : number, windowID: number) {
        super(EventType.OpenTab, tabID, windowID, UNKOWN_URL);
    }
}

/**
 * Close tab event
 */
export class CloseTabEvent extends BrowserEvent {
    /**
     * Creates an instance of close tab event.
     * @param tabID The ID of the closed tab
     * @param windowID The ID of the window the tab was associated to
     * @param url The URL opened in the tab as it was closed
     */
    constructor(tabID : number, windowID: number, url? : string) {
        super(EventType.CloseTab, tabID, windowID, url ? url : UNKOWN_URL);
    }
}

/**
 * Switch tab event
 */
export class SwitchTabEvent extends BrowserEvent {
    private _newTabID : number;
    /**
     * Creates an instance of switch tab event.
     * @param tabID The ID of the previous tab
     * @param windowID The ID of the window the active tab changed inside of
     * @param newTabID The ID of the newly focused tab
     * @param url the URL opened in the newly focused tab
     */
    constructor(tabID : number, windowID: number, newTabID : number, url : string) {
        super(EventType.SwitchTab, tabID, windowID, url);
        this._newTabID = newTabID;
    }

    /**
     * Gets new tab id
     */
    public get newTabID() : number {
        return this._newTabID;
    }
    /**
     * Sets new tab id
     */
    public set newTabID(newTabID : number) {
        this._newTabID = newTabID;
    }
}

/**
 * Navigation event
 */
export class NavigationEvent extends BrowserEvent {
    /**
     * Creates an instance of navigation event.
     * @param tabID The ID of the tab in which the navigation occured
     * @param windowID The ID of the window the tab with tabID belongs to
     * @param url The URL that was navigated to
     */
    constructor(tabID : number, windowID: number, url : string) {
        super(EventType.Navigation, tabID, windowID, url);
    }
}