import { IListener, EventType } from '../../Shared/SharedDeclarations'
import { BrowserEvent } from '../../Shared/SharedDeclarations'
import { ButtonClickEvent, TextSelectionEvent, TextInputEvent, HoverEvent } from './DOMEvents';

/**
 * Listener responsible to gather all releveant events happening in the website context.
 * Responsible for injecting and controlling the content script into websites when appropriate.
 */
export default class DOMListener implements IListener {

    /**
     * Non-structured clonable data appears on firefox, as executescript tries to use the
     * contentscripts' last statements' value. As parcel adds some special stuff at the end of
     * each contentscript, this results in an harmless error. A workaround would be to append ", undefined" or similar to the contentscript,
     * which could done before shipping (i would add this to the buildscript, if there was an operating-system-independent way to do so).
     * see https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/API/tabs/executeScript#Return_value
     */
    private static readonly injectionErrorIgnorePattern = new RegExp('(non-structured-clonable data)', 'i');

    /**
     * Do not attempt injecting scripts into priviledged sites
     */
    private static readonly priviledgedSitesPattern = new RegExp('^(chrome://|about:)', 'i');

    private _callBack: (event: BrowserEvent) => void;
    constructor(callback: (event: BrowserEvent) => void) {
        this._callBack = callback;
    }
    public start(): void {
        chrome.tabs.query({}, (result : chrome.tabs.Tab[]) => {
            for (let tab of result) {
                if (tab.id && tab.url && !DOMListener.priviledgedSitesPattern.test(tab.url))
                    this.executeScript(tab.id);
            }
            chrome.runtime.onMessage.addListener(this.onDOMEventReceived);
            chrome.webNavigation.onDOMContentLoaded.addListener(this.injectEventRecorder);
        });
    }

    public stop(): void {
        chrome.runtime.onMessage.removeListener(this.onDOMEventReceived);
        chrome.webNavigation.onDOMContentLoaded.removeListener(this.injectEventRecorder);
        chrome.tabs.query({}, (result : chrome.tabs.Tab[]) => {
            for (let tab of result)
                if (tab.id && tab.url && !DOMListener.priviledgedSitesPattern.test(tab.url))
                    chrome.tabs.sendMessage(tab.id, "Stop");
        });
    }
    /**
     * Inject event recorder into a website.
     */
    private injectEventRecorder = (details? : chrome.webNavigation.WebNavigationFramedCallbackDetails) => {
        if (details && !DOMListener.priviledgedSitesPattern.test(details.url) && details.frameId === 0) {
            this.executeScript(details.tabId);
        };
    }

    /**
     * Executes contentscript in a tab
     * @param tabID the tabID to inject to script into
     */
    private executeScript(tabID : number) : void {
        chrome.tabs.executeScript(tabID, { file: 'Listeners/DOM/ContentScript/DOMEventRecorder.js' }, () => {
            if (chrome.runtime.lastError) {
                if (!DOMListener.injectionErrorIgnorePattern.test(chrome.runtime.lastError.message!))
                    console.error("Could not inject contentscript: " + chrome.runtime.lastError.message);
            }
        });
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
        let event : BrowserEvent | undefined;
        const parsedObj : any = JSON.parse(request);
        if (!parsedObj || !parsedObj._type)
            return undefined;
        try {
            switch(parsedObj._type) {
                case(EventType.ButtonClick):
                    event = ButtonClickEvent.deserialize(parsedObj, sender);
                    break;
                case(EventType.TextSelection):
                    event = TextSelectionEvent.deserialize(parsedObj, sender);
                    break;
                case(EventType.TextInput):
                    event = TextInputEvent.deserialize(parsedObj, sender);
                    break;
                case(EventType.Hover):
                    event = HoverEvent.deserialize(parsedObj, sender);
                    break;
                default:
                    throw(`Received unexpected eventtype: ${parsedObj._type}`);
            }
        } catch (e) {
            console.error(e);
            return undefined;
        }
        return event;
    }
}