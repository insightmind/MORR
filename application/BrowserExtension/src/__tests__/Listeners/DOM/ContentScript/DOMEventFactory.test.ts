import DOMEventFactory from '../../../../Listeners/DOM/ContentScript/DOMEventFactory'
import { EventType, BrowserEvent } from '../../../../Shared/SharedDeclarations'
import { HoverEvent, TextInputEvent, TextSelectionEvent, ButtonClickEvent, DOMEventTypes } from '../../../../Listeners/DOM/DOMEvents'
import {sampleDocument} from '../../../../__mock__/client'
const sampleTarget = {tagName : "BUTTON", title : "sample target", value : "sampleinput", type : "text",
getAttribute : function(attr : string) {return (attr == "href") ? "https://sample.com/redirect" : null;}};
const sampleTextTarget = {tagName : "INPUT", type : "text", title : "sample target", value : "sampleinput",
getAttribute : function(attr : string) {return (attr == "href") ? "https://sample.com/redirect" : null;}};
const WAITTIME = 30; //how long to wait before counting callback-invocations

let factory : DOMEventFactory;

beforeEach(() => {
    factory = new DOMEventFactory();
});

//Check if jsdom is setup correctly. Might help identify an erroneous configuration which will make all tests fail
test("TestSetup", () => {
    expect(window.location.href).toBe(sampleDocument.url);
    expect(document.location.href).toBe(sampleDocument.url);
})

//create a ButtonClickEvent (base scenario)
test("Create ButtonClickEvent (success, type button)", done => {
    factory.createEvent(<Event>(<any>{type : DOMEventTypes.CLICK, target : sampleTarget}))
    .then((event? : BrowserEvent) => {
        expect(event).not.toBeUndefined;
        expect(event).toBeInstanceOf(ButtonClickEvent);
        if(event) {
            let sEvent = <ButtonClickEvent>event;
            expect(sEvent.type).toBe(EventType.ButtonClick);
            expect(sEvent.buttonTitle).toBe(sampleTarget.title);
            expect(sEvent.url.href).toBe(sampleDocument.url);
            expect(sEvent.buttonHref).toBe("https://sample.com/redirect");
            done();
        } else {
            fail("event is undefined or of unexpected type");
            done();
        }
    })
    .catch((e : Error) => {
        fail(e);
        done();
    });
});

//create a TextSelectionEvent (base scenario), triggered by mouseup
test("Create TextSelectionEvent (success, type mouseup)", done => {
    factory.createEvent(<Event>(<any>{type : DOMEventTypes.MOUSEUP, target : sampleTarget}))
    .then((event? : BrowserEvent) => {
        expect(event).not.toBeUndefined;
        expect(event).toBeInstanceOf(TextSelectionEvent);
        if (event) {
            let sEvent = <TextSelectionEvent>event;
            expect(sEvent.type).toBe(EventType.TextSelection);
            expect(sEvent.textSelection).toBe(sampleDocument.selection);
            expect(sEvent.url.href).toBe(sampleDocument.url);
            done();
        } else {
            fail("event is undefined or of unexpected type");
            done();
        }
    });
});

//create a TextInputEvent (base scenario)
test("Create TextInputEvent (success)", done => {
    factory.createEvent(<Event>(<any>{type : DOMEventTypes.CHANGE, target : sampleTextTarget}))
    .then((event? : BrowserEvent) => {
        expect(event).not.toBeUndefined;
        expect(event).toBeInstanceOf(TextInputEvent);
        if (event) {
            let sEvent = <TextInputEvent>event;
            expect(sEvent.type).toBe(EventType.TextInput);
            expect(sEvent.text).toBe(sampleTarget.value);
            expect(sEvent.url.href).toBe(sampleDocument.url);
            done();
        } else {
            fail("event is undefined or of unexpected type");
            done();
        }
    });
});

//create a HoverEvent (base scenario, no interrupts)
test("Create HoverEvent (success)", done => {
    factory.createEvent(<Event>(<any>{type : DOMEventTypes.MOUSEENTER, target : sampleTarget}))
    .then((event? : BrowserEvent) => {
        expect(event).not.toBeUndefined;
        expect(event).toBeInstanceOf(HoverEvent);
        if (event) {
            let sEvent = <HoverEvent>event;
            expect(sEvent.type).toBe(EventType.Hover);
            expect(sEvent.target).toBe(sampleTarget.title);
            expect(sEvent.url.href).toBe(sampleDocument.url);
            done();
        } else {
            fail("event is undefined or of unexpected type");
            done();
        }
    });
});

//create a ButtonClickEvent (base scenario)
test("Create ButtonClickEvent (no element title)", done => {
    const basetarget = {tagName : "BUTTON", value : "sampleinput", type : "text",
    getAttribute : function(attr : string) {return (attr == "href") ? "https://sample.com/redirect" : null;}};
    const anotherTarget = {...basetarget, name : "element name"};
    const anotherTarget2 = {...basetarget, id : "element id"};
    factory.createEvent(<Event>(<any>{type : DOMEventTypes.CLICK, target : anotherTarget}))
    .then((event? : BrowserEvent) => {
        expect(event).not.toBeUndefined;
        if(event) {
            let sEvent = <ButtonClickEvent>event;
            expect(sEvent.buttonTitle).toBe(anotherTarget.name);
            done();
        } else {
            fail("event is undefined or of unexpected type");
            done();
        }
    })
    .catch((e : Error) => {
        fail(e);
        done();
    });

    factory.createEvent(<Event>(<any>{type : DOMEventTypes.CLICK, target : anotherTarget2}))
    .then((event? : BrowserEvent) => {
        expect(event).not.toBeUndefined;
        if(event) {
            let sEvent = <ButtonClickEvent>event;
            expect(sEvent.buttonTitle).toBe(anotherTarget2.id);
            done();
        } else {
            fail("event is undefined or of unexpected type");
            done();
        }
    })
    .catch((e : Error) => {
        fail(e);
        done();
    });
});