namespace RealTimePassengerInformation

open System
open System.Diagnostics.CodeAnalysis
open System.Net.Http
open System.Runtime.CompilerServices
open Newtonsoft.Json

module Service =
    /// <summary>
    /// The union of errors types that can cause a query to the RTPI service
    /// from the library to fail.
    /// </summary>
    type public ApiError =
        | UserError
        | NoResults
        | InternalLibraryError
        | NetworkError
        | ScheduledServiceDowntime
        | ServiceError

    module Client =
        type public T = {
            HttpHandler : HttpMessageHandler
        }

        let public defaultClient = {HttpHandler = new HttpClientHandler()}

        let internal getEndpointContent (client:T) (uri:string) =
            async {
                try
                    use client = new HttpClient(client.HttpHandler, false)
                    use! response = Async.AwaitTask<HttpResponseMessage>(client.GetAsync(uri))
                    if response.IsSuccessStatusCode then
                        let! content = Async.AwaitTask<string>(response.Content.ReadAsStringAsync())
                        return Ok content
                    else
                        return Error InternalLibraryError
                with
                | :? ArgumentNullException -> return Error UserError
                | :? HttpRequestException  -> return Error NetworkError
            }

    module Endpoints =
        open Shared.Formatting

        type internal Endpoint =
            | BusStopInformation
            | OperatorInformation
            | RealTimeBusInformation
            | RouteInformation
            | RouteListInformation
            | TimetableInformation

        let internal defaultServiceEndpoint = "https://data.smartdublin.ie/cgi-bin/rtpi"

        let rec internal reduceParameters parameters =
            match parameters with
            | []                     -> []
            | (_, None)::ps          -> reduceParameters ps
            | (name, Some value)::ps -> (name,value)::(reduceParameters ps)

        let internal buildUri (baseUri:string) apiEndpoint parameters =
            let keyValueToParameterTerm ((key,value) : string * string) =
                String.Format(inv, "{0}={1}", key, value)

            List.map keyValueToParameterTerm parameters
            |> List.fold
                (fun acc term ->
                    String.Format(inv, "{0}{1}&", acc, term))
                String.Empty
            |> fun parameterString ->
                String.Format(inv, "{0}{1}",
                    parameterString, keyValueToParameterTerm ("format","json"))
            |> fun parameterString ->
                String.Format(inv, "{0}/{1}?{2}",
                    baseUri,
                    apiEndpoint.ToString().ToLowerInvariant(),
                    parameterString)

    module Models =
        type internal ResponseCode =
            | Success = 0
            | NoResults = 1
            | MissingParameter = 2
            | InvalidParameter = 3
            | ScheduledDowntime = 4
            | UnexpectedSystemError = 5

        [<ExcludeFromCodeCoverage>]
        module Definitions =
            type internal RouteStop =
                struct
                    [<JsonProperty(PropertyName = "stopid", Required = Required.Always)>]
                    val mutable StopId : int
                    [<JsonProperty(PropertyName = "displaystopid", Required = Required.Always)>]
                    val mutable DisplayStopId : int
                    [<JsonProperty(PropertyName = "shortname", Required = Required.Always)>]
                    val mutable ShortName : string
                    [<JsonProperty(PropertyName = "shortnamelocalized", Required = Required.Always)>]
                    val mutable ShortNameLocalized : string
                    [<JsonProperty(PropertyName = "fullname", Required = Required.Always)>]
                    val mutable FullName : string
                    [<JsonProperty(PropertyName = "fullnamelocalized", Required = Required.Always)>]
                    val mutable FullNameLocalized : string
                    [<JsonProperty(PropertyName = "latitude", Required = Required.Always)>]
                    val mutable Latitude : float
                    [<JsonProperty(PropertyName = "longitude", Required = Required.Always)>]
                    val mutable Longitude : float

                    new (stopId, displayStopId, shortName, shortNameLocalized, fullName, fullNameLocalized, latitude, longitude) =
                        {
                            StopId = stopId;
                            DisplayStopId = displayStopId;
                            ShortName = shortName;
                            ShortNameLocalized = shortNameLocalized;
                            FullName = fullName;
                            FullNameLocalized = fullNameLocalized;
                            Latitude = latitude;
                            Longitude = longitude
                        }
                end

            type internal StopOperator =
                struct
                    [<JsonProperty(PropertyName = "name", Required = Required.Always)>]
                    val mutable OperatorName : string
                    [<JsonProperty(PropertyName = "routes", Required = Required.Always)>]
                    val mutable Routes : string list

                    new (operatorName, routes) =
                        {
                            OperatorName = operatorName;
                            Routes = routes
                        }
                end

            type internal BusStopInformationModel =
                struct
                    [<JsonProperty(PropertyName = "stopid", Required = Required.Always)>]
                    val mutable StopId : int
                    [<JsonProperty(PropertyName = "displaystopid", Required = Required.Always)>]
                    val mutable DisplayStopId : int
                    [<JsonProperty(PropertyName = "shortname", Required = Required.Always)>]
                    val mutable ShortName : string
                    [<JsonProperty(PropertyName = "shortnamelocalized", Required = Required.Always)>]
                    val mutable ShortNameLocalized : string
                    [<JsonProperty(PropertyName = "fullname", Required = Required.Always)>]
                    val mutable FullName : string
                    [<JsonProperty(PropertyName = "fullnamelocalized", Required = Required.Always)>]
                    val mutable FullNameLocalized : string
                    [<JsonProperty(PropertyName = "latitude", Required = Required.Always)>]
                    val mutable Latitude : float
                    [<JsonProperty(PropertyName = "longitude", Required = Required.Always)>]
                    val mutable Longitude : float
                    [<JsonProperty(PropertyName = "lastupdated", Required = Required.Always)>]
                    val mutable LastUpdated : string
                    [<JsonProperty(PropertyName = "operators", Required = Required.Always)>]
                    val mutable Operators : StopOperator list

                    new (stopId, displayStopId, shortName, shortNameLocalized, fullName, fullNameLocalized, latitude, longitude, lastUpdated, operators) =
                        {
                            StopId = stopId;
                            DisplayStopId = displayStopId;
                            ShortName = shortName;
                            ShortNameLocalized = shortNameLocalized;
                            FullName = fullName;
                            FullNameLocalized = fullNameLocalized;
                            Latitude = latitude;
                            Longitude = longitude;
                            LastUpdated = lastUpdated;
                            Operators = operators;
                        }
                end

            type internal FullTimetableBusInformationModel =
                struct
                    [<JsonProperty(PropertyName = "startdayofweek", Required = Required.Always)>]
                    val mutable StartDayOfWeek : string
                    [<JsonProperty(PropertyName = "enddayofweek", Required = Required.Always)>]
                    val mutable EndDayOfWeek : string
                    [<JsonProperty(PropertyName = "destination", Required = Required.Always)>]
                    val mutable Destination : string
                    [<JsonProperty(PropertyName = "destinationlocalized", Required = Required.Always)>]
                    val mutable DestinationLocalized : string
                    [<JsonProperty(PropertyName = "lastupdated", Required = Required.Always)>]
                    val mutable LastUpdated : string
                    [<JsonProperty(PropertyName = "departures", Required = Required.Always)>]
                    val mutable Departures : string list

                    new (startDayOfWeek, endDayOfWeek, destination, destinationLocalized, lastUpdated, departures) =
                        {
                            StartDayOfWeek = startDayOfWeek;
                            EndDayOfWeek = endDayOfWeek;
                            Destination = destination;
                            DestinationLocalized = destinationLocalized;
                            LastUpdated = lastUpdated;
                            Departures = departures;
                        }
                end

            type internal OperatorInformationModel =
                struct
                    [<JsonProperty(PropertyName = "operatorreference", Required = Required.Always)>]
                    val mutable OperatorReference : string
                    [<JsonProperty(PropertyName = "operatorname", Required = Required.Always)>]
                    val mutable OperatorName : string
                    [<JsonProperty(PropertyName = "operatordescription", Required = Required.Always)>]
                    val mutable OperatorDescription : string

                    new (operatorReference, operatorName, operatorDescription) =
                        {
                            OperatorReference = operatorReference;
                            OperatorName = operatorName;
                            OperatorDescription = operatorDescription;
                        }
                end

            type internal RealTimeBusInformationModel =
                struct
                    [<JsonProperty(PropertyName = "arrivaldatetime", Required = Required.Always)>]
                    val mutable ArrivalDateTime : string
                    [<JsonProperty(PropertyName = "duetime", Required = Required.Always)>]
                    val mutable DueTime : string
                    [<JsonProperty(PropertyName = "departuredatetime", Required = Required.Always)>]
                    val mutable DepartureDateTime : string
                    [<JsonProperty(PropertyName = "departureduetime", Required = Required.Always)>]
                    val mutable DepartureDueTime : string
                    [<JsonProperty(PropertyName = "scheduledarrivaldatetime", Required = Required.Always)>]
                    val mutable ScheduledArrivalDateTime : string
                    [<JsonProperty(PropertyName = "scheduleddeparturedatetime", Required = Required.Always)>]
                    val mutable ScheduledDepartureDateTime : string
                    [<JsonProperty(PropertyName = "destination", Required = Required.Always)>]
                    val mutable Destination : string
                    [<JsonProperty(PropertyName = "destinationlocalized", Required = Required.Always)>]
                    val mutable DestinationLocalized : string
                    [<JsonProperty(PropertyName = "origin", Required = Required.Always)>]
                    val mutable Origin : string
                    [<JsonProperty(PropertyName = "originlocalized", Required = Required.Always)>]
                    val mutable OriginLocalized : string
                    [<JsonProperty(PropertyName = "direction", Required = Required.Always)>]
                    val mutable Direction : string
                    [<JsonProperty(PropertyName = "operator", Required = Required.Always)>]
                    val mutable OperatorReferenceCode : string
                    [<JsonProperty(PropertyName = "additionalinformation", Required = Required.Always)>]
                    val mutable AdditionalInformation : string
                    [<JsonProperty(PropertyName = "lowfloorstatus", Required = Required.Always)>]
                    val mutable LowFloorStatus : string
                    [<JsonProperty(PropertyName = "route", Required = Required.Always)>]
                    val mutable Route : string
                    [<JsonProperty(PropertyName = "sourcetimestamp", Required = Required.Always)>]
                    val mutable SourceTimeStamp : string

                    new (arrivalDateTime, dueTime, departureDateTime, departureDueTime, scheduledArrivalDateTime, scheduledDepartureDateTime, destination, destinationLocalized, origin, originLocalized, direction, operatorReferenceCode, additionalInformation, lowFloorStatus, route, sourceTimeStamp) =
                        {
                            ArrivalDateTime = arrivalDateTime;
                            DueTime = dueTime;
                            DepartureDateTime = departureDateTime;
                            DepartureDueTime = departureDueTime;
                            ScheduledArrivalDateTime = scheduledArrivalDateTime;
                            ScheduledDepartureDateTime = scheduledDepartureDateTime;
                            Destination = destination;
                            DestinationLocalized = destinationLocalized;
                            Origin = origin;
                            OriginLocalized = originLocalized;
                            Direction = direction;
                            OperatorReferenceCode = operatorReferenceCode;
                            AdditionalInformation = additionalInformation;
                            LowFloorStatus = lowFloorStatus;
                            Route = route;
                            SourceTimeStamp = sourceTimeStamp;
                        }
                end

            type internal RouteInformationModel =
                struct
                    [<JsonProperty(PropertyName = "operator", Required = Required.Always)>]
                    val mutable OperatorName : string
                    [<JsonProperty(PropertyName = "origin", Required = Required.Always)>]
                    val mutable Origin : string
                    [<JsonProperty(PropertyName = "originlocalized", Required = Required.Always)>]
                    val mutable OriginLocalized : string
                    [<JsonProperty(PropertyName = "destination", Required = Required.Always)>]
                    val mutable Destination : string
                    [<JsonProperty(PropertyName = "destinationlocalized", Required = Required.Always)>]
                    val mutable DestinationLocalized : string
                    [<JsonProperty(PropertyName = "lastupdated", Required = Required.Always)>]
                    val mutable LastUpdated : string
                    [<JsonProperty(PropertyName = "stops", Required = Required.Always)>]
                    val mutable Stops : RouteStop list

                    new (operatorName, origin, originLocalized, destination, destinationLocalized, lastUpdated, stops) =
                        {
                            OperatorName = operatorName;
                            Origin = origin;
                            OriginLocalized = originLocalized;
                            Destination = destination;
                            DestinationLocalized = destinationLocalized;
                            LastUpdated = lastUpdated;
                            Stops = stops;
                        }
                end

            type internal RouteListInformationModel =
                struct
                    [<JsonProperty(PropertyName = "operator", Required = Required.Always)>]
                    val mutable OperatorReference : string
                    [<JsonProperty(PropertyName = "route", Required = Required.Always)>]
                    val mutable Route : string

                    new (operatorReference, route) =
                        {
                            OperatorReference = operatorReference;
                            Route = route;
                        }
                end

            type internal TimetableBusInformationModel =
                struct
                    [<JsonProperty(PropertyName = "arrivaldatetime", Required = Required.Always)>]
                    val mutable ArrivalDateTime : string
                    [<JsonProperty(PropertyName = "destination", Required = Required.Always)>]
                    val mutable Destination : string
                    [<JsonProperty(PropertyName = "destinationlocalized", Required = Required.Always)>]
                    val mutable DestinationLocalized : string
                    [<JsonProperty(PropertyName = "operator", Required = Required.Always)>]
                    val mutable OperatorName : string
                    [<JsonProperty(PropertyName = "lowfloorstatus", Required = Required.Always)>]
                    val mutable LowFloorStatus : string
                    [<JsonProperty(PropertyName = "route", Required = Required.Always)>]
                    val mutable Route : string

                    new (arrivalDateTime, destination, destinationLocalized, operatorName, lowFloorStatus, route) =
                        {
                            ArrivalDateTime = arrivalDateTime;
                            Destination = destination;
                            DestinationLocalized = destinationLocalized;
                            OperatorName = operatorName;
                            LowFloorStatus = lowFloorStatus;
                            Route = route;
                        }
                end

            type internal ServiceResponseModel<'a> =
                struct
                    [<JsonProperty(PropertyName = "errorcode", Required = Required.Always)>]
                    val mutable ErrorCode : ResponseCode
                    [<JsonProperty(PropertyName = "errormessage", Required = Required.Always)>]
                    val mutable ErrorMessage : string
                    [<JsonProperty(PropertyName = "stopid", Required = Required.Default)>]
                    val mutable StopId : Nullable<int>
                    [<JsonProperty(PropertyName = "route", Required = Required.Default)>]
                    val mutable Route : string
                    [<JsonProperty(PropertyName = "numberofresults", Required = Required.Always)>]
                    val mutable NumberOfResults : int
                    [<JsonProperty(PropertyName = "timestamp", Required = Required.Always)>]
                    val mutable Timestamp : string
                    [<JsonProperty(PropertyName = "results", Required = Required.Always)>]
                    val mutable Results : 'a list

                    new (errorCode, errorMessage, stopId, route, numberOfResults, timeStamp, results) =
                        {
                            ErrorCode = errorCode;
                            ErrorMessage = errorMessage;
                            StopId = stopId;
                            Route = route;
                            NumberOfResults = numberOfResults;
                            Timestamp = timeStamp;
                            Results = results
                        }
                end

        let internal serviceDateTimeFormat = "dd/MM/yyyy HH:mm:ss"

        let internal serviceTimeSpanFormat = "hh\\:mm\\:ss"

        let internal deserializeServiceResponseModel<'a> j
            : Result<Definitions.ServiceResponseModel<'a>, ApiError> =
                try
                    Ok (JsonConvert.DeserializeObject<Definitions.ServiceResponseModel<'a>> j)
                with :? JsonException -> Error InternalLibraryError

        let internal validateServiceResponseModel (m:Definitions.ServiceResponseModel<'a>) =
            match m.ErrorCode with
            | ResponseCode.Success               -> Ok m.Results
            | ResponseCode.NoResults             -> Error NoResults
            | ResponseCode.MissingParameter      -> Error InternalLibraryError
            | ResponseCode.InvalidParameter      -> Error InternalLibraryError
            | ResponseCode.ScheduledDowntime     -> Error ScheduledServiceDowntime
            | ResponseCode.UnexpectedSystemError -> Error ServiceError
            | _                                  -> Error InternalLibraryError

        let internal validateSingleResult (results:'a list) =
            match results with
            | result::[] -> Ok result
            | _          -> Error InternalLibraryError

[<assembly: InternalsVisibleTo("RealTimePassengerInformation.UnitTests")>]
do()
