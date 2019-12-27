import { DownloadEvent } from './DownloadEvents'

export class DownloadEventFactory {
    /**
     * Asynchronously create a new DownloadEvent from a DownLoadItem
     * @param downloadItem The DownloadItem created by the browser when a download was started.
     * @returns a Promise which will be resolved when the DownloadEvent was created
     */
    public createEvent(downloadItem: chrome.downloads.DownloadItem) : Promise<DownloadEvent> {
        return new Promise((resolve) => {
            let activeTab : chrome.tabs.Tab;
            //as chrome.tabs.query is asynchronous but can't be awaited, this is the reason
            //why the whole function is asynchronous
            chrome.tabs.query({active: true, currentWindow: true}, function(tabs) {
                activeTab= tabs[0];
                let tabID = activeTab.id ? activeTab.id : 0;
                let windowID =  activeTab.windowId;
                let mimeType = downloadItem.mime;
                let fileURL = downloadItem.url;
                let url = activeTab.url ? activeTab.url : "";
                resolve(new DownloadEvent(tabID, windowID, mimeType, fileURL, url));
            });
        });
    }
}