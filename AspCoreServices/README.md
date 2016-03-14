AspCoreServices
=========================================================================

Objective is to have a .NET web application running under Docker, such
that:

* Main application has dependencies which are only available on a private
  NuGet server.


Pre-requisites
-------------------------------------------------------------------------

The following are pre-requisites to running this proof:

* Docker QuickStart;
* Private NuGet server, running on `localhost:5000`.


Steps
-------------------------------------------------------------------------

* Run 'Docker QuickStart Terminal'. Please note that the `clean.cmd`
  batch file will not work in a standard command prompt!

* Build Ohm project in Release build.

* Run `nuget-push.cmd`, which will bundle the class library as a
  .nupkg and push it onto the private server.

* Run `docker-build.cmd` which will build a new docker image based on
  the instructions in the `Dockerfile`.

* Run `docker-run.cmd` to run the web application.

  ```
$ ./docker-run.cmd
Hosting environment: Production
Now listening on: http://0.0.0.0:5001
Application started. Press Ctrl+C to shut down.
```

* Access the following URL:
  * http://192.168.99.100:5001/api.wadl
  * http://192.168.99.100:5001/api/regression/native

* Once you are done, run `clean.cmd` to remove all container instances,
  as well as the image you just built.


Networking / Docker Quickstart
-------------------------------------------------------------------------

```
.-------------.             .-------------.             .-------------.
|             |             |   docker    |             |             |
|    host     | <---------> |   virtual   | <---------> |  container  |
|             |        eth0 |   machine   | docker0     |             |
'-------------'        eth1 '-------------'             '-------------'
```

In Windows, the Docker Virtual Machine will see two interfaces which
provide connectivity to the host machine:

* eth0 will be NAT, as provided by the host. While this interface grants
  guest access to the network connectivity of the host (including
  internet), it does not allow for the host to communicate with the
  guest, unless port forwarding is done.

* eth1 will be a 'Host-only Adapter', mapping to a virtual network card
  similar to 'VirtualBox Host-Only Ethernet Adapter #x'. This network
  provides symmetric access between host and guest. This will have a 
  fixed address of `192.168.99.100`.

* In the host, the 'VirtualBox Host-Only Ethernet Adapter #x' network
  interface will have a fixed address of `192.168.99.1`.


Additionally, with regards to the networking between the Docker VM and
the container:

* The docker VM will have a `docker0` network interface which it uses
  to communicate with all of the container instances;

* You can see which IP addresses have been granted on this bridge
  network with the command `docker network inspect bridge`, where you
  will see the following fragment:

  ```
        "Containers": {
            "d540021dfc47d30e4f595f27cca98512b8693f02d67b907713759a8e41cc2e76": {
                "Name": "cranky_elion",
                "EndpointID": "37b88065317786cd460b5ecefa72f330e504f51223e2fabd18612e54d46ce192",
                "MacAddress": "02:42:ac:11:00:02",
                "IPv4Address": "172.17.0.2/16",
                "IPv6Address": ""
            }
        },
```

* In order to 'bridge' the gap between the host and the container 
  instance, we use the `-p H:G` flag when running the container. This
  adds a port forwarding rule whereby the port H on the Docker VM is
  forwarded to the container port G.

As such, accessing `192.168.99.100:5001`:

* Connects to port `:5001` on the Docker VM at `192.168.99.100`;
* ... which is forwarded to port ':5001' on the container.


Additional `ssh` console
-------------------------------------------------------------------------

If you'd like to have another SSH console:

* Start `putty` and connect to `192.168.99.100:22`;
* Credential is `docker` / `tcuser`;
* Escalation is performed with `sudo -i`.

Please note that while `docker-machine ssh` from a command-line does the
same (and even logs you in automatically because of the SSH key), the 
console output is mangled because of color codes which the command-line
doesn't understand.
