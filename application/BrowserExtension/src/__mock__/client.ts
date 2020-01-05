/* Mock DOM
 * Only works if jest option "testEnvironment" is not "jsdom"
 * otherwise global.document and global.window would be readonly
 * with default values
 */
const jsdom = require("jsdom");
const { JSDOM } = jsdom;
export const sampleDocument = {
    url: "http://sample.com/",
    referrer: "https://example.com/",
    contentType: "text/html",
    includeNodeLocations: true,
    storageQuota: 10000000
};
const dom = new JSDOM(``, sampleDocument);
const globalAny:any = global;
globalAny.document = dom.window.document
globalAny.window = dom.window