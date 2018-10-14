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
    
    let sampleRoute = "4"

    let sampleOperatorReference = "bac"

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

        [<Fact>]
        let ``getFullTimetableInformation_E2E`` () =
            let result =
                getFullTimetableInformation defaultClient sampleBusStop sampleRoute
                |> Async.RunSynchronously
            match result with
            | Error err ->
                raise (
                    new XunitException(
                        String.Format(
                            "E2E test returned failure result '{0}'.",
                            err.ToString())))
            | Ok _      -> ignore ()

    module OperatorInformation =
        open RealTimePassengerInformation.Bus.OperatorInformation

        [<Fact>]
        let ``getOperatorInformation_E2E`` () =
            let result =
                getOperatorInformation defaultClient
                |> Async.RunSynchronously
            match result with
            | Error err ->
                raise (
                    new XunitException(
                        String.Format(
                            "E2E test returned failure result '{0}'.",
                            err.ToString())))
            | Ok _      -> ignore ()

    module RealTimeBusInformation =
        open RealTimePassengerInformation.Bus.RealTimeBusInformation

        // TODO - need to accept 'No results' as success.

    module RouteInformation =
        open RealTimePassengerInformation.Bus.RouteInformation

        [<Fact>]
        let ``getRouteInformation_E2E`` () =
            let result =
                getRouteInformation defaultClient sampleBusStop sampleRoute sampleOperatorReference
                |> Async.RunSynchronously
            match result with
            | Error err ->
                raise (
                    new XunitException(
                        String.Format(
                            "E2E test returned failure result '{0}'.",
                            err.ToString())))
            | Ok _      -> ignore ()

    module RouteListInformation =
        open RealTimePassengerInformation.Bus.RouteListInformation