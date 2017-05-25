module Types
open System

type TvDbShow = { aliases: string[]; id: int; seriesName: string }
type TvDbSearchResponse = { data: TvDbShow[] }
type TvDbLinks = { first: Nullable<int>; last: Nullable<int>; next: Nullable<int>; prev: Nullable<int> }
type TvDbEpisode = { absoluteNumber: Nullable<int>; airedEpisodeNumber: Nullable<int>; airedSeason: Nullable<int>; episodeName: string; firstAired: string }
type TvDbEpisodeResult = { data: TvDbEpisode[]; links: TvDbLinks }