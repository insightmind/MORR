/**
 * DOM Event types. Required for content script.
 */
export enum DOMEventTypes {
    CLICK = 'click',
	CHANGE = 'change', //change event usually happens when the element loses focus again (e. g. when the user switches to the next input field in a form)
	DBLCLICK = 'dblclick',
	//KEYDOWN = 'keydown',
	//MOUSEENTER = 'mouseenter',
	MOUSEUP = 'mouseup', //for text selection events
	MOUSEDOWN = 'mousedown',
	SELECT = 'select', //user selects a dropdown entry or similar
	SUBMIT = 'submit',
	SEARCH = 'search',
	//INPUT = 'input', //fires for every single character input into a field
	FOCUS = 'focus',
}

/**
 * Browser Event types
 */
export enum EventType {
	//Defined events
	Navigation = "Navigation",
	OpenTab = "OpenTab",
	CloseTab = "CloseTab",
	SwitchTab = "SwitchTab",
	TextInput = "TextInput",
	ButtonClick = "ButtonClick",
	Hover = "Hover",
	TextSelection = "TextSelection",
	Download = "Download",
	Generic = "Generic",
}

/**
 * The listener interface.
 */
export interface IListener {
    /**
	 * Start listening for events.
	 */
	start() : void;
    /**
	 * Stop listening for events.
	 */
	stop() : void;
}

/**
 * Define the constructor signature of IListener 
 */
export interface IListenerConstructor {
    new(callback: (event: BrowserEvent) => void): IListener;
}

/**
 * IEvent interface mirroring the interface in the MORR application
 */
export interface IEvent {
	timeStamp : Date;
	issuingModule : number;
	type : EventType;
	serialize() : string;
}

/**
 * A generic Browser event. All specific browser events inherit from this
 */
export class BrowserEvent implements IEvent{
	private _timeStamp : Date;
	private _issuingModule : number = 0;
	private _type : EventType;
	private _windowID : number;
	private _tabID : number;
	private _url? : string;
	/**
	 * Creates an instance of browser event.
	 * @param type The Eventtype
	 * @param tabID the tabID
	 * @param windowID the windowID
	 * @param [url] the url of the website opened in tab tabID
	 */
	constructor(type : EventType, tabID : number, windowID: number, url? : string) {
		this._type = type;
		this._timeStamp = new Date();
		this._tabID = tabID;
		this._windowID = windowID;
	}
	/**
	 * Serializes browser event
	 * @returns a JSON string representing the event
	 */
	serialize(): string {
		return JSON.stringify(this);
	}

	/*
	 * Getters and Setters
	 */
	public get timeStamp() : Date {
		return this._timeStamp;
	}
	public get issuingModule() {
		return this._issuingModule;
	}
	public get type() {
		return this._type;
	}
	public set type(type : EventType) {
		this._type = type;
	}
	public get windowID() : number {
		return this._windowID;
	}
	public set windowID(windowID : number) {
		this._windowID = windowID;
	}
	public get tabID() : number {
		return this._tabID;
	}
	public set tabID(tabID : number) {
		this._tabID = tabID;
	}
	public get url() : string {
		if (this._url)
			return this._url;
		else
			return "";
	}
	public set url(url : string) {
		this._url = url;
	}
}