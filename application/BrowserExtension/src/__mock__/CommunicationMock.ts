import { ICommunicationStrategy } from '../ApplicationInterface'

/**
 * Mock for the ICommunicationStrategy.
 * 
 * Instead of actually establishing and using a connection, this class will just print appropriate messages to console.
 * 
 * All calls should succeed after a short time.
 */
export default class CommunicationMock implements ICommunicationStrategy {
    private readonly CONNECTIONDELAYMS : number = 3000; //artificial delay for establishing a connection
    private readonly SENDDELAYMS : number = 50; // artificial delay for send-calls
    private readonly STARTDELAYMS : number = 2000; //artificial delay until start-signal is received
    private _onStopCallback: () => void = () => {};
    establishConnection(): Promise<void> {
        console.log("CommMock: Attempting to establish connection.")
        return new Promise((resolve, reject) => {
            setTimeout(() => {console.log("CommMock: Connection established."); resolve();}, this.CONNECTIONDELAYMS);
        });
    }
    requestConfig(): Promise<string> {
        return new Promise((resolve, reject) => {
            setTimeout(() => {console.log("CommMock: Config received."); resolve('{"enabled":"true", "mock":"true"}')}, this.SENDDELAYMS);
        });
    }
    waitForStart(): Promise<void> {
        return new Promise((resolve, reject) => {
            setTimeout(() => {console.log("CommMock: Start signal received."); resolve()}, this.STARTDELAYMS);
        });
    }
    sendData(data: string): Promise<string> {
        return new Promise((resolve, reject) => {
            setTimeout(() => {console.log(`CommMock: Sent data: ${data}`); resolve()}, this.SENDDELAYMS);
        });
    }

    public addOnStopListener(callback: () => void) : void {
        this._onStopCallback = callback;
    }
}