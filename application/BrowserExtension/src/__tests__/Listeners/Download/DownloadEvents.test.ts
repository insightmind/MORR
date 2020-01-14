import { DownloadEvent } from '../../../Listeners/Download/DownloadEvents'
import { EventType } from '../../../Shared/SharedDeclarations'
const sample = {_issuingModule : 0, _type : EventType.Download, _windowID : 3, _tabID : 7,  _url : new URL("http://sample.com"),
                _fileURL : new URL("http://sample.com/download/file2.png"), _mimeType : "PNG"};


let event : DownloadEvent;

beforeEach(() => {
    event = new DownloadEvent(sample._tabID, sample._windowID, sample._mimeType, sample._fileURL.href, sample._url.href);
});
/** 
 * Test if the DownloadEvent constructor correctly assigns the passed value to its fields.
*/
test("DownloadEvent Constructor", () => {
    expect(event).toMatchObject(sample);
    expect(event.fileURL.href).toEqual(sample._fileURL.href);
    expect(event.url.href).toEqual(sample._url.href);
})

test("DownloadEvent invalid fileURL", () => {
    expect(() => {new DownloadEvent(sample._tabID, sample._windowID, sample._mimeType, "samplecom", sample._url.href)}).toThrow("Invalid URL");
})

/** 
 * Test if the getters specific to DownloadEvent return the desired values.
*/
test("DownloadEvent Getters", () => {
    expect(event.mimeType).toEqual(sample._mimeType);
    expect(event.fileURL.href).toEqual(sample._fileURL.href);
})

/** 
 * Test if the setters specific to DownloadEvent set the desired values
*/
test("DownloadEvent Setters", () => {
    event.fileURL = new URL("http://alt.sample.com/download/file.bmp")
    expect(event.fileURL.href).toBe("http://alt.sample.com/download/file.bmp");
    event.mimeType = "BMP";
    expect(event.mimeType).toBe("BMP");
})