/**
 * The interface to the MORR main application
 */
export default interface ICommunicationStrategy {
    /**
     * Asynchronously establish a connection with the main application.
     * Implementations may retry indefinetly until a connection could be established automatically or return an error upon failure.
     * @returns a Promise which will be resolved when a connection was established
     */
    establishConnection() : Promise<void>;

    /**
     * Asynchronously request the configuration from the MORR main application.
     * @returns a Promise which will be filled with the config string when resolved
     */
    requestConfig() : Promise<string>;

    /**
     * Asynchronously wait for the start signal coming from the MORR main application.
     * @returns a Promise which will be resolved when a start-signal is received
     */
    waitForStart() : Promise<void>;

    /**
     * Asynchronously send serialized (event-) data to the MORR main application.
     * @param data The serialized data to send
     * @returns a Promise which fill be resolved when the data was successfully sent
     */
    sendData(data : string) : Promise<string>;

    addOnStopListener(callback: () => void) : void;
}