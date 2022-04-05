# Cross-platform Support

Cross-platform support is sometimes also referred to as multi-platform support.

## Building on Different Platforms

Whether you are developing on macOS or Windows, on ARM or Intel, on a 32-bit or 64-bit machine, the developer experience for Scaffold has been designed to be consistent as much as possible. For the most consistent cross-platform experience, we recommend using a 64-bit development machine with Visual Studio Code (vscode) as your Integrated Development Environment (IDE).

## Building for Different Platforms

### .NET

The default publishing mode _framework-dependent_ in .NET Core or .NET 5 and later applications produces cross-platform binaries ending in `.dll`. Applications published in this way can be run with the `dotnet <filename.dll>` command and can be run on any platform where a compatible version of .NET is installed. Platform-specific executables are also produced but can be switched off.

It is also possible to specify the target platform when publishing platform-specific executables.

For more information about publishing .NET applications, please visit the [_.NET application publishing overview_](https://docs.microsoft.com/dotnet/core/deploying).

### Docker

Building container images that target different platforms is possible with [Docker Buildx](https://docs.docker.com/buildx/working-with-buildx). It is also possible to put images that target different platforms under the same image name and tag when pushing to a container registry. For information on how to do this, please check out the [_Leverage multi-CPU architecture support_](https://docs.docker.com/desktop/multi-arch) page in the Docker documentation.
