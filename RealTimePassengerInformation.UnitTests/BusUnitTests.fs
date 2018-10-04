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
