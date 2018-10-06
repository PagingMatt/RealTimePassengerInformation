namespace RealTimePassengerInformation.UnitTests

open System
open System.Net.Http
open System.Threading
open System.Net
open System.Threading.Tasks
open Xunit.Sdk

module Helpers =
    type internal TestHttpMessageHandler(ex, content, code) =
        inherit HttpMessageHandler()

        let Ex : Exception option = ex
        let Content : HttpContent option = content
        let Code : HttpStatusCode option = code

        override this.SendAsync ((_:HttpRequestMessage), (_:CancellationToken)) =
            match Ex with
            | Some ex -> Task.FromResult(raise ex)
            | None ->
            match Content with
            | Some content ->
                let response = new HttpResponseMessage(HttpStatusCode.OK)
                response.Content <- content
                Task.FromResult<HttpResponseMessage>(response)
            | None ->
            match Code with
            | Some code ->
                let response = new HttpResponseMessage(code)
                Task.FromResult<HttpResponseMessage>(response)
            | None -> raise (XunitException("TestHttpMessageHandler helper class setup incorrect."))
