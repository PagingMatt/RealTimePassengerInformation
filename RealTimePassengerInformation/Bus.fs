namespace RealTimePassengerInformation

open System
open System.Runtime.CompilerServices
open Service
open Service.Client
open Service.Endpoints
open Service.Models
open Shared.Operators

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

    module BusStopInformation =
        open Shared.Formatting

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

        let internal make (m:BusStopInformationModel) = {
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
            LastUpdated = DateTime.ParseExact(m.LastUpdated, serviceDateTimeFormat, inv)
            Operators = List.map (fun (o:StopOperator) -> {Name = o.OperatorName; Routes = o.Routes}) m.Operators
        }

        let getBusStopInformation (stopId:int) : Async<Result<T, ApiError>> =
            buildUri defaultServiceEndpoint BusStopInformation [("stopid",stopId.ToString())]
            |> getEndpointContent defaultHandler
            >>> deserializeServiceResponseModel<BusStopInformationModel>
            >>> validateServiceResponseModel
            >>> validateSingleResult
            >>< make

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
