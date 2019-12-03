/**
 * The interface to the MORR main application
 */
export default interface IApplicationInterface {
    /**
     * Establish a connection with the main application.
     * Implementations may retry indefinetly until a connection could be established automatically or return an error upon failure.
     * @param onSuccess Callback function, invoked if connection was established
     * @param onFail Callback function, invoked upon errors not handeled by the implementation.
     */
    establishConnection(onSuccess : (response? : string) => void, onFail : (response? : string) => void) : void;

    /**
     * Request the configuration from the MORR main application.
     * @param onSuccess Callback function, invoked if request succeeds
     * @param onFail Callback function, invoked if request fails
     */
    requestConfig(onSuccess : (response? : string) => void, onFail : (response? : string) => void) : void;

    /**
     * Wait for the start signal coming from the from the MORR main application.
     * @param onSuccess Callback function, invoked if request succeeds
     * @param onFail Callback function, invoked if request fails
     */
    waitForStart(onStart : (response? : string) => void, onFail : (response? : string) => void) : void;

    /**
     * Send serialized (event-) data to the MORR main application.
     * @param data The serialized data to send
     * @param onSuccess Callback function, invoked if sending succeeds
     * @param onFail Callback function, invoked if sending fails
     */
    sendData(data : string, onSuccess : (response? : string) => void, onFail : (response? : string) => void) : void;
}