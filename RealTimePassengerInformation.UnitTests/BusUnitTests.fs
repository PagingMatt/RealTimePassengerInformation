namespace RealTimePassengerInformation.UnitTests

open System
open Xunit

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
