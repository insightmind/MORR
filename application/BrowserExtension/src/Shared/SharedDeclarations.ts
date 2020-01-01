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
	private _url : URL;
	/**
	 * Creates an instance of browser event.
	 * @param type The Eventtype
	 * @param tabID the tabID
	 * @param windowID the windowID
	 * @param [url] the url of the website opened in tab tabID
	 * @throws {TypeError}, if url string is not a valid URL
	 */
	constructor(type : EventType, tabID : number, windowID: number, url : string) {
		this._type = type;
		this._timeStamp = new Date();
		this._tabID = tabID;
		this._windowID = windowID;
		this._url = new URL(url);
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
	public set timeStamp(date : Date) {
		this._timeStamp = date;
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
	public get url() : URL {
		return this._url;
	}
	public set url(url : URL) {
		this._url = url;
	}
}