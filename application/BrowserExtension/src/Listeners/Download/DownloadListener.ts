import { IListener, EventType } from '../../Shared/SharedDeclarations'
import { BrowserEvent } from '../../Shared/SharedDeclarations'
import { DownloadEvent } from './DownloadEvents';

/**
 * Listener for downloads.
 */
export default class DownloadListener implements IListener {
    private _callBack: (event: BrowserEvent) => void;
    constructor(callback: (event: BrowserEvent) => void) {
        this._callBack = callback;
    }
    public start(): void {
        throw new Error("Method not implemented.");
    }
    public stop(): void {
        throw new Error("Method not implemented.");
    }

    /**
     * Creates download event
     * @param downloadItem 
     * @returns download event 
     */
    private createDownloadEvent(downloadItem: chrome.downloads.DownloadItem) : DownloadEvent {
        throw new Error("Method not implemented.");
    }
}