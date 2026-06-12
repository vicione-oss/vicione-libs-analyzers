# ViciOne Core Runtime Analyzers

## Solution filter and VSIX package

This repository contains a solution filter file called `ViciOne.Core.Runtime.Analyzers.CI.slnf` which is used for the build process in the CI. This file excludes the VSIX package project as long as it can´t be built in CI.

**Important**: If it´s possible to build the VSIX package in CI, then the `<PropertyGroup>` containing the versioning in the file `src/ViciOne.Core.Runtime.Analyzers.Vsix/Directory.Build.targets` must be removed to keep the versioning in CI working properly.
