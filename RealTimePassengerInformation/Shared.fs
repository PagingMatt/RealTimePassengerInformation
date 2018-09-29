namespace RealTimePassengerInformation

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
