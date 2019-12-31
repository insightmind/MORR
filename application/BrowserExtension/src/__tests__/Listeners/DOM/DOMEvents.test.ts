import { TextInputEvent, ButtonClickEvent, HoverEvent, TextSelectionEvent } from '../../../Listeners/DOM/DOMEvents'
import { EventType } from '../../../Shared/SharedDeclarations'
import { url } from 'inspector';

const sample : any = {_issuingModule : 0, _windowID : 3, _tabID : 7,  _url : new URL("http://sample.com")};

describe("TextInputEvent Tests", () => {
    const mySample = {...sample};
    mySample._text = "Selected Text";
    mySample._target = "tgtBtn"; 
    mySample._type = EventType.TextInput;
    let evt : TextInputEvent;
    beforeEach(() => {
        evt = new TextInputEvent(sample._tabID, sample._windowID, mySample._text,
                                mySample._target, mySample._url.href);
    });

    //test if the objects' constructor correctly sets its fields
    test("Constructor", () => {
        expect(evt).not.toBeUndefined();
        expect(evt).toMatchObject(mySample);
        expect(evt.url).toStrictEqual(mySample._url);
    });

    //Test if the specific getters return the desired values.
    test("Getters", () => {
        expect(evt.text).toBe(mySample._text);
        expect(evt.target).toBe(mySample._target);
    });

    //Test if the specific setters set the desired values
    test("Setters", () => {
        const newText = "another Text";
        const newTarget = "changed Target";
        evt.text = newText
        evt.target = newTarget
        let localSample = {...mySample};
        localSample._text = newText;
        localSample._target = newTarget;
        expect(evt).toMatchObject(localSample);
    });
});

describe("ButtonClickEvent Tests", () => {
    const mySample = {...sample};
    mySample._buttonTitle = "Click Me!";
    mySample._buttonHref = "https://sample.com/redirect";
    let evt : ButtonClickEvent;
    beforeEach(() => {
        evt = new ButtonClickEvent(mySample._tabID, mySample._windowID, mySample._buttonTitle,
                                        mySample._url, mySample._buttonHref);
    });

    test("Constructor", () => {
        expect(evt).not.toBeUndefined();
        expect(evt).toMatchObject(mySample);
        expect(evt.url).toEqual(mySample._url);
    });

    test("Constructor no HREF", () => {
        let localEvent = new ButtonClickEvent(mySample._tabID, mySample._windowID, mySample._buttonTitle,
            mySample._url);
        let localSample = {...mySample};
        localSample._buttonHref = undefined;
        expect(localEvent).not.toBeUndefined();
        expect(localEvent).toMatchObject(localSample);
        expect(evt.url).toStrictEqual(mySample._url);
    });

    test("Getters", () => {
        expect(evt.buttonTitle).toBe(mySample._buttonTitle);
        expect(evt.buttonHref).toBe(mySample._buttonHref);
    });

    test("Setters", () => {
        const newTitle = "Don't Click Me!";
        const newHref = "localhost:80";
        evt.buttonTitle = newTitle;
        evt.buttonHref = newHref;
        let localSample = {...mySample};
        localSample._buttonTitle = newTitle;
        localSample._buttonHref = newHref;
        expect(evt).toMatchObject(localSample);
    });
});

describe("HoverEvent Tests", () => {
    const mySample = {...sample};
    mySample._target = "tgtBtn";
    let evt : HoverEvent;

    beforeEach(() => {
        evt = new HoverEvent(sample._tabID, sample._windowID, mySample._target, mySample._url.href);
    });

    test("Constructor", () => {
        expect(evt).not.toBeUndefined();
        expect(evt).toMatchObject(mySample);
        expect(evt.url).toStrictEqual(mySample._url);
    });

    test("Getters", () => {
        expect(evt.target).toBe(mySample._target);
    });

    test("Setters", () => {
        evt.target = "New Target";
        const localSample = {...mySample};
        localSample._target = "New Target";
        expect(evt).toMatchObject(localSample);
    })
});

describe("TextSelectionEvent Tests", () => {
    const mySample = {...sample};
    mySample._textSelection = "Selected Text";
    let evt : TextSelectionEvent;

    beforeEach(() => {
        evt = new TextSelectionEvent(sample._tabID, sample._windowID, mySample._textSelection,
                                    mySample._url.href);
    });

    test("Constructor", () => {
        expect(evt).toMatchObject(mySample);
        expect(evt.url).toStrictEqual(mySample._url);
    });

    test("Getters", () => {
        expect(evt.textSelection).toBe(mySample._textSelection);
    });

    test("Setters", () => {
        evt.textSelection = "new selection";
        expect(evt.textSelection).toBe("new selection");
    })
});