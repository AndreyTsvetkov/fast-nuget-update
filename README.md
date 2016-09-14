# fast-nuget-update
A simple command line utility to update nuget package installed to many projects of a large solution, when standart tooling is just too slow.

## usage

  -n, --name                Required. name of the package to find

  -v, --version             Required. version of the package to set

  -s, --skip-downloading    if set, no attempts would occur to download the
                            package from package sources in the app.config file

