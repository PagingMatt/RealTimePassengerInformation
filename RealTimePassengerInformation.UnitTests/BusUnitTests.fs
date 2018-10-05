namespace RealTimePassengerInformation.UnitTests

open System
open RealTimePassengerInformation.Service
open RealTimePassengerInformation.Service.Models
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

    module DailyTimeTableInformation =
        open RealTimePassengerInformation.Bus.DailyTimeTableInformation

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

    module RouteListInformation =
        open RealTimePassengerInformation.Bus.RouteListInformation
