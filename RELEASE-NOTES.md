Version `7.1.0` 
==========================
 **06/29/2018**

### Improvements
* Added IPv6 support for validations..

Version `7.0.2` 
==========================
 **02/20/2018**

### Improvements
* New feature, added configuration class. Allow setting of the Configuration when creating Inquiry and Update.

Version `7.0.1` 
==========================
 **01/11/2018**

### Improvements
* check for valid data (not null).


Version `7.0.0` 
==========================
 **09/12/2017**

### Improvements
* Introducing configuration key
* Removed the SALT phrase

Version `6.5.2` 
==========================
 **08/08/2017**

### Improvements
* Remove parameters `CCYY`(*ExpirationYear*) and `CCMM`(*ExpirationMonth*) from the SDK.

Version `6.5.1` 
==========================
 **06/12/2017**

### New Features
* More payment types are added: `Apple Pay, BPAY, Carte Bleue, ELV, Green Dot MoneyPak, GiroPay, Interac, Mercade Pago, Neteller, Single Euro Payments, Poli, Skrill/Moneybookers, Sofort, Token`. 

### Improvements
* Added new `enums` definitions: `InquiryTypes`, `UpdateTypes`
* Nuget package available for downloads.
* Excluding development dependencies of `docfx.console` in `Nuget package`.

Version `6.5.0`
==========================
**05/29/2017**

### New Features
* SALT phrase configurable as a app setting(key/value) in `app.config`.
    Set `Ris.Khash.Salt` in your `app.config` file for this to work.

### Improvements
* Update `docfx.console` to version *2.16.8*

Version `6.4.2` 
==========================
 **04/06/2017**

### Improvements
* Minor improvements for integration tests logging

### Bugfixes
* Fixed build issue with docFX (*ver 2.16.2*) and Visual Studio install Path   

Version `6.4.0` 
==========================
**03/30/2017**

### Improvements
* Secure communication between client and server now using **TLS v1.2**
* Added `Power Shell` scripts for easier compilation, build, unit and integration tests, .net documentation generation, and packaging
* General source code improvements and modernization
* Using `DocFX` documentation generation tool (*ver. 2.15.5*) for API reference. 
* General .net framework enhancements to 4.5 (*.NET framework 4.5* or later is recommended).

Version `6.3.0`
==========================
**02/24/2015**

### New Features
* Added support for API key authentication. Client certificate validation is still supported,
    but is now deprecated. Set `Ris.API.Key` in your `app.config` file for this to work.
* DEPRECATED: EPTOK IS NO LONGER AVAILABLE 

Version `6.0.0` 
==========================
**08/01/2014**

### New Features
* Added support for new `Kount Central` RIS query modes 'J' and 'W'.
