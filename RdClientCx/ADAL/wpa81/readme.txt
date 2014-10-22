Description : ADAL for Windows Phone 8.1
Version     : 2.10.10910.1511 
Date        : Thursday, September 11 2014
URL         : http://www.nuget.org/packages/Microsoft.IdentityModel.Clients.ActiveDirectory

----------------------------------------------------------------------------------
Update Microsoft.IdentityModel.Clients.ActiveDirectory.winmd from NuGet:

1. Create a new WinPhone 8.1 project or use an existing one in Visual Studio.

2. Open Package Manager Console (TOOLS -> NuGet Package Manager -> Package Manager Console).

3. Run the following command to grab the latest official version:
   Install-Package Microsoft.IdentityModel.Clients.ActiveDirectory

   To grab a pre-released version, run the command with -Pre option. You also need to specify which version you want to grab. For example:
   Install-Package Microsoft.IdentityModel.Clients.ActiveDirectory -Version 2.8.10804.1442-rc -Pre

4. Then you will find the library files available under following path of your WinPhone 8.1 project, for example:
   \packages\Microsoft.IdentityModel.Clients.ActiveDirectory.2.9.10826.1824\lib\wpa81\

5. Copy the files to source depot:
   \termsrv\CloudDv\Externals\ADAL\wpa81\

----------------------------------------------------------------------------------
Grab pre-released version from MyGet:

1. Create a new WinPhone 8.1 project or use an existing one in Visual Studio.

2. If you have not done so, add the following package source to NuGet Package Manager (TOOLS -> Options -> NuGet Package Manager -> Package Sources):
   http://www.myget.org/f/azureadwebstacknightly

3. Open Package Manager Console (TOOLS -> NuGet Package Manager -> Package Manager Console).

4. Choose the custom package source from the pull-down menu in the Package Manager Console.

5. Run the following command, as an example, to grab a specific version:
   Install-Package Microsoft.IdentityModel.Clients.ActiveDirectory -Version 2.10.10910.1511 -Pre

6. Then you will find the library files available under following path of your WinPhone 8.1 project, for example:
   \packages\Microsoft.IdentityModel.Clients.ActiveDirectory.2.10.10910.1511\lib\wpa81\

7. Copy the files to source depot:
   \termsrv\CloudDv\Externals\ADAL\wpa81\