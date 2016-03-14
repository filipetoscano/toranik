AspCoreDocker
=========================================================================

Objective is to have a .NET console application running under Docker, but
where the main application has dependencies which are only available on a 
private NuGet server.


Pre-requisites
-------------------------------------------------------------------------

The following are pre-requisites to running this proof:

* Docker QuickStart;
* Private NuGet server, running on `localhost:5000`.


Steps
-------------------------------------------------------------------------

* Run 'Docker QuickStart Terminal'. Please note that the `clean.cmd`
  batch file will not work in a standard command prompt!

* Build DockerLibrary project in Release build.

* Run `nuget-push.cmd`, which will bundle the class library as a
  .nupkg and push it onto the private server.

* Run `docker-build.cmd` which will build a new docker image based on
  the instructions in the `Dockerfile`. The first build *will* take
  long since it requires the `microsoft/aspnet:1.0.0-rc1-update1-coreclr`
  base image to be downloaded. Example shown below:

    ```
$ ./docker-build.cmd
Sending build context to Docker daemon 246.3 kB
Step 1 : FROM microsoft/aspnet:1.0.0-rc1-update1-coreclr
1.0.0-rc1-update1-coreclr: Pulling from microsoft/aspnet

a3ed95caeb02: Download complete
59a8f12b05c4: Download complete
a37091d1f8ae: Downloading [=============>                                     ] 18.37 MB/68.39 MB
8e8bd35687f7: Download complete
c5fb8b208f3d: Download complete
8ccdb186c8a8: Download 
```

* Run `docker-run.cmd` to run the console application.

    ```
$ ./docker-run.cmd
3/14/2016 3:41:37 PM
Ran!
```

* Once you are done, run `clean.cmd` to remove all container instances,
  as well as the image you just built.


Removed dependencies
-------------------------------------------------------------------------

Default projects will have the following dependencies added by default.
They were removed to reduce the build time of the container.

```
"Microsoft.CSharp": "4.0.1-beta-23516",
"System.Collections": "4.0.11-beta-23516",
"System.Linq": "4.0.1-beta-23516",
"System.Threading": "4.0.11-beta-23516"
```
