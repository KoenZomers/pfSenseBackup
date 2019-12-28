# pfSenseBackup
pfSense Backup allows you to backup the complete configuration of your pfSense or OPNSense server using this command line application. It is easy to include this in a larger script for your backups and schedule it i.e. with the Windows Task Scheduler or in an Azure Function v2. It supports pfSense installations running at least pfSense version 1.2 up to the most recent pfSense version 2.4.4 and OPNSense 19.7. It can be ran on a Windows machine, Mac, Linux and ARM devices such as the Raspberry Pi.

## Download

[Download the latest version](../../releases/latest)

## Release Notes

2.5 - released December 21, 2019 - [download](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv2.5.zip) - 30 Mb

- Added support for connecting to OPNSense
- Migration to .NET Core - builds are multiplaform (Windows, Linux, Mac) and self-contained (no need to install any framework at the cost of a huge increase in file size)
  Big thanks to [mnaiman](https://github.com/mnaiman) for providing this update through [PR #6](https://github.com/KoenZomers/pfSenseBackup/pull/6)

2.4.3 - released July 3, 2017 - [download](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv2.4.3.zip) - 10 kb

- Added support for connecting to pfSense servers using TLS v1.1 and 1.2. Thanks to Yannick Molinet for pointing this out.

2.4.2 - released February 22, 2017 - [download](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv2.4.2.zip) - 10 kb

- There was a minor modification to the backup page in pfSense 2.3.3. Added support for 2.3.3 and made it the default version. So if you're on 2.3.3 you don't need to provide the -v flag. If you're still on 2.3 you need to provide the -v 2.3 still.

2.4.1 - released April 6, 2016 - [download](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv2.4.1.zip) - 10 kb

- Incorrectly referenced the update to being for pfSense 3.2 whereas it should have been 2.3. Changed it in this minor update. Be sure to use the -v 2.3 flag.

2.4 - released April 5, 2016 - [download](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv2.4.zip) - 10 kb

- Added support for pfSense version 2.3 RC. Provide the -v 2.3 flag to instruct pfSenseBackup to use the protocol added for pfSense v2.3
- pfSenseBackup now requires the Microsoft .NET framework v4.6 to be installed (was 4.0 before) as pfSense v2.3 requires TLS 1.2 for SSL. Prior versions of the .NET Framework only support TLS 1.0. This requirement is regardless of which pfSense version you want to target.

[Version History](https://github.com/KoenZomers/pfSenseBackup/blob/master/VersionHistory.md)

## System Requirements

This tool is self contained and does not have any dependencies on any frameworks. Under [releases](../../releases/latest) you will find compiled versions for Windows, Linux, iOS both on x64 machines and on ARM devices.

## Usage Instructions

1. Copy pfSenseBackup.exe to any location on a Windows machine where you want to use the tool
2. Run pfSenseBackup.exe to see the command line options
3. Run pfSenseBackup.exe with the appropriate command line options to connect to your pfSense or OPNSense server and download the backup

![](./Documentation/Images/Help.png)

![](./Documentation/Images/SampleExecution.png)

## Feedback

Any kind of feedback is welcome! Feel free to drop me an e-mail at koen@zomers.eu or [create an issue](https://github.com/KoenZomers/pfSenseBackup/issues)
