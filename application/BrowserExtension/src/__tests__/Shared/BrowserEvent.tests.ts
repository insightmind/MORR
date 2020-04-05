import { BrowserEvent, EventType } from '../../Shared/SharedDeclarations'
import { URL } from 'url';

let testData = {_issuingModule : 0, _type : EventType.Generic, _windowID : 3, _tabID : 7,  _url : new URL("http://kit.edu")};

test("Event Constructor", () => {
    let evt : BrowserEvent = new BrowserEvent(testData._type, testData._tabID, testData._windowID, testData._url.toString());
    expect(evt).toMatchObject(testData);
})

test("Event Getters", () => {
    let initTime = new Date();
    let evt : BrowserEvent = new BrowserEvent(testData._type, testData._tabID, testData._windowID, testData._url.toString());
    expect(evt.issuingModule).toBe(0);
    expect(evt.tabID).toBe(7);
    expect(evt.windowID).toBe(3);
    expect(evt.type).toBe("Generic");
    expect(evt.url.toString()).toEqual("http://kit.edu/");
    expect(evt.timeStamp.valueOf()).toBeGreaterThanOrEqual(initTime.valueOf());
    expect(evt.timeStamp.valueOf()).toBeLessThanOrEqual(new Date().valueOf());
})

test("Event Setters", () => {
    let evt : BrowserEvent = new BrowserEvent(testData._type, testData._tabID, testData._windowID, testData._url.toString());
    evt.type = EventType.ButtonClick;
    expect(evt.type).toBe(EventType.ButtonClick);
    evt.tabID = 2;
    expect(evt.tabID).toBe(2);
    evt.windowID = 4;
    expect(evt.windowID).toBe(4);
    evt.url = new URL("https://duckduckgo.com/");
    expect(evt.url.href).toBe("https://duckduckgo.com/");
})

test("Event serialization", () => {
    let evt : BrowserEvent = new BrowserEvent(testData._type, testData._tabID, testData._windowID, testData._url.toString());
    expect(evt.serialize()).toBe("{\"_issuingModule\":0,\"_type\":\"Generic\",\"_timeStamp\":"
                                + JSON.stringify(evt.timeStamp) + ",\"_tabID\":7,\"_windowID\":3,\"_url\":\"http://kit.edu/\"}");
})

test("Event serialization (no underscore)", () => {
    let evt : BrowserEvent = new BrowserEvent(testData._type, testData._tabID, testData._windowID, testData._url.toString());
    expect(evt.serialize(true)).toBe("{\"issuingModule\":0,\"type\":\"Generic\",\"timeStamp\":"
                                + JSON.stringify(evt.timeStamp) + ",\"tabID\":7,\"windowID\":3,\"url\":\"http://kit.edu/\"}");
})