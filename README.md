# pfSenseBackup
pfSense Backup allows you to backup the complete configuration of your pfSense server using this command line Windows application. It is easy to include this in a larger script for your backups and schedule it i.e. with the Windows Task Scheduler.

## Download

[Download the latest version](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv2.4.zip)

## Release Notes

2.4 - released April 5, 2016 - [download](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv2.4.zip) - 10 kb

- Added support for pfSense version 3.2 RC. Provide the -v 3.2 flag to instruct pfSenseBackup to use the protocol added for pfSense v3.2
- pfSenseBackup now requires the Microsoft .NET framework v4.6 to be installed (was 4.0 before) as pfSense v3.2 requires TLS 1.2 for SSL. Prior versions of the .NET Framework only support TLS 1.0. This requirement is regardless of which pfSense version you want to target.

[Version History](https://github.com/KoenZomers/pfSenseBackup/blob/master/VersionHistory.md)

## Usage Instructions

1. Copy pfSenseBackup.exe to any location on a Windows machine where you want to use the tool
2. Run pfSenseBackup.exe to see the command line options
3. Run pfSenseBackup.exe with the appropriate command line options to connect to your pfSense server and download the backup

![](./Documentation/Images/Help.png)

![](./Documentation/Images/SampleExecution.png)

## Feedback

Any kind of feedback is welcome! Feel free to drop me an e-mail at mail@koenzomers.nl