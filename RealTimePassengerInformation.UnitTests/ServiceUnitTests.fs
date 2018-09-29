﻿namespace RealTimePassengerInformation.UnitTests

open Xunit

module Service =
    module Client =
        open RealTimePassengerInformation.Service.Client

    module Endpoints =
        open RealTimePassengerInformation.Service.Endpoints

        [<Fact>]
        let internal ``reduceParameters_EmptyList_ReturnsEmptyList`` () =
            Assert.Empty(reduceParameters [])

        [<Fact>]
        let internal ``reduceParameters_NoneValue_Removed`` () =
            Assert.Empty(reduceParameters [("Name1", None)])

        [<Fact>]
        let internal ``reduceParameters_SomeValue_Included`` () =
            let reducedParameters =
                reduceParameters [("Name1", (Some "Value1"))]
            Assert.NotEmpty(reducedParameters)
            Assert.Single(reducedParameters) |> ignore
            Assert.Equal(("Name1","Value1"), List.head reducedParameters)

        [<Fact>]
        let internal ``reduceParameters_MixedSomeNone_JustSomeIncluded`` () =
            let reducedParameters =
                reduceParameters
                    [
                        "Name1",(Some "Value1");
                        "Name2",None;
                        "Name3",(Some "Value3");
                        "Name4", None
                    ]
            Assert.Equal(2, List.length reducedParameters)
            match reducedParameters with
            | (name1,value1)::(name2,value2)::[] ->
                Assert.Equal("Name1", name1)
                Assert.Equal("Value1", value1)
                Assert.Equal("Name3", name2)
                Assert.Equal("Value3", value2)
            | _ -> Assert.True(false)

        [<Fact>]
        let internal ``buildUri_BusStopInformation_BuildsExpectedUri`` () =
            let expectedUri = defaultServiceEndpoint + "/busstopinformation?format=json"
            let actualUri = buildUri defaultServiceEndpoint BusStopInformation []
            Assert.Equal(expectedUri, actualUri)

        [<Fact>]
        let internal ``buildUri_OperatorInformation_BuildsExpectedUri`` () =
            let expectedUri = defaultServiceEndpoint + "/operatorinformation?format=json"
            let actualUri = buildUri defaultServiceEndpoint OperatorInformation []
            Assert.Equal(expectedUri, actualUri)

        [<Fact>]
        let internal ``buildUri_RealTimeBusInformation_BuildsExpectedUri`` () =
            let expectedUri = defaultServiceEndpoint + "/realtimebusinformation?format=json"
            let actualUri = buildUri defaultServiceEndpoint RealTimeBusInformation []
            Assert.Equal(expectedUri, actualUri)

        [<Fact>]
        let internal ``buildUri_RouteInformation_BuildsExpectedUri`` () =
            let expectedUri = defaultServiceEndpoint + "/routeinformation?format=json"
            let actualUri = buildUri defaultServiceEndpoint RouteInformation []
            Assert.Equal(expectedUri, actualUri)

        [<Fact>]
        let internal ``buildUri_RouteListInformation_BuildsExpectedUri`` () =
            let expectedUri = defaultServiceEndpoint + "/routelistinformation?format=json"
            let actualUri = buildUri defaultServiceEndpoint RouteListInformation []
            Assert.Equal(expectedUri, actualUri)

        [<Fact>]
        let internal ``buildUri_TimetableInformation_BuildsExpectedUri`` () =
            let expectedUri = defaultServiceEndpoint + "/timetableinformation?format=json"
            let actualUri = buildUri defaultServiceEndpoint TimetableInformation []
            Assert.Equal(expectedUri, actualUri)

        [<Fact>]
        let internal ``buildUri_SomeParameters_ParametersAppendedToUri`` () =
            let expectedUri = defaultServiceEndpoint + "/busstopinformation?k1=v1&k2=v2&format=json"
            let actualUri = buildUri defaultServiceEndpoint BusStopInformation [("k1","v1");("k2","v2")]
            Assert.Equal(expectedUri, actualUri)

    module Models =
        open RealTimePassengerInformation.Service
        open RealTimePassengerInformation.Service.Models
        open Xunit.Sdk

        [<Theory>]
        [<InlineData(@"}")>]
        let internal ``deserializeServiceResponseModel_InvalidJson_ErrorInternalLibraryError`` json =
            Assert.Equal(Error ExternalServiceError, deserializeServiceResponseModel json)

        [<Theory>]
        [<InlineData(@"{'errormessage':'','numberofresults':'1','timestamp':'','results':['a']}")>]
        [<InlineData(@"{'errorcode':'0','numberofresults':'1','timestamp':'','results':['a']}")>]
        [<InlineData(@"{'errorcode':'0','errormessage':'','timestamp':'','results':['a']}")>]
        [<InlineData(@"{'errorcode':'0','errormessage':'','numberofresults':'1','results':['a']}")>]
        [<InlineData(@"{'errorcode':'0','errormessage':'','numberofresults':'1','timestamp':''}")>]
        let internal ``deserializeServiceResponseModel_ValidJsonMissingRequiredField_ErrorInternalLibraryError`` json =
            Assert.Equal(Error ExternalServiceError, deserializeServiceResponseModel json)

        [<Theory>]
        [<InlineData(@"{'errorcode':'0','errormessage':'','numberofresults':'1','timestamp':'','results':['a']}")>]
        [<InlineData(@"{'errorcode':'0','errormessage':'','stopid':'1','numberofresults':'1','timestamp':'','results':['a']}")>]
        [<InlineData(@"{'errorcode':'0','errormessage':'','route':'1','numberofresults':'1','timestamp':'','results':['a']}")>]
        [<InlineData(@"{'errorcode':'0','errormessage':'','stopid':'1','route':'1','numberofresults':'1','timestamp':'','results':['a']}")>]
        let internal ``deserializeServiceResponseModel_ValidJson_OkDeserializedResult`` json =
            let result = deserializeServiceResponseModel<string> json
            match result with
            | Ok model ->
                Assert.Equal(ResponseCode.Success, model.ErrorCode)
                Assert.Equal("", model.ErrorMessage)
                Assert.Equal(1, model.NumberOfResults)
                Assert.Equal("", model.Timestamp)
                Assert.Single(model.Results) |> ignore
                Assert.Equal("a", model.Results.Head)
            | _ ->
                raise (XunitException("Deserialization of valid JSON failed."))
                
        [<Theory>]
        [<InlineData(@"{'errorcode':'0','errormessage':'','stopid':'1','numberofresults':'1','timestamp':'','results':['a']}")>]
        [<InlineData(@"{'errorcode':'0','errormessage':'','stopid':'1','route':'1','numberofresults':'1','timestamp':'','results':['a']}")>]
        let internal ``deserializeServiceResponseModel_ValidJsonOptionalProperties_OkDeserializedResultStopId`` json =
            let result = deserializeServiceResponseModel<string> json
            match result with
            | Ok model ->
                Assert.Equal(1, model.StopId.Value)
            | _ ->
                raise (XunitException("Deserialization of valid JSON failed."))
                
        [<Theory>]
        [<InlineData(@"{'errorcode':'0','errormessage':'','route':'1','numberofresults':'1','timestamp':'','results':['a']}")>]
        [<InlineData(@"{'errorcode':'0','errormessage':'','stopid':'1','route':'1','numberofresults':'1','timestamp':'','results':['a']}")>]
        let internal ``deserializeServiceResponseModel_ValidJsonOptionalProperties_OkDeserializedResultRoute`` json =
            let result = deserializeServiceResponseModel<string> json
            match result with
            | Ok model ->
                Assert.Equal("1", model.Route)
            | _ ->
                raise (XunitException("Deserialization of valid JSON failed."))
