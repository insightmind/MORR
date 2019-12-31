import { IListener, EventType } from '../../Shared/SharedDeclarations'
import { BrowserEvent } from '../../Shared/SharedDeclarations'
import { ButtonClickEvent } from './DOMEvents';

/**
 * Listener responsible to gather all releveant events happening in the website context.
 * Responsible for injecting and controlling the content script into websites when appropriate.
 */
export default class DOMListener implements IListener {
    private _callBack: (event: BrowserEvent) => void;
    constructor(callback: (event: BrowserEvent) => void) {
        chrome.webNavigation.onDOMContentLoaded.addListener(this.injectEventRecorder);
        this._callBack = callback;
    }
    public start(): void {
        chrome.runtime.onMessage.addListener(this.onDOMEventReceived);
    }
    public stop(): void {
        chrome.runtime.onMessage.removeListener(this.onDOMEventReceived);
    }
    /**
     * Inject event recorder into a website.
     */
    private injectEventRecorder = (details? : chrome.webNavigation.WebNavigationFramedCallbackDetails) => {
        if (details && details.frameId === 0) {
            if (details.url.startsWith("chrome://"))
                return;
            chrome.tabs.executeScript(details.tabId, { file: 'Listeners/DOM/ContentScript/DOMEventRecorder.js' }, () => {
                if (chrome.runtime.lastError)
                    console.log("Could not inject contentscript: " + chrome.runtime.lastError.message);
            });
        };
    }
    /**
     * Called when a contest script sends back serialized event data.
     */
    private onDOMEventReceived = (request : any, sender : chrome.runtime.MessageSender, sendResponse : (response? : any) => void) => {
        let event = DOMListener.deserializeDOMEvent(request, sender);
        if (event)
            this._callBack(event);
    }
    /**
     * Deserializes domevent
     * @param request the data sent by the content script
     * @param sender the sender of the message
     * @returns domevent 
     */
    private static deserializeDOMEvent(request : any, sender : chrome.runtime.MessageSender) : BrowserEvent | undefined {
        const fromTab = sender.tab;
        let tabID : number = -1;
        let windowID : number = -1;
        if (fromTab && fromTab.id) {
            tabID = fromTab.id;
            if (fromTab.windowId)
                windowID = fromTab.windowId;
        }
        let parsedEvent : any = JSON.parse(request);
        let event : BrowserEvent | undefined;
        switch(parsedEvent._type) {
            case(EventType.ButtonClick):
                event = new ButtonClickEvent(tabID, windowID, parsedEvent._buttonTitle, parsedEvent._url, parsedEvent._buttonHref);
                break;
        }
        return event;
    }
}