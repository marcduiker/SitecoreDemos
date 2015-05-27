# SitecoreDemos
Sitecore solutions for presentations and demos. Just for learning purposes, don't expect production quality code.

[![Build status](https://ci.appveyor.com/api/projects/status/t9aiupbwhshdnhw2/branch/master?svg=true)](https://ci.appveyor.com/project/marcduiker/sitecoredemos/branch/master)

# How to get it all working

## Prerequisites
1.	SQL Server 20008 R2 or higher (Express version will do)
2.	IIS 7 or higher
3.	Visual Studio 2013
4.	A Git client
3.	Sitecore Installation Manager (SIM) 1.3 Update 8 (optional, but it makes my life a lot easier)
4.	Zipped webroot of Sitecore 8 rev 150427
5.	Valid Sitecore license

## Procedure

### Installing Sitecore
-	Install Sitecore 8 rev 150427 using SIM in a local folder such as `c:\sitecore\instances\`. You can name the instance whatever you want. I've named it `scdemo`. I'll refer to it as `<instance>` in the some following steps.

### Get the source
-	Clone `https://github.com/marcduiker/SitecoreDemos.git` into a local folder such as `c:\git\sitecoredemos\`.

### Visual Studio
1.	Open the `source\SitecoreDemos\SitecoreDemos.sln` solution in Visual Studio.
2.	Change the dataFolder value in `App_Config\Include\DataFolder.config` to match the path of the data folder of the instance you've just installed (in my case this is: `c:\dev\sitecore\instances\scdemo\Data`).
3.	Change the `rootPath` value of the `serializationProvider` element in `App_Config\Include\Serialization.config` to match serialization folder in the git repository. In my case this is `c:\git\sitecoredemos\webroot\scdemo\Data\serialization\`.
4.	Update the `App_Config\ConnectionStrings.config` to use the correct connection strings.
5.	Update the target location of the `Local scdemo` publish profile if you haven't named the Sitecore instance `scdemo`.
6.	Enable NuGet package restore for the solution.
7.	Restore the missing NuGet packages.
8.	Build the solution.
9.	Select the `SitecoreDemos.Web` project and publish it using the `Local scdemo` publish profile.
10.	Open a browser and navigate to the `http://<instance>/unicorn.aspx` page (in my case `http://scdemo/unicorn.aspx`).
11.	In case you see an access denied message, log in with the usual admin password.
12.	Hit the `Sync Default Configuration Now` button and wait until the synchronization is complete. Now the custom Sitecore items are rehydrated again in Sitecore.
13.	Navigate to `http://<instance>`. There should be no search result. This is because the search index has not been built yet.
14.	Navigate to `http://<instance>/sitecore` and open the Indexing Manager (Control Panel > Indexing > Indexing Manager).
15.	Select the `custom_search_master_index` and hit Rebuild. We now have an index with some content items.
16.	Navigate to `http://<instance>`. There should be a search result now.