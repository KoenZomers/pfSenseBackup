# pfSenseBackup

## Version History

2.2 - released August 11, 2015 - [download](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv2.2.zip) - 10 kb

- Added option -e to have pfSense encrypt the backup as requested by Michal Naiman

2.1 - released June 28, 2015 - [download](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv2.1.zip) - 10 kb

- Usernames and passwords sent to pfSense will now first be UrlEncoded. This allows usernames and passwords to be used with special characters like the + sign. Thanks to Marc Mittelman for pointing out this issue!
- Retargeted the .NET Framework to v4.0 as v2.0 to which it was set before isn't shipped by default anymore on modern Windows client & server operating systems

2.0 - released May 20, 2015 - [download](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv2.0.zip) - 9 kb

- Changed the way parameters are parsed so that it now also is possible to use the -o parameter and specify a location which contains spaces. Use double quotes to wrap the location in case it contains a space. I.e. -o "c:\my backups" or -o "c:\backups\my backup file.xml" or you may omit the quotes if you don't have any spaces -o c:\backups or -o c:\backups\mybackup.xml or -o "c:\backups\mybackup.xml". Thanks to David Zachhuber for pointing out this issue!

1.9 - released February 20, 2015 - [download](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv1.9.zip) - 9 kb

- Added support for pfSense version 2.2
- Added the option to use the -o parameter to specify an existing folder. The backup file will be stored inside that folder using the filename as provided by pfSense, which is in the format config-<servername>-<year><month><day><hour><minute><second>.xml. It is still possible to use the -o parameter to provide a full path including filename as well.

1.8 - released November 2, 2013 - [download](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv1.8.zip) - 9 kb

- Bugfix: when not using the -o switch, it would crash (thanks to Mauro Bornet for pointing this out)
- Added extra check to make sure the user account under which the tool is run has write rights to wherever the backup contents are to be written to

1.7 - released August 27, 2013 - [download](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv1.7.zip) - 12 kb

- Added error handling on illegal characters in the output filename (based on feedback from Dan Peterson)

1.6 - released August 11, 2013 - [download](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv1.6.zip) - 9 kb

- Added the -silent parameter to avoid output for usage in batch scripts (based on request of Anthony Cassinelli)

1.5 - released May 8, 2013 - [download](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv1.5.zip) - 12 kb

-     Fixed the -o parameter to specify a location for the backup file which got broken in v1.4

1.4 - released May 8, 2013 - [download](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv1.4.zip) - 12 kb

- Support has been added for pfSense version 2.1 released on or after May 2013. This now includes an anti cross site scripting security token in the login page which caused the application to have to be rewritten.
- Default is now version 2.1 of the protocol which expects the security token in the login page. If you want to backup a pfSense 2.x version released before May 2013, please specify -v 2.0 as a command line argument.
- Application code has been rewritten to be cleaner and provide support for the different protocols used to communicate with the different pfSense versions

1.3 - released February 28, 2012 - [download](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv1.3.zip) - 12 kb

- Support has been added for pfSense v1.x by Athanasios Oikonomou. Thanks for contributing!

1.2 - released January 26, 2012 - [download](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv1.2.zip) - 12 kb

- Added code to ignore all SSL related issues when using the -useSSL option

1.1 - released January 25, 2012 - [download](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv1.1.zip) - 7 kb

- Added -useSSL option to have the tool use HTTPS to connect to pfSense instead of HTTP
- Clarified with a sample that a port number can be added to the -s server directive

1.0 - released January 23, 2012 - [download](https://github.com/KoenZomers/pfSenseBackup/raw/master/Releases/pfSenseBackupv1.0.zip) - 6 kb

- Initial version