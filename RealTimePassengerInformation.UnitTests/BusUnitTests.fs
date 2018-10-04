namespace RealTimePassengerInformation.UnitTests

open System
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
        open RealTimePassengerInformation.Service
        open RealTimePassengerInformation.Service.Models

        [<Fact>]
        let ``make_LastUpdatedInvalidFormat_ErrorInternalLibraryError`` () =
            let model = new BusStopInformationModel(0, 0, "", "", "", "", 0.0, 0.0, "2018-10-04T23:27:00Z", [])
            Assert.Equal((Error InternalLibraryError), (make model))

        [<Fact>]
        let ``make_ValidModel_OkStopIdSetToStopIdInModel`` () =
            let model = new BusStopInformationModel(1, 0, "", "", "", "", 0.0, 0.0, "04/10/2018 23:27:00", [])
            match make model with
            | Ok result -> Assert.Equal(1, result.StopId)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

        [<Fact>]
        let ``make_ValidModel_OkDisplayStopIdSetToDisplayStopIdInModel`` () =
            let model = new BusStopInformationModel(0, 1, "", "", "", "", 0.0, 0.0, "04/10/2018 23:27:00", [])
            match make model with
            | Ok result -> Assert.Equal(1, result.DisplayedStopId)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

        [<Fact>]
        let ``make_ValidModel_OkShortEnglishNameSetToShortNameInModel`` () =
            let model = new BusStopInformationModel(0, 0, "a", "", "", "", 0.0, 0.0, "04/10/2018 23:27:00", [])
            match make model with
            | Ok result -> Assert.Equal("a", result.ShortName.EnglishName)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

        [<Fact>]
        let ``make_ValidModel_OkShortIrishNameSetToShortNameLocalizedInModel`` () =
            let model = new BusStopInformationModel(0, 0, "", "a", "", "", 0.0, 0.0, "04/10/2018 23:27:00", [])
            match make model with
            | Ok result -> Assert.Equal("a", result.ShortName.IrishName)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

        [<Fact>]
        let ``make_ValidModel_OkFullEnglishNameSetToFullNameInModel`` () =
            let model = new BusStopInformationModel(0, 0, "", "", "a", "", 0.0, 0.0, "04/10/2018 23:27:00", [])
            match make model with
            | Ok result -> Assert.Equal("a", result.FullName.EnglishName)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

        [<Fact>]
        let ``make_ValidModel_OkFullIrishNameSetToFullNameLocalizedInModel`` () =
            let model = new BusStopInformationModel(0, 0, "", "", "", "a", 0.0, 0.0, "04/10/2018 23:27:00", [])
            match make model with
            | Ok result -> Assert.Equal("a", result.FullName.IrishName)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

        [<Fact>]
        let ``make_ValidModel_OkLatitudeSetToLatitudeInModel`` () =
            let model = new BusStopInformationModel(0, 0, "", "", "", "", 1.0, 0.0, "04/10/2018 23:27:00", [])
            match make model with
            | Ok result -> Assert.Equal(1.0, result.Latitude)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

        [<Fact>]
        let ``make_ValidModel_OkLatitudeSetToLongitudeInModel`` () =
            let model = new BusStopInformationModel(0, 0, "", "", "", "", 0.0, 1.0, "04/10/2018 23:27:00", [])
            match make model with
            | Ok result -> Assert.Equal(1.0, result.Longitude)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

        [<Fact>]
        let ``make_ValidModel_OkLastUpdatedSetToLastUpdatedInModel`` () =
            let model = new BusStopInformationModel(0, 0, "", "", "", "", 0.0, 0.0, "04/10/2018 23:27:00", [])
            match make model with
            | Ok result -> Assert.Equal(new DateTime(2018, 10, 4, 23, 27, 0), result.LastUpdated)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

        [<Fact>]
        let ``make_ValidModel_OkOperatorsMappedFromOperatorsInModel`` () =
            let stopOperator = new StopOperator("a", ["b";"c"])
            let model = new BusStopInformationModel(0, 0, "", "", "", "", 0.0, 0.0, "04/10/2018 23:27:00", [stopOperator])
            match make model with
            | Ok result -> Assert.Equal<BusStopOperator list>([{Name="a"; Routes=["b";"c"]}], result.Operators)
            | _         -> raise (XunitException("Make did not result in Ok <result>."))

    module DailyTimeTableInformation =
        open RealTimePassengerInformation.Bus.DailyTimeTableInformation

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
