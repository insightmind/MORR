/**
 * The interface to the MORR main application
 */
export default interface IApplicationInterface {
    /**
     * Request the configuration from the MORR main application.
     * @param onSuccess callback function, invoked if request succeeds
     * @param onFail callback function, invoked if request fails
     */
    requestConfig(onSuccess : (response? : string) => void, onFail : (response? : string) => void) : void;

    /**
     * Wait for the start signal coming from the from the MORR main application.
     * @param onSuccess callback function, invoked if request succeeds
     * @param onFail callback function, invoked if request fails
     */
    waitForStart(onStart : (response? : string) => void, onFail : (response? : string) => void) : void;

    /**
     * Send serialized (event-) data to the MORR main application.
     * @param data the serialized data
     * @param onSuccess callback function, invoked if sending succeeds
     * @param onFail callback function, invoked if sending fails
     */
    sendData(data : string, onSuccess : (response? : string) => void, onFail : (response? : string) => void) : void;
}