import DownloadListener from '../../../Listeners/Download/DownloadListener'
import { BrowserEvent } from '../../../Shared/SharedDeclarations';
import { chrome, resetChrome } from '../../../__mock__/chrome_mock'
import { DownloadEvent } from '../../../Listeners/Download/DownloadEvents';
import { thisExpression } from '@babel/types';

const WAITTIME = 30; //how long to wait before counting callback-invocations
const globalAny:any = global;

const mockCallback = jest.fn((evt : BrowserEvent) => 0);
let listener : DownloadListener;

beforeAll(() => {
    globalAny.chrome = chrome
})

beforeEach(() => {
    listener = new DownloadListener(mockCallback);
    mockCallback.mockReset();
    resetChrome();
    chrome.tabs.query.yields([
        {id: 7, url: 'http://sample.com', windowId : 5},
        {id: 13, url: 'https://google.com', windowId: 3}
    ]);
})

/**
 * Test if the constructor runs through and does not call the passed function.
 */
test("Constructor", () => {
    expect(listener).toBeInstanceOf(DownloadListener);
    expect(mockCallback).toHaveBeenCalledTimes(0);
})

/**
 * Test if the event created by DownloadListener has its fields set correctly.
 */
test("Capture Event", async (done) => {
    listener.start();
    chrome.downloads.onCreated.trigger({url : "https://sample.com/downloads/plan.png", mime : "PNG", state : "in_progress"});
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
    chrome.downloads.onCreated.trigger({url : "https://sample.com/downloads/plan.png", mime : "PNG", state : "in_progress"});
    listener.start();
    chrome.downloads.onCreated.trigger({url : "https://sample2.com/downloads/plan2.png", mime : "PNG", state : "in_progress"})
    listener.stop();
    chrome.downloads.onCreated.trigger({url : "https://sample3.com/downloads/plan3.png", mime : "PNG", state : "in_progress"})
    await new Promise((resolve) => setTimeout(() => resolve(), WAITTIME))
    expect(mockCallback).toHaveBeenCalledTimes(1);
    expect((mockCallback.mock.calls[0][0] as DownloadEvent).fileURL.href).toBe("https://sample2.com/downloads/plan2.png");
    expect((mockCallback.mock.calls[0][0] as DownloadEvent).tabID).toBe(7);
})
/**
 * Browsers (chrome) sometimes trigger downloads.onCreate for every download in history, therefore
 * the Listener should not create events for downloadItems with state "complete" or "interrupted"
 */
test("DownloadListener ignore completed/interrupted", done => {
    listener.start();
    chrome.downloads.onCreated.trigger({url : "https://sample.com/downloads/plan.png", mime : "PNG", state : "interrupted"});
    chrome.downloads.onCreated.trigger({url : "https://sample.com/downloads/plan.png", mime : "PNG", state : "complete"});
    new Promise((resolve) => setTimeout(() => resolve(), WAITTIME))
    .then(() => {
        expect(mockCallback).toHaveBeenCalledTimes(0);
        done();
    });
})