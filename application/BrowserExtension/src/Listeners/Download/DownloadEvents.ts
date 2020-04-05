import { BrowserEvent, EventType } from '../../Shared/SharedDeclarations'
/**
 * Download event
 */
export class DownloadEvent extends BrowserEvent {
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
        super(EventType.Download, tabID, windowID, url);
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
        return this._mimeType;
    }
    /**
     * Sets file mime type
     */
    public set mimeType(mimeType : string) {
        this._mimeType = mimeType;
    }
}