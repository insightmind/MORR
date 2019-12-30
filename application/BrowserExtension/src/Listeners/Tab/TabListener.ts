import { IListener, EventType } from '../../Shared/SharedDeclarations'
import { BrowserEvent } from '../../Shared/SharedDeclarations'
import TabEventFactory from './TabEventFactory'

/**
 * Listener for tab-related events.
 */
export default class TabListener implements IListener {
    private _callBack: (event: BrowserEvent) => void;
    private lastActiveTabs : chrome.tabs.Tab[]; //[0] is the active Tab, [1] is the last active Tab
    private factory : TabEventFactory;
    constructor(callback: (event: BrowserEvent) => void) {
        this._callBack = callback;
        this.factory = new TabEventFactory();
        this.lastActiveTabs = new Array(2);
        this.updateActiveTab();
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
    private onUpdatedCallback = (tabId : number, changeInfo : chrome.tabs.TabChangeInfo, tab : chrome.tabs.Tab) : void => {
        this.updateActiveTab()
        .then(() => {
            if (changeInfo.url)
                this._callBack(this.factory.createNavigationEvent(tabId, changeInfo, this.lastActiveTabs[0]));
        });
    }

    /**
     * To trigger when a tab is activated
     */
    private onActivatedCallback = (activeInfo : chrome.tabs.TabActiveInfo) : void => {
        this.updateActiveTab()
        .then(() => this._callBack(this.factory.createSwitchTabEvent(activeInfo, this.lastActiveTabs[1])));
    }
    /**
     * To trigger when a tab is removed
     */
    private onRemovedCallback = (tabId : number, removeInfo: chrome.tabs.TabRemoveInfo) : void => {
        this._callBack(this.factory.createCloseTabEvent(tabId, removeInfo, this.lastActiveTabs[0]));
        this.updateActiveTab();
    }

    /**
     * To trigger when a tab is created
     */
    private onCreatedCallback = (tab : chrome.tabs.Tab) : void => {
        if (tab.id)
            this._callBack(this.factory.createOpenTabEvent(tab));
    }

    private updateActiveTab = () : Promise<void> => {
        return new Promise((resolve, reject) => {
            chrome.tabs.query({active: true, currentWindow: true}, (tabs) => {
                if (tabs.length == 0 || !tabs[0])
                    reject("Tab query did not return an entry.");
                if (tabs[0].id != this.lastActiveTabs[1].id) {
                    //active tab has changed, move back in array
                    this.lastActiveTabs[1] = this.lastActiveTabs[0];
                }
                //update active tab info
                this.lastActiveTabs[0] = tabs[0];
                resolve();
            });
        });
    }
}