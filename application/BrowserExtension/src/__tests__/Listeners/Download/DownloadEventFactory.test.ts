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

test('Create DownloadEvent', async () => {
    let now : Date = new Date();
    let factory : DownloadEventFactory = new DownloadEventFactory();
    let item = {url : "http://kit.edu/downloads/plan.png", mime : "PNG"};
    chrome.tabs.query.yields([
        {id: 7, url: 'http://kit.edu', windowId : 5}, 
        {id: 13, url: 'https://google.com', windowId: 3}
    ]);
    let evt : DownloadEvent = await factory.createEvent(item as chrome.downloads.DownloadItem);
    expect(evt).toMatchObject({_tabID : 7, _windowID : 5, _mimeType : "PNG"});
    expect(evt.fileURL.href).toEqual("http://kit.edu/downloads/plan.png");
    expect(evt.url.href).toEqual("http://kit.edu/");
    expect(evt.timeStamp.valueOf()).toBeGreaterThanOrEqual(now.valueOf());
    expect(evt.timeStamp.valueOf()).toBeLessThanOrEqual(new Date().valueOf());
});

/**
 * Test if createEvent fails if no URL is provided
 */
test('Create DownloadEvent missing URL', () => {
    let now : Date = new Date();
    let factory : DownloadEventFactory = new DownloadEventFactory();
    let item = {url : "http://kit.edu/downloads/plan.png", mime : "PNG"};
    chrome.tabs.query.yields([
        {windowId : 27},
    ]);
    return expect(factory.createEvent(item as chrome.downloads.DownloadItem)).rejects.not.toBeNaN();
});

/**
 * Test if tabID is set to 0 if none can be obtained by query
 */
test('Create DownloadEvent missing tabID', async () => {
    let now : Date = new Date();
    let factory : DownloadEventFactory = new DownloadEventFactory();
    let item = {url : "http://kit.edu/downloads/plan.png", mime : "PNG"};
    chrome.tabs.query.yields([
        {url: 'http://kit.edu', windowId : 5},
    ]);
    let evt : DownloadEvent = await factory.createEvent(item as chrome.downloads.DownloadItem);
    expect(evt).toMatchObject({_tabID : 0, _windowID : 5, _mimeType : "PNG"});
});