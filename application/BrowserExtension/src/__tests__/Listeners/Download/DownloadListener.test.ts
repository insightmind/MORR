import DownloadListener from '../../../Listeners/Download/DownloadListener'
import { BrowserEvent } from '../../../Shared/SharedDeclarations';
import { chrome, resetChrome } from '../../../__mock__/chrome_mock'
import { DownloadEvent } from '../../../Listeners/Download/DownloadEvents';
import { thisExpression } from '@babel/types';

const WAITTIME = 200; //how long to wait before counting callback-invocations
const globalAny:any = global;

const mockCallback = jest.fn((evt : BrowserEvent) => 0);

beforeAll(() => {
    globalAny.chrome = chrome
})

beforeEach(() => {
    mockCallback.mockReset();
    resetChrome();
})

/**
 * Test if the constructor runs through and does not call the passed function.
 */
test("Constructor", () => {
    let listener = new DownloadListener(mockCallback);
    expect(listener).toBeInstanceOf(DownloadListener);
    expect(mockCallback).toHaveBeenCalledTimes(0);
})

/**
 * Test if the event created by DownloadListener has its fields set correctly.
 */
test("Capture Event", async (done) => {
    let listener = new DownloadListener(mockCallback);
    listener.start();
    chrome.tabs.query.yields([
        {id: 7, url: 'http://sample.com', windowId : 5}, 
        {id: 13, url: 'https://google.com', windowId: 3}
    ]);
    chrome.downloads.onCreated.trigger({url : "https://sample.com/downloads/plan.png", mime : "PNG"});
    new Promise((resolve) => setTimeout(() => resolve(), WAITTIME))
    .then(() => {
        expect(mockCallback).toHaveBeenCalledTimes(1);
        let evt = mockCallback.mock.calls[0][0] as DownloadEvent;
        expect(evt).toMatchObject(
            {_tabID : 7, _windowID : 5, _mimeType : "PNG"});
        expect(evt.fileURL.href).toBe("https://sample.com/downloads/plan.png");
        expect(evt.url.href).toBe("http://sample.com/");
        done();
    });
})

/**
 * Test if DownloadListener created events if and only if running
 */
test("DownloadListener Create-Start-Stop", async() => {
    let listener = new DownloadListener(mockCallback);
    chrome.downloads.onCreated.trigger({url : "https://sample.com/downloads/plan.png", mime : "PNG"});
    chrome.tabs.query.yields([
        {id: 7, url: 'http://sample.com', windowId : 5}, 
        {id: 13, url: 'https://sample2.com', windowId: 3},
    ]);
    listener.start();
    chrome.downloads.onCreated.trigger({url : "https://sample2.com/downloads/plan2.png", mime : "PNG"})
    listener.stop();
    chrome.downloads.onCreated.trigger({url : "https://sample3.com/downloads/plan3.png", mime : "PNG"})
    await new Promise((resolve) => setTimeout(() => resolve(), WAITTIME))
    expect(mockCallback).toHaveBeenCalledTimes(1);
    expect((mockCallback.mock.calls[0][0] as DownloadEvent).fileURL.href).toBe("https://sample2.com/downloads/plan2.png");
    expect((mockCallback.mock.calls[0][0] as DownloadEvent).tabID).toBe(7);
})