import { chrome, resetChrome } from '../__mock__/chrome_mock'
const globalAny:any = global;
globalAny.chrome = chrome
import PostHTTPInterface from '../ApplicationInterface/PostHTTPInterface'

let comm : PostHTTPInterface;


beforeAll(() => {
    jest.resetAllMocks();
    comm = new PostHTTPInterface("http://localhost:60024");
})

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

test("Establish connection (wrong response)", done => {
    class Response {
        json() {
            return {application: "Internet Explorer", response: "Ok"}; //wrong application identifier
        } 
    }
    let fetch = (...args : any[]) => new Promise((resolve) => resolve(new Response));
    globalAny.fetch = fetch;
    comm.establishConnection().then(() => fail("Connection should not be accepted for invalid application")).catch(() => done());
})