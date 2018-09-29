namespace RealTimePassengerInformation

open System
open System.Runtime.CompilerServices
open Service
open Service.Client
open Service.Endpoints
open Service.Models
open Shared.Formatting
open Shared.Operators

module Bus =
    type public BusStopName = {
        EnglishName : string;
        IrishName   : string;
    }

    type public Day =
        | Sunday
        | Monday
        | Tuesday
        | Wednesday
        | Thursday
        | Friday
        | Saturday

    let deserializeDay day =
        match day with
        | "Sunday"    -> Some Sunday
        | "Monday"    -> Some Monday
        | "Tuesday"   -> Some Tuesday
        | "Wednesday" -> Some Wednesday
        | "Thursday"  -> Some Thursday
        | "Friday"    -> Some Friday
        | "Saturday"  -> Some Saturday
        | _           -> None

    let parseDateTimeExn s =
        DateTime.ParseExact(s, serviceDateTimeFormat, inv)

    let parseTimeSpanExn s =
        TimeSpan.ParseExact(s, serviceTimeSpanFormat, inv)

    module BusStopInformation =
        type public BusStopOperator = {
            Name   : string;
            Routes : string list;
        }

        type public T = {
            StopId          : int;
            DisplayedStopId : int;
            ShortName       : BusStopName;
            FullName        : BusStopName;
            Latitude        : float;
            Longitude       : float;
            LastUpdated     : DateTime;
            Operators       : BusStopOperator list;
        }

        let internal make (m:BusStopInformationModel) =
            try
                let parsedLastUpdated = parseDateTimeExn m.LastUpdated
                Ok {
                    StopId = m.StopId
                    DisplayedStopId = m.DisplayStopId
                    ShortName = {
                        EnglishName = m.ShortName
                        IrishName = m.ShortNameLocalized
                    }
                    FullName = {
                        EnglishName = m.FullName
                        IrishName = m.FullNameLocalized
                    }
                    Latitude = m.Latitude
                    Longitude = m.Longitude
                    LastUpdated = parsedLastUpdated
                    Operators = List.map (fun (o:StopOperator) -> {Name = o.OperatorName; Routes = o.Routes}) m.Operators
                }
            with :? FormatException -> Error InternalLibraryError

        let getBusStopInformation (stopId:int) : Async<Result<T, ApiError>> =
            [("stopid",stopId.ToString())]
            |> buildUri defaultServiceEndpoint BusStopInformation
            |> getEndpointContent defaultHandler
            >>> deserializeServiceResponseModel<BusStopInformationModel>
            >>> validateServiceResponseModel
            >>> validateSingleResult
            >>> make

    module FullTimeTableInformation =
        type public TimeTableEntry = {
            StartDayOfWeek : Day;
            EndDayOfWeek   : Day;
            Destination    : BusStopName;
            LastUpdated    : DateTime
            Departures     : TimeSpan list
        }

        type public T = {
            StopId           : int;
            Route            : string;
            TimeTableEntries : TimeTableEntry list;
        }

        let internal makeSafe (m:FullTimetableBusInformationModel) = {
            StartDayOfWeek = Sunday
            EndDayOfWeek = Sunday
            Destination = {
                EnglishName = m.Destination
                IrishName = m.DestinationLocalized
            }
            LastUpdated = DateTime.MinValue
            Departures = []
        }

        let internal make mapSucceeding (m:FullTimetableBusInformationModel) =
            let safeRecord =  makeSafe m
            if not(mapSucceeding) then (safeRecord,mapSucceeding) else
            try
                match deserializeDay m.StartDayOfWeek with
                | None -> (safeRecord,false)
                | Some startDay ->
                match deserializeDay m.EndDayOfWeek with
                | None -> (safeRecord,false)
                | Some endDay ->
                let parsedLastUpdated = parseDateTimeExn m.LastUpdated
                let parsedDepartures = List.map parseTimeSpanExn m.Departures
                ({
                    safeRecord with
                        StartDayOfWeek = startDay;
                        EndDayOfWeek   = endDay;
                        LastUpdated    = parsedLastUpdated;
                        Departures     = parsedDepartures
                }, mapSucceeding)
            with :? FormatException -> (safeRecord,false)

        let public getFullTimetableInformation (stopid:int) (route:string)
            : Async<Result<T, ApiError>> =
                [("type","week");("stopid",stopid.ToString());("routeid",route)]
                |> buildUri defaultServiceEndpoint TimetableInformation
                |> getEndpointContent defaultHandler
                >>> deserializeServiceResponseModel<FullTimetableBusInformationModel>
                >>> validateServiceResponseModel
                >>< List.mapFold make true
                >>> fun (rs,mapSucceeded) ->
                        if mapSucceeded then (Ok {
                            StopId = stopid
                            Route = route
                            TimeTableEntries = rs
                        })
                        else Error InternalLibraryError

    module OperatorInformation =
        type public Operator = {
            ReferenceCode : string;
            Name          : string;
            Description   : string;
        }

        type public T = Operator list

        let internal make (m:OperatorInformationModel) = {
            ReferenceCode = m.OperatorReference
            Name = m.OperatorName
            Description = m.OperatorDescription
        }

        let public getOperatorInformation () : Async<Result<T, ApiError>> =
            buildUri defaultServiceEndpoint OperatorInformation []
            |> getEndpointContent defaultHandler
            >>> deserializeServiceResponseModel<OperatorInformationModel>
            >>> validateServiceResponseModel
            >>< List.map make

    module RealTimeBusInformation =
        type public BoardStatus =
            | Due
            | ExpectedInMinutes of int

        type public RealTimeSlot = {
            Expected    : DateTime;
            Scheduled   : DateTime;
            BoardStatus : BoardStatus;
        }

        type public RealTimeArrivalInformation = {
            Arrival               : RealTimeSlot;
            Departure             : RealTimeSlot;
            Origin                : BusStopName;
            Destination           : BusStopName;
            Direction             : string;
            OperatorName          : string;
            AdditionalInformation : string;
            HasLowFloor           : bool;
            Route                 : string;
            SourceTimeStamp       : DateTime;
        }

        type public T = {
            StopId   : int;
            Arrivals : RealTimeArrivalInformation list;
        }

        let internal makeSafe (m:RealTimeBusInformationModel) = {
            Arrival =  {
                Expected = DateTime.MinValue
                Scheduled = DateTime.MinValue
                BoardStatus = Due
            }
            Departure =  {
                Expected = DateTime.MinValue
                Scheduled = DateTime.MinValue
                BoardStatus = Due
            }
            Origin = {
                EnglishName = m.Origin
                IrishName = m.OriginLocalized
            }
            Destination = {
                EnglishName = m.Destination
                IrishName = m.DestinationLocalized
            }
            Direction = m.Direction
            OperatorName = m.OperatorName
            AdditionalInformation = m.AdditionalInformation
            HasLowFloor = false
            Route = m.Route
            SourceTimeStamp = DateTime.MinValue
        }

        let deserializeBoardStatus boardStatus =
            match boardStatus with
            | "Due" -> Some Due
            | mins  ->
                try Some (ExpectedInMinutes (Int32.Parse mins))
                with :? FormatException -> None

        let deserializeLowFloorStatus lowFloorStatus =
            match lowFloorStatus with
            | "yes" -> Some true
            | "no"  -> Some false
            | _     -> None

        let internal make mapSucceeding (m:RealTimeBusInformationModel) =
            let safeRecord =  makeSafe m
            if not(mapSucceeding) then (safeRecord,mapSucceeding) else
            try
                match deserializeBoardStatus m.DueTime with
                | None -> (safeRecord,false)
                | Some arrivalDue ->
                match deserializeBoardStatus m.DepartureDueTime with
                | None -> (safeRecord,false)
                | Some departureDue ->
                match deserializeLowFloorStatus m.LowFloorStatus with
                | None -> (safeRecord,false)
                | Some lowFloorStatus ->
                let parsedSourceTimeStamp = parseDateTimeExn m.SourceTimeStamp
                let parsedArrivalExpected = parseDateTimeExn m.ArrivalDateTime
                let parsedArrivalScheduled = parseDateTimeExn m.ScheduledArrivalDateTime
                let parsedDepartureExpected = parseDateTimeExn m.DepartureDateTime
                let parsedDepartureScheduled = parseDateTimeExn m.ScheduledDepartureDateTime
                ({
                    safeRecord with
                        Arrival =  {
                            Expected = parsedArrivalExpected
                            Scheduled = parsedArrivalScheduled
                            BoardStatus = arrivalDue
                        }
                        Departure =  {
                            Expected = parsedDepartureExpected
                            Scheduled = parsedDepartureScheduled
                            BoardStatus = departureDue
                        }
                        HasLowFloor = lowFloorStatus
                        SourceTimeStamp = parsedSourceTimeStamp
                }, mapSucceeding)
            with :? FormatException -> (safeRecord,false)

        let public getRealTimeBusInformation (stopid:int) (route:string)
            : Async<Result<T, ApiError>> =
                [("stopid",stopid.ToString());("routeid",route)]
                |> buildUri defaultServiceEndpoint RealTimeBusInformation
                |> getEndpointContent defaultHandler
                >>> deserializeServiceResponseModel<RealTimeBusInformationModel>
                >>> validateServiceResponseModel
                >>< List.mapFold make true
                >>> fun (rs,mapSucceeded) ->
                        if mapSucceeded then (Ok {
                            StopId = stopid
                            Arrivals = rs
                        })
                        else Error InternalLibraryError

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
            LastUpdated  : DateTime;
            Stops        : BusStopInformation list;
        }

        let internal makeBusStopInformation (m:RouteStop) = {
            StopId = m.StopId
            DisplayedStopId = m.DisplayStopId
            ShortName = {
                EnglishName = m.ShortName
                IrishName = m.ShortNameLocalized
            }
            FullName = {
                EnglishName = m.FullName
                IrishName = m.FullNameLocalized
            }
            Latitude = m.Latitude
            Longitude = m.Longitude
        }

        let internal make mapSucceeding (m:RouteInformationModel) =
            let safeRecord = {
                OperatorName = m.OperatorName
                Origin = {
                    EnglishName = m.Origin
                    IrishName = m.OriginLocalized
                }
                Destination = {
                    EnglishName = m.Destination
                    IrishName = m.DestinationLocalized
                }
                LastUpdated = DateTime.MinValue
                Stops = List.map makeBusStopInformation m.Stops
            }
            if not(mapSucceeding) then (safeRecord,mapSucceeding) else
            try
                let parsedLastUpdated = parseDateTimeExn m.LastUpdated
                {safeRecord with LastUpdated=parsedLastUpdated},mapSucceeding
            with :? FormatException -> (safeRecord,false)

        let public getRouteInformation (route:string) (operatorReference:string)
            : Async<Result<T list, ApiError>> =
                [("routeid",route);("operator",operatorReference)]
                |> buildUri defaultServiceEndpoint RouteInformation
                |> getEndpointContent defaultHandler
                >>> deserializeServiceResponseModel<RouteInformationModel>
                >>> validateServiceResponseModel
                >>< List.mapFold make true 
                >>> fun (rs,mapSucceeded) ->
                        if mapSucceeded then Ok rs
                        else Error InternalLibraryError

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

[<assembly: InternalsVisibleTo("RealTimePassengerInformation.UnitTests")>]
do()
