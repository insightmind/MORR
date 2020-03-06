import { chrome, resetChrome } from '../__mock__/chrome_mock'
const globalAny:any = global;
globalAny.chrome = chrome
import PostHTTPInterface from '../ApplicationInterface/PostHTTPInterface'
import { ButtonClickEvent } from '../Listeners/DOM/DOMEvents';

//Suppress console output for tests
let log = () => undefined;
globalAny.console.log = log;
globalAny.console.error = log;

let comm : PostHTTPInterface;
const invalidAppString : string = "Connection should not be accepted for invalid application";
const WAITTIME = 30; //how long to wait before counting callback-invocations
const sample : any = {_issuingModule : 0, _windowID : 3, _tabID : 7,  _url : new URL("http://sample.com")};

beforeAll(() => {
    jest.resetAllMocks();
})

beforeEach(() => {
    comm = new PostHTTPInterface("http://localhost:60024");
})

//confirm that connection is established when receiving a valid answer
test("Establish connection", done => {
    class Response {
        json() {
            return {application: "MORR", response: "Ok"};
        } 
    }
    let fetch = jest.fn((...args : any[]) => new Promise((resolve) => resolve(new Response)));
    globalAny.fetch = fetch;
    comm.establishConnection().then(() => {
        expect(fetch.mock.calls[0][0]).toEqual("http://localhost:60024"); //check that interface contacted correct address
        done();
    }).catch((e) => fail(e));
})

//confirm that connection is not established when receiving an invalid answer
test("Establish connection (wrong response)", done => {
    class Response {
        json() {
            return {application: "Internet Explorer", response: "Ok"}; //wrong application identifier
        } 
    }
    let fetch = (...args : any[]) => new Promise((resolve) => resolve(new Response));
    globalAny.fetch = fetch;
    comm.establishConnection().then(() => fail(invalidAppString)).catch(() => done());
})

//confirm that config is correctly received
test("Request config", done => {
    class Response {
        json() {
            return {application: "MORR", config: "Somevalue"}; //wrong application identifier
        } 
    }
    let fetch = (...args : any[]) => new Promise((resolve) => resolve(new Response));
    globalAny.fetch = fetch;
    comm.requestConfig().then((config) => {expect(config).toBe("Somevalue"); done();});
})

//confirm that requestConfig rejects when receiving an invalid answer
test("Request config (wrong response)", done => {
    class Response {
        json() {
            return {application: "CMD.exe", config: "Somevalue"}; //wrong application identifier
        } 
    }
    let fetch = (...args : any[]) => new Promise((resolve) => resolve(new Response));
    globalAny.fetch = fetch;
    comm.requestConfig().then(() => fail(invalidAppString)).catch(() => done());
})

//confirm that waitForStart is resolved when receiving the expected answer
test("Wait for start", done => {
    class Response {
        json() {
            return {application: "MORR", response: "Start"};
        } 
    }
    let fetch = jest.fn((...args : any[]) => new Promise((resolve) => resolve(new Response)));
    globalAny.fetch = fetch;
    comm.waitForStart().then(() => done());
})

//confirm that waitForStart is rejected when receiving an unexpected answer
test("Wait for start (invalid response)", done => {
    class Response {
        json() {
            return {application: "MORR", response: "Ok"};
        } 
    }
    let fetch = jest.fn((...args : any[]) => new Promise((resolve) => resolve(new Response)));
    globalAny.fetch = fetch;
    comm.waitForStart().then(() => fail("Should only resolve when recieiving Start-command")).catch(() => done());
})

//confirm that the onStopCallback passed to the PostHTTPInterface is invoked when receiving a stop-message
test("OnStopListener", done => {
    class Response {
        private running : boolean = false;
        json() {
            console.log("Running is now " + this.running);
            this.running = !this.running;
            if (this.running)
                return {application: "MORR", response: "Start"};
            else
                return {application: "MORR", response: "Stop"};
        }
    }
    let resp = new Response();
    let fetch = jest.fn((...args : any[]) => new Promise((resolve) => resolve(resp)));
    globalAny.fetch = fetch;
    let callback = jest.fn();
    comm.addOnStopListener(callback);
    comm.waitForStart().then(() => {
        setTimeout(() => {
            expect(callback).toHaveBeenCalledTimes(1);
            done();
        }, WAITTIME);

    });
})

//confirm that sending data works
test("Send data", done => {
    class Response {
        json() {
            return {application: "MORR", response: "Ok"};
        } 
    }
    const mySample = {...sample};
    mySample._buttonTitle = "Click Me!";
    mySample._buttonHref = "https://sample.com/redirect";
    let fetch = jest.fn((...args : any[]) => new Promise((resolve) => resolve(new Response)));
    globalAny.fetch = fetch;
    let data = new ButtonClickEvent(mySample._tabID, mySample._windowID, mySample._buttonTitle,mySample._url).serialize();
    comm.sendData(data).then(() => {
        expect(fetch).toHaveBeenCalledWith("http://localhost:60024", {
            method : "POST",
            body: `{"request" : "sendData", "data" : ${data}}`,
        });
        done();
    })
})

//confirm that sending data also throws when receiving an invalid answer
test("Send data (invalid answer)", done => {
    class Response {
        json() {
            return {application: "cmd.exe", response: "Ok"};
        } 
    }
    const mySample = {...sample};
    mySample._buttonTitle = "Click Me!";
    mySample._buttonHref = "https://sample.com/redirect";
    let fetch = jest.fn((...args : any[]) => new Promise((resolve) => resolve(new Response)));
    globalAny.fetch = fetch;
    comm.sendData(new ButtonClickEvent(mySample._tabID, mySample._windowID, mySample._buttonTitle,mySample._url).serialize()).then(() => {
        fail(() => invalidAppString);
    }).catch(() => done());
})

//confirm that sendData will pass through Stop-commands
test("Send data (stop answer)", done => {
    class Response {
        json() {
            return {application: "MORR", response: "Stop"};
        } 
    }
    const mySample = {...sample};
    mySample._buttonTitle = "Click Me!";
    mySample._buttonHref = "https://sample.com/redirect";
    let fetch = jest.fn((...args : any[]) => new Promise((resolve) => resolve(new Response)));
    globalAny.fetch = fetch;
    let callback = jest.fn();
    comm.addOnStopListener(callback);
    comm.sendData(new ButtonClickEvent(mySample._tabID, mySample._windowID, mySample._buttonTitle,mySample._url).serialize()).then((resp) => {
        expect(resp).toEqual("Stop");
        setTimeout(() => {
            expect(callback).toBeCalledTimes(1);
            done();
        }, WAITTIME);
    });
})

//test if PostHTTPInterface will create a port-entry in browser storage when the extension is installed.
test("Set port", () => {
    expect(chrome.storage.local.set.calledOnceWith({"port": "No Port Set!"})).toBeFalsy();
    chrome.runtime.onInstalled.trigger({reason : "install"});
    expect(chrome.storage.local.set.calledOnceWith({"port": "No Port Set!"})).toBeTruthy();
    test.only("Set port", () => {
        chrome.runtime.onInstalled.trigger({reason : "someelse"});
        expect(chrome.storage.local.set.calledTwice).toBeFalsy();
    })
})