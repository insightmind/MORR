import { EventType } from '../../../Shared/SharedDeclarations'
import { OpenTabEvent, CloseTabEvent, NavigationEvent, SwitchTabEvent, UNKOWN_URL } from '../../../Listeners/Tab/TabEvents'

const sample : any = {_issuingModule : 0, _windowID : 3, _tabID : 7,  _url : new URL("http://sample.com")};

describe("OpenTabEvent Tests", () => {
    let event : OpenTabEvent;
    let initTime : Date;
    beforeEach(() => {
        initTime = new Date();
        event = new OpenTabEvent(sample._tabID, sample._windowID);
    })
    test("Constructor", () => {
        let mySample = sample;
        mySample._type = EventType.OpenTab;
        expect(event).toMatchObject(mySample);
        expect(event.url.href).toBe(UNKOWN_URL);
        expect(event.timeStamp.valueOf()).toBeLessThanOrEqual(new Date().valueOf());
        expect(event.timeStamp.valueOf()).toBeGreaterThanOrEqual(initTime.valueOf());
    });
});

describe("CloseTabEvent Tests", () => {
    let initTime : Date;
    beforeEach(() => {
        initTime = new Date();

    })
    test("Constructor", () => {
        let event = new CloseTabEvent(sample._tabID, sample._windowID, sample._url.href);
        let mySample = sample;
        mySample._type = EventType.CloseTab;
        expect(event).toMatchObject(mySample);
        expect(event.url.href).toBe(sample._url.href);
        expect(event.timeStamp.valueOf()).toBeLessThanOrEqual(new Date().valueOf());
        expect(event.timeStamp.valueOf()).toBeGreaterThanOrEqual(initTime.valueOf());
    });
    test("Constructor no URL", () => {
        let event = new CloseTabEvent(sample._tabID, sample._windowID);
        let mySample = sample;
        mySample._type = EventType.CloseTab;
        expect(event).toMatchObject(mySample);
        expect(event.url.href).toBe(UNKOWN_URL);
        expect(event.timeStamp.valueOf()).toBeLessThanOrEqual(new Date().valueOf());
        expect(event.timeStamp.valueOf()).toBeGreaterThanOrEqual(initTime.valueOf());
    });
})

describe("NavigationEvent Tests", () => {
    let event : NavigationEvent;
    let initTime : Date;
    beforeEach(() => {
        initTime = new Date();
        event = new NavigationEvent(sample._tabID, sample._windowID, sample._url.href);
    })
    test("Constructor", () => {
        let mySample = sample;
        mySample._type = EventType.Navigation;
        expect(event).toMatchObject(mySample);
        expect(event.url.href).toBe(sample._url.href);
        expect(event.timeStamp.valueOf()).toBeLessThanOrEqual(new Date().valueOf());
        expect(event.timeStamp.valueOf()).toBeGreaterThanOrEqual(initTime.valueOf());
    });
})

describe("SwitchTabEvent Tests", () => {
    let event : SwitchTabEvent;
    let initTime : Date;
    beforeEach(() => {
        initTime = new Date();
        event = new SwitchTabEvent(sample._tabID, sample._windowID, 8, sample._url.href);
    })
    test("Constructor", () => {
        let mySample = sample;
        mySample._type = EventType.SwitchTab;
        mySample._newTabID = 8;
        expect(event).toMatchObject(mySample);
        expect(event.url.href).toBe(sample._url.href);
        expect(event.timeStamp.valueOf()).toBeLessThanOrEqual(new Date().valueOf());
        expect(event.timeStamp.valueOf()).toBeGreaterThanOrEqual(initTime.valueOf());
    });
    
    /** 
    * Test if the specific getters return the desired values.
    */
    test("Getters", () => {
        expect(event.newTabID).toBe(8);
    })
    /** 
     * Test if the specific setters set the desired values
    */
    test("Setters", () => {
        let mySample = sample;
        mySample._type = EventType.SwitchTab;
        mySample._newTabID = 8;
        expect(event).toMatchObject(mySample);
        event.newTabID = 24;
        mySample._newTabID = 24;
        expect(event).toMatchObject(mySample);
    })
})