import TabEventFactory from '../../../Listeners/Tab/TabEventFactory'
import { chrome, resetChrome } from '../../../__mock__/chrome_mock'
import { OpenTabEvent, CloseTabEvent, NavigationEvent, SwitchTabEvent, UNKOWN_URL } from '../../../Listeners/Tab/TabEvents'

const WAITTIME = 200; //how long to wait before counting callback-invocations
const globalAny:any = global;
const item = {url : "http://sample.com/downloads/plan.png", id : 5, windowId : 2};

let factory : TabEventFactory;

beforeAll(() => {
    globalAny.chrome = chrome
})

beforeAll(() => {
    factory = new TabEventFactory();
})

test('Create OpenTabEvent', () => {
    let evt : OpenTabEvent = factory.createOpenTabEvent(item as chrome.tabs.Tab);
    expect(evt).toMatchObject({_tabID : item.id, _windowID : item.windowId});
});

//check if the factory correctly creates an OpenTabEvent if no tabID was created
test('Create OpenTabEvent no ID', () => {
    let myItem : any = {url : item.url, windowId : item.windowId};
    let evt : OpenTabEvent = factory.createOpenTabEvent(myItem as chrome.tabs.Tab);
    expect(evt).toMatchObject({_tabID : 0, _windowID : item.windowId});
});

test('Create CloseTabEvent', () => {
    let evt : CloseTabEvent = factory.createCloseTabEvent(item.id, {windowId : item.windowId} as chrome.tabs.TabRemoveInfo, item as chrome.tabs.Tab);
    expect(evt).toMatchObject({_tabID : item.id, _windowID : item.windowId});
    expect(evt.url.href).toBe(item.url);
})

//check if the URL is correctly set to unknown, if the url-info of the now-closed tab is no longer accessible
test('Create CloseTabEvent no ID-match', () => {
    let evt : CloseTabEvent = factory.createCloseTabEvent(item.id + 1, {windowId : item.windowId} as chrome.tabs.TabRemoveInfo, item as chrome.tabs.Tab);
    expect(evt).toMatchObject({_tabID : item.id + 1, _windowID : item.windowId});
    expect(evt.url.href).toBe(UNKOWN_URL);
})

test('Create NavigationEvent', () => {
    let evt : NavigationEvent = factory.createNavigationEvent(item.id, {url: item.url} as chrome.tabs.TabChangeInfo, item as chrome.tabs.Tab);
    expect(evt).toMatchObject({_tabID : item.id, _windowID : item.windowId});
    expect(evt.url.href).toBe(item.url);
})

//if changeInfo does not contain an URL, navigation event creation should not be possible
test('Create NavigationEvent missing URL', () => {
    expect(() => factory.createNavigationEvent(item.id, {} as chrome.tabs.TabChangeInfo, item as chrome.tabs.Tab)).toThrow();
})

test('Create SwitchTabEvent', () => {
    let evt : SwitchTabEvent = factory.createSwitchTabEvent({tabId : 105} as chrome.tabs.TabActiveInfo, item as chrome.tabs.Tab);
    expect(evt).toMatchObject({_tabID : item.id, _windowID : item.windowId, _newTabID : 105});
    expect(evt.url.href).toBe(item.url);
})

//check if SwitchTabEvent is correctly created if no previous tabID is available
test('Create SwitchTabEvent no prevID', () => {
    let myItem : any = {url : item.url, windowId : item.windowId};
    let evt : SwitchTabEvent = factory.createSwitchTabEvent({tabId : 105} as chrome.tabs.TabActiveInfo, myItem as chrome.tabs.Tab);
    expect(evt).toMatchObject({_tabID : 0, _windowID : item.windowId, _newTabID : 105});
    expect(evt.url.href).toBe(item.url);
})