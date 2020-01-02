import { BrowserEvent } from '../../../Shared/SharedDeclarations'
import { DOMEventTypes } from '../DOMEvents'
import { TextInputEvent, ButtonClickEvent, HoverEvent, TextSelectionEvent } from '../DOMEvents';
import DOMEventFactory from './DOMEventFactory'

//workaround for the fact that "document.removeListener" will not correctly
//remove the eventlisteners if directly passing this.handleEvent
//TODO: check for more idiomatic alternative
let handleEvent : (domEvent : Event) => void;

/**
 * Recorder injected into the website to capture DOM events.
 * This class is "single use" only. Once stopped, it is
 * not expected to be started again. Instead, the script should be reinjected.
 */
class DOMEventRecorder {
	private factory : DOMEventFactory;

	constructor() {
		this.factory = new DOMEventFactory();
		handleEvent = this.handleEvent;
	}
	
	/**
	 * Starts domevent recorder.
	 */
	public start() : void {
		//attach listeners to all wanted eventtypes
		Object.values(DOMEventTypes).forEach((key : string) => {
			document.addEventListener(key, handleEvent, {
				capture: true,
				passive: true,
			});
		});
		document.body.style.backgroundColor = "yellow"; //TODO: remove. Just there for testing purposes
		chrome.runtime.onMessage.addListener(this.handleMessage);
	}
	/**
	 * Stops domevent recorder. After this, this script is done.
	 * To restart the script on an open site, simply inject this script again.
	 */
	public stop() : void {
		document.body.style.backgroundColor = "white"; //TODO: remove. Just there for testing purposes
		Object.values(DOMEventTypes).forEach((key : string) => {
			document.removeEventListener(key, handleEvent, {
				capture: true
			});
		});
		chrome.runtime.onMessage.removeListener(this.handleMessage);
	}

	/**
	 * Sends event to the background script.
	 * @param event the event to serialize and send.
	 */
	private sendEvent(event : BrowserEvent) {
		if (event)
			chrome.runtime.sendMessage(event.serialize());
	}

	private handleEvent = (domEvent : Event) : void => {
		console.log(domEvent); //TODO: remove, debug-use only
		if (!domEvent.isTrusted) //events are trused if invoked by the user, untrusted if invoked by a script
			return;
		this.factory.createEvent(domEvent)
		.then((event) => {
			if (event) {
				this.sendEvent(event);
			}
		});
	}

	/**
	 * If the "Stop" keyword is sent from the background script, stop the DOMEventRecorder
	 */
	private handleMessage = (message : any) : void => {
		if (message == "Stop")
			this.stop();
	}
}

const script = new DOMEventRecorder();
//we expect the script to be only injected when the recorder is actually supposed to run
script.start();