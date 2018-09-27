namespace RealTimePassengerInformation

open System
open System.Globalization
open System.Runtime.CompilerServices

module Bus =
    type public BusStopName = {
        EnglishName : string;
        IrishName   : string;
    }

    type public Day =
        | Sunday    = 0
        | Monday    = 1
        | Tuesday   = 2
        | Wednesday = 3
        | Thursday  = 4
        | Friday    = 5
        | Saturday  = 6

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

        /// <summary>
        /// Default base URI that the real-time bus service is deployed to.
        /// </summary>
        let public defaultServiceEndpoint = "https://data.smartdublin.ie/cgi-bin/rtpi"

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

    module BusStopInformation =
        type public BusStopOperator = {
            Name   : string;
            Routes : string list;
        }

        type public T = {
            StopId          : string;
            DisplayedStopId : string;
            ShortName       : BusStopName;
            FullName        : BusStopName;
            Latitude        : float;
            Longitude       : float;
            Operators       : BusStopOperator list;
        }

    module OperatorInformation =
        type public T = {
            ReferenceCode : string;
            Name          : string;
            Description   : string;
        }

    module RealTimeBusInformation =
        type public RealTimeSlot = {
            Expected  : DateTime;
            Scheduled : DateTime;
        }

        type public RealTimeArrivalInformation = {
            Arrival               : RealTimeSlot;
            Departure             : RealTimeSlot;
            BoardStatus           : string;
            Origin                : BusStopName;
            Destination           : BusStopName;
            Direction             : string;
            OperatorName          : string;
            AdditionalInformation : string;
            HasLowFloor           : bool;
            Route                 : string;
        }

        type public T = {
            StopId   : string;
            Arrivals : RealTimeArrivalInformation;
        }

    module RouteInformation =
        type public BusStopInformation = {
            StopId          : string;
            DisplayedStopId : string;
            ShortName       : BusStopName;
            FullName        : BusStopName;
            Latitude        : float;
            Longitude       : float;
        }

        type public T = {
            OperatorName : string;
            Origin       : BusStopName;
            Destination  : BusStopName;
            Stops        : BusStopInformation list;
        }

    module RouteListInformation =
        type public T = {
            OperatorReferenceCode : string;
            Routes                : string list;
        }

    module DailyTimeTableInformation =
        type public TimeTableEntry = {
            Arrival      : DateTime;
            Destination  : BusStopName;
            OperatorName : string;
            HasLowFloor  : bool;
            Route        : string;
        }

        type public T = {
            StopId           : string;
            TimeTableEntries : TimeTableEntry list;
        }

    module FullTimeTableInformation =
        type public TimeTableEntry = {
            StartDayOfWeek : Day;
            EndDayOfWeek   : Day;
            Destination    : BusStopName;
            Departures     : TimeSpan list
        }

        type public T = {
            StopId           : string;
            Route            : string;
            TimeTableEntries : TimeTableEntry list;
        }

[<assembly: InternalsVisibleTo("RealTimePassengerInformation.UnitTests")>]
do()
