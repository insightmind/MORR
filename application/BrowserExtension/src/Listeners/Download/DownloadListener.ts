import { IListener, EventType } from '../../Shared/SharedDeclarations'
import { BrowserEvent } from '../../Shared/SharedDeclarations'
import { DownloadEvent } from './DownloadEvents';
import { DownloadEventFactory } from './DownloadEventFactory';

/**
 * Listener for downloads.
 */
export default class DownloadListener implements IListener {
    private _callBack: (event: BrowserEvent) => void;
    private factory : DownloadEventFactory;
    /**
     * Creates an instance of DownloadListener.
     * @param callback The function to invoke on created events.
     */
    constructor(callback: (event: BrowserEvent) => void) {
        this._callBack = callback;
        this.factory = new DownloadEventFactory();
    }
    /**
     * Starts the DownloadListener
     */
    public start(): void {
        chrome.downloads.onCreated.addListener(this.onDownloadStarted);
    }

    /**
     * Stops the DownloadListener
     */
    public stop(): void {
        chrome.downloads.onCreated.removeListener(this.onDownloadStarted);
    }

    /**
     * Creates download event
     * @param downloadItem 
     * @returns download event 
     */
    private onDownloadStarted = (downloadItem: chrome.downloads.DownloadItem) : void => {
        this.factory.createEvent(downloadItem)
        .then((event : DownloadEvent) => {
            this._callBack(event)
        });
    }
}