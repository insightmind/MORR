import { BrowserEvent } from '../../../Shared/SharedDeclarations'
import { DOMEventTypes } from '../DOMEvents'
import { TextInputEvent, ButtonClickEvent, HoverEvent, TextSelectionEvent } from '../DOMEvents';
import DOMEventFactory from './DOMEventFactory'
/**
 * Recorder injected into the website to capture DOM events.
 */
class DOMEventRecorder {
	private factory : DOMEventFactory;
	constructor() {
		this.factory = new DOMEventFactory();
	}
	
	/**
	 * Starts domevent recorder.
	 */
	public start() : void {
		Object.values(DOMEventTypes).forEach((key : string) => {
			document.addEventListener(key, (evt) => {console.log(evt); this.handleEvent(evt);}, {
				capture: true,
				passive: true,
			});
		});
		document.body.style.backgroundColor = "yellow"; //TODO: remove. Just there for testing purposes
	}
	/**
	 * Stops domevent recorder
	 */
	public stop() : void {
		Object.values(DOMEventTypes).forEach(function (key : string) {
			document.removeEventListener(key, (evt) => {console.log(evt)}, {
				capture: true,
			});
		});
		document.body.style.backgroundColor = "white";
	}

	/**
	 * Sends event to the background script.
	 * @param event the event to serialize and send.
	 */
	private sendEvent(event : BrowserEvent) {
		throw new Error("Method not implemented.");
	}

	private handleEvent = (domEvent : Event) : void => {
		let event : BrowserEvent | undefined = this.factory.createEvent(domEvent);
		if (event)
			chrome.runtime.sendMessage(event.serialize());
	}
}

const script = new DOMEventRecorder();
script.start();