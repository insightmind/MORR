export default interface IApplicationInterface {
    requestConfig(onSuccess : (response? : string) => void, onFail : (response? : string) => void) : void;
    waitForStart(onStart : (response? : string) => void, onFail : (response? : string) => void) : void;
    sendData(data : string, onSuccess : (response? : string) => void, onFail : (response? : string) => void) : void;
}