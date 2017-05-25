module Crawler

open Newtonsoft.Json
open System
open System.IO
open System.Net
open TvShowManager.Interfaces
open System.Text
open Newtonsoft.Json.Linq
open System.Threading.Tasks
open TvShowManager
open Types


type TvDbCrawler() = 
   
    let mutable apikey = ""
    let mutable user = ""
    let mutable userkey = ""

    let mutable token = ""

    let extractValue (line:string) = 
        (line.Split '=').[1].Trim()

    let readcredentials = 
        match File.ReadAllLines("./credentials.cred") with
        |[|keyline;userline;pwline|] ->
            apikey <- keyline |> extractValue
            user <- userline |> extractValue
            userkey <- pwline |> extractValue
        |_ -> raise (Exception "invalid configuration")


    let apiUrl = "https://api.thetvdb.com"

    let login = async {
            use client = new WebClient()
            let body = "{\"apikey\":\"" + apikey + "\", \"username\":\"" + user + "\", \"userkey\":\"" + userkey + "\"}" 
            client.Headers.Set("Content-Type", "application/json")
            let! response = client.UploadDataTaskAsync(apiUrl + "/login", "POST",  body |> Encoding.UTF8.GetBytes) |> Async.AwaitTask
            let jsonToken = JObject.Parse(Encoding.UTF8.GetString response)
            return jsonToken.["token"] |> string } 

    let getAuthorizedClient t =
        let client = new WebClient()
        client.Headers.Set("Content-Type", "application/json")
        client.Headers.Set("Authorization", "Bearer " + t)
        client.Headers.Set("Accept-Language", "en")
        client 

    let searchShow show = async {
        let client = getAuthorizedClient token
        let! response = client.AsyncDownloadString(Uri(apiUrl + "/search/series?name=" + show))
        return JsonConvert.DeserializeObject<TvDbSearchResponse>(response) }

    let getEpisodes show = async {
        let client = getAuthorizedClient token
        let! response = client.AsyncDownloadString(Uri(apiUrl + "/series/" + (show.id |> string) + "/episodes"))

        return JsonConvert.DeserializeObject<TvDbEpisodeResult>(response) }
               
    let choose (options: (int*string) seq) =
        options
        |> Seq.iter(fun (idx, desc) -> printfn "[%d]: %s" idx desc)
        printf "Which option? "
        let input = Console.ReadLine()
        input |> int

    let makeSeasons (showname: string) (episodes : TvDbEpisode seq)=
        episodes
        |> Seq.groupBy(fun e -> e.airedSeason)
        |> Seq.map(fun (season, eps) -> 
            let s = Season()
            s.ShowName <- showname
            s.Number <- if season.HasValue then season.Value else 0
            s.Episodes <- eps |> Seq.map(fun dbep -> 
                let ep = Episode()
                ep.FirstAired <- DateTime.Parse(dbep.firstAired)
                ep.Name <- dbep.episodeName
                ep.Number <- if dbep.airedEpisodeNumber.HasValue then dbep.airedEpisodeNumber.Value else 0
                ep)
            s)

    interface IEpisodeCrawler with 
        member this.DownloadEpisodeListAsync showName = 
            let intern = async {
                readcredentials
                let! showResult = searchShow showName
                let shows = showResult.data
                let choice = choose(shows |> Seq.mapi(fun idx s -> (idx, s.seriesName)))
                let show = shows.[choice]
                let! episodeResult = getEpisodes show
                let episodes = episodeResult.data

                let seasons = makeSeasons show.seriesName episodes
                let epList = EpisodeList()
                epList.Seasons <- seasons
                epList.ShowName <- show.seriesName
                return epList
            }
            Async.StartAsTask(intern)
