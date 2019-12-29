export const chrome = require('sinon-chrome');
export function resetChrome() {
    chrome.flush();
    chrome.reset();
}