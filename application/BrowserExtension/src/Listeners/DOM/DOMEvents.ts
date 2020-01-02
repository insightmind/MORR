import { BrowserEvent, EventType } from '../../Shared/SharedDeclarations'
/**
 * DOM Event types. Required for content script.
 */
export enum DOMEventTypes {
    CLICK = 'click',
	CHANGE = 'change', //change event usually happens when the element loses focus again (e. g. when the user switches to the next input field in a form)
	DBLCLICK = 'dblclick',
	//KEYDOWN = 'keydown',
	MOUSEENTER = 'mouseenter',
	MOUSEUP = 'mouseup', //for text selection events
	//MOUSEDOWN = 'mousedown',
	SELECT = 'select', //user selects a dropdown entry or similar
	SUBMIT = 'submit',
	SEARCH = 'search',
	//INPUT = 'input', //fires for every single character input into a field
	FOCUS = 'focus',
}

/**
 * Text input event
 */
export class TextInputEvent extends BrowserEvent {
    private _text : string;
    private _target: string;
    /**
     * Creates an instance of text input event.
     * @param tabID The ID of the tab in which the event occured in
     * @param windowID The ID of the window the tab with tabID belongs to
     * @param text The input text
     * @param target A string description of the input target
     * @param url The URL of the opened webpage
     */
    constructor(tabID : number, windowID: number, text : string, target : string, url : string) {
        super(EventType.TextInput, tabID, windowID, url);
        this._text = text;
        this._target = target;
    }

    /**
     * Deserializes TextInputEvent
     * @param parsed the object parsed from a JSON string
     * @param [sender] the sender of the serialized event
     * @returns the deserialized TextInputEvent
     * @throws if parsed does not contain all necessary fields to create the event
     */
    public static deserialize(parsed : any, sender? : chrome.runtime.MessageSender) : TextInputEvent {
        let tabID = parsed._tabID;
        let windowID = parsed._windowID;
        if (sender && sender.tab) {
            tabID = sender.tab.id;
            windowID = sender.tab.windowId;
        }
        let evt = new TextInputEvent(tabID, windowID, parsed._text, parsed._target, parsed._url);
        evt.timeStamp = new Date(parsed._timeStamp);
        return evt;
    }

    /**
     * Gets text
     */
    public get text() : string {
        return this._text;
    }
    /**
     * Sets text
     */
    public set text(text : string) {
        this._text = text;
    }

    /**
     * Gets target
     */
    public get target() : string {
        return this._target;
    }
    /**
     * Sets target
     */
    public set target(target : string) {
        this._target = target;
    }
}

/**
 * Button click event
 */
export class ButtonClickEvent extends BrowserEvent {
    private _buttonTitle : string;
    private _buttonHref? : string;
    /**
     * Creates an instance of button click event.
     * @param tabID The ID of the tab in which the event occured in
     * @param windowID The ID of the window the tab with tabID belongs to
     * @param buttonTitle The title of the clicked button or similar element
     * @param url The URL of the opened webpage
     * @param [buttonHref] The href of the clicked element, if applicable
     */
    constructor(tabID : number, windowID: number, buttonTitle : string, url : string, buttonHref? : string,) {
        super(EventType.ButtonClick, tabID, windowID, url);
        this._buttonTitle = buttonTitle;
        this._buttonHref = buttonHref;
    }

    /**
     * Deserializes button click event from parsed JSON object
     * @param parsed the object parsed from a JSON string
     * @param [sender] the sender of the serialized event
     * @returns the deserialized ButtonClickEvent
     * @throws if parsed does not contain all necessary fields to create the event
     */
    public static deserialize(parsed : any, sender? : chrome.runtime.MessageSender) : ButtonClickEvent {
        let tabID = parsed._tabID;
        let windowID = parsed._windowID;
        if (sender && sender.tab) {
            tabID = sender.tab.id;
            windowID = sender.tab.windowId;
        }
        let evt = new ButtonClickEvent(tabID, windowID, parsed._buttonTitle, parsed._url, parsed._buttonHref);
        evt.timeStamp = new Date(parsed._timeStamp);
        return evt;
    }

    /**
     * Gets button title
     */
    public get buttonTitle() : string {
        return this._buttonTitle;
    }
    /**
     * Sets button title
     */
    public set buttonTitle(text : string) {
        this._buttonTitle = text;
    }

    /**
     * Gets button href
     */
    public get buttonHref() : string | undefined {
        return this._buttonHref;
    }
    /**
     * Sets button href
     */
    public set buttonHref(target : string | undefined) {
        this._buttonHref = target;
    }
}

/**
 * Hover event
 */
export class HoverEvent extends BrowserEvent {
    public static readonly HOVERDELAYMS = 1000;
    private _target : string;
    /**
     * Creates an instance of hover event.
     * @param tabID The ID of the tab in which the event occured in
     * @param windowID The ID of the window the tab with tabID belongs to
     * @param target The hovered element.
     * @param url The URL of the opened webpage
     */
    constructor(tabID : number, windowID: number, target : string, url : string) {
        super(EventType.Hover, tabID, windowID, url);
        this._target = target;
    }

    /**
     * Deserializes hover event
     * @param parsed the object parsed from a JSON string
     * @param [sender] the sender of the serialized event
     * @returns the deserialized HoverEvent
     * @throws if parsed does not contain all necessary fields to create the event
     */
    public static deserialize(parsed : any, sender? : chrome.runtime.MessageSender) : HoverEvent {
        let tabID = parsed._tabID;
        let windowID = parsed._windowID;
        if (sender && sender.tab) {
            tabID = sender.tab.id;
            windowID = sender.tab.windowId;
        }
        let evt = new HoverEvent(tabID, windowID, parsed._target, parsed._url);
        evt.timeStamp = new Date(parsed._timeStamp);
        return evt;
    }

    /**
     * Gets target
     */
    public get target() : string {
        return this._target;
    }
    /**
     * Sets target
     */
    public set target(target : string) {
        this._target = target;
    }
}

/**
 * Text selection event
 */
export class TextSelectionEvent extends BrowserEvent {
    private _textSelection : string;
    /**
     * Creates an instance of text selection event.
     * @param tabID The ID of the tab in which the event occured in
     * @param windowID The ID of the window the tab with tabID belongs to
     * @param textSelection The selected text
     * @param url The URL of the opened webpage
     */
    constructor(tabID : number, windowID: number, textSelection : string, url : string) {
        super(EventType.TextSelection, tabID, windowID, url);
        this._textSelection = textSelection;
    }

    /**
     * Deserializes text selection event
     * @param parsed the object parsed from a JSON string
     * @param [sender] the sender of the serialized event
     * @returns the deserialized TextSelectionEvent
     * @throws if parsed does not contain all necessary fields to create the event
     */
    public static deserialize(parsed : any, sender? : chrome.runtime.MessageSender) : TextSelectionEvent {
        let tabID = parsed._tabID;
        let windowID = parsed._windowID;
        if (sender && sender.tab) {
            tabID = sender.tab.id;
            windowID = sender.tab.windowId;
        }
        let evt = new TextSelectionEvent(tabID, windowID, parsed._textSelection, parsed._url);
        evt.timeStamp = new Date(parsed._timeStamp);
        return evt;
    }

    /**
     * Gets text selection
     */
    public get textSelection() : string {
        return this._textSelection;
    }
    /**
     * Sets text selection
     */
    public set textSelection(textSelection : string) {
        this._textSelection = textSelection;
    }
}