# RealTimePassengerInformation

An F# implementation of an unofficial client library for the RTPI service provided for Dublin public transport by the National Transport Authority.

## Continuous integration

<sup>Windows </sup>[![Build status](https://ci.appveyor.com/api/projects/status/6oqf0manxpf59h01/branch/master?svg=true)](https://ci.appveyor.com/project/m-harrison/realtimepassengerinformation/branch/master)
[![AppVeyor tests branch](https://img.shields.io/appveyor/tests/m-harrison/realtimepassengerinformation/master.svg)](https://ci.appveyor.com/project/m-harrison/realtimepassengerinformation)
[![codecov](https://codecov.io/gh/m-harrison/RealTimePassengerInformation/branch/master/graph/badge.svg)](https://codecov.io/gh/m-harrison/RealTimePassengerInformation)
<sup>Ubuntu 14.04 & Mac OS X </sup>[![Build Status](https://travis-ci.org/m-harrison/RealTimePassengerInformation.svg?branch=master)](https://travis-ci.org/m-harrison/RealTimePassengerInformation)

## How to use

### Common stuff

The middle tier logic for handling the HTTP calls to the service is handled by the `RealTimePassengerInformation.Service.Client` module. A `Client.T` must be passed into any of the library's public functions. The module provides the `defaultClient` value that should normally be used.

### Get information about a bus stop

To get information about a single bus stop by its ID use the `getBusStopInformation` function.

```fsharp
open RealTimePassengerInformation.Bus
open RealTimePassengerInformation.Service
...
let busStopId : int = ...
...
let info : Async<Result<BusStopInformation.T, ApiError>> =
        BusStopInformation.getBusStopInformation
            Client.defaultClient busStopId
```

Searching bus stops by operator and name are coming soon.

### Get the timetable for a route at a given bus stop

To get information about a given route at a given bus stop use the `getFullTimetableInformation` function.

```fsharp
open RealTimePassengerInformation.Bus
open RealTimePassengerInformation.Service
...
let busStopId : int = ...
...
let busStopRoute : string = ...
...
let info : Async<Result<FullTimeTableInformation.T, ApiError>> =
        FullTimeTableInformation.getFullTimetableInformation
            Client.defaultClient busStopId busStopRoute
```

### Get all the operators known to RTPI

To get a list of all operators known to RTPI use the `getOperatorInformation` function.

```fsharp
open RealTimePassengerInformation.Bus
open RealTimePassengerInformation.Service
...
let info : Async<Result<OperatorInformation.T, ApiError>> =
        OperatorInformation.getOperatorInformation
            Client.defaultClient
```

### Get a list of real-time arrivals for a given bus stop

To get a list of the real-time arrivals expected at any bus stop use the `getRealTimeBusInformation` function.

```fsharp
open RealTimePassengerInformation.Bus
open RealTimePassengerInformation.Service
...
let busStopId : int = ...
...
let info : Async<Result<RealTimeBusInformation.T, ApiError>> =
        RealTimeBusInformation.getRealTimeBusInformation
            Client.defaultClient busStopId
```

### Get information about a given route (run by a given operator)

To get information about a route run by a given operator use the `getRouteInformation` function.

```fsharp
open RealTimePassengerInformation.Bus
open RealTimePassengerInformation.Service
...
let route : string = ...
...
let operatorReferenceCode : string = ...
...
let info : Async<Result<RouteInformation.T list, ApiError>> =
        RealTimeBusInformation.getRouteInformation
            Client.defaultClient route operatorReferenceCode
```

This may be refactored in the future as a `list` result may not be required.

I will also look into removing the need for the operator parameter, either from the calls to the RTPI service or via some additional logic that will insert it for the caller.

### Get a list of all routes (associated with their operators)

The `RouteListInformation` module provides functions to do this.

#### Not filtering by operator

To get a summary of all routes run regardless of operator use the `getRouteListInformation` function.

```fsharp
open RealTimePassengerInformation.Bus
open RealTimePassengerInformation.Service
...
let info : Async<Result<RouteListInformation.T list, ApiError>> =
        RouteListInformation.getRouteListInformation
            Client.defaultClient
```

#### Filtering for a given operator

To get routes run by just one operator use the `getRouteListInformationForOperator` function.

```fsharp
open RealTimePassengerInformation.Bus
open RealTimePassengerInformation.Service
...
let operatorReferenceCode : string = ...
...
let info : Async<Result<RouteListInformation.T, ApiError>> =
        RouteListInformation.getRouteListInformationForOperator
            Client.defaultClient operatorReferenceCode
```

If you just need information for one operator let this function do the filtering for you, as it is actually the RTPI service that does the filtering which lowers the amount of data transfered over the network.

## Acknowledgements

### .gitignore

The `.gitignore` file for this repository was generated by [gitignore.io](https://www.gitignore.io/). To generate the same file go [here](https://www.gitignore.io/api/fsharp,visualstudio).

### RTPI Documentation

The documentation for the RTPI service that I referred to for this work is available under a [CC-BY-4.0](https://creativecommons.org/licenses/by/4.0/) license from the [National Transport Authority](https://data.smartdublin.ie/organization/national-transport-authority) and is available for reference [here](https://data.smartdublin.ie/dataset/real-time-passenger-information-rtpi-for-dublin-bus-bus-eireann-luas-and-irish-rail). The National Transport Authority in no way endorses me or this project.

### Test count reporting

The badge reporting passed/failed test count from [AppVeyor](https://www.appveyor.com/) is generated by the _'AppVeyor tests branch'_ badge on [shields.io](https://shields.io/#/examples/build).

### Code coverage reporting

Configuring the `appveyor.yml` to upload the [OpenCover](https://github.com/OpenCover/opencover) test coverage results to [CodeCov](https://codecov.io/) was done by following the guidance on [this blog post](https://www.appveyor.com/blog/2017/03/17/codecov/). The OpenCover test coverage results themselves are generated by [Coverlet](https://github.com/tonerdo/coverlet) using `dotnet test` following the guidance [here](https://github.com/tonerdo/coverlet/blob/master/README.md).