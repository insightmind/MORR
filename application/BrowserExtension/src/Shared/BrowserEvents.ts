import * as MORR from './SharedDeclarations'


/**
 * Open tab event
 */
export class OpenTabEvent extends MORR.BrowserEvent {
    constructor(tabID : number, windowID: number, url? : string) {
        super(MORR.EventType.OpenTab, tabID, windowID, url);
    }
}

/**
 * Close tab event
 */
export class CloseTabEvent extends MORR.BrowserEvent {
    constructor(tabID : number, windowID: number, url? : string) {
        super(MORR.EventType.CloseTab, tabID, windowID, url);
    }
}

/**
 * Switch tab event
 */
export class SwitchTabEvent extends MORR.BrowserEvent {
    private _newTabID : number;
    constructor(tabID : number, windowID: number, newTabID : number, url? : string) {
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
    constructor(tabID : number, windowID: number, url? : string) {
        super(MORR.EventType.Navigation, tabID, windowID, url);
    }
}

/**
 * Text input event
 */
export class TextInputEvent extends MORR.BrowserEvent {
    private _text : string;
    private _target: string;
    constructor(tabID : number, windowID: number, text : string, target : string, url? : string) {
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
    constructor(tabID : number, windowID: number, buttonTitle : string, buttonHref? : string, url? : string) {
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
    constructor(tabID : number, windowID: number, target : string, url? : string) {
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
    constructor(tabID : number, windowID: number, textSelection : string, url? : string) {
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
    private _fileURL : string;
    constructor(tabID : number, windowID : number, mimeType : string, fileURL : string, url? : string) {
        super(MORR.EventType.Download, tabID, windowID, url);
        this._mimeType = mimeType;
        this._fileURL = fileURL;
    }

    /**
     * Gets file url
     */
    public get fileURL() : string {
        return this._fileURL;
    }
    /**
     * Sets file url
     */
    public set fileURL(fileURL : string) {
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