import * as jquery from 'jquery';
import IApplicationInterface from './IApplicationInterface';
export default class PostHTTPInterface implements IApplicationInterface {
    private listenerURL : string;
    constructor(url: string) {
        this.listenerURL = url;
    }
    public requestConfig(onSuccess : (response? : string) => void, onFail : (response? : string) => void) : void {
        let postData = { 
            "action": "requestConfig" 
        };
        console.log("Posting getConfig request: ");
        jquery.post( this.listenerURL, postData, function(response : any){
            console.log("Got response: " + response);
        }).done(function (data) {
            console.log("Receive config succeeded.");
            onSuccess();
        }).fail(function (jqXHR : JQuery.jqXHR<any>, textStatus : JQuery.Ajax.ErrorTextStatus, errorThrown : string) {
            if (errorThrown)
                console.log(errorThrown);
            onFail();
        });
        console.log("Sent getConfig request: ");
    }

    public waitForStart = (onStart : (response? : string) => void, onFail : (response? : string) => void) => {
        let postData = { 
            "action": "waitForStart" 
        };
        let appResponse : string;
        let recallFunc = this.waitForStart; //get Handle on this function as this will be shadowed inside the post function
        console.log("Posting waitForStart request: ");
        jquery.post( this.listenerURL, postData, function(response : any){
            console.log("Got response: " + response);
            appResponse = response;
        }).done(function (data) {
            if (appResponse == "start" || appResponse) { //for now we don't care about the response
                console.log("Received start signal.");
                onStart();
            } else {
                console.log("Received response which was not start signal: " + appResponse);
                onFail(appResponse);
            }
        }).fail(function (jqXHR : JQuery.jqXHR<any>, textStatus : JQuery.Ajax.ErrorTextStatus, errorThrown : string) {
            if (textStatus.toString() == "error") {
                onFail();
            } else if (textStatus.toString() == "timeout") {
                setTimeout(recallFunc, 5000);
            }
        });
        console.log("Sent waitForStart request: ");
    }
    
    public sendData(data : string, onSuccess : (response? : string) => void, onFail : (response? : string) => void) : void{
        let postData = {
            "action" : "sendEvent",
            "data": data
        };
        console.log("Sending post data");
        jquery.post( this.listenerURL, postData, function(response : any){
            console.log("Got response: " + response);
        }).done(function (data) {
            console.log("Completed data send.");
            onSuccess();
        }).fail(function (jqXHR : JQuery.jqXHR<any>, textStatus : JQuery.Ajax.ErrorTextStatus, errorThrown : string) {
            console.log(errorThrown);
            onFail();
        });
        console.log("Sent post data");
    }
    
}
