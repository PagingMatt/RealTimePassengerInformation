namespace RealTimePassengerInformation

open System.Runtime.CompilerServices

module Shared =
    module Operators =
        let internal (>>>) x f =
            async {
                let! optionValue = x
                match optionValue with
                | Ok value  -> return f value
                | Error err -> return (Error err)
            }

        let internal (>><) x f =
            async {
                let! optionValue = x
                match optionValue with
                | Ok value  -> return Ok (f value)
                | Error err -> return (Error err)
            }

    module Formatting =
        open System.Globalization

        let internal inv = CultureInfo.InvariantCulture

[<assembly: InternalsVisibleTo("RealTimePassengerInformation.UnitTests")>]
do()
