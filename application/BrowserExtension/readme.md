# Requirements and first-time-setup
In order to build and test the browser extension, you need **NodeJS** and **npm** (usually comes bundled with NodeJS). You can install those standalone, or install the **NodeJS Development** component for Visual Studio.

## Aquiring the dependencies
To install all dependencies, simply run `npm install`. This command should also be run everytime the dependencies change (reflected by changes to the file _./package-lock.json_).

# Building
To build the browser extension, two commands are available:
* `npm run build` will build a release-configuration of the extension. This means all comments will be stripped from the files, and the resulting javascript-code will be minified. The target directory is _./dist_
* `npm run dbuild` will build a debug-configuration of the extension. This will preserve the code-structure and comments. Use this build to test the browserextension in your browser, as it is much easier to match the javascript code to the typescript source, should errors occur. The target directory is _./debug_

# Testing
* To run all tests, execute `npm run test`. If you want to execute a specific testsuite only, you can append the filename to the command, e.g.
`npm run test src/__tests__/Shared/BrowserEvent.tests.ts`
* To also run testcoverage, execute `npm run testcoverage`. This will print out a testcoverage report on the commandline and create an _index.html_ file in the _./coverage/lcov-report/_ directory, which can be viewed with any browser.

# Loading as temporary extension
To test the extension before packing and distributing it via chrome webstore and addons.mozilla, you want to load the unpacked extension as temporary extension in the respective browser. Both browsers support reloading the extension with one click to test changes made on-the-fly.

### Mozilla Firefox
To load the unpacked extension in Firefox, navigate to **about:debugging**, and, if applicable, click on **This Firefox** (might not be necessary depending on your version). Then, choose the option **Load Temporary Add-on**, and then choose any file in the webextensions' _./dist_ or _./debug_ directory (e.g. the _manifest.json_).
The extension is now loaded. To view the console-output and errors of the backgroundscript, click on **Inspect** or **Debug** (again, depending on your Firefox version.). The console-output and errors of the contentscript are shown in the regular browser console (**F12** -> **Console**). You can reload/remove the extension anytime from the **about:debugging** site. It will also automatically be removed when you close the browser.

### Google Chrome
To load the unpacked extension in Google Chrome, navigate to **chrome://extensions** and activate **Developer mode** (top right corner). Next, click on **Load unpacked** and choose the _./dist_ or _./debug_ folder (the whole folder, not a file inside it).
The extension is now loaded. To view the console-output and errors of the backgroundscript, click on **background page**. The console-output and errors of the contentscript are shown in the regular browser console (**F12** -> **Console**). You can reload/remove the extension anytime from the **chrome://extensions** site.

### Setting the communication port
In order for the browser extension to be able to communicate with the MORR main application, a port (and optionally a directory) have to be set in the browser extension's options/preferences. In chrome those can be excessed on **chrome://extensions -> Details -> Extension Options**, or by right-clicking on the MORR extension icon next to the address bar and choosing **Options**. In Mozilla Firefox, navigate to **about:addons** instead and choose **Preferences** next to the MORR extension. Examples for valid port settings are _60024_ and _60024/johndoe_. Internally, the address is then set as _http://localhost:\<port\>_.

**Note**: The browser extension will always attempt communication with the MORR instance listening on the port number (60024 in the above example), regardless of directory suffix. If the directory does not match the directory set in the MORR application however, the connection will be refused and thus events not sent.

# Setup files
A short explanation of the most important (non-source) files:
* _jest.config.js_: Configuration of the Test-Framework (Jest)
* _package.json_: Project manifest. Configuration of build-scripts, dependencies and various metadata.
* _package-lock.json_: Auto-generated. Exact versions of each depenency to be installed when running `npm install`
* _tsconfig.json_: Typescript configuration. Mainly affects which warnings/errors should be produced when building.
* _materials/manifest.json_: Browser extension manifest. Contains metadata used by the browser for setup and display of the extension.