# pfSenseBackup

> Notice: Through the years I have enjoyed building and maintaining this code. I have been surprised to see how many people started using this plugin and how often around the globe I met people that asked me if I was the person behind the plugin. Time however is scarse and as much as I would like to, I'm having too many projects to work on in my spare time, so I need to get rid of some, this unfortunately being one. I will therefore no longer maintain this code. Feel free to fork off of it and make your own version out of it. I'll be happy to link to it.

[![Continuous Integration Build](https://github.com/KoenZomers/pfSenseBackup/actions/workflows/ci.yaml/badge.svg)](https://github.com/KoenZomerspfSenseBackup/actions/workflows/ci.yaml) ![](https://img.shields.io/github/downloads/koenzomers/pfSenseBackup/total.svg) ![](https://img.shields.io/github/issues/koenzomers/pfSenseBackup.svg) [![MIT license](https://img.shields.io/badge/License-MIT-blue.svg)](https://lbesson.mit-license.org/) [![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com)

pfSense Backup allows you to backup the complete configuration of your pfSense or OPNSense server using this command line application. It is easy to include this in a larger script for your backups and schedule it i.e. with the Windows Task Scheduler or in an Azure Function. It supports pfSense installations running at least pfSense version 1.2 up to the most recent pfSense version including pfSense Plus and OPNSense 19.7. It can be ran on a Windows machine, Mac, Linux and ARM devices such as the Raspberry Pi.

## Download

[Download the latest version](../../releases/latest)

## Release Notes

2.7.0 - released September 3, 2023 [download](https://github.com/KoenZomers/pfSenseBackup/releases/tag/2.7.0)

- Compiled against .NET 6
- Added option for including "Backup extra data" in the backup file
- Added Windows 32 bit version
- Added OSX ARM64 version

2.6.0 - released February 21, 2021 [download](https://github.com/KoenZomers/pfSenseBackup/releases/tag/2.6.0)

- Added support for pfSense v2.5.0
- Upgraded to .NET Core 3.1

[Version History](https://github.com/KoenZomers/pfSenseBackup/blob/master/VersionHistory.md)

## System Requirements

This tool is self contained and does not have any dependencies on any frameworks. Under [releases](../../releases/latest) you will find compiled versions for Windows, Linux, iOS both on x64 machines and on ARM devices.

## Usage Instructions on Windows

1. Copy the downloaded .exe from releases to any location on a Windows machine where you want to use the tool
2. Run the downloaded .exe to see the command line options
3. Run the downloaded .exe with the appropriate command line options to connect to your pfSense or OPNSense server and download the backup

![](./Documentation/Images/Help.png)

![](./Documentation/Images/SampleExecution.png)

## Feedback

Any kind of feedback is welcome! Feel free to drop me an e-mail at koen@zomers.eu or [create an issue](https://github.com/KoenZomers/pfSenseBackup/issues)
