# fast-nuget-update
A simple command line utility to bulk update nuget packages in many projects, just to speed it up.

## the case

Consider you work with a huge Visual Studio solution, containing 100 projects. 
All of them reference some utility library, say `MyUtil v1.0.0` as a nuget package. 
One day a library update arrives (`MyUtil 1.0.1`). 

You open the VS Manage Solution Nuget references window, press 'Update', and... the Visual Studio hangs for 10 minutes.

Just to make it faster in simple cases, this utility was introduced.

## restrictions

It is **not** in any sense a replacement of nuget.exe. It is a very silly and simple tool. 

When we want to update our `MyUtil` to `1.0.1` it does one simple replacement: 
- in each folder where exists a pair of `*.csproj` and `packages.config`:
- in `packages.config` version of `MyUtil` is raised to **`1.0.1`**
- in `csproj` we find all the references under paths `packages\MyUtil.x.x.x\` and update them to the path `packages\MyUtil.1.0.1\`

## usage

The tool must be run from the solution folder, assuming the `packages` folder is the child of current and so are the project folders. 

  -n, --name                Required. name of the package to find

  -v, --version             Required. version of the package to set

  -s, --skip-downloading    if set, no attempts would occur to download the
                            package from package sources in the app.config file

