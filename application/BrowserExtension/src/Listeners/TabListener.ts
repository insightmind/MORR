import { IListener, EventType } from '../Shared/SharedDeclarations'
import { BrowserEvent } from '../Shared/SharedDeclarations'
import { NavigationEvent, SwitchTabEvent, OpenTabEvent, CloseTabEvent } from '../Shared/BrowserEvents'

/**
 * Listener for tab-related events.
 */
export default class TabListener implements IListener {
    private _callBack: (event: BrowserEvent) => void;
    constructor(callback: (event: BrowserEvent) => void) {
        this._callBack = callback;
    }
    public start() : void {
        chrome.tabs.onUpdated.addListener(this.onUpdatedCallback);
        chrome.tabs.onActivated.addListener(this.onActivatedCallback);
        chrome.tabs.onRemoved.addListener(this.onRemovedCallback);
        chrome.tabs.onCreated.addListener(this.onCreatedCallback);
    }
    public stop(): void {
        chrome.tabs.onUpdated.removeListener(this.onUpdatedCallback);
        chrome.tabs.onActivated.removeListener(this.onActivatedCallback);
        chrome.tabs.onRemoved.removeListener(this.onRemovedCallback);
        chrome.tabs.onCreated.removeListener(this.onCreatedCallback);
    }

    /**
     * To trigger when a tab is updated
     */
    private onUpdatedCallback = (tabId : number, changeInfo : chrome.tabs.TabChangeInfo, tab : chrome.tabs.Tab) => {
        throw new Error("Method not implemented.");
    }
    /**
     * To trigger when a tab is activated
     */
    private onActivatedCallback = (activeInfo : chrome.tabs.TabActiveInfo) => {
        throw new Error("Method not implemented.");
    }
    /**
     * To trigger when a tab is removed
     */
    private onRemovedCallback = (tabId : number, removeInfo: chrome.tabs.TabRemoveInfo) => {
        throw new Error("Method not implemented.");
    }

    /**
     * To trigger when a tab is created
     */
    private onCreatedCallback = (tab : chrome.tabs.Tab) => {
        throw new Error("Method not implemented.");
    }

    /**
     * Creates navigation event
     * @param tabId 
     * @param changeInfo 
     * @param tab 
     * @returns navigation event 
     */
    private createNavigationEvent(tabId : number, changeInfo : chrome.tabs.TabChangeInfo, tab : chrome.tabs.Tab) : NavigationEvent {
        throw new Error("Method not implemented.");
    }

    /**
     * Creates open tab event
     * @param tab 
     * @returns open tab event 
     */
    private createOpenTabEvent(tab : chrome.tabs.Tab) : OpenTabEvent {
        throw new Error("Method not implemented.");
    }

    /**
     * Creates close tab event
     * @param tabId 
     * @param removeInfo 
     * @returns close tab event 
     */
    private createCloseTabEvent(tabId : number, removeInfo: chrome.tabs.TabRemoveInfo) : CloseTabEvent {
        throw new Error("Method not implemented.");
    }
    /**
     * Creates switch tab event
     * @param activeInfo 
     */
    private createSwitchTabEvent(activeInfo : chrome.tabs.TabActiveInfo) {
        throw new Error("Method not implemented.");
    }
}