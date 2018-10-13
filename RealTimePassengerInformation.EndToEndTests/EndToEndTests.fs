namespace RealTimePassengerInformation.EndToEndTests

open System
open RealTimePassengerInformation.Definitions
open RealTimePassengerInformation.Service
open RealTimePassengerInformation.Service.Client
open Xunit
open Xunit.Sdk

module Bus =
    // Use "O'Connell St, O'Connell Bridge" as a sample stop
    let sampleBusStop = 273

    module BusStopInformation =
        open RealTimePassengerInformation.Bus.BusStopInformation

        [<Fact>]
        let ``getBusStopInformation_E2E`` () =
            let result =
                getBusStopInformation defaultClient sampleBusStop
                |> Async.RunSynchronously
            match result with
            | Error err ->
                raise (
                    new XunitException(
                        String.Format(
                            "E2E test returned failure result '{0}'.",
                            err.ToString())))
            | Ok _      -> ignore ()

    module FullTimeTableInformation =
        open RealTimePassengerInformation.Bus.FullTimeTableInformation

    module OperatorInformation =
        open RealTimePassengerInformation.Bus.OperatorInformation

    module RealTimeBusInformation =
        open RealTimePassengerInformation.Bus.RealTimeBusInformation

    module RouteInformation =
        open RealTimePassengerInformation.Bus.RouteInformation

    module RouteListInformation =
        open RealTimePassengerInformation.Bus.RouteListInformation