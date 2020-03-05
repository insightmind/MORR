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

test("StartAll", () => {
    manager.startAll();
    expect(startmock).toHaveBeenCalledTimes(1);
    expect(stopmock).toHaveBeenCalledTimes(0);
})

test("Constructor", () => {
    manager = new ListenerManager((event : BrowserEvent) => {return;});
})

test("StopAll", () => {
    manager.startAll();
    manager.stopAll();
    expect(startmock).toHaveBeenCalledTimes(1);
    expect(stopmock).toHaveBeenCalledTimes(1);
})

test("StartAll (already started)", () => {
    manager.startAll();
    manager.startAll();
    expect(startmock).toHaveBeenCalledTimes(1);
})

test("StopAll (already stopped)", () => {
    manager.stopAll();
    expect(startmock).toHaveBeenCalledTimes(0);
})