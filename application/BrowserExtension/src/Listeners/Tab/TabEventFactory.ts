import { NavigationEvent, SwitchTabEvent, OpenTabEvent, CloseTabEvent } from './TabEvents'

/**
 * The TabEventFactory is responsible for creating BrowserEvent objects from the infor- mation a TabListener gathers.
 */
export default class TabEventFactory {

    /**
     * Creates navigation event
     * @param tabId The ID of the tab which provided the TabChangeInfo.
     * @param changeInfo The changeInfo object raised when the navigation occured.
     * @param tab The tabobject corresponding to tabId
     * @returns NavigationEvent
     */
    public createNavigationEvent(tabId : number, changeInfo : chrome.tabs.TabChangeInfo, tab : chrome.tabs.Tab) : NavigationEvent {
        return new NavigationEvent(tabId, tab.windowId, tab.url!);
    }

    /**
     * Creates open tab event
     * @param tab The tab to create the event from.
     * @returns OpenTabEvent
     */
    public createOpenTabEvent(tab : chrome.tabs.Tab) : OpenTabEvent {
        return new OpenTabEvent(tab.id ? tab.id : 0, tab.windowId);
    }

    /**
     * Creates close tab event
     * @param tabId The ID of the tab which notified the caller.
     * @param removeInfo The TabRemoveInfo to create the event from.
     * @param tab The tabobject corresponding to tabId
     * @returns CloseTabEvent
     */
    public createCloseTabEvent(tabId : number, removeInfo: chrome.tabs.TabRemoveInfo, tab : chrome.tabs.Tab) : CloseTabEvent {
        //in case the tab was closed while inactive, the url can not be recovered
        let url : string | undefined = (tab.url && tabId == tab.id) ? tab.url : undefined;
        return (new CloseTabEvent(tabId, removeInfo.windowId, url));
    }

    /**
     * Creates switch tab event
     * @param activeInfo The TabActiveInfo to create the event from.
     * @param prevtab The active tab before the switch occured.
     * @returns  SwitchTabEvent
     */
    public createSwitchTabEvent(activeInfo : chrome.tabs.TabActiveInfo, prevtab : chrome.tabs.Tab) : SwitchTabEvent {
        return new SwitchTabEvent(prevtab.id ? prevtab.id : 0, prevtab.windowId, activeInfo.tabId, prevtab.url!);
    }
}