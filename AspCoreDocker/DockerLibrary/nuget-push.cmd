@echo off

nuget pack Package.nuspec -IncludeReferencedProjects -Prop Configuration=Release

nuget push DockerLibrary.1.0.0.nupkg -s http://localhost:5000/ 4FF2A7A5-5593-470F-A1F2-C3B865440318
