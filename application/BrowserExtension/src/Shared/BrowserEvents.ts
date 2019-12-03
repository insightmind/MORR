import * as MORR from './SharedDeclarations'


/**
 * Open tab event
 */
export class OpenTabEvent extends MORR.BrowserEvent {
    /**
     * Creates an instance of open tab event.
     * @param tabID the ID of the newly created tab
     * @param windowID the ID of the window the tab was created in
     * @param url empty, as the new tab was just created
     */
    constructor(tabID : number, windowID: number) {
        super(MORR.EventType.OpenTab, tabID, windowID, "");
    }
}

/**
 * Close tab event
 */
export class CloseTabEvent extends MORR.BrowserEvent {
    /**
     * Creates an instance of close tab event.
     * @param tabID The ID of the closed tab
     * @param windowID The ID of the window the tab was associated to
     * @param url The URL opened in the tab as it was closed
     */
    constructor(tabID : number, windowID: number, url : string) {
        super(MORR.EventType.CloseTab, tabID, windowID, url);
    }
}

/**
 * Switch tab event
 */
export class SwitchTabEvent extends MORR.BrowserEvent {
    private _newTabID : number;
    /**
     * Creates an instance of switch tab event.
     * @param tabID The ID of the previous tab
     * @param windowID The ID of the window the active tab changed inside of
     * @param newTabID The ID of the newly focused tab
     * @param url the URL opened in the newly focused tab
     */
    constructor(tabID : number, windowID: number, newTabID : number, url : string) {
        super(MORR.EventType.SwitchTab, tabID, windowID, url);
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
export class NavigationEvent extends MORR.BrowserEvent {
    /**
     * Creates an instance of navigation event.
     * @param tabID The ID of the tab in which the navigation occured
     * @param windowID The ID of the window the tab with tabID belongs to
     * @param url The URL that was navigated to
     */
    constructor(tabID : number, windowID: number, url : string) {
        super(MORR.EventType.Navigation, tabID, windowID, url);
    }
}

/**
 * Text input event
 */
export class TextInputEvent extends MORR.BrowserEvent {
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
        super(MORR.EventType.TextInput, tabID, windowID, url);
        this._text = text;
        this._target = target;
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
export class ButtonClickEvent extends MORR.BrowserEvent {
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
        super(MORR.EventType.ButtonClick, tabID, windowID, url);
        this._buttonTitle = buttonTitle;
        this._buttonHref = buttonHref;
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
export class HoverEvent extends MORR.BrowserEvent {
    private _target : string;
    /**
     * Creates an instance of hover event.
     * @param tabID The ID of the tab in which the event occured in
     * @param windowID The ID of the window the tab with tabID belongs to
     * @param target The hovered element.
     * @param url The URL of the opened webpage
     */
    constructor(tabID : number, windowID: number, target : string, url : string) {
        super(MORR.EventType.Hover, tabID, windowID, url);
        this._target = target;
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
export class TextSelectionEvent extends MORR.BrowserEvent {
    private _textSelection : string;
    /**
     * Creates an instance of text selection event.
     * @param tabID The ID of the tab in which the event occured in
     * @param windowID The ID of the window the tab with tabID belongs to
     * @param textSelection The selected text
     * @param url The URL of the opened webpage
     */
    constructor(tabID : number, windowID: number, textSelection : string, url : string) {
        super(MORR.EventType.TextSelection, tabID, windowID, url);
        this._textSelection = textSelection;
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

/**
 * Download event
 */
export class DownloadEvent extends MORR.BrowserEvent {
    private _mimeType : string;
    private _fileURL : URL;
    /**
     * Creates an instance of download event.
     * @param tabID The ID of the tab in which the event occured in
     * @param windowID The ID of the window the tab with tabID belongs to
     * @param mimeType A string describing the MIME type of the downloaded file.
     * @param fileURL The URL of the downloaded file
     * @param url The URL of the opened webpage
     * @throws {TypeError} if fileURL is not a valid URL
     */
    constructor(tabID : number, windowID : number, mimeType : string, fileURL : string, url : string) {
        super(MORR.EventType.Download, tabID, windowID, url);
        this._mimeType = mimeType;
        this._fileURL = new URL(fileURL);
    }

    /**
     * Gets file url
     */
    public get fileURL() : URL {
        return this._fileURL;
    }
    /**
     * Sets file url
     */
    public set fileURL(fileURL : URL) {
        this._fileURL = fileURL;
    }
    /**
     * Gets file mime type
     */
    public get mimeType() : string {
        return this.mimeType;
    }
    /**
     * Sets file mime type
     */
    public set mimeType(mimeType : string) {
        this._mimeType = mimeType;
    }
}