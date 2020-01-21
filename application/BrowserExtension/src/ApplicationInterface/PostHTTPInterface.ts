import ICommunicationStrategy from './ICommunicationStrategy';
const URLPrefix = "http://localhost:";
const portRegex = new RegExp("^([1-9][0-9]{0,4})(\/\\w+)*$");
chrome.runtime.onInstalled.addListener((details : chrome.runtime.InstalledDetails) => {
    if (details.reason == "install") {
        chrome.storage.local.set({"port": "No Port Set!"});
    }
});
/**
 * Application Interface using the HTTP-POST. Expects a HTTPListener on the main application side.
 */
export default class PostHTTPInterface implements ICommunicationStrategy {
    private _onStopCallback: (error? : boolean) => void = () => {};
    /**
     * URL of the HTTPListener attached to the main application.
     */
    private listenerURL : string;
    private gotURL : Promise<void>; //resolve when URL is set
    constructor(url?: string) {
        if (url) {
            this.listenerURL = url;
            this.gotURL = new Promise((resolve) => {
                resolve();
            });
        }

        else {
            this.listenerURL = "undefined";
            this.gotURL = new Promise((resolve) => {
                chrome.storage.local.get(['port'], (result) => {
                    this.listenerURL = URLPrefix + result.port;
                    resolve();
                });
            })
        }
        chrome.storage.onChanged.addListener((changes : any, areaname : string) => {
            if (areaname == "local" && changes.port.newValue && portRegex.test(changes.port.newValue)) {
                let port = Number(portRegex.exec(changes.port.newValue)![1]);
                if (port >= 80 && port <= 65536) {
                    this.listenerURL = URLPrefix + changes.port.newValue;
                    console.log("Set new URI: " + this.listenerURL);
                    return;
                }
            }
            console.error("Post HTTPListener Received invalid URL prefix, ignoring");
        });
    }
    /**
     * Establishes connection to the MORR main application
     * @returns A Promise which will be resolved when the connection is estab-
lished successfully or rejected upon connection failure or unexpected response
     */
    public establishConnection(): Promise<void> {
        return new Promise ((resolve, reject) => {
            this.gotURL
            .then(() => {
                fetch(this.listenerURL, {
                    method : "POST",
                    body: JSON.stringify({request : "connect"}),
                }).then(
                    response => response.json()
                ).then((response) => {
                    if (response.application == "MORR" && response.response == "Ok") {
                        console.log("Connection established");
                        resolve();
                    } else {
                        throw("Unexpected Answer");
                    }
                }).catch((e) => {
                    reject(e);
                });
            })
        });
    }

    /**
     * Asynchronously request a configuration string from the MORR application.
     * @returns A Promise which will be resolved and filled with the configuration string
     * as soon as the configuration string is received or rejected upon connection failure
     * or unexpected response.
     */
    public requestConfig(): Promise<string> {
        return new Promise ((resolve, reject) => {
            fetch(this.listenerURL, {
                method : "POST",
                body: JSON.stringify({request : "config"}),

            }).then(
                response => response.json()
            ).then((response) => {
                if (response.application == "MORR" && response.config) {
                    console.log("Got config");
                    resolve(<string>response.config);
                } else {
                    throw("Unexpected answer");
                }
            }).catch((e) => {
                console.error(`POSTHTTPInterface error (conf): ${e}`);
                reject(e);
            })
        });
    }

    /**
     * Asynchronously await a start signal from the MORR application.
     * @returns A Promise which will be resolved when the start signal is received
     * or rejected upon connection failure or timeout.
     */
    public waitForStart(): Promise<void> {
        return new Promise ((resolve, reject) => {
            fetch(this.listenerURL, {
                method : "POST",
                body: JSON.stringify({request : "start"}),
            }).then(
                response => response.json()
            ).then((response) => {
                if (response.application == "MORR" && response.response == "Start") {
                    this.enqueueStop();
                    resolve();
                } else {
                    throw("Unexpected answer");
                }
            }).catch((e) => {
                console.error(`POSTHTTPInterface error (waitstart): ${e}`);
                reject(e);
            })
        });
    }

    /**
     * Asynchronously send data to the MORR application.
     * @param data the JSON-data to be sent.
     * @returns A Promise which will be resolved when the data is sent successfully
     * or rejected upon connection failure or unexpected response.
     */
    public sendData(data: string): Promise<string> {
        return new Promise ((resolve, reject) => {
            fetch(this.listenerURL, {
                method : "POST",
                body: `{"request" : "sendData", "data" : ${data}}`,
            }).then(
                response => response.json()
            ).then((response) => {
                if (response.application == "MORR" && response.response == "Ok") {
                    resolve("Ok");
                } else if (response.application == "MORR" && response.response == "Stop") {
                    this._onStopCallback(false);
                    resolve("Stop");
                } else {
                    throw("Unexpected answer");
                }
            }).catch((e) => {
                console.error(`POSTHTTPInterface error (send): ${e}`);
                reject(e);
            })
        });
    }

    public addOnStopListener(callback: (error? : boolean) => void) : void {
        this._onStopCallback = callback;
    }

    private enqueueStop = () : void => {
        fetch(this.listenerURL, {
            method : "POST",
            body: JSON.stringify({request : "waitStop"}),
        })
        .then(
            response => response.json()
        )
        .then((response) => {
            if (response.application == "MORR" && response.response == "Stop") {
                this._onStopCallback(false);
            } else {
                throw("Unexpected answer");
            }
        })
        .catch((e) => {
            this._onStopCallback(true);
        });
    }
}
