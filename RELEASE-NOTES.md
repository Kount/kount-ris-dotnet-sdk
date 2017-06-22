Version `6.5.1` changes
-------------------------
 06/12/2017

1. More payment types are added: `Apple Pay, BPAY, Carte Bleue, ELV, Green Dot MoneyPak, GiroPay, Interac, Mercade Pago, Neteller, Single Euro Payments, Poli, Skrill/Moneybookers, Sofort, Token`. 
2. Added new `enums` definitions: `InquiryTypes`, `UpdateTypes`
3. [[Nuget package available for downloads.|https://www.nuget.org/packages/Kount.Net.RisSDK]]
4. Excluding development dependencies of `docfx.console` in `Nuget package`.

Version `6.5.0` changes 
-------------------------
05/29/2017

1. SALT phrase configurable as a app setting(key/value) in `app.config`.
    Set `Ris.Khash.Salt` in your `app.config` file for this to work.
2. Update `docfx.console` to version *2.16.8*

Version `6.4.2` changes
---------------------------
 04/06/2017

1. Minor improvements for integration tests logging
2. Fixed build issue with docFX (*ver 2.16.2*) and Visual Studio install Path   

Version `6.4.0` changes
---------------------------
03/30/2017

1. Secure communication between client and server now using **TLS v1.2**
2. Added `Power Shell` scripts for easier compilation, build, unit and integration tests, .net documentation generation, and packaging
3. General source code improvements and modernization
4. Using `DocFX` documentation generation tool (*ver. 2.15.5*) for API reference. 
5. General .net framework enhancements to 4.5 (*.NET framework 4.5* or later is recommended).

Version `6.3.0` changes
--------------------------
02/24/2015

1. Added support for API key authentication. Client certificate validation is still supported,
    but is now deprecated. Set `Ris.API.Key` in your `app.config` file for this to work.
2. `EPTOK` new field gets auto-populated when setting a credit card payment. Kount only saves this
    if the merchant has certain 3rd party call-outs enabled.

Version `6.0.0` changes
--------------------------
08/01/2014

1. Added support for new `Kount Central` RIS query modes 'J' and 'W'.