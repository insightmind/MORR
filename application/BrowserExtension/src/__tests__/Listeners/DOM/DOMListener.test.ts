import { chrome, resetChrome } from '../../../__mock__/chrome_mock'
import { BrowserEvent, EventType } from '../../../Shared/SharedDeclarations';
import { ButtonClickEvent, HoverEvent, TextInputEvent, TextSelectionEvent } from '../../../Listeners/DOM/DOMEvents'
import DOMListener from '../../../Listeners/DOM/DOMListener';

const WAITTIME = 30; //how long to wait before counting callback-invocations
const globalAny:any = global;

const mockCallback = jest.fn((evt : BrowserEvent) => 0);
const tabs : any[] = [
    {id: 7, url: 'http://sample.com', windowId : 5}, 
    {id: 13, url: 'https://google.com', windowId: 3}
];

let listener : DOMListener;

//helper function, waits for WAITTIME milliseconds
function wait() : Promise<void> {
    return new Promise((resolve) => setTimeout(() => resolve(), WAITTIME));
}

beforeAll(() => {
    globalAny.chrome = chrome
});

beforeEach(() => {
    listener = new DOMListener(mockCallback);
    mockCallback.mockReset();
    resetChrome();
    chrome.tabs.query.yields(tabs);
});

/**
 * Confirm consturctor working and not invoking the callback.
 */
test('Constructor', () => {
    expect(listener).toBeInstanceOf(DOMListener);
    expect(mockCallback).toHaveBeenCalledTimes(0);
})

/**
 * Test if the listener calls executescript on all available tabs when started
 */
test('Start', (done) => {
    listener.start();
    wait().then(() => {
        expect(chrome.tabs.executeScript.callCount).toBe(tabs.length);
        expect(chrome.tabs.executeScript.getCall(0).args[0]).toBe(tabs[0].id);
        expect(chrome.tabs.executeScript.getCall(1).args[0]).toBe(tabs[1].id);
        done();
    });
});

/**
 * Test if the listener informs all available tabs when stopped
 */
test('Stop', (done) => {
    listener.stop();
    wait().then(() => {
        expect(chrome.tabs.sendMessage.callCount).toBe(tabs.length);
        for (let i = 0; i < chrome.tabs.sendMessage.callCount; i++) {
            expect(chrome.tabs.sendMessage.getCall(i).args[1]).toBe("Stop");
        }
        expect(chrome.tabs.sendMessage.getCall(0).args[0]).toBe(7);
        expect(chrome.tabs.sendMessage.getCall(1).args[0]).toBe(13);
        done();
    })
});

/**
 * Test if the listener calls executescript on newly loaded websites while running
 */
test('Inject script', () => {
    listener.start();
    chrome.webNavigation.onDOMContentLoaded.trigger({frameId : 0, tabId : 5});
    expect(chrome.tabs.executeScript.callCount).toBe(tabs.length + 1);
    expect(chrome.tabs.executeScript.getCall(tabs.length).args[0]).toBe(5);
});

/**
 * The following tests check if the listener correctly treats incoming serialized events (sent from contentscript)
 */
test('Receive serialized ButtonClickEvent', () => {
    listener.start();
    chrome.runtime.onMessage.trigger(new ButtonClickEvent(0, 0, "some Text", "https://sample2.com/", "https://sample.com/downloads").serialize(), {tab : {id : 2, windowId : 9}});
    expect(mockCallback).toHaveBeenCalledTimes(1);
    let evt = mockCallback.mock.calls[0][0] as ButtonClickEvent;
    expect(evt).toMatchObject({_tabID : 2, _windowID : 9, _buttonTitle : "some Text", _type : EventType.ButtonClick, _buttonHref : "https://sample.com/downloads"});
    expect(evt.url).toStrictEqual(new URL("https://sample2.com/"));
    expect(evt).toBeInstanceOf(ButtonClickEvent);
});

test('Receive serialized HoverEvent', () => {
    listener.start();
    chrome.runtime.onMessage.trigger(new HoverEvent(0, 0, "sampleTextField", "https://sample2.com/").serialize(), {tab : {id : 2, windowId : 9}});
    expect(mockCallback).toHaveBeenCalledTimes(1);
    let evt = mockCallback.mock.calls[0][0] as BrowserEvent;
    expect(evt).toMatchObject({_tabID : 2, _windowID : 9, _target : "sampleTextField", _type : EventType.Hover});
    expect(evt.url).toStrictEqual(new URL("https://sample2.com/"));
    expect(evt).toBeInstanceOf(HoverEvent);
});

test('Receive serialized TextInputEvent', () => {
    listener.start();
    chrome.runtime.onMessage.trigger(new TextInputEvent(0, 0, "sampletext", "sometextfield", "https://sample2.com/").serialize(), {tab : {id : 2, windowId : 9}});
    expect(mockCallback).toHaveBeenCalledTimes(1);
    let evt = mockCallback.mock.calls[0][0] as BrowserEvent;
    expect(evt).toMatchObject({_tabID : 2, _windowID : 9, _text : "sampletext", _target : "sometextfield", _type : EventType.TextInput});
    expect(evt.url).toStrictEqual(new URL("https://sample2.com/"));
    expect(evt).toBeInstanceOf(TextInputEvent);
});

test('Receive serialized TextSelectionEvent', () => {
    listener.start();
    chrome.runtime.onMessage.trigger(new TextSelectionEvent(0, 0, "sampletext", "https://sample2.com/").serialize(), {tab : {id : 2, windowId : 9}});
    expect(mockCallback).toHaveBeenCalledTimes(1);
    let evt = mockCallback.mock.calls[0][0] as BrowserEvent;
    expect(evt).toMatchObject({_tabID : 2, _windowID : 9, _textSelection : "sampletext", _type : EventType.TextSelection});
    expect(evt.url).toStrictEqual(new URL("https://sample2.com/"));
    expect(evt).toBeInstanceOf(TextSelectionEvent);
});