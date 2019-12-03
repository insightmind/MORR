import { IListener, EventType } from '../Shared/SharedDeclarations'
import { BrowserEvent } from '../Shared/SharedDeclarations'

/**
 * Listener responsible to gather all releveant events happening in the website context.
 * Responsible for injecting and controlling the content script into websites when appropriate.
 */
export default class DOMListener implements IListener {
    private _callBack: (event: BrowserEvent) => void;
    constructor(callback: (event: BrowserEvent) => void) {
        this._callBack = callback;
    }
    public start(): void {
        chrome.webNavigation.onDOMContentLoaded.addListener(this.injectEventRecorder);
        chrome.runtime.onMessage.addListener(this.onDOMEventReceived);
    }
    public stop(): void {
        chrome.webNavigation.onDOMContentLoaded.removeListener(this.injectEventRecorder);
        chrome.runtime.onMessage.removeListener(this.onDOMEventReceived);
    }
    /**
     * Inject event recorder into a website.
     */
    private injectEventRecorder = (details? : chrome.webNavigation.WebNavigationFramedCallbackDetails) => {
        return new Promise((resolve, reject) => {
            if (!details || details.frameId === 0) {
                chrome.tabs.executeScript({ file: 'ContentScript/DOMEventRecorder.js' }, () => {
                    if (chrome.runtime.lastError) reject(chrome.runtime.lastError);
                    else resolve();
                });
            } else resolve();
        });
    }
    /**
     * Called when a contest script sends back serialized event data.
     */
    private onDOMEventReceived = (request : any, sender : chrome.runtime.MessageSender, sendResponse : (response? : any) => void) => {
        this._callBack(DOMListener.deserializeDOMEvent(request, sender));
    }
    /**
     * Deserializes domevent
     * @param request the data sent by the content script
     * @param sender the sender of the message
     * @returns domevent 
     */
    private static deserializeDOMEvent(request : any, sender : chrome.runtime.MessageSender) : BrowserEvent {
        const fromTab = sender.tab;
        let tabID : number = -1;
        let windowID : number = -1;
        if (fromTab && fromTab.id) {
            tabID = fromTab.id;
            if (fromTab.windowId)
                windowID = fromTab.windowId;
        }
        let parsedEvent = JSON.parse(request);
        let event = new BrowserEvent(parsedEvent.type, tabID, windowID, parsedEvent.url);
        return event;
    }
}