namespace RealTimePassengerInformation

open System
open System.Runtime.CompilerServices
open Service
open RealTimePassengerInformation.Definitions
open RealTimePassengerInformation.Service.Client
open RealTimePassengerInformation.Service.Endpoints
open RealTimePassengerInformation.Service.Models
open RealTimePassengerInformation.Shared.Formatting
open RealTimePassengerInformation.Shared.Operators

module Bus =
    /// <summary>
    /// Bus stop names in RTPI are in both Irish and English.
    /// </summary>
    type public BusStopName = {
        EnglishName : string;
        IrishName   : string;
    }

    /// <summary>
    /// Timetables in RTPI are per-day and so this union type is used to
    /// strongly type this.
    /// </summary>
    type public Day =
        | Sunday
        | Monday
        | Tuesday
        | Wednesday
        | Thursday
        | Friday
        | Saturday

    let internal deserializeDay day =
        match day with
        | "Sunday"    -> Some Sunday
        | "Monday"    -> Some Monday
        | "Tuesday"   -> Some Tuesday
        | "Wednesday" -> Some Wednesday
        | "Thursday"  -> Some Thursday
        | "Friday"    -> Some Friday
        | "Saturday"  -> Some Saturday
        | _           -> None

    let internal parseDateTimeExn s =
        DateTime.ParseExact(s, serviceDateTimeFormat, inv)

    let internal parseTimeSpanExn s =
        TimeSpan.ParseExact(s, serviceTimeSpanFormat, inv)

    module BusStopInformation =
        /// <summary>
        /// Operators of a given bus stop have a name and a collection of
        /// routes that serve that bus stop.
        /// </summary>
        type public BusStopOperator = {
            Name   : string;
            Routes : string list;
        }

        /// <summary>
        /// BusStopInformation type capturing all meta-data about a given bus
        /// stop.
        /// </summary>
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

        /// <summary>
        /// Asynchronously gets information about a given bus stop.
        /// </summary>
        /// <param name="client">
        /// Client type to pass through when calling the service.
        /// </param>
        /// <param name="stopId">
        /// ID of the bus stop to get information for.
        /// </param>
        /// <returns>
        /// Information about the bus stop with ID <param name="stopId" />.
        /// </returns>
        let public getBusStopInformation (client:Client.T) (stopId:int) : Async<Result<T, ApiError>> =
            [("stopid",stopId.ToString())]
            |> buildUri defaultServiceEndpoint BusStopInformation
            |> getEndpointContent client
            >>> deserializeServiceResponseModel<BusStopInformationModel>
            >>> validateServiceResponseModel
            >>> validateSingleResult
            >>> make

    module FullTimeTableInformation =
        /// <summary>
        /// Timetable entries have a range limited by a start and end day.
        /// They have a destination which is the name of the terminating bus
        /// stop, a list of times when buses depart and the DateTime of the
        /// last time the timetable entry was updated in RTPI.
        /// </summary>
        type public TimeTableEntry = {
            StartDayOfWeek : Day;
            EndDayOfWeek   : Day;
            Destination    : BusStopName;
            LastUpdated    : DateTime
            Departures     : TimeSpan list
        }

        /// <summary>
        /// FullTimeTableInformation type capturing all timetable information
        /// for a given route which serves a given bus stop.
        /// </summary>
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

        /// <summary>
        /// Asynchronously gets the timetable information for a given route
        /// serving a given bus stop.
        /// </summary>
        /// <param name="client">
        /// Client type to pass through when calling the service.
        /// </param>
        /// <param name="stopId">
        /// ID of the bus stop to get timetable information for.
        /// </param>
        /// <param name="route">
        /// Route serving bus stop to get timetable information for.
        /// </param>
        /// <returns>
        /// Timetable information for the <param name="route" /> serving the bus
        /// stop with ID <param name="stopId" />.
        /// </returns>
        let public getFullTimetableInformation (client:Client.T) (stopId:int) (route:string)
            : Async<Result<T, ApiError>> =
                [("type","week");("stopid",stopId.ToString());("routeid",route)]
                |> buildUri defaultServiceEndpoint TimetableInformation
                |> getEndpointContent client
                >>> deserializeServiceResponseModel<FullTimetableBusInformationModel>
                >>> validateServiceResponseModel
                >>< List.mapFold make true
                >>> fun (rs,mapSucceeded) ->
                        if mapSucceeded then (Ok {
                            StopId = stopId
                            Route = route
                            TimeTableEntries = rs
                        })
                        else Error InternalLibraryError

    module OperatorInformation =
        /// <summary>
        /// Operator meta-data has a reference code unique to the operator, a
        /// name and a description.
        /// </summary>
        type public Operator = {
            ReferenceCode : string;
            Name          : string;
            Description   : string;
        }

        /// <summary>
        /// OperatorInformation type is a collection of meta-data for route
        /// operators.
        /// </summary>
        type public T = Operator list

        let internal make (m:OperatorInformationModel) = {
            ReferenceCode = m.OperatorReference
            Name = m.OperatorName
            Description = m.OperatorDescription
        }

        /// <summary>
        /// Asynchronously gets the meta-data for all route operators known to
        /// RTPI.
        /// </summary>
        /// <param name="client">
        /// Client type to pass through when calling the service.
        /// </param>
        /// <returns>
        /// Meta-data for all route operators known to RTPI.
        /// </returns>
        let public getOperatorInformation (client:Client.T) : Async<Result<T, ApiError>> =
            buildUri defaultServiceEndpoint OperatorInformation []
            |> getEndpointContent client
            >>> deserializeServiceResponseModel<OperatorInformationModel>
            >>> validateServiceResponseModel
            >>< List.map make

    module RealTimeBusInformation =
        /// <summary>
        /// The status of arrivals seen on bus stop electronic boards is a union
        /// of 'Due' and the number of minutes until 'Due'.
        /// </summary>
        type public BoardStatus =
            | Due
            | ExpectedInMinutes of int

        /// <summary>
        /// Buses are either going in the 'Inbound' or 'Outbound' directions.
        /// </summary>
        type public Direction =
            | Inbound
            | Outbound

        /// <summary>
        /// Times in data from the real-time endpoint have three properties:
        /// expected time, originally scheduled time and the status on the bus
        /// stop electronic board.
        /// </summary>
        type public RealTimeSlot = {
            Expected    : DateTime;
            Scheduled   : DateTime;
            BoardStatus : BoardStatus;
        }

        /// <summary>
        /// Information about real-time arrivals includes the arrival/departure
        /// time-slots, the origin/destination bus stops, the direction, the
        /// operator, any additional information about the arrival, the low
        /// floor status of the arriving bus, the route and the source time of
        /// the information.
        /// </summary>
        type public RealTimeArrivalInformation = {
            Arrival               : RealTimeSlot;
            Departure             : RealTimeSlot;
            Origin                : BusStopName;
            Destination           : BusStopName;
            Direction             : Direction;
            OperatorReferenceCode : string;
            AdditionalInformation : string;
            HasLowFloor           : bool;
            Route                 : string;
            SourceTimeStamp       : DateTime;
        }

        /// <summary>
        /// RealTimeBusInformation type capturing all real-time arrivals for a
        /// given bus stop.
        /// </summary>
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
            Direction = Inbound
            OperatorReferenceCode = m.OperatorReferenceCode
            AdditionalInformation = m.AdditionalInformation
            HasLowFloor = false
            Route = m.Route
            SourceTimeStamp = DateTime.MinValue
        }

        let internal deserializeBoardStatus boardStatus =
            match boardStatus with
            | "Due" -> Some Due
            | mins  ->
                try Some (ExpectedInMinutes (Int32.Parse mins))
                with :? FormatException -> None

        let internal deserializeDirection direction =
            match direction with
            | "Inbound"  -> Some Inbound
            | "Outbound" -> Some Outbound
            | _          -> None

        let internal deserializeLowFloorStatus lowFloorStatus =
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
                match deserializeDirection m.Direction with
                | None -> (safeRecord,false)
                | Some direction ->
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
                        Direction = direction
                        HasLowFloor = lowFloorStatus
                        SourceTimeStamp = parsedSourceTimeStamp
                }, mapSucceeding)
            with :? FormatException -> (safeRecord,false)

        /// <summary>
        /// Asynchronously gets the real-time arrival information for a given
        /// bus stop.
        /// </summary>
        /// <param name="client">
        /// Client type to pass through when calling the service.
        /// </param>
        /// <param name="stopId">
        /// ID of the bus stop to get real-time arrival information for.
        /// </param>
        /// <returns>
        /// Real-time arrival information for the bus stop with ID
        /// <param name="stopId" />.
        /// </returns>
        let public getRealTimeBusInformation (client:Client.T) (stopid:int)
            : Async<Result<T, ApiError>> =
                [("stopid",stopid.ToString())]
                |> buildUri defaultServiceEndpoint RealTimeBusInformation
                |> getEndpointContent client
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
        /// <summary>
        /// Information about a bus stop served by a route includes the
        /// identifier for the bus stop, the short/full names for the stop and
        /// the location of the stop.
        /// </summary>
        type public BusStopInformation = {
            StopId          : int;
            DisplayedStopId : int;
            ShortName       : BusStopName;
            FullName        : BusStopName;
            Latitude        : float;
            Longitude       : float;
        }

        /// <summary>
        /// RouteInformation type captures information about a given route run
        /// by a given operator including origin/destination and all stops
        /// served.
        /// </summary>
        type public T = {
            Route        : string;
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

        let internal make route mapSucceeding (m:RouteInformationModel) =
            let safeRecord = {
                Route = route
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

        /// <summary>
        /// Asynchronously gets information about a given route run by a given
        /// operator.
        /// </summary>
        /// <param name="client">
        /// Client type to pass through when calling the service.
        /// </param>
        /// <param name="route">
        /// Route to get information for.
        /// </param>
        /// <param name="operatorReference">
        /// Reference code of operator running the route to get information
        /// for.
        /// </param>
        /// <returns>
        /// Information about <param name="route" /> run by given operator with
        /// reference code <param name="operatorReference" />.
        /// </returns>
        let public getRouteInformation(client:Client.T) (route:string) (operatorReference:string)
            : Async<Result<T list, ApiError>> =
                [("routeid",route);("operator",operatorReference)]
                |> buildUri defaultServiceEndpoint RouteInformation
                |> getEndpointContent client
                >>> deserializeServiceResponseModel<RouteInformationModel>
                >>> validateServiceResponseModel
                >>< List.mapFold (make route) true
                >>> fun (rs,mapSucceeded) ->
                        if mapSucceeded then Ok rs
                        else Error InternalLibraryError

    module RouteListInformation =
        /// <summary>
        /// RouteListInformation type capturing all routes run by a given
        /// operator.
        /// </summary>
        type public T = {
            OperatorReferenceCode : string;
            Routes                : string list;
        }

        let internal foldOrderedOperatorRouteList acc (m:RouteListInformationModel) =
            match acc with
            | []         -> (m.OperatorReference, m.Route::[])::[]
            | (o,rs)::os ->
                if o = m.OperatorReference then (o,m.Route::rs)::os
                else (m.OperatorReference, m.Route::[])::acc

        let internal getRouteListModel (client:Client.T) args =
            buildUri defaultServiceEndpoint RouteListInformation args
            |> getEndpointContent client
            >>> deserializeServiceResponseModel<RouteListInformationModel>
            >>> validateServiceResponseModel

        let internal groupByOperator models =
            List.sortBy (fun (m:RouteListInformationModel) -> m.OperatorReference) models
            |> List.fold foldOrderedOperatorRouteList []

        /// <summary>
        /// Asynchronously gets information about all routes run by all
        /// operators.
        /// </summary>
        /// <param name="client">
        /// Client type to pass through when calling the service.
        /// </param>
        /// <returns>
        /// Information about all routes run by all operators.
        /// </returns>
        let public getRouteListInformation (client:Client.T) : Async<Result<T list, ApiError>> =
                getRouteListModel client []
                >>< groupByOperator
                >>< List.map (fun (o,rs) -> {OperatorReferenceCode = o; Routes = rs})

        /// <summary>
        /// Asynchronously gets information about all routes run by a given
        /// operator.
        /// </summary>
        /// <param name="client">
        /// Client type to pass through when calling the service.
        /// </param>
        /// <param name="operatorReferenceCode">
        /// Reference code of operator to get information for about all the
        /// routes it runs.
        /// </param>
        /// <returns>
        /// Information about all routes run by operator with reference code
        /// <param name="operatorReferenceCode" />
        /// </returns>
        let public getRouteListInformationForOperator (client:Client.T) operatorReferenceCode
            : Async<Result<T, ApiError>> =
                getRouteListModel client [("operator",operatorReferenceCode)]
                >>< groupByOperator
                >>> fun os ->
                        match os with
                        | (o,rs)::[] -> Ok {OperatorReferenceCode=o; Routes=rs;}
                        | _          -> Error InternalLibraryError

[<assembly: InternalsVisibleTo("RealTimePassengerInformation.UnitTests")>]
do()
