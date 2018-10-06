namespace RealTimePassengerInformation

open System
open System.Net.Http
open System.Runtime.CompilerServices
open Newtonsoft.Json
open RealTimePassengerInformation.Definitions

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
