# Deterministic Builds

Deterministic builds refer to the property of a build process where given the same inputs it produces the same outputs, even if that build process happens on different machines. Deterministic builds are also sometimes referred to as reproducible or hermetic builds.

Generally when given the same inputs, build processes should produce the same outputs byte-for-byte. In practice this level of determinism is often very difficult to achieve. Some reasons for this include;

- Ambient values in the build environment that the build process depends on such as the date or time.
- Implicit assumptions about the build environment that should not change but do across different machines such as operating system, system architecture (e.g. 32-bit vs. 64-bit) or compiler versions.
- Referencing an external dependency and not pinning the version used in the build process.
- Using a distribution or packaging format that is inherently non-deterministic such as a `zip` file (includes date/time metadata) or Docker image (uses randomly generated image identifiers).

## Cake Build

To help achieve deterministic builds, Scaffold uses a [Cake script](../build.cake) to build, test and publish a release. To run the script, run the command;

    dotnet cake

You will need the `Cake.Tool` installed on your local machine. You can do this quickly by running;

    dotnet tool restore

The output of the Cake script will appear under the `Artifacts` directory.

For more information about Cake, please visit <https://cakebuild.net>.
