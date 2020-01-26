# Usage Guide

## Configuration File

The configuration file is being used to define global and module specific options. Therefore a configuration file is required for any recording or processing session.

It is encoded using the JSON format and its file extension is `.morr`

--

### Example

```json
{
  "MORR.Core.Modules.GlobalModuleConfiguration": {
    "EnabledModules": [
      "MORR.Core.Data.Capture.Metadata.MetadataCapture",
      "MORR.Core.CLI.Demo.DemoProducer, CLI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
      "MORR.Modules.Keyboard.KeyboardModule, Keyboard.MORR-Module, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
      "MORR.Modules.Webbrowser.WebbrowserModule, Webbrowser.MORR-Module, Version=1.0.0.0, Culture=neutral"
    ]
  },
  "MORR.Core.Recording.RecordingConfiguration": {
    "Encoder": "MORR.Core.CLI.Output.OutputFormatter, CLI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
  }
}
```

This example should be sufficient for testing the current state of the application.

--

### GlobalModuleConfiguration

Identifier: 

```
MORR.Core.Modules.GlobalModuleConfiguration
```

The GlobalModuleConfiguration is responsible for module loading configuration. Therefore the current configuration only offers a single property `EnabledModules`.

Some identifiers need to specify the module it is located in as well as other properties such as `Version`, `Culture` and `PublicKeyToken`.

This is currently a limitation that will likely be removed in a future version of the application. However in this version please always use `<YOUR_MODULE>, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null`.

Currently available modules:

**MetadataCapture** 
*(required)*

Identifier: 

```
MORR.Core.Data.Capture.Metadata.MetadataCapture
```

MetadataCapture is an internal module in MORR and is used to capture all generated events and offer them to the encoder. Therefore this module is required for correct operation.

**DemoProducer**
*(optional)*

Identifier: 

```
MORR.Core.CLI.Demo.DemoProducer, CLI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
```

DemoProducer is an example producer, which produces an `DemoEvent` each 0.1 seconds. It is therefore only used for demonstration purposes.

**KeyboardModule**
*(optional)*

Identifier: 

```
MORR.Modules.Keyboard.KeyboardModule, Keyboard.MORR-Module, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
```

The KeyboardModule detects clicks on the keyboard and produces `KeyboardInteractEvents`. These are defined in the Functional Specifications Document in chapter 6.3.3.

**WebbrowserModule**
*(optional)*

Identifier: 

```
MORR.Modules.Webbrowser.WebbrowserModule, Webbrowser.MORR-Module, Version=1.0.0.0, Culture=neutral
```

The Webbrowser Module detects BrowserEvents offered by the Webextension. For more information on how to use the WebBrowser Extension. Please take a look at the upcoming chapter. The BrowserEvents are defined in the Functional Specification Document in chapter 6.3.5.

--

### RecordingConfiguration 

The RecordingConfiguration contains information about the encoder and decoder which should be used for recording or processing.

#### Encoder *(required)*

Identifier:

```
Encoder
```

You can use this property to define the encoder which should be used by the application for a recording session.

Currently available encoders are:

**OutputFormatter**
*(recommended)*

The output formatter allows the CLI to show the recorded events via the commandline. Therefore it is encouraged to use this encoder when recording using the commandline. However no persistent file or container is created.


#### Decoder *(optional)*

```
Decoder
```

Currently there are no decoders present in this version. Therefore please do not use this property.

## CommandLineInterface

The main usage of the MORR application is currently possible only via the CommandLineInterface(CLI).

You can start the CLI by doing the following steps:

1. Open `cmd.exe`
2. Navigate to the application folder: `~ cd <YOUR_PATH>`
3. Execute CLI: `~ CLI`

### Help

```
CLI help
```

Using this command the application will show you the list of all available commands and a help text for each.

### Version

```
CLI version
```

Using this command the application will show you the current version of the application.

### Validate

The validate command is used to validate the configuration file which is used for a recording or processing session.

```
CLI validate [-c|--config] [-v|--verbose]
```

##### Options

```-c|--config``` *(required)*: Defines the location of the configuration file, which should be validated.

```-v|--verbose``` *(optional)*: Enables verbose output of the application.


##### Example
```
CLI validate -c config.morr -v
```

This will validate the configuration file located at `/config.morr` and run using verbose output.

### Record

The record command is used to start a recording session with a given configuration.

```
CLI record [-c|--config] [-v|--verbose]
```

##### Options

```-c|--config``` *(required)*: Defines the location of the configuration file, which should be used for the recording session.

```-v|--verbose``` *(optional)*: Enables verbose output of the application.

##### Example
```
CLI record -c config.morr -v
```

This will start a recording session with the given configuration file located at `/config.morr` and using verbose output.

### Process

This command is currently not functional as no decoder is available.
The process command is used to start a processing session with a given configuration and input file.

```
CLI process [-c|--config] [-i|--inputFile]  [-v|--verbose]
```

##### Options

```-c|--config``` *(required)*: Defines the location of the configuration file, which should be used for the recording session.

```-i|--inputFile``` *(required)*: Defines the location of the input container or file in which metadata from a previous recording is located.

```-v|--verbose``` *(optional)*: Enables verbose output of the application.

##### Example
```
CLI process -c config.morr -i recording.mp4 -v
```

This will start a processing session with the given configuration file located at `/config.morr`, using the input mp4 file located at `/recording.mp4` and using verbose output.

## BrowserExtension



## User Application

The user application is currently not connected to the Core of MORR. However you can still launch the User Application by starting `UI.exe` in the MORR folder.

After the successful launch of the application you will find a tray icon in the taskbar of Windows.

### User Interactions

If you click the tray icon a popup will show up either to start a recording or stop the current recording. This however does not yet resolve in an actual start or stop of the recording as the UI is not connected to the main application.

If you right click the tray icon you will see multiple options.
You can select exit, to stop the application.