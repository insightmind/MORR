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

export enum EventType {
	Navigation = "Navigation",
	OpenTab = "OpenTab",
	CloseTab = "CloseTab",
	SwitchTab = "SwitchTab",

	//DOM events
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

	Generic = "Generic",
}

export interface IListener {
    start() : void;
    stop() : void;
}

/**
 * Define the constructor signature of IListener 
 */
export interface IListenerConstructor {
    new(callback: (event: BrowserEvent) => void): IListener;
}

export interface IEvent {
	timeStamp : Date;
	issuingModule : number;
	type : EventType;
}

export class BrowserEvent implements IEvent{
	private _timeStamp : Date;
	private _issuingModule : number = 0;
	private _type : EventType;
	private _windowID : number;
	private _tabID : number;
	private _url? : string;
	constructor(type : EventType, tabID : number, windowID: number) {
		this._type = type;
		this._timeStamp = new Date();
		this._tabID = tabID;
		this._windowID = windowID;
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