import { chrome, resetChrome } from '../__mock__/chrome_mock'
const globalAny:any = global;
globalAny.chrome = chrome
import BackgroundScript from '../background'
import PostHTTPInterface from '../ApplicationInterface/PostHTTPInterface'
import ListenerManager from '../ListenerManager'

let bgscript : BackgroundScript;

//Suppress console output for tests
let log = () => undefined;
globalAny.console.log = log;
globalAny.console.error = log;

let connectionMock : any;
let configMock : any;
let waitStartMock : any;
let sendDataMock : any;

const WAITTIME = 30; //how long to wait before counting callback-invocations

beforeEach(() => {
    jest.resetAllMocks();
    jest.mock('../ApplicationInterface/PostHTTPInterface')
    jest.mock('../ListenerManager')
})

//Check if invoking run calls the correct functions on the communication interface mock
test("Regular run", (done) => {
    setUpCommonMocks();
    let startAllMock = ListenerManager.prototype.startAll = jest.fn();
    bgscript = new BackgroundScript();
    bgscript.run();
    setTimeout(() => {
        expect(connectionMock).toBeCalledTimes(1);
        expect(waitStartMock).toBeCalledTimes(1);
        expect(configMock).toBeCalledTimes(1);
        expect(startAllMock).toBeCalledTimes(1);
        expect(sendDataMock).toBeCalledTimes(0);
        done();
    }, WAITTIME);
})

//Check if backgroundscript takes appropriate action when its stop-callback is invoked
test("Run stop", (done) => {
    setUpCommonMocks();
    let stopcallback : (error? : boolean) => void;
    let onStopListenerCallback = PostHTTPInterface.prototype.addOnStopListener = jest.fn((...args: any[]) => {stopcallback = args[0];});
    let startAllMock = ListenerManager.prototype.startAll = jest.fn();
    let stopAllMock = ListenerManager.prototype.stopAll = jest.fn();
    bgscript = new BackgroundScript();
    bgscript.run();
    new Promise((resolve) => {
        setTimeout(() => {
            expect(connectionMock).toBeCalledTimes(1);
            expect(waitStartMock).toBeCalledTimes(1);
            expect(configMock).toBeCalledTimes(1);
            expect(onStopListenerCallback).toBeCalledTimes(1);
            expect(startAllMock).toBeCalledTimes(1);
            expect(stopAllMock).toBeCalledTimes(0);
            expect(sendDataMock).toBeCalledTimes(0);
            expect(stopcallback).not.toBeUndefined();
            stopcallback();
            resolve();
        }, WAITTIME);
    })
    .then(() => {
        setTimeout(() => {
            expect(connectionMock).toBeCalledTimes(1);
            expect(waitStartMock).toBeCalledTimes(2);
            expect(configMock).toBeCalledTimes(1);
            expect(sendDataMock).toBeCalledTimes(0);
            expect(stopAllMock).toBeCalledTimes(1);
            done();
        }, WAITTIME);
    })
})

//Check if backgroundscript takes appropriate action when its stop-callback is invoked with an errorstatus
test("Run stop on error", (done) => {
    setUpCommonMocks();
    let stopcallback : (error? : boolean) => void;
    let onStopListenerCallback = PostHTTPInterface.prototype.addOnStopListener = jest.fn((...args: any[]) => {stopcallback = args[0];});
    let startAllMock = ListenerManager.prototype.startAll = jest.fn();
    let stopAllMock = ListenerManager.prototype.stopAll = jest.fn();
    bgscript = new BackgroundScript();
    bgscript.run();
    new Promise((resolve) => {
        setTimeout(() => {
            expect(connectionMock).toBeCalledTimes(1);
            expect(waitStartMock).toBeCalledTimes(1);
            expect(configMock).toBeCalledTimes(1);
            expect(onStopListenerCallback).toBeCalledTimes(1);
            expect(startAllMock).toBeCalledTimes(1);
            expect(stopAllMock).toBeCalledTimes(0);
            expect(sendDataMock).toBeCalledTimes(0);
            expect(stopcallback).not.toBeUndefined();
            jest.useFakeTimers();
            stopcallback(true);
            resolve();
        }, WAITTIME);
    })
    .then(() => {
        jest.runOnlyPendingTimers();
        jest.useRealTimers();
        setTimeout(() => {
            expect(connectionMock).toBeCalledTimes(2);
            expect(configMock).toBeCalledTimes(2);
            expect(waitStartMock).toBeCalledTimes(2);
            expect(sendDataMock).toBeCalledTimes(0);
            expect(stopAllMock).toBeCalledTimes(1);
            done();
        }, WAITTIME);
    })
})

function setUpCommonMocks() {
    connectionMock = PostHTTPInterface.prototype.establishConnection = jest.fn().mockImplementation(() => new Promise((resolve) => {
        resolve();
    }));
    configMock = PostHTTPInterface.prototype.requestConfig = jest.fn(() => new Promise<string>((resolve) => {
        resolve("Some Config");
    }));
    waitStartMock = PostHTTPInterface.prototype.waitForStart = jest.fn(() => new Promise((resolve) => {
        resolve();
    }));
    sendDataMock = PostHTTPInterface.prototype.sendData = jest.fn(() => new Promise((resolve) => {
        resolve();
    }));
}