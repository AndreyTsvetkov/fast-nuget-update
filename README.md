# fast-nuget-update [![build static](https://ci.appveyor.com/api/projects/status/ckwopqwiws29cxmn/branch/master?svg=true)](https://ci.appveyor.com/project/AndreyTS/fast-nuget-update)
A command line utility for bulk updating nuget packages references.   


## case

Consider you work with a huge Visual Studio solution, containing 100 projects. 
All of them reference some utility library, say `MyUtil v1.0.0` as a nuget package. 
One day a library update `MyUtil 1.0.1` arrives. 

You open the `Manage Nuget packages` window, press 'Update', and... the Visual Studio hangs for 10 minutes.

Just to make it faster in simple cases, this utility was introduced.

## restrictions

It is **not** in any sense a replacement of nuget.exe. It is a very silly and simple tool. 


It does :
- **not** analyze any dependecies
- **not** support packages where main dll name differs from the package name
- **not** care whether the project is included in the solution or just is laid in the sln folder

## usage

The tool must be run from the solution folder, assuming the `packages` folder is the child of solution and so are the project folders. 

So, given a folder tree be like that:

    /MySln/
        /packages/
        /MySln.sln
        /FirstProject/
            /FirstProject.csproj
            /packages.config
        /...
        /LastProject/
            /LastProject.csproj
            /packages.config

If we run this cmd:

        cd MySln
        fast-nuget-update --name MyUtil --version 1.0.1

All projects in MySln folder which referenced **any** version of `MyUtil` will change their references to the `1.0.1` version. 

### Parameters list
  -n, --name                
Required. The name of the package to update

  -v, --version             
Required. The version of the package to set

  -s, --skip-downloading    
Optional. Flag. If it set, no attempts would occur to download the
                            package from package sources in the app.config file

