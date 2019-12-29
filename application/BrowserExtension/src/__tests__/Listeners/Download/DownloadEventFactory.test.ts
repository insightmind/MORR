import { DownloadEventFactory } from '../../../Listeners/Download/DownloadEventFactory'
import { BrowserEvent } from '../../../Shared/SharedDeclarations';
import { DownloadEvent } from '../../../Listeners/Download/DownloadEvents';
const chrome = require('sinon-chrome/extensions');

const globalAny:any = global;

beforeAll(() => {
    globalAny.chrome = chrome
})

test('Factory Constructor Test', () => {
    expect(new DownloadEventFactory()).toBeInstanceOf(DownloadEventFactory);
})

test('Factory Create DownloadEvent', async () => {
    let now : Date = new Date();
    let factory : DownloadEventFactory = new DownloadEventFactory();
    let item = {url : "http://kit.edu/downloads/plan.png", mime : "PNG"};
    chrome.tabs.query.yields([
        {id: 7, url: 'http://kit.edu', windowId : 5}, 
        {id: 13, url: 'https://google.com', windowId: 3}
      ]);
    let evt : DownloadEvent = await factory.createEvent(item as chrome.downloads.DownloadItem);
    expect(evt).toMatchObject({_tabID : 7, _windowID : 5, _fileURL : new URL("http://kit.edu/downloads/plan.png"), _url : new URL("http://kit.edu"), _mimeType : "PNG"});
    expect(evt.timeStamp.valueOf()).toBeGreaterThanOrEqual(now.valueOf());
    expect(evt.timeStamp.valueOf()).toBeLessThanOrEqual(new Date().valueOf());
});