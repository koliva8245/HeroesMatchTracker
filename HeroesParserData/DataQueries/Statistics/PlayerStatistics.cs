﻿using Heroes.ReplayParser;
using HeroesIcons;
using HeroesParserData.Models.DbModels;
using HeroesParserData.Properties;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;

namespace HeroesParserData.DataQueries
{
    public static partial class Query
    {
        internal class PlayerStatistics
        {
            public static int ReadTotalStatTypeForCharacter(StatType statType, Season season, GameMode gameMode, string character)
            {
                var replayBuild = Utilities.GetSeasonReplayBuild(season);

                string type = string.Empty;
                if (statType == StatType.assists)
                    type = "Assists";
                else if (statType == StatType.deaths)
                    type = "Deaths";
                else if (statType == StatType.kills)
                    type = "SoloKills";

                string gameModeString;
                if (gameMode != GameMode.Cooperative)
                    gameModeString = GameModeQueryString(false);
                else
                    gameModeString = GameModeQueryString(true);

                using (var db = new HeroesParserDataContext())
                {
                    if (statType == StatType.wins)
                    {
                        int? wins = db.Database.SqlQuery<int?>($@"SELECT Count(IsWinner) FROM ReplayMatchPlayers mp
                                                                  JOIN Replays r
                                                                  ON mp.ReplayId = r.ReplayId
                                                                  WHERE PlayerId = @PlayerId AND IsWinner = 1 AND Character = @Character AND {gameModeString} AND ReplayBuild >= @ReplayBuildBegin AND ReplayBuild < @ReplayBuildEnd",
                                                                  new SQLiteParameter("@PlayerId", Settings.Default.UserPlayerId),
                                                                  new SQLiteParameter("@Character", character),
                                                                  new SQLiteParameter("@ReplayBuildBegin", replayBuild.Item1),
                                                                  new SQLiteParameter("@ReplayBuildEnd", replayBuild.Item2),
                                                                  new SQLiteParameter("@GameMode", gameMode)).FirstOrDefault();
                        return wins.HasValue ? wins.Value : 0;
                    }
                    else
                    {
                        int? amount = db.Database.SqlQuery<int?>($@"SELECT Sum(mpsr.{type}) FROM ReplayMatchPlayers mp
                                                                    JOIN Replays r
                                                                    JOIN ReplayAllHotsPlayers hp
                                                                    JOIN ReplayMatchPlayerScoreResults mpsr 
                                                                    ON mp.ReplayId = mpsr.ReplayId AND 
                                                                    mp.ReplayId = r.ReplayId AND 
                                                                    mp.PlayerId = mpsr.PlayerId AND
                                                                    mp.PlayerId = hp.PlayerId AND
                                                                    mp.PlayerId = mpsr.PlayerId
                                                                    WHERE mp.PlayerId = @PlayerId AND Character = @Character AND {gameModeString} AND ReplayBuild >= @ReplayBuildBegin AND ReplayBuild < @ReplayBuildEnd",
                                                                    new SQLiteParameter("@PlayerId", Settings.Default.UserPlayerId),
                                                                    new SQLiteParameter("@Character", character),
                                                                    new SQLiteParameter("@ReplayBuildBegin", replayBuild.Item1),
                                                                    new SQLiteParameter("@ReplayBuildEnd", replayBuild.Item2),
                                                                    new SQLiteParameter("@GameMode", gameMode)).FirstOrDefault();
                        return amount.HasValue ? amount.Value : 0;
                    }
                }
            }

            public static int ReadTotalStatTypeForMapCharacter(StatType statType, Season season, GameMode gameMode, string mapName, string character)
            {
                var replayBuild = Utilities.GetSeasonReplayBuild(season);

                string type = string.Empty;
                if (statType == StatType.assists)
                    type = "Assists";
                else if (statType == StatType.deaths)
                    type = "Deaths";
                else if (statType == StatType.kills)
                    type = "SoloKills";

                string gameModeString;
                if (gameMode != GameMode.Cooperative)
                    gameModeString = GameModeQueryString(false);
                else
                    gameModeString = GameModeQueryString(true);

                using (var db = new HeroesParserDataContext())
                {
                    if (statType == StatType.wins)
                    {
                        int? wins = db.Database.SqlQuery<int?>($@"SELECT Count(IsWinner) FROM ReplayMatchPlayers mp
                                                                  JOIN Replays r
                                                                  ON mp.ReplayId = r.ReplayId
                                                                  WHERE PlayerId = @PlayerId AND IsWinner = 1 AND Character = @Character AND {gameModeString} AND ReplayBuild >= @ReplayBuildBegin AND ReplayBuild < @ReplayBuildEnd AND MapName = @MapName",
                                                                  new SQLiteParameter("@PlayerId", Settings.Default.UserPlayerId),
                                                                  new SQLiteParameter("@Character", character),
                                                                  new SQLiteParameter("@ReplayBuildBegin", replayBuild.Item1),
                                                                  new SQLiteParameter("@ReplayBuildEnd", replayBuild.Item2),
                                                                  new SQLiteParameter("@GameMode", gameMode),
                                                                  new SQLiteParameter("@MapName", mapName)).FirstOrDefault();
                        return wins.HasValue ? wins.Value : 0;
                    }
                    else
                    {
                        int? amount = db.Database.SqlQuery<int?>($@"SELECT Sum(mpsr.{type}) FROM ReplayMatchPlayers mp
                                                                    JOIN Replays r
                                                                    JOIN ReplayAllHotsPlayers hp
                                                                    JOIN ReplayMatchPlayerScoreResults mpsr 
                                                                    ON mp.ReplayId = mpsr.ReplayId AND 
                                                                    mp.ReplayId = r.ReplayId AND 
                                                                    mp.PlayerId = mpsr.PlayerId AND
                                                                    mp.PlayerId = hp.PlayerId AND
                                                                    mp.PlayerId = mpsr.PlayerId
                                                                    WHERE mp.PlayerId = @PlayerId AND Character = @Character AND {gameModeString} AND ReplayBuild >= @ReplayBuildBegin AND ReplayBuild < @ReplayBuildEnd AND MapName = @MapName",
                                                                    new SQLiteParameter("@PlayerId", Settings.Default.UserPlayerId),
                                                                    new SQLiteParameter("@Character", character),
                                                                    new SQLiteParameter("@ReplayBuildBegin", replayBuild.Item1),
                                                                    new SQLiteParameter("@ReplayBuildEnd", replayBuild.Item2),
                                                                    new SQLiteParameter("@GameMode", gameMode),
                                                                    new SQLiteParameter("@MapName", mapName)).FirstOrDefault();
                        return amount.HasValue ? amount.Value : 0;
                    }
                }
            }

            public static int ReadMapWins(Season season, GameMode gameMode, string map)
            {
                var replayBuild = Utilities.GetSeasonReplayBuild(season);

                string gameModeString;
                if (gameMode != GameMode.Cooperative)
                    gameModeString = GameModeQueryString(false);
                else
                    gameModeString = GameModeQueryString(true);

                using (var db = new HeroesParserDataContext())
                {
                    int? wins = db.Database.SqlQuery<int?>($@"SELECT Count(IsWinner) FROM ReplayMatchPlayers mp
                                                              JOIN Replays r
                                                              ON mp.ReplayId = r.ReplayId
                                                              WHERE PlayerId = @PlayerId AND MapName = @Map AND IsWinner = 1 AND {gameModeString} AND ReplayBuild >= @ReplayBuildBegin AND ReplayBuild < @ReplayBuildEnd",
                                                              new SQLiteParameter("@PlayerId", Settings.Default.UserPlayerId),
                                                              new SQLiteParameter("@Map", map),
                                                              new SQLiteParameter("@ReplayBuildBegin", replayBuild.Item1),
                                                              new SQLiteParameter("@ReplayBuildEnd", replayBuild.Item2),
                                                              new SQLiteParameter("@GameMode", gameMode)).FirstOrDefault();
                    return wins.HasValue ? wins.Value : 0;
                }
            }

            public static int ReadMapLosses(Season season, GameMode gameMode, string map)
            {
                var replayBuild = Utilities.GetSeasonReplayBuild(season);

                string gameModeString;
                if (gameMode != GameMode.Cooperative)
                    gameModeString = GameModeQueryString(false);
                else
                    gameModeString = GameModeQueryString(true);

                using (var db = new HeroesParserDataContext())
                {
                    int? wins = db.Database.SqlQuery<int?>($@"SELECT Count(IsWinner) FROM ReplayMatchPlayers mp
                                                              JOIN Replays r
                                                              ON mp.ReplayId = r.ReplayId
                                                              WHERE PlayerId = @PlayerId AND MapName = @Map AND IsWinner = 0 AND {gameModeString} AND ReplayBuild >= @ReplayBuildBegin AND ReplayBuild < @ReplayBuildEnd",
                                                              new SQLiteParameter("@PlayerId", Settings.Default.UserPlayerId),
                                                              new SQLiteParameter("@Map", map),
                                                              new SQLiteParameter("@ReplayBuildBegin", replayBuild.Item1),
                                                              new SQLiteParameter("@ReplayBuildEnd", replayBuild.Item2),
                                                              new SQLiteParameter("@GameMode", gameMode)).FirstOrDefault();
                    return wins.HasValue ? wins.Value : 0;
                }
            }

            public static int ReadGameModeTotalGames(Season season, GameMode gameMode)
            {
                var replayBuild = Utilities.GetSeasonReplayBuild(season);

                using (var db = new HeroesParserDataContext())
                {
                    int? wins = db.Database.SqlQuery<int?>($@"SELECT Count(IsWinner) FROM ReplayMatchPlayers mp
                                                              JOIN Replays r
                                                              ON mp.ReplayId = r.ReplayId
                                                              WHERE PlayerId = @PlayerId AND GameMode = @GameMode AND ReplayBuild >= @ReplayBuildBegin AND ReplayBuild < @ReplayBuildEnd",
                                                              new SQLiteParameter("@PlayerId", Settings.Default.UserPlayerId),
                                                              new SQLiteParameter("@ReplayBuildBegin", replayBuild.Item1),
                                                              new SQLiteParameter("@ReplayBuildEnd", replayBuild.Item2),
                                                              new SQLiteParameter("@GameMode", gameMode)).FirstOrDefault();
                    return wins.HasValue ? wins.Value : 0;
                }
            }

            public static int ReadGameModeTotalWins(Season season, GameMode gameMode)
            {
                var replayBuild = Utilities.GetSeasonReplayBuild(season);

                using (var db = new HeroesParserDataContext())
                {
                    int? wins = db.Database.SqlQuery<int?>($@"SELECT Count(IsWinner) FROM ReplayMatchPlayers mp
                                                              JOIN Replays r
                                                              ON mp.ReplayId = r.ReplayId
                                                              WHERE PlayerId = @PlayerId AND IsWinner = 1 AND GameMode = @GameMode AND ReplayBuild >= @ReplayBuildBegin AND ReplayBuild < @ReplayBuildEnd",
                                                              new SQLiteParameter("@PlayerId", Settings.Default.UserPlayerId),
                                                              new SQLiteParameter("@ReplayBuildBegin", replayBuild.Item1),
                                                              new SQLiteParameter("@ReplayBuildEnd", replayBuild.Item2),
                                                              new SQLiteParameter("@GameMode", gameMode)).FirstOrDefault();
                    return wins.HasValue ? wins.Value : 0;
                }
            }

            public static List<ReplayMatchPlayer> ReadListOfMatchPlayerHeroes(Season season, GameMode gameMode)
            {
                var replayBuild = Utilities.GetSeasonReplayBuild(season);

                string gameModeString;
                if (gameMode != GameMode.Cooperative)
                    gameModeString = GameModeQueryString(false);
                else
                    gameModeString = GameModeQueryString(true);
                using (var db = new HeroesParserDataContext())
                {
                    if (gameMode != GameMode.Cooperative)
                    {
                        return db.ReplayMatchPlayers.Where(x => x.PlayerId == Settings.Default.UserPlayerId && x.Replay.GameMode == gameMode && x.Replay.ReplayBuild >= replayBuild.Item1 && x.Replay.ReplayBuild < replayBuild.Item2)
                                                    .Include(x => x.Replay)
                                                    .ToList();
                    }
                    else
                    {
                        return db.ReplayMatchPlayers.Where(x => x.PlayerId == Settings.Default.UserPlayerId && x.Replay.GameMode >= GameMode.Cooperative && x.Replay.ReplayBuild >= replayBuild.Item1 && x.Replay.ReplayBuild < replayBuild.Item2)
                                                    .Include(x => x.Replay)
                                                    .ToList();
                    }
                }
            }

            public static double ReadTotalScoreResult(PlayerScoreResultTypes scoreResultTypes, Season season, GameMode gameMode, string mapName, string character)
            {
                var replayBuild = Utilities.GetSeasonReplayBuild(season);

                string gameModeString;
                if (gameMode != GameMode.Cooperative)
                    gameModeString = GameModeQueryString(false);
                else
                    gameModeString = GameModeQueryString(true);

                using (var db = new HeroesParserDataContext())
                {
                    double? amount = db.Database.SqlQuery<double?>($@"SELECT Sum(mpsr.{scoreResultTypes}) FROM ReplayMatchPlayers mp
                                                                    JOIN Replays r
                                                                    JOIN ReplayAllHotsPlayers hp
                                                                    JOIN ReplayMatchPlayerScoreResults mpsr 
                                                                    ON mp.ReplayId = mpsr.ReplayId AND 
                                                                    mp.ReplayId = r.ReplayId AND 
                                                                    mp.PlayerId = mpsr.PlayerId AND
                                                                    mp.PlayerId = hp.PlayerId AND
                                                                    mp.PlayerId = mpsr.PlayerId
                                                                    WHERE mp.PlayerId = @PlayerId AND Character = @Character AND {gameModeString} AND ReplayBuild >= @ReplayBuildBegin AND ReplayBuild < @ReplayBuildEnd AND MapName = @MapName",
                                                                    new SQLiteParameter("@PlayerId", Settings.Default.UserPlayerId),
                                                                    new SQLiteParameter("@Character", character),
                                                                    new SQLiteParameter("@ReplayBuildBegin", replayBuild.Item1),
                                                                    new SQLiteParameter("@ReplayBuildEnd", replayBuild.Item2),
                                                                    new SQLiteParameter("@GameMode", gameMode),
                                                                    new SQLiteParameter("@MapName", mapName)).FirstOrDefault();
                    return amount.HasValue ? amount.Value : 0;
                }
            }

            public static int ReadTotalMatchAwards(MVPAwardType mvpAwardType, Season season, GameMode gameMode, string character)
            {
                var replayBuild = Utilities.GetSeasonReplayBuild(season);

                string gameModeString;
                if (gameMode != GameMode.Cooperative)
                    gameModeString = GameModeQueryString(false);
                else
                    gameModeString = GameModeQueryString(true);

                using (var db = new HeroesParserDataContext())
                {
                    int? amount = db.Database.SqlQuery<int?>($@"SELECT Count(ma.Award) FROM ReplayMatchPlayers mp
                                                                JOIN Replays r
                                                                JOIN ReplayAllHotsPlayers hp
                                                                JOIN ReplayMatchAwards ma 
                                                                ON mp.ReplayId = ma.ReplayId AND 
                                                                mp.ReplayId = r.ReplayId AND 
                                                                mp.PlayerId = ma.PlayerId AND
                                                                mp.PlayerId = hp.PlayerId AND
                                                                mp.PlayerId = ma.PlayerId
                                                                WHERE mp.PlayerId = @PlayerId AND Character = @Character AND {gameModeString} AND ReplayBuild >= @ReplayBuildBegin AND ReplayBuild < @ReplayBuildEnd AND Award = @AwardType",
                                                                new SQLiteParameter("@PlayerId", Settings.Default.UserPlayerId),
                                                                new SQLiteParameter("@Character", character),
                                                                new SQLiteParameter("@ReplayBuildBegin", replayBuild.Item1),
                                                                new SQLiteParameter("@ReplayBuildEnd", replayBuild.Item2),
                                                                new SQLiteParameter("@GameMode", gameMode),
                                                                new SQLiteParameter("@AwardType", mvpAwardType.ToString())).FirstOrDefault();
                    return amount.HasValue ? amount.Value : 0;
                }
            }

            private static string GameModeQueryString(bool allOrSingle)
            {
                if (allOrSingle)
                    return "GameMode >= 3";
                else
                    return "GameMode = @GameMode";
            }
        }
    }
}