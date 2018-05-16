namespace DiscardChanges.SillyNameMaker

open System.IO
open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs.Host
open Newtonsoft.Json

module MakeName =

  type QueryParameters = {
    color: string
    number: decimal
  } 

  type QueryResult = {
    parameters: QueryParameters
  }

  type RequestModel = { 
    queryResult: QueryResult
  }

  type SimpleResponse = {
    textToSpeech: string
  }

  type RichResponseItem = {
    simpleResponse: SimpleResponse
  }

  type RichResponse = {
    items: RichResponseItem list
  }

  type GooglePayload = {
    richResponse: RichResponse
  }

  type Payload = {
    google: GooglePayload
  }

  type ResponseModel = {
    payload: Payload
  }

  [<FunctionName("MakeName")>]
  let run ([<HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)>] req:HttpRequest, log: TraceWriter) =   
    log.Info("C# HTTP trigger function processed a request.")
    use reader = new StreamReader(req.Body)
    let requestBody = reader.ReadToEnd()
    log.Info("Request Body: ", requestBody)
    let data = JsonConvert.DeserializeObject<RequestModel>(requestBody)
    
    let color = data.queryResult.parameters.color    
    let number = data.queryResult.parameters.number |> int
    let responsePhrase = sprintf "Hello, %s %d" color number
    let response = { payload = { google = { richResponse = { items = [{ simpleResponse = { textToSpeech = responsePhrase }}]}}}}
    
    new JsonResult(response) :> IActionResult
