namespace RealTimePassengerInformation

open System
open System.Globalization

module Bus =
    module Service =
        /// <summary>
        /// Union of available endpoints for the real-time bus service.
        /// </summary>
        type Endpoint =
            | BusStopInformation
            | OperatorInformation
            | RealTimeBusInformation
            | RouteListInformation
            | TimetableInformation

        /// <summary>
        /// Enumeration of possible response codes from the service.
        /// </summary>
        type ResponseCode =
            | Success               = 0
            | NoResults             = 1
            | MissingParameter      = 2
            | InvalidParameter      = 3
            | ScheduledDowntime     = 4
            | UnexpectedSystemError = 5

        /// <summary>
        /// Default base URI that the real-time bus service is deployed to.
        /// </summary>
        let defaultServiceEndpoint = "https://data.smartdublin.ie/cgi-bin/rtpi"

        /// <summary>
        /// Recursively reduces parameters bound to Option monad.
        /// </summary>
        /// <param name="parameters">
        /// Parameters to reduce.
        /// </param>
        /// <returns>
        /// Only parameters where there was 'Some value' with value
        /// pulled up form the monad.
        /// </returns>
        let rec reduceParameters parameters =
            match parameters with
            | []                     -> []
            | (_, None)::ps          -> reduceParameters ps
            | (name, Some value)::ps -> (name,value)::(reduceParameters ps)

        /// <summary>
        /// Builds the absolute URI with parameters to call and appends JSON
        /// format parameter.
        /// </summary>
        /// <param name="baseUri">
        /// Base URI the service was deployed to.
        /// </param>
        /// <param name="apiEndpoint">
        /// Relative Endpoint union value for the particular REST method.
        /// </param>
        /// <param name="parameterKeyValueList">
        /// (key,value) mapping of parameters to put in URI.
        /// </param>
        /// <returns>
        /// URI to call for endpoint with parameters at deployed location.
        /// </returns>
        let buildUri (baseUri:string) apiEndpoint parameterKeyValueList =
            // Helper to format key, value pairs into URI parameter terms.
            let keyValueToParameterTerm ((key,value) : string * string) =
                String.Format(
                    CultureInfo.InvariantCulture, "{0}={1}", key, value)

            // Helper to tail-recursively join URI parameter terms into
            // parameter string.
            let rec joinParameterTerms acc terms =
                match terms with
                | []       ->
                    String.Format(
                            CultureInfo.InvariantCulture,
                            "{0}{1}",
                            acc,
                            (keyValueToParameterTerm ("format","json")))
                | term::ts ->
                    joinParameterTerms
                        (String.Format(
                            CultureInfo.InvariantCulture, "{0}{1}&", acc, term))
                        ts

            // Map parameters to terms and build string via helpers.
            List.map keyValueToParameterTerm parameterKeyValueList
            |> joinParameterTerms ""
            |> fun parameterString ->
                String.Format(
                    CultureInfo.InvariantCulture,
                    "{0}/{1}?{2}",
                    baseUri,
                    apiEndpoint.ToString().ToLowerInvariant(),
                    parameterString)
