# MORR - Medical Online Research Recorder

![Build + Test](https://github.com/insightmind/MORR/workflows/Build%20+%20Test/badge.svg)

## The project

MORR is an application which collects information about user behavior during online research, particularly in the medical field. The application enables recording of user interactions, such as mouse, keyboard and web browser events. The recorded data is stored and can later viewed and worked with, for example for gaining insights on research behavior by using them as training data for machine learning.

## Target platform

### Operating System
- Windows 10 64bit (1809 and later)

### Supported Browsers
- Google Chrome (version 57 and later)
- Mozilla Firefox (version 57 and later)

### Development tools
- The main application was developed in C# using **Microsoft Visual Studio** and the .NET Core SDK. It also contains a few components written in C++.
- The application supports extension by adding additional modules managed with **MEF** (Managed Extensibility Framework)
- The browser extension is written in TypeScript and built with **npm** (NodeJS)

## Wiki
The [Wiki](https://github.com/insightmind/MORR/wiki) contains instructions on how to install and use this software as well as guides for developers who wish to contribute to and extend the software.