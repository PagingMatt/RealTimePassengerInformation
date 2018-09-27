namespace RealTimePassengerInformation

open System
open System.Collections.Generic
open System.Globalization
open System.Runtime.CompilerServices
open Newtonsoft.Json

module Service =
    type internal Endpoint =
        | BusStopInformation
        | OperatorInformation
        | RealTimeBusInformation
        | RouteInformation
        | RouteListInformation
        | TimetableInformation

    type internal ResponseCode =
        | Success               = 0
        | NoResults             = 1
        | MissingParameter      = 2
        | InvalidParameter      = 3
        | ScheduledDowntime     = 4
        | UnexpectedSystemError = 5

    type internal ServiceResponse<'a> =
        struct
            [<JsonProperty(PropertyName = "errorcode", Required = Required.Always)>]
            val mutable ErrorCode : int
            [<JsonProperty(PropertyName = "errormessage", Required = Required.Always)>]
            val mutable ErrorMessage : string
            [<JsonProperty(PropertyName = "stopid", Required = Required.DisallowNull)>]
            val mutable StopId : int option
            [<JsonProperty(PropertyName = "route", Required = Required.DisallowNull)>]
            val mutable Route : string
            [<JsonProperty(PropertyName = "numberofresults", Required = Required.Always)>]
            val mutable NumberOfResults : string
            [<JsonProperty(PropertyName = "timestamp", Required = Required.Always)>]
            val mutable Timestamp : string
            [<JsonProperty(PropertyName = "results", Required = Required.Always)>]
            val mutable Results : IEnumerable<'a>
        end

    let internal defaultServiceEndpoint = "https://data.smartdublin.ie/cgi-bin/rtpi"

    let private inv = CultureInfo.InvariantCulture

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

[<assembly: InternalsVisibleTo("RealTimePassengerInformation.UnitTests")>]
do()
