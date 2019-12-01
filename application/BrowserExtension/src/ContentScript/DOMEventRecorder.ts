'use strict';

import { DOMEventTypes, BrowserEvent } from '../Shared/SharedDeclarations'
import { TextInputEvent, ButtonClickEvent, HoverEvent, TextSelectionEvent, DownloadEvent } from '../Shared/BrowserEvents';

/**
 * Recorder injected into the website to capture DOM events.
 */
class DOMEventRecorder {
	/**
	 * Starts domevent recorder.
	 */
	public start() : void {
		throw new Error("Method not implemented.");
	}
	/**
	 * Stops domevent recorder
	 */
	public stop() : void {
		throw new Error("Method not implemented.");
	}

	/**
	 * Sends event to the background script.
	 * @param event the event to serialize and send.
	 */
	private sendEvent(event : BrowserEvent) {
		throw new Error("Method not implemented.");
	}

	/**
	 * Creates text input event
	 * @param ev The event which occured on the website
	 * @returns text input event 
	 */
	private createTextInputEvent(ev : any) : TextInputEvent {
		throw new Error("Method not implemented.");
	}
	/**
	 * Creates button click event
	 * @param ev The event which occured on the website
	 * @returns button click event 
	 */
	private createButtonClickEvent(ev : any) : ButtonClickEvent {
		throw new Error("Method not implemented.");
	}
	/**
	 * Creates hover event
	 * @param ev The event which occured on the website
	 * @returns hover event 
	 */
	private createHoverEvent(ev : any) : HoverEvent {
		throw new Error("Method not implemented.");
	}
	/**
	 * Creates text selection event
	 * @param ev The event which occured on the website
	 * @returns text selection event 
	 */
	/**
	 * Params domevent recorder
	 * @param ev The event which occured on the website
	 * @returns text selection event 
	 */
	private createTextSelectionEvent(ev : any) : TextSelectionEvent {
		throw new Error("Method not implemented.");
	}
	/**
	 * Creates download event
	 * @param ev The event which occured on the website
	 * @returns download event 
	 */
	private createDownloadEvent(ev : any) : DownloadEvent {
		throw new Error("Method not implemented.");
	}
}