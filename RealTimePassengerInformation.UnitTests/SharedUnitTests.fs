namespace RealTimePassengerInformation.UnitTests

open Xunit

module Shared =
    module Operators =
        open RealTimePassengerInformation.Shared.Operators
        open System.Threading.Tasks

        [<Fact>]
        let internal ``>>>_Error_Error`` () =
            let result =
                Async.RunSynchronously
                    (Async.AwaitTask(Task.FromResult<Result<unit, unit>>(Error ())) >>> fun x -> Ok x)
            Assert.Equal<Result<unit, unit>>((Error ()), result)

        [<Fact>]
        let internal ``>>>_OkX_AppliesFToX`` () =
            let result =
                Async.RunSynchronously
                    (Async.AwaitTask(Task.FromResult<Result<int, unit>>(Ok 1)) >>> fun x -> Ok (x+1))
            Assert.Equal<Result<int, unit>>((Ok 2), result)

        [<Fact>]
        let internal ``>><_Error_Error`` () =
            let result =
                Async.RunSynchronously
                    (Async.AwaitTask(Task.FromResult<Result<unit, unit>>(Error ())) >>< fun x -> x)
            Assert.Equal<Result<unit, unit>>((Error ()), result)

        [<Fact>]
        let internal ``>><_OkX_OkAppliesFToX`` () =
            let result =
                Async.RunSynchronously
                    (Async.AwaitTask(Task.FromResult<Result<int, unit>>(Ok 1)) >>< fun x -> x+1)
            Assert.Equal<Result<int, unit>>((Ok 2), result)

    module Formatting =
        open RealTimePassengerInformation.Shared.Formatting
