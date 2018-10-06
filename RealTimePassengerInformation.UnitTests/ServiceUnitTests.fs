namespace RealTimePassengerInformation.UnitTests

open System
open System.Globalization
open System.Net
open System.Net.Http
open RealTimePassengerInformation.Service
open RealTimePassengerInformation.UnitTests.Helpers
open Xunit
open Xunit.Sdk

module Service =
    module Client =
        open RealTimePassengerInformation.Service.Client

        [<Fact>]
        let ``getEndpointContent_NullMessageHandler_ErrorUserError`` () =
            let client = {HttpHandler = null}
            let asyncResult = getEndpointContent client "http://localhost"
            Assert.Equal((Error UserError), Async.RunSynchronously asyncResult)

        [<Fact>]
        let ``getEndpointContent_NetworkIssue_ErrorNetworkError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler((Some (upcast new HttpRequestException())), None, None)}
            let asyncResult = getEndpointContent client "http://localhost"
            Assert.Equal((Error NetworkError), Async.RunSynchronously asyncResult)

        [<Fact>]
        let ``getEndpointContent_ContentReturned_OkContentAsString`` () =
            let content : HttpContent = upcast new StringContent("Content.")
            let client = {HttpHandler = new TestHttpMessageHandler(None, Some content, None)}
            let asyncResult = getEndpointContent client "http://localhost"
            Assert.Equal((Ok "Content."), Async.RunSynchronously asyncResult)

        [<Theory>]
        [<InlineData(HttpStatusCode.BadRequest)>]
        [<InlineData(HttpStatusCode.NotFound)>]
        [<InlineData(HttpStatusCode.InternalServerError)>]
        let ``getEndpointContent_NotSuccessStatusCode_ErrorInternalLibraryError`` code =
            let client = {HttpHandler = new TestHttpMessageHandler(None, None, (Some code))}
            let asyncResult = getEndpointContent client "http://localhost"
            Assert.Equal((Error InternalLibraryError), Async.RunSynchronously asyncResult)

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
        open RealTimePassengerInformation.Definitions
        open RealTimePassengerInformation.Service.Models

        [<Fact>]
        let internal ``serviceDateTimeFormat_ForDateTimeFromService_Parsable`` () =
            let sampleDateTimeString = "29/09/2018 20:24:35"
            let expectedDateTime = new DateTime(2018, 9, 29, 20, 24, 35)
            Assert.Equal(
                expectedDateTime,
                DateTime.ParseExact(
                    sampleDateTimeString,
                    serviceDateTimeFormat,
                    CultureInfo.InvariantCulture))

        [<Fact>]
        let internal ``serviceDateTimeFormat_ForTimeSpanFromService_Parsable`` () =
            let sampleTimeSpanString = "20:24:35"
            let expectedTimeSpan = new TimeSpan(20, 24, 35)
            Assert.Equal(
                expectedTimeSpan,
                TimeSpan.ParseExact(
                    sampleTimeSpanString,
                    serviceTimeSpanFormat,
                    CultureInfo.InvariantCulture))

        [<Theory>]
        [<InlineData(@"}")>]
        let internal ``deserializeServiceResponseModel_InvalidJson_ErrorInternalLibraryError`` json =
            Assert.Equal(Error InternalLibraryError, deserializeServiceResponseModel json)

        [<Theory>]
        [<InlineData(@"{'errormessage':'','numberofresults':'1','timestamp':'','results':['a']}")>]
        [<InlineData(@"{'errorcode':'0','numberofresults':'1','timestamp':'','results':['a']}")>]
        [<InlineData(@"{'errorcode':'0','errormessage':'','timestamp':'','results':['a']}")>]
        [<InlineData(@"{'errorcode':'0','errormessage':'','numberofresults':'1','results':['a']}")>]
        [<InlineData(@"{'errorcode':'0','errormessage':'','numberofresults':'1','timestamp':''}")>]
        let internal ``deserializeServiceResponseModel_ValidJsonMissingRequiredField_ErrorInternalLibraryError`` json =
            Assert.Equal(Error InternalLibraryError, deserializeServiceResponseModel json)

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

        [<Fact>]
        let ``validateServiceResponseModel_ErrorCodeSuccess_OkResults`` () =
            let model = new ServiceResponseModel<string>(ResponseCode.Success, "", new Nullable<int>(), "", 3, "", ["a";"b";"c"])
            let validationResult = validateServiceResponseModel model
            Assert.Equal((Ok ["a";"b";"c"]), validationResult)

        [<Fact>]
        let ``validateServiceResponseModel_ErrorCodeNoResults_ErrorNoResults`` () =
            let model = new ServiceResponseModel<string>(ResponseCode.NoResults, "", new Nullable<int>(), "", 0, "", [])
            let validationResult = validateServiceResponseModel model
            Assert.Equal((Error NoResults), validationResult)

        [<Fact>]
        let ``validateServiceResponseModel_ErrorCodeMissingParameter_ErrorInternalLibraryError`` () =
            let model = new ServiceResponseModel<string>(ResponseCode.MissingParameter, "", new Nullable<int>(), "", 0, "", [])
            let validationResult = validateServiceResponseModel model
            Assert.Equal((Error InternalLibraryError), validationResult)

        [<Fact>]
        let ``validateServiceResponseModel_ErrorCodeInvalidParameter_ErrorInternalLibraryError`` () =
            let model = new ServiceResponseModel<string>(ResponseCode.InvalidParameter, "", new Nullable<int>(), "", 0, "", [])
            let validationResult = validateServiceResponseModel model
            Assert.Equal((Error InternalLibraryError), validationResult)

        [<Fact>]
        let ``validateServiceResponseModel_ErrorCodeScheduledDowntime_ErrorScheduledServiceDowntime`` () =
            let model = new ServiceResponseModel<string>(ResponseCode.ScheduledDowntime, "", new Nullable<int>(), "", 0, "", [])
            let validationResult = validateServiceResponseModel model
            Assert.Equal((Error ScheduledServiceDowntime), validationResult)

        [<Fact>]
        let ``validateServiceResponseModel_ErrorCodeScheduledDowntime_ErrorServiceError`` () =
            let model = new ServiceResponseModel<string>(ResponseCode.UnexpectedSystemError, "", new Nullable<int>(), "", 0, "", [])
            let validationResult = validateServiceResponseModel model
            Assert.Equal((Error ServiceError), validationResult)

        [<Fact>]
        let ``validateServiceResponseModel_ErrorCodeUnknownEnumValue_ErrorInternalLibraryError`` () =
            let model = new ServiceResponseModel<string>(enum<ResponseCode>(-1), "", new Nullable<int>(), "", 0, "", [])
            let validationResult = validateServiceResponseModel model
            Assert.Equal((Error InternalLibraryError), validationResult)

        [<Fact>]
        let internal ``validateSingleResult_EmptyList_ErrorInternalLibraryError`` () =
            Assert.Equal(Error InternalLibraryError, validateSingleResult [])

        [<Fact>]
        let internal ``validateSingleResult_TwoElementList_ErrorInternalLibraryError`` () =
            Assert.Equal(Error InternalLibraryError, validateSingleResult ["a";"b"])

        [<Fact>]
        let internal ``validateSingleResult_OneElementList_OkWithTheElement`` () =
            let element = "a"
            Assert.Equal(Ok element, validateSingleResult [element])