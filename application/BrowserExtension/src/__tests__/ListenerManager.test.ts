import { chrome, resetChrome } from '../__mock__/chrome_mock'
import ListenerManager from '../ListenerManager'
import TabListener from '../Listeners/Tab/TabListener'
import { BrowserEvent } from '../Shared/SharedDeclarations';

jest.mock('../Listeners/Tab/TabListener')

const WAITTIME = 30; //how long to wait before counting callback-invocations
const globalAny:any = global;
const startmock = TabListener.prototype.start = jest.fn();
const stopmock = TabListener.prototype.stop = jest.fn();
let manager : ListenerManager;

beforeAll(() => {
    globalAny.chrome = chrome
})

beforeEach(() => {
    manager = new ListenerManager((event : BrowserEvent) => {return;});
    jest.resetAllMocks();
})

//check if startall calls start on one of the listeners
test("StartAll", () => {
    manager.startAll();
    expect(startmock).toHaveBeenCalledTimes(1);
    expect(stopmock).toHaveBeenCalledTimes(0);
})

//check if stopall calls stop on one of the listeners
test("StopAll", () => {
    manager.startAll();
    manager.stopAll();
    expect(startmock).toHaveBeenCalledTimes(1);
    expect(stopmock).toHaveBeenCalledTimes(1);
})

//check if startall does not perform excessive start-calls when called multiple times
test("StartAll (already started)", () => {
    manager.startAll();
    manager.startAll();
    expect(startmock).toHaveBeenCalledTimes(1);
})

//check if stopall does not perform excessive stop-calls when called multiple times
test("StopAll (already stopped)", () => {
    manager.stopAll();
    expect(startmock).toHaveBeenCalledTimes(0);
})