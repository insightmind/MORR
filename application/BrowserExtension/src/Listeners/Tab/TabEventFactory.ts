import { NavigationEvent, SwitchTabEvent, OpenTabEvent, CloseTabEvent } from './TabEvents'

export default class TabEventFactory {
    /**
     * Creates navigation event
     * @param tabId 
     * @param changeInfo 
     * @param tab 
     * @returns navigation event 
     */
    public createNavigationEvent(tabId : number, changeInfo : chrome.tabs.TabChangeInfo, tab : chrome.tabs.Tab) : NavigationEvent {
        return new NavigationEvent(tabId, tab.windowId, tab.url!);
    }

    /**
     * Creates open tab event
     * @param tab 
     * @returns open tab event 
     */
    public createOpenTabEvent(tab : chrome.tabs.Tab) : OpenTabEvent {
        return new OpenTabEvent(tab.id ? tab.id : 0, tab.windowId);
    }

    /**
     * Creates close tab event
     * @param tabId 
     * @param removeInfo 
     * @returns close tab event 
     */
    public createCloseTabEvent(tabId : number, removeInfo: chrome.tabs.TabRemoveInfo, tab : chrome.tabs.Tab) : CloseTabEvent {
        return (new CloseTabEvent(tabId, removeInfo.windowId, tab.url!));
    }
    /**
     * Creates switch tab event
     * @param activeInfo 
     */
    public createSwitchTabEvent(activeInfo : chrome.tabs.TabActiveInfo, prevtab : chrome.tabs.Tab) {
        return new SwitchTabEvent(prevtab.id ? prevtab.id : 0, prevtab.windowId, activeInfo.tabId, prevtab.url!);
    }
}