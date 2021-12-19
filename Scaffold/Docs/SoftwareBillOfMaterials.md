# Software Bill of Materials

Modern software is made up of many third-party components, and these can be expressed in a _Software Bill of Materials_. A Software Bill of Materials is often used in supply chain analysis to;

- Check that the software complies to any licensing requirements.
- Identify software dependencies with known vulnerabilities in them.

For more information about Software Bill of Materials, please take a look at;

<https://www.ntia.gov/sbom>

## CycloneDX

Included in Scaffold is a Cake script target which can be used to generate a Software Bill of Materials in the OWASP [CycloneDX](https://cyclonedx.org) format. This format can be provided to various industry tools including OWASP [Dependency Track](https://dependencytrack.org) for analysis.

To produce a Software Bill of Material, run;

    dotnet cake --Target=Audit

The output will be located in the _Artifacts_ directory.
