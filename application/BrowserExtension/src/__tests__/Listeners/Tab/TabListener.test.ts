import { chrome, resetChrome } from '../../../__mock__/chrome_mock'
import { BrowserEvent, EventType } from '../../../Shared/SharedDeclarations';
import TabListener from '../../../Listeners/Tab/TabListener';
import { UNKOWN_URL } from '../../../Listeners/Tab/TabEvents'

const WAITTIME = 30; //how long to wait before counting callback-invocations
const globalAny:any = global;

const mockCallback = jest.fn((evt : BrowserEvent) => 0);

let listener : TabListener;

//helper function, waits for WAITTIME milliseconds
function wait() : Promise<void> {
    return new Promise((resolve) => setTimeout(() => resolve(), WAITTIME));
}

beforeAll(() => {
    globalAny.chrome = chrome
})

beforeEach(() => {
    listener = new TabListener(mockCallback);
    mockCallback.mockReset();
    resetChrome();
    chrome.tabs.query.withArgs({active : true, currentWindow : true}).yields([
        {id: 7, url: 'http://sample.com', windowId : 5, currentWindow : true, active: true},
    ]);
})

// Test if the constructor runs through and does not call the passed function.
test("Constructor", () => {
    expect(listener).toBeInstanceOf(TabListener);
    expect(mockCallback).toHaveBeenCalledTimes(0);
})

//Test if the callback is called four times if four different events occur
test("Start Capture", done => {
    listener.start();
    expect(mockCallback).toHaveBeenCalledTimes(0);
    chrome.tabs.onCreated.trigger({id : 5, windowId : 3});
    chrome.tabs.onActivated.trigger({tabId : 5, windowId : 3});
    chrome.tabs.onUpdated.trigger(5, {windowId : 3, url: "https://sample.com/test"}, {tabId : 5, windowId : 3});
    chrome.tabs.onRemoved.trigger(5, {windowId : 3});
    wait().then(() => {
        expect(mockCallback).toHaveBeenCalledTimes(4);
        done();
    })
})

//test if stopping a capture results in the callback not being invoked
test("Stop Capture", done => {
    listener.start();
    expect(mockCallback).toHaveBeenCalledTimes(0);
    listener.stop();
    chrome.tabs.onCreated.trigger({id : 5, windowId : 3});
    chrome.tabs.onActivated.trigger({tabId : 5, windowId : 3});
    chrome.tabs.onUpdated.trigger(5, {windowId : 3, url: "https://sample.com/test"}, {tabId : 5, windowId : 3});
    chrome.tabs.onRemoved.trigger(5, {windowId : 3});
    wait().then(() => {
        expect(mockCallback).toHaveBeenCalledTimes(0);
        done();
    });
})

//capture an OpenTabEvent and check its fields
test("Capture OpenTabEvent", done => {
    listener.start();
    expect(mockCallback).toHaveBeenCalledTimes(0);
    chrome.tabs.onCreated.trigger({id : 5, windowId : 3});
    wait().then(() => {
        expect(mockCallback).toHaveBeenCalledTimes(1);
        let evt = mockCallback.mock.calls[0][0];
        expect(evt).not.toBeUndefined;
        expect(evt).toMatchObject({_tabID : 5, _windowID : 3, _type: EventType.OpenTab})
        expect(evt.url.href).toBe(UNKOWN_URL);
        done();
    });
})

//capture an CloseTabEvent and check its fields
test("Capture CloseTabEvent", done => {
    listener.start();
    expect(mockCallback).toHaveBeenCalledTimes(0);
    chrome.tabs.onCreated.trigger({id : 7, windowId : 3});
    chrome.tabs.query.withArgs({active : true, currentWindow : true}).yields([
        {id: 7, url: 'https://sample.com/test', windowId : 5, currentWindow : true, active: true},
    ]);
    chrome.tabs.onUpdated.trigger(7, {windowId : 3, url: "https://sample.com/test"}, {tabId : 5, windowId : 3});
    chrome.tabs.onRemoved.trigger(7, {windowId : 3});
    wait().then(() => {
        expect(mockCallback).toHaveBeenCalledTimes(3);
        let evt : BrowserEvent;
        for (let i = 0; i < 3; i++) {
            if (mockCallback.mock.calls[i][0].type == EventType.CloseTab)
                evt = mockCallback.mock.calls[i][0];
        }
        expect(evt!).not.toBeUndefined;
        expect(evt!).toMatchObject({_tabID : 7, _windowID : 3, _type: EventType.CloseTab})
        expect(evt!.url.href).toBe("https://sample.com/test");
        done();
    });
})

//capture an NavigationEvent and check its fields
test("Capture NavigationEvent", done => {
    listener.start();
    expect(mockCallback).toHaveBeenCalledTimes(0);
    chrome.tabs.onUpdated.trigger(5, {windowId : 3, url: "https://sample.com/test"}, {tabId : 5, windowId : 3});
    wait().then(() => {
        expect(mockCallback).toBeCalledTimes(1);
        let evt : BrowserEvent = mockCallback.mock.calls[0][0];
        expect(evt).not.toBeUndefined;
        expect(evt).toMatchObject({_tabID : 5, _windowID : 3, _type: EventType.Navigation})
        expect(evt.url.href).toBe("https://sample.com/test");
        done();
    })
})

//Confirm that the listener does not create an event for TabUpdates which are not URL-changes
test("Ignore Non-navigation updates", done => {
    listener.start();
    expect(mockCallback).toHaveBeenCalledTimes(0);
    chrome.tabs.onUpdated.trigger(5, {windowId : 3}, {tabId : 5, windowId : 3});
    wait().then(() => {
        done();
    })
})

//capture an SwitchTabEvent and check its fields
test("Capture SwitchTabEvent", done => {
    listener.start();
    expect(mockCallback).toHaveBeenCalledTimes(0);
    chrome.tabs.onActivated.trigger({tabId : 5, windowId : 3});
    wait().then(() => {
        expect(mockCallback).toBeCalledTimes(1);
        let evt : BrowserEvent = mockCallback.mock.calls[0][0];
        expect(evt).not.toBeUndefined;
        expect(evt).toMatchObject({_tabID : 7, _windowID : 5, _type: EventType.SwitchTab, _newTabID: 5});
        expect(evt.url.href).toBe("http://sample.com/");
        done();
    })
})