namespace RealTimePassengerInformation.UnitTests

open System
open System.Net
open System.Net.Http
open RealTimePassengerInformation.Service
open RealTimePassengerInformation.Service.Client
open RealTimePassengerInformation.Service.Models
open RealTimePassengerInformation.UnitTests.Helpers
open Xunit
open Xunit.Sdk

module Bus =
    open RealTimePassengerInformation.Bus

    [<Fact>]
    let internal ``deserializeDay_Sunday_SomeSunday`` () =
        Assert.Equal((Some Sunday), (deserializeDay "Sunday"))

    [<Fact>]
    let internal ``deserializeDay_Monday_SomeMonday`` () =
        Assert.Equal((Some Monday), (deserializeDay "Monday"))

    [<Fact>]
    let internal ``deserializeDay_Tuesday_SomeTuesday`` () =
        Assert.Equal((Some Tuesday), (deserializeDay "Tuesday"))

    [<Fact>]
    let internal ``deserializeDay_Wednesday_SomeWednesday`` () =
        Assert.Equal((Some Wednesday), (deserializeDay "Wednesday"))

    [<Fact>]
    let internal ``deserializeDay_Thursday_SomeThursday`` () =
        Assert.Equal((Some Thursday), (deserializeDay "Thursday"))

    [<Fact>]
    let internal ``deserializeDay_Friday_SomeFriday`` () =
        Assert.Equal((Some Friday), (deserializeDay "Friday"))

    [<Fact>]
    let internal ``deserializeDay_Saturday_SomeSaturday`` () =
        Assert.Equal((Some Saturday), (deserializeDay "Saturday"))

    [<Theory>]
    [<InlineData(null)>]
    [<InlineData("")>]
    [<InlineData("foo")>]
    let internal ``deserializeDay_Unrecognised_None`` day =
        Assert.Equal(None, (deserializeDay day))

    [<Theory>]
    [<InlineData("sunday")>]
    [<InlineData("monday")>]
    [<InlineData("tuesday")>]
    [<InlineData("wednesday")>]
    [<InlineData("thursday")>]
    [<InlineData("friday")>]
    [<InlineData("saturday")>]
    let internal ``deserializeDay_ValidDayLowerCase_None`` day =
        Assert.Equal(None, (deserializeDay day))

    [<Fact>]
    let internal ``parseDateTimeExn_InvalidFormat_ThrowsFormatException`` () =
        let invalidDateTime = "2018-10-04T23:27:00Z"
        let throwingAction () = (parseDateTimeExn invalidDateTime |> ignore)
        Assert.Throws<FormatException>(throwingAction)

    [<Fact>]
    let internal ``parseDateTimeExn_ValidFormat_Parsable`` () =
        let sampleDateTimeString = "29/09/2018 20:24:35"
        let expectedDateTime = new DateTime(2018, 9, 29, 20, 24, 35)
        Assert.Equal(
            expectedDateTime,
            parseDateTimeExn sampleDateTimeString)

    [<Fact>]
    let internal ``parseTimeSpanExn_InvalidFormat_ThrowsFormatException`` () =
        let invalidTimeSpan = "23:27:00Z"
        let throwingAction () = (parseTimeSpanExn invalidTimeSpan |> ignore)
        Assert.Throws<FormatException>(throwingAction)

    [<Fact>]
    let internal ``parseTimeSpanExn_ValidFormat_Parsable`` () =
        let sampleTimeSpanString = "20:24:35"
        let expectedTimeSpan = new TimeSpan(20, 24, 35)
        Assert.Equal(
            expectedTimeSpan,
            parseTimeSpanExn sampleTimeSpanString)

    module BusStopInformation =
        open RealTimePassengerInformation.Bus.BusStopInformation

        [<Fact>]
        let ``make_LastUpdatedInvalidFormat_ErrorInternalLibraryError`` () =
            let model = new BusStopInformationModel(1, 2, "a", "b", "c", "d", 0.1, 0.2, "2018-10-04T23:27:00Z", [])
            Assert.Equal((Error InternalLibraryError), (make model))

        [<Fact>]
        let ``make_ValidModel_OkStopIdSetToStopIdInModel`` () =
            let model = new BusStopInformationModel(1, 2, "a", "b", "c", "d", 0.1, 0.2, "04/10/2018 23:27:00", [])
            match make model with
            | Ok result -> Assert.Equal(1, result.StopId)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

        [<Fact>]
        let ``make_ValidModel_OkDisplayStopIdSetToDisplayStopIdInModel`` () =
            let model = new BusStopInformationModel(1, 2, "a", "b", "c", "d", 0.1, 0.2, "04/10/2018 23:27:00", [])
            match make model with
            | Ok result -> Assert.Equal(2, result.DisplayedStopId)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

        [<Fact>]
        let ``make_ValidModel_OkShortEnglishNameSetToShortNameInModel`` () =
            let model = new BusStopInformationModel(1, 2, "a", "b", "c", "d", 0.1, 0.2, "04/10/2018 23:27:00", [])
            match make model with
            | Ok result -> Assert.Equal("a", result.ShortName.EnglishName)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

        [<Fact>]
        let ``make_ValidModel_OkShortIrishNameSetToShortNameLocalizedInModel`` () =
            let model = new BusStopInformationModel(1, 2, "a", "b", "c", "d", 0.1, 0.2, "04/10/2018 23:27:00", [])
            match make model with
            | Ok result -> Assert.Equal("b", result.ShortName.IrishName)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

        [<Fact>]
        let ``make_ValidModel_OkFullEnglishNameSetToFullNameInModel`` () =
            let model = new BusStopInformationModel(1, 2, "a", "b", "c", "d", 0.1, 0.2, "04/10/2018 23:27:00", [])
            match make model with
            | Ok result -> Assert.Equal("c", result.FullName.EnglishName)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

        [<Fact>]
        let ``make_ValidModel_OkFullIrishNameSetToFullNameLocalizedInModel`` () =
            let model = new BusStopInformationModel(1, 2, "a", "b", "c", "d", 0.1, 0.2, "04/10/2018 23:27:00", [])
            match make model with
            | Ok result -> Assert.Equal("d", result.FullName.IrishName)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

        [<Fact>]
        let ``make_ValidModel_OkLatitudeSetToLatitudeInModel`` () =
            let model = new BusStopInformationModel(1, 2, "a", "b", "c", "d", 0.1, 0.2, "04/10/2018 23:27:00", [])
            match make model with
            | Ok result -> Assert.Equal(0.1, result.Latitude)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

        [<Fact>]
        let ``make_ValidModel_OkLatitudeSetToLongitudeInModel`` () =
            let model = new BusStopInformationModel(1, 2, "a", "b", "c", "d", 0.1, 0.2, "04/10/2018 23:27:00", [])
            match make model with
            | Ok result -> Assert.Equal(0.2, result.Longitude)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

        [<Fact>]
        let ``make_ValidModel_OkLastUpdatedSetToLastUpdatedInModel`` () =
            let model = new BusStopInformationModel(1, 2, "a", "b", "c", "d", 0.1, 0.2, "04/10/2018 23:27:00", [])
            match make model with
            | Ok result -> Assert.Equal(new DateTime(2018, 10, 4, 23, 27, 0), result.LastUpdated)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

        [<Fact>]
        let ``make_ValidModel_OkOperatorsMappedFromOperatorsInModel`` () =
            let stopOperator = new StopOperator("e", ["f";"g"])
            let model = new BusStopInformationModel(1, 2, "a", "b", "c", "d", 0.1, 0.2, "04/10/2018 23:27:00", [stopOperator])
            match make model with
            | Ok result -> Assert.Equal<BusStopOperator list>([{Name="e"; Routes=["f";"g"]}], result.Operators)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

        [<Fact>]
        let ``getBusStopInformation_ServiceErrorInResponseStatusCode_ErrorInternalLibraryError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler(None, None, (Some HttpStatusCode.BadRequest))}
            let result = getBusStopInformation client 1
            Assert.Equal<Result<BusStopInformation.T, ApiError>>(Error InternalLibraryError, Async.RunSynchronously result)

        [<Fact>]
        let ``getBusStopInformation_UserErrorInClientSetup_ErrorUserError`` () =
            let client = {HttpHandler = null}
            let result = getBusStopInformation client 1
            Assert.Equal<Result<BusStopInformation.T, ApiError>>(Error UserError, Async.RunSynchronously result)

        [<Fact>]
        let ``getBusStopInformation_NetworkErrorOnRequest_ErrorNetworkError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler((Some (upcast new HttpRequestException())), None, None)}
            let result = getBusStopInformation client 1
            Assert.Equal<Result<BusStopInformation.T, ApiError>>(Error NetworkError, Async.RunSynchronously result)

        [<Fact>]
        let ``getBusStopInformation_CannotDeserializeClientResponse_ErrorInternalLibraryError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent("{"))), None)}
            let result = getBusStopInformation client 1
            Assert.Equal<Result<BusStopInformation.T, ApiError>>(Error InternalLibraryError, Async.RunSynchronously result)

        [<Theory>]
        [<InlineData(@"{'errorcode':'1','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'2','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'3','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'4','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'5','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'6','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        let ``getBusStopInformation_ResultIsServiceFailureResult_Error`` response =
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent(response))), None)}
            let result = getBusStopInformation client 1
            match Async.RunSynchronously result with
            | Error _ -> ignore
            | Ok _    -> raise (new XunitException("Error response code did not fail call."))

        [<Fact>]
        let ``getBusStopInformation_ResultIsNotSingleResult_ErrorInternalLibraryError`` () =
            let responseResult = @"{'stopid':'0','displaystopid':'0','shortname':'','shortnamelocalized':'','fullname':'','fullnamelocalized':'','latitude':'0.1','longitude':'0.2','lastupdated':'','operators':[]}"
            let response = @"{'errorcode':'0','errormessage':'','numberofresults':'2','timestamp':'','results':[" + responseResult + "," + responseResult + "]}"
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent(response))), None)}
            let result = getBusStopInformation client 1
            Assert.Equal<Result<BusStopInformation.T, ApiError>>(Error InternalLibraryError, Async.RunSynchronously result)

        [<Fact>]
        let ``getBusStopInformation_ResponseValid_OkResponse`` () =
            let responseResult = @"{'stopid':'1','displaystopid':'2','shortname':'a','shortnamelocalized':'b','fullname':'c','fullnamelocalized':'d','latitude':'0.1','longitude':'0.2','lastupdated':'06/10/2018 16:15:00','operators':[{'name':'e','routes':['f']}]}"
            let response = @"{'errorcode':'0','errormessage':'','numberofresults':'1','timestamp':'','results':[" + responseResult + "]}"
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent(response))), None)}
            let unwrappedResult = getBusStopInformation client 1 |> Async.RunSynchronously
            match unwrappedResult with
            | Error _   -> raise (new XunitException("Valid response should have 'Ok ...' result."))
            | Ok result ->
                Assert.Equal(1, result.StopId)
                Assert.Equal(2, result.DisplayedStopId)
                Assert.Equal("a", result.ShortName.EnglishName)
                Assert.Equal("b", result.ShortName.IrishName)
                Assert.Equal("c", result.FullName.EnglishName)
                Assert.Equal("d", result.FullName.IrishName)
                Assert.Equal(0.1, result.Latitude)
                Assert.Equal(0.2, result.Longitude)
                Assert.Equal(new DateTime(2018, 10, 6, 16, 15, 0), result.LastUpdated)
                Assert.Equal<BusStopOperator list>([{Name="e"; Routes=["f"]}], result.Operators)

    module FullTimeTableInformation =
        open RealTimePassengerInformation.Bus.FullTimeTableInformation

        [<Fact>]
        let ``makeSafe_ValidModel_SetsStartDayOfWeekToSunday`` () =
            let model = new FullTimetableBusInformationModel("Monday", "Tuesday", "a", "b", "04/10/2018 23:27:00", ["c"])
            Assert.Equal(Sunday, (makeSafe model).StartDayOfWeek)

        [<Fact>]
        let ``makeSafe_ValidModel_SetsEndDayOfWeekToSunday`` () =
            let model = new FullTimetableBusInformationModel("Monday", "Tuesday", "a", "b", "04/10/2018 23:27:00", ["c"])
            Assert.Equal(Sunday, (makeSafe model).EndDayOfWeek)

        [<Fact>]
        let ``makeSafe_ValidModel_SetsDestinationEnglishNameToDestinationInModel`` () =
            let model = new FullTimetableBusInformationModel("Monday", "Tuesday", "a", "b", "04/10/2018 23:27:00", ["c"])
            Assert.Equal("a", (makeSafe model).Destination.EnglishName)

        [<Fact>]
        let ``makeSafe_ValidModel_SetsDestinationIrishNameToDestinationLocalizedInModel`` () =
            let model = new FullTimetableBusInformationModel("Monday", "Tuesday", "a", "b", "04/10/2018 23:27:00", ["c"])
            Assert.Equal("b", (makeSafe model).Destination.IrishName)

        [<Fact>]
        let ``makeSafe_ValidModel_SetsLastUpdatedToDateTimeMin`` () =
            let model = new FullTimetableBusInformationModel("Monday", "Tuesday", "a", "b", "04/10/2018 23:27:00", ["c"])
            Assert.Equal(DateTime.MinValue, (makeSafe model).LastUpdated)

        [<Fact>]
        let ``makeSafe_ValidModel_SetsDeparturesToEmptyList`` () =
            let model = new FullTimetableBusInformationModel("Monday", "Tuesday", "a", "b", "04/10/2018 23:27:00", ["c"])
            Assert.Equal<TimeSpan list>([], (makeSafe model).Departures)

        [<Fact>]
        let ``getFullTimetableInformation_ServiceErrorInResponseStatusCode_ErrorInternalLibraryError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler(None, None, (Some HttpStatusCode.BadRequest))}
            let result = getFullTimetableInformation client 1 "1"
            Assert.Equal<Result<FullTimeTableInformation.T, ApiError>>(Error InternalLibraryError, Async.RunSynchronously result)

        [<Fact>]
        let ``getFullTimetableInformation_UserErrorInClientSetup_ErrorUserError`` () =
            let client = {HttpHandler = null}
            let result = getFullTimetableInformation client 1 "1"
            Assert.Equal<Result<FullTimeTableInformation.T, ApiError>>(Error UserError, Async.RunSynchronously result)

        [<Fact>]
        let ``getFullTimetableInformation_NetworkErrorOnRequest_ErrorNetworkError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler((Some (upcast new HttpRequestException())), None, None)}
            let result = getFullTimetableInformation client 1 "1"
            Assert.Equal<Result<FullTimeTableInformation.T, ApiError>>(Error NetworkError, Async.RunSynchronously result)

        [<Fact>]
        let ``getFullTimetableInformation_CannotDeserializeClientResponse_ErrorInternalLibraryError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent("{"))), None)}
            let result = getFullTimetableInformation client 1 "1"
            Assert.Equal<Result<FullTimeTableInformation.T, ApiError>>(Error InternalLibraryError, Async.RunSynchronously result)

        [<Theory>]
        [<InlineData(@"{'errorcode':'1','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'2','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'3','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'4','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'5','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'6','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        let ``getFullTimetableInformation_ResultIsServiceFailureResult_Error`` response =
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent(response))), None)}
            let result = getFullTimetableInformation client 1 "1"
            match Async.RunSynchronously result with
            | Error _ -> ignore
            | Ok _    -> raise (new XunitException("Error response code did not fail call."))

        [<Theory>]
        [<InlineData(@"{'startdayofweek':'','enddayofweek':'Tuesday','destination':'a','destinationlocalized':'b','lastupdated':'06/10/2018 16:15:00','departures':['01:01:01','02:02:02']}")>]
        [<InlineData(@"{'startdayofweek':'Monday','enddayofweek':'','destination':'a','destinationlocalized':'b','lastupdated':'06/10/2018 16:15:00','departures':['01:01:01','02:02:02']}")>]
        [<InlineData(@"{'startdayofweek':'Monday','enddayofweek':'Tuesday','destination':'a','destinationlocalized':'b','lastupdated':'06/10/2018T16:15:00','departures':['01:01:01','02:02:02']}")>]
        [<InlineData(@"{'startdayofweek':'Monday','enddayofweek':'Tuesday','destination':'a','destinationlocalized':'b','lastupdated':'06/10/2018 16:15:00','departures':['01:01:01Z','02:02:02']}")>]
        let ``getFullTimetableInformation_CannotDeserializeInMake_ErrorInternalLibraryError`` responseResult =
            let response = @"{'errorcode':'0','errormessage':'','numberofresults':'1','timestamp':'06/10/2018 16:15:00','results':[" + responseResult + "]}"
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent(response))), None)}
            let unwrappedResult = getFullTimetableInformation client 1 "1" |> Async.RunSynchronously
            Assert.Equal(Error InternalLibraryError, unwrappedResult)

        [<Fact>]
        let ``getFullTimetableInformation_ResponseValid_OkResponse`` () =
            let responseResult = @"{'startdayofweek':'Monday','enddayofweek':'Tuesday','destination':'a','destinationlocalized':'b','lastupdated':'06/10/2018 16:15:00','departures':['01:01:01','02:02:02']}"
            let response = @"{'errorcode':'0','errormessage':'','numberofresults':'1','timestamp':'06/10/2018 16:15:00','results':[" + responseResult + "]}"
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent(response))), None)}
            let unwrappedResult = getFullTimetableInformation client 1 "1" |> Async.RunSynchronously
            match unwrappedResult with
            | Error _   -> raise (new XunitException("Valid response should have 'Ok ...' result."))
            | Ok result ->
                Assert.Equal(1, result.StopId)
                Assert.Equal("1", result.Route)
                Assert.Equal<TimeTableEntry list>(
                    [
                        {
                            StartDayOfWeek=Monday;
                            EndDayOfWeek=Tuesday;
                            Destination={EnglishName="a";IrishName="b"};
                            LastUpdated=new DateTime(2018, 10, 6, 16, 15, 0);
                            Departures=[new TimeSpan(1,1,1);new TimeSpan(2,2,2)]
                        }
                    ], result.TimeTableEntries)

    module OperatorInformation =
        open RealTimePassengerInformation.Bus.OperatorInformation

        [<Fact>]
        let ``make_ValidModel_ReferenceCodeSetToReferenceCodeInModel`` () =
            let model = new OperatorInformationModel("a", "b", "c")
            Assert.Equal("a", (make model).ReferenceCode)

        [<Fact>]
        let ``make_ValidModel_NameCodeSetToNameInModel`` () =
            let model = new OperatorInformationModel("a", "b", "c")
            Assert.Equal("b", (make model).Name)

        [<Fact>]
        let ``make_ValidModel_DescriptionSetToDescriptionInModel`` () =
            let model = new OperatorInformationModel("a", "b", "c")
            Assert.Equal("c", (make model).Description)

        [<Fact>]
        let ``getOperatorInformation_ServiceErrorInResponseStatusCode_ErrorInternalLibraryError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler(None, None, (Some HttpStatusCode.BadRequest))}
            let result = getOperatorInformation client
            Assert.Equal<Result<OperatorInformation.T, ApiError>>(Error InternalLibraryError, Async.RunSynchronously result)

        [<Fact>]
        let ``getOperatorInformation_UserErrorInClientSetup_ErrorUserError`` () =
            let client = {HttpHandler = null}
            let result = getOperatorInformation client
            Assert.Equal<Result<OperatorInformation.T, ApiError>>(Error UserError, Async.RunSynchronously result)

        [<Fact>]
        let ``getOperatorInformation_NetworkErrorOnRequest_ErrorNetworkError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler((Some (upcast new HttpRequestException())), None, None)}
            let result = getOperatorInformation client
            Assert.Equal<Result<OperatorInformation.T, ApiError>>(Error NetworkError, Async.RunSynchronously result)

        [<Fact>]
        let ``getOperatorInformation_CannotDeserializeClientResponse_ErrorInternalLibraryError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent("{"))), None)}
            let result = getOperatorInformation client
            Assert.Equal<Result<OperatorInformation.T, ApiError>>(Error InternalLibraryError, Async.RunSynchronously result)

        [<Theory>]
        [<InlineData(@"{'errorcode':'1','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'2','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'3','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'4','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'5','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'6','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        let ``getOperatorInformation_ResultIsServiceFailureResult_Error`` response =
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent(response))), None)}
            let result = getOperatorInformation client
            match Async.RunSynchronously result with
            | Error _ -> ignore
            | Ok _    -> raise (new XunitException("Error response code did not fail call."))

        [<Fact>]
        let ``getOperatorInformation_ResponseValid_OkResponse`` () =
            let responseResult = @"{'operatorreference':'a','operatorname':'b','operatordescription':'c'}"
            let response = @"{'errorcode':'0','errormessage':'','numberofresults':'1','timestamp':'06/10/2018 16:15:00','results':[" + responseResult + "]}"
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent(response))), None)}
            let unwrappedResult = getOperatorInformation client |> Async.RunSynchronously
            match unwrappedResult with
            | Error _   -> raise (new XunitException("Valid response should have 'Ok ...' result."))
            | Ok result ->
                Assert.Equal<T>(
                    [
                        {
                            ReferenceCode="a";
                            Name="b";
                            Description="c";
                        }
                    ], result)

    module RealTimeBusInformation =
        open RealTimePassengerInformation.Bus.RealTimeBusInformation

        [<Fact>]
        let ``makeSafe_ValidModel_ArrivalExpectedSetToDateTimeMin`` () =
            let model = new RealTimeBusInformationModel("a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p")
            Assert.Equal(DateTime.MinValue, (makeSafe model).Arrival.Expected)

        [<Fact>]
        let ``makeSafe_ValidModel_ArrivalScheduledSetToDateTimeMin`` () =
            let model = new RealTimeBusInformationModel("a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p")
            Assert.Equal(DateTime.MinValue, (makeSafe model).Arrival.Scheduled)

        [<Fact>]
        let ``makeSafe_ValidModel_ArrivalBoardStatusSetToDue`` () =
            let model = new RealTimeBusInformationModel("a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p")
            Assert.Equal(Due, (makeSafe model).Arrival.BoardStatus)

        [<Fact>]
        let ``makeSafe_ValidModel_DepartureExpectedSetToDateTimeMin`` () =
            let model = new RealTimeBusInformationModel("a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p")
            Assert.Equal(DateTime.MinValue, (makeSafe model).Departure.Expected)

        [<Fact>]
        let ``makeSafe_ValidModel_DepartureScheduledSetToDateTimeMin`` () =
            let model = new RealTimeBusInformationModel("a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p")
            Assert.Equal(DateTime.MinValue, (makeSafe model).Departure.Scheduled)

        [<Fact>]
        let ``makeSafe_ValidModel_DepartureBoardStatusSetToDue`` () =
            let model = new RealTimeBusInformationModel("a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p")
            Assert.Equal(Due, (makeSafe model).Departure.BoardStatus)

        [<Fact>]
        let ``makeSafe_ValidModel_OriginEnglishNameSetToOriginInModel`` () =
            let model = new RealTimeBusInformationModel("a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p")
            Assert.Equal(model.Origin, (makeSafe model).Origin.EnglishName)

        [<Fact>]
        let ``makeSafe_ValidModel_OriginIrishNameSetToOriginLocalizedInModel`` () =
            let model = new RealTimeBusInformationModel("a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p")
            Assert.Equal(model.OriginLocalized, (makeSafe model).Origin.IrishName)

        [<Fact>]
        let ``makeSafe_ValidModel_DestinationEnglishNameSetToDestinationInModel`` () =
            let model = new RealTimeBusInformationModel("a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p")
            Assert.Equal(model.Destination, (makeSafe model).Destination.EnglishName)

        [<Fact>]
        let ``makeSafe_ValidModel_DestinationIrishNameSetToDestinationLocalizedInModel`` () =
            let model = new RealTimeBusInformationModel("a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p")
            Assert.Equal(model.DestinationLocalized, (makeSafe model).Destination.IrishName)

        [<Fact>]
        let ``makeSafe_ValidModel_DirectionSetToDirectionInModel`` () =
            let model = new RealTimeBusInformationModel("a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p")
            Assert.Equal(model.Direction, (makeSafe model).Direction)

        [<Fact>]
        let ``makeSafe_ValidModel_OperatorReferenceCodeSetToOperatorReferenceCodeInModel`` () =
            let model = new RealTimeBusInformationModel("a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p")
            Assert.Equal(model.OperatorReferenceCode, (makeSafe model).OperatorReferenceCode)

        [<Fact>]
        let ``makeSafe_ValidModel_AdditionalInformationSetToAdditionalInformationInModel`` () =
            let model = new RealTimeBusInformationModel("a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p")
            Assert.Equal(model.AdditionalInformation, (makeSafe model).AdditionalInformation)

        [<Fact>]
        let ``makeSafe_ValidModel_HasLowFloorSetToFalse`` () =
            let model = new RealTimeBusInformationModel("a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p")
            Assert.False((makeSafe model).HasLowFloor)

        [<Fact>]
        let ``makeSafe_ValidModel_RouteSetToRouteInModel`` () =
            let model = new RealTimeBusInformationModel("a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p")
            Assert.Equal(model.Route, (makeSafe model).Route)

        [<Fact>]
        let ``makeSafe_ValidModel_SourceTimeStampSetToDateTimeMin`` () =
            let model = new RealTimeBusInformationModel("a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p")
            Assert.Equal(DateTime.MinValue, (makeSafe model).SourceTimeStamp)

        [<Fact>]
        let ``deserializeBoardStatus_Due_SomeDue`` () =
            Assert.Equal(Some Due, deserializeBoardStatus "Due")

        [<Theory>]
        [<InlineData(1)>]
        [<InlineData(15)>]
        let ``deserializeBoardStatus_IntMinutes_SomeExpectedInMinutesInt`` mins =
            Assert.Equal(Some (ExpectedInMinutes mins), deserializeBoardStatus (mins.ToString()))

        [<Theory>]
        [<InlineData("")>]
        [<InlineData("one")>]
        let ``deserializeBoardStatus_NotDueOrInt_None`` mins =
            Assert.Equal(None, deserializeBoardStatus mins)

        [<Fact>]
        let ``deserializeBoardStatus_DueLowerCase_None`` () =
            Assert.Equal(None, deserializeBoardStatus "due")

        [<Fact>]
        let ``deserializeLowFloorStatus_yes_SomeTrue`` () =
            Assert.Equal(Some true, deserializeLowFloorStatus "yes")

        [<Fact>]
        let ``deserializeLowFloorStatus_no_SomeFalse`` () =
            Assert.Equal(Some false, deserializeLowFloorStatus "no")

        [<Theory>]
        [<InlineData("Yes")>]
        [<InlineData("YES")>]
        [<InlineData("No")>]
        [<InlineData("NO")>]
        let ``deserializeLowFloorStatus_NotLowerCaseYesNo_None`` lowFloorStatus =
            Assert.Equal(None, deserializeLowFloorStatus lowFloorStatus)

        [<Theory>]
        [<InlineData("")>]
        [<InlineData("false")>]
        [<InlineData("true")>]
        [<InlineData("0")>]
        [<InlineData("1")>]
        let ``deserializeLowFloorStatus_Unrecognised_None`` lowFloorStatus =
            Assert.Equal(None, deserializeLowFloorStatus lowFloorStatus)

        [<Fact>]
        let ``getRealTimeBusInformation_ServiceErrorInResponseStatusCode_ErrorInternalLibraryError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler(None, None, (Some HttpStatusCode.BadRequest))}
            let result = getRealTimeBusInformation client 1
            Assert.Equal<Result<RealTimeBusInformation.T, ApiError>>(Error InternalLibraryError, Async.RunSynchronously result)

        [<Fact>]
        let ``getRealTimeBusInformation_UserErrorInClientSetup_ErrorUserError`` () =
            let client = {HttpHandler = null}
            let result = getRealTimeBusInformation client 1
            Assert.Equal<Result<RealTimeBusInformation.T, ApiError>>(Error UserError, Async.RunSynchronously result)

        [<Fact>]
        let ``getRealTimeBusInformation_NetworkErrorOnRequest_ErrorNetworkError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler((Some (upcast new HttpRequestException())), None, None)}
            let result = getRealTimeBusInformation client 1
            Assert.Equal<Result<RealTimeBusInformation.T, ApiError>>(Error NetworkError, Async.RunSynchronously result)

        [<Fact>]
        let ``getRealTimeBusInformation_CannotDeserializeClientResponse_ErrorInternalLibraryError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent("{"))), None)}
            let result = getRealTimeBusInformation client 1
            Assert.Equal<Result<RealTimeBusInformation.T, ApiError>>(Error InternalLibraryError, Async.RunSynchronously result)

        [<Theory>]
        [<InlineData(@"{'errorcode':'1','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'2','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'3','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'4','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'5','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'6','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        let ``getRealTimeBusInformation_ResultIsServiceFailureResult_Error`` response =
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent(response))), None)}
            let result = getRealTimeBusInformation client 1
            match Async.RunSynchronously result with
            | Error _ -> ignore
            | Ok _    -> raise (new XunitException("Error response code did not fail call."))

        [<Theory>]
        [<InlineData(@"{'arrivaldatetime':'06/10/2018T16:15:00','duetime':'1','departuredatetime':'06/10/2018 16:15:00','departureduetime':'2','scheduledarrivaldatetime':'06/10/2018 16:15:00','scheduleddeparturedatetime':'06/10/2018 16:15:00','destination':'a','destinationlocalized':'b','origin':'c','originlocalized':'d','direction':'Inbound','operator':'e','additionalinformation':'f','lowfloorstatus':'yes','route':'3','sourcetimestamp':'06/10/2018 16:15:00','stops':[]}")>]
        [<InlineData(@"{'arrivaldatetime':'06/10/2018 16:15:00','duetime':'x','departuredatetime':'06/10/2018 16:15:00','departureduetime':'2','scheduledarrivaldatetime':'06/10/2018 16:15:00','scheduleddeparturedatetime':'06/10/2018 16:15:00','destination':'a','destinationlocalized':'b','origin':'c','originlocalized':'d','direction':'Inbound','operator':'e','additionalinformation':'f','lowfloorstatus':'yes','route':'3','sourcetimestamp':'06/10/2018 16:15:00','stops':[]}")>]
        [<InlineData(@"{'arrivaldatetime':'06/10/2018 16:15:00','duetime':'1','departuredatetime':'06/10/2018T16:15:00','departureduetime':'2','scheduledarrivaldatetime':'06/10/2018 16:15:00','scheduleddeparturedatetime':'06/10/2018 16:15:00','destination':'a','destinationlocalized':'b','origin':'c','originlocalized':'d','direction':'Inbound','operator':'e','additionalinformation':'f','lowfloorstatus':'yes','route':'3','sourcetimestamp':'06/10/2018 16:15:00','stops':[]}")>]
        [<InlineData(@"{'arrivaldatetime':'06/10/2018 16:15:00','duetime':'1','departuredatetime':'06/10/2018 16:15:00','departureduetime':'x','scheduledarrivaldatetime':'06/10/2018 16:15:00','scheduleddeparturedatetime':'06/10/2018 16:15:00','destination':'a','destinationlocalized':'b','origin':'c','originlocalized':'d','direction':'Inbound','operator':'e','additionalinformation':'f','lowfloorstatus':'yes','route':'3','sourcetimestamp':'06/10/2018 16:15:00','stops':[]}")>]
        [<InlineData(@"{'arrivaldatetime':'06/10/2018 16:15:00','duetime':'1','departuredatetime':'06/10/2018 16:15:00','departureduetime':'2','scheduledarrivaldatetime':'06/10/2018T16:15:00','scheduleddeparturedatetime':'06/10/2018 16:15:00','destination':'a','destinationlocalized':'b','origin':'c','originlocalized':'d','direction':'Inbound','operator':'e','additionalinformation':'f','lowfloorstatus':'yes','route':'3','sourcetimestamp':'06/10/2018 16:15:00','stops':[]}")>]
        [<InlineData(@"{'arrivaldatetime':'06/10/2018 16:15:00','duetime':'1','departuredatetime':'06/10/2018 16:15:00','departureduetime':'2','scheduledarrivaldatetime':'06/10/2018 16:15:00','scheduleddeparturedatetime':'06/10/2018T16:15:00','destination':'a','destinationlocalized':'b','origin':'c','originlocalized':'d','direction':'Inbound','operator':'e','additionalinformation':'f','lowfloorstatus':'yes','route':'3','sourcetimestamp':'06/10/2018 16:15:00','stops':[]}")>]
        [<InlineData(@"{'arrivaldatetime':'06/10/2018 16:15:00','duetime':'1','departuredatetime':'06/10/2018 16:15:00','departureduetime':'2','scheduledarrivaldatetime':'06/10/2018 16:15:00','scheduleddeparturedatetime':'06/10/2018 16:15:00','destination':'a','destinationlocalized':'b','origin':'c','originlocalized':'d','direction':'Inbound','operator':'e','additionalinformation':'f','lowfloorstatus':'x','route':'3','sourcetimestamp':'06/10/2018 16:15:00','stops':[]}")>]
        [<InlineData(@"{'arrivaldatetime':'06/10/2018 16:15:00','duetime':'1','departuredatetime':'06/10/2018 16:15:00','departureduetime':'2','scheduledarrivaldatetime':'06/10/2018 16:15:00','scheduleddeparturedatetime':'06/10/2018 16:15:00','destination':'a','destinationlocalized':'b','origin':'c','originlocalized':'d','direction':'Inbound','operator':'e','additionalinformation':'f','lowfloorstatus':'yes','route':'3','sourcetimestamp':'06/10/2018T16:15:00','stops':[]}")>]
        let ``getRealTimeBusInformation_CannotDeserializeInMake_ErrorInternalLibraryError`` responseResult =
            let response = @"{'errorcode':'0','errormessage':'','numberofresults':'1','timestamp':'06/10/2018 16:15:00','results':[" + responseResult + "]}"
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent(response))), None)}
            let unwrappedResult = getRealTimeBusInformation client 1 |> Async.RunSynchronously
            Assert.Equal(Error InternalLibraryError, unwrappedResult)

        [<Theory>]
        [<InlineData(@"{'arrivaldatetime':'06/10/2018 16:15:00','duetime':'1','departuredatetime':'06/10/2018 16:15:00','departureduetime':'2','scheduledarrivaldatetime':'06/10/2018 16:15:00','scheduleddeparturedatetime':'06/10/2018 16:15:00','destination':'a','destinationlocalized':'b','origin':'c','originlocalized':'d','direction':'Inbound','operator':'e','additionalinformation':'f','lowfloorstatus':'yes','route':'3','sourcetimestamp':'06/10/2018 16:15:00'}")>]
        let ``getFullTimetableInformation_ResponseValid_OkResponse`` responseResult =
            let response = @"{'errorcode':'0','errormessage':'','stopid':'1','numberofresults':'1','timestamp':'06/10/2018 16:15:00','results':[" + responseResult + "]}"
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent(response))), None)}
            let unwrappedResult = getRealTimeBusInformation client 1 |> Async.RunSynchronously
            match unwrappedResult with
            | Error e   -> raise (new XunitException("Valid response should have 'Ok ...' result."))
            | Ok result ->
                Assert.Equal<T>(
                    {
                        StopId=1;
                        Arrivals=[
                            {
                                Arrival={
                                    Expected=new DateTime(2018,10,6,16,15,0);
                                    Scheduled=new DateTime(2018,10,6,16,15,0);
                                    BoardStatus=(ExpectedInMinutes 1)
                                };
                                Departure={
                                    Expected=new DateTime(2018,10,6,16,15,0);
                                    Scheduled=new DateTime(2018,10,6,16,15,0);
                                    BoardStatus=(ExpectedInMinutes 2)
                                };
                                Origin={EnglishName="c";IrishName="d"};
                                Destination={EnglishName="a";IrishName="b"};
                                Direction="Inbound";
                                OperatorReferenceCode="e";
                                AdditionalInformation="f";
                                HasLowFloor=true;
                                Route="3";
                                SourceTimeStamp=new DateTime(2018,10,6,16,15,0)
                            }
                        ]
                    }, result)

    module RouteInformation =
        open RealTimePassengerInformation.Bus.RouteInformation

        [<Fact>]
        let ``make_ValidModel_OkStopIdSetToStopIdInModel`` () =
            let model = new RouteStop(1, 2, "a", "b", "c", "d", 0.1, 0.2)
            Assert.Equal(1, (makeBusStopInformation model).StopId)

        [<Fact>]
        let ``make_ValidModel_OkDisplayStopIdSetToDisplayStopIdInModel`` () =
            let model = new RouteStop(1, 2, "a", "b", "c", "d", 0.1, 0.2)
            Assert.Equal(2, (makeBusStopInformation model).DisplayedStopId)

        [<Fact>]
        let ``make_ValidModel_OkShortEnglishNameSetToShortNameInModel`` () =
            let model = new RouteStop(1, 2, "a", "b", "c", "d", 0.1, 0.2)
            Assert.Equal("a", (makeBusStopInformation model).ShortName.EnglishName)

        [<Fact>]
        let ``make_ValidModel_OkShortIrishNameSetToShortNameLocalizedInModel`` () =
            let model = new RouteStop(1, 2, "a", "b", "c", "d", 0.1, 0.2)
            Assert.Equal("b", (makeBusStopInformation model).ShortName.IrishName)

        [<Fact>]
        let ``make_ValidModel_OkFullEnglishNameSetToFullNameInModel`` () =
            let model = new RouteStop(1, 2, "a", "b", "c", "d", 0.1, 0.2)
            Assert.Equal("c", (makeBusStopInformation model).FullName.EnglishName)

        [<Fact>]
        let ``make_ValidModel_OkFullIrishNameSetToFullNameLocalizedInModel`` () =
            let model = new RouteStop(1, 2, "a", "b", "c", "d", 0.1, 0.2)
            Assert.Equal("d", (makeBusStopInformation model).FullName.IrishName)

        [<Fact>]
        let ``make_ValidModel_OkLatitudeSetToLatitudeInModel`` () =
            let model = new RouteStop(1, 2, "a", "b", "c", "d", 0.1, 0.2)
            Assert.Equal(0.1, (makeBusStopInformation model).Latitude)

        [<Fact>]
        let ``make_ValidModel_OkLatitudeSetToLongitudeInModel`` () =
            let model = new RouteStop(1, 2, "a", "b", "c", "d", 0.1, 0.2)
            Assert.Equal(0.2, (makeBusStopInformation model).Longitude)

        [<Fact>]
        let ``getRouteInformation_ServiceErrorInResponseStatusCode_ErrorInternalLibraryError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler(None, None, (Some HttpStatusCode.BadRequest))}
            let result = getRouteInformation client "1" "a"
            Assert.Equal<Result<RouteInformation.T list, ApiError>>(Error InternalLibraryError, Async.RunSynchronously result)

        [<Fact>]
        let ``getRouteInformation_UserErrorInClientSetup_ErrorUserError`` () =
            let client = {HttpHandler = null}
            let result = getRouteInformation client "1" "a"
            Assert.Equal<Result<RouteInformation.T list, ApiError>>(Error UserError, Async.RunSynchronously result)

        [<Fact>]
        let ``getRouteInformation_NetworkErrorOnRequest_ErrorNetworkError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler((Some (upcast new HttpRequestException())), None, None)}
            let result = getRouteInformation client "1" "a"
            Assert.Equal<Result<RouteInformation.T list, ApiError>>(Error NetworkError, Async.RunSynchronously result)

        [<Fact>]
        let ``getRouteInformation_CannotDeserializeClientResponse_ErrorInternalLibraryError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent("{"))), None)}
            let result = getRouteInformation client "1" "a"
            Assert.Equal<Result<RouteInformation.T list, ApiError>>(Error InternalLibraryError, Async.RunSynchronously result)

        [<Theory>]
        [<InlineData(@"{'errorcode':'1','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'2','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'3','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'4','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'5','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'6','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        let ``getRouteInformation_ResultIsServiceFailureResult_Error`` response =
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent(response))), None)}
            let result = getRouteInformation client "1" "a"
            match Async.RunSynchronously result with
            | Error _ -> ignore
            | Ok _    -> raise (new XunitException("Error response code did not fail call."))

        [<Theory>]
        [<InlineData(@"{'operator':'a','origin':'b','originlocalized':'c','destination':'d','destinationlocalized':'e','lastupdated':'06/10/2018T16:15:00','stops':[]}")>]
        let ``getRouteInformation_CannotDeserializeInMake_ErrorInternalLibraryError`` responseResult =
            let response = @"{'errorcode':'0','errormessage':'','numberofresults':'1','timestamp':'06/10/2018 16:15:00','results':[" + responseResult + "]}"
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent(response))), None)}
            let unwrappedResult = getRouteInformation client "1" "a" |> Async.RunSynchronously
            Assert.Equal(Error InternalLibraryError, unwrappedResult)

        [<Fact>]
        let ``getFullTimetableInformation_ResponseValid_OkResponse`` () =
            let responseResult = @"{'operator':'a','origin':'b','originlocalized':'c','destination':'d','destinationlocalized':'e','lastupdated':'06/10/2018 16:15:00','stops':[{stopid:'2','displaystopid':'3','shortname':'f','shortnamelocalized':'g','fullname':'h','fullnamelocalized':'i','latitude':'0.1','longitude':'0.2'}]}"
            let response = @"{'errorcode':'0','errormessage':'','route':'1','numberofresults':'1','timestamp':'06/10/2018 16:15:00','results':[" + responseResult + "]}"
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent(response))), None)}
            let unwrappedResult = getRouteInformation client "1" "a" |> Async.RunSynchronously
            match unwrappedResult with
            | Error e   -> raise (new XunitException("Valid response should have 'Ok ...' result."))
            | Ok result ->
                Assert.Equal<T list>(
                    [
                        {
                            OperatorName="a";
                            Origin={EnglishName="b";IrishName="c"};
                            Destination={EnglishName="d";IrishName="e"};
                            LastUpdated=new DateTime(2018,10,6,16,15,0);
                            Stops=[
                                {
                                    StopId=2;
                                    DisplayedStopId=3;
                                    ShortName={EnglishName="f";IrishName="g"};
                                    FullName={EnglishName="h";IrishName="i"};
                                    Latitude=0.1;
                                    Longitude=0.2;
                                }
                            ];
                        }
                    ], result)

    module RouteListInformation =
        open RealTimePassengerInformation.Bus.RouteListInformation

        [<Fact>]
        let ``getRouteListInformation_ServiceErrorInResponseStatusCode_ErrorInternalLibraryError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler(None, None, (Some HttpStatusCode.BadRequest))}
            let result = getRouteListInformation client
            Assert.Equal<Result<RouteListInformation.T list, ApiError>>(Error InternalLibraryError, Async.RunSynchronously result)

        [<Fact>]
        let ``getRouteListInformation_UserErrorInClientSetup_ErrorUserError`` () =
            let client = {HttpHandler = null}
            let result = getRouteListInformation client
            Assert.Equal<Result<RouteListInformation.T list, ApiError>>(Error UserError, Async.RunSynchronously result)

        [<Fact>]
        let ``getRouteListInformation_NetworkErrorOnRequest_ErrorNetworkError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler((Some (upcast new HttpRequestException())), None, None)}
            let result = getRouteListInformation client
            Assert.Equal<Result<RouteListInformation.T list, ApiError>>(Error NetworkError, Async.RunSynchronously result)

        [<Fact>]
        let ``getRouteListInformation_CannotDeserializeClientResponse_ErrorInternalLibraryError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent("{"))), None)}
            let result = getRouteListInformation client
            Assert.Equal<Result<RouteListInformation.T list, ApiError>>(Error InternalLibraryError, Async.RunSynchronously result)

        [<Theory>]
        [<InlineData(@"{'errorcode':'1','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'2','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'3','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'4','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'5','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'6','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        let ``getRouteListInformation_ResultIsServiceFailureResult_Error`` response =
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent(response))), None)}
            let result = getRouteListInformation client
            match Async.RunSynchronously result with
            | Error _ -> ignore
            | Ok _    -> raise (new XunitException("Error response code did not fail call."))

        [<Fact>]
        let ``getRouteListInformation_ResponseValid_OkResponse`` () =
            let responseResult = @"{'operator':'a','route':'b'},{'operator':'a','route':'c'},{'operator':'d','route':'e'}"
            let response = @"{'errorcode':'0','errormessage':'','numberofresults':'1','timestamp':'06/10/2018 16:15:00','results':[" + responseResult + "]}"
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent(response))), None)}
            let unwrappedResult = getRouteListInformation client |> Async.RunSynchronously
            match unwrappedResult with
            | Error _   -> raise (new XunitException("Valid response should have 'Ok ...' result."))
            | Ok result ->
                Assert.Equal<T list>(
                    [
                        {
                            OperatorReferenceCode="d";
                            Routes=["e"]
                        };
                        {
                            OperatorReferenceCode="a";
                            Routes=["c";"b"]
                        }
                    ], result)

        [<Fact>]
        let ``getRouteListInformationForOperator_ServiceErrorInResponseStatusCode_ErrorInternalLibraryError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler(None, None, (Some HttpStatusCode.BadRequest))}
            let result = getRouteListInformationForOperator client "a"
            Assert.Equal<Result<RouteListInformation.T, ApiError>>(Error InternalLibraryError, Async.RunSynchronously result)

        [<Fact>]
        let ``getRouteListInformationForOperator_UserErrorInClientSetup_ErrorUserError`` () =
            let client = {HttpHandler = null}
            let result = getRouteListInformationForOperator client "a"
            Assert.Equal<Result<RouteListInformation.T, ApiError>>(Error UserError, Async.RunSynchronously result)

        [<Fact>]
        let ``getRouteListInformationForOperator_NetworkErrorOnRequest_ErrorNetworkError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler((Some (upcast new HttpRequestException())), None, None)}
            let result = getRouteListInformationForOperator client "a"
            Assert.Equal<Result<RouteListInformation.T, ApiError>>(Error NetworkError, Async.RunSynchronously result)

        [<Fact>]
        let ``getRouteListInformationForOperator_CannotDeserializeClientResponse_ErrorInternalLibraryError`` () =
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent("{"))), None)}
            let result = getRouteListInformationForOperator client "a"
            Assert.Equal<Result<RouteListInformation.T, ApiError>>(Error InternalLibraryError, Async.RunSynchronously result)

        [<Theory>]
        [<InlineData(@"{'errorcode':'1','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'2','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'3','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'4','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'5','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        [<InlineData(@"{'errorcode':'6','errormessage':'','numberofresults':'0','timestamp':'','results':[]}")>]
        let ``getRouteListInformationForOperator_ResultIsServiceFailureResult_Error`` response =
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent(response))), None)}
            let result = getRouteListInformationForOperator client "a"
            match Async.RunSynchronously result with
            | Error _ -> ignore
            | Ok _    -> raise (new XunitException("Error response code did not fail call."))

        [<Fact>]
        let ``getRouteListInformationForOperator_MultipleOperatorsReturned_ErrorInternalLibraryError`` () =
            let responseResult = @"{'operator':'a','route':'b'},{'operator':'a','route':'c'},{'operator':'d','route':'e'}"
            let response = @"{'errorcode':'0','errormessage':'','numberofresults':'1','timestamp':'06/10/2018 16:15:00','results':[" + responseResult + "]}"
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent(response))), None)}
            let unwrappedResult = getRouteListInformationForOperator client "a" |> Async.RunSynchronously
            Assert.Equal(Error InternalLibraryError, unwrappedResult)

        [<Fact>]
        let ``getRouteListInformationForOperator_ResponseValid_OkResponse`` () =
            let responseResult = @"{'operator':'a','route':'b'},{'operator':'a','route':'c'}"
            let response = @"{'errorcode':'0','errormessage':'','numberofresults':'1','timestamp':'06/10/2018 16:15:00','results':[" + responseResult + "]}"
            let client = {HttpHandler = new TestHttpMessageHandler(None, (Some (upcast new StringContent(response))), None)}
            let unwrappedResult = getRouteListInformationForOperator client "a" |> Async.RunSynchronously
            match unwrappedResult with
            | Error _   -> raise (new XunitException("Valid response should have 'Ok ...' result."))
            | Ok result ->
                Assert.Equal<T>(
                    {
                            OperatorReferenceCode="a";
                            Routes=["c";"b"]
                    } , result)
