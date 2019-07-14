﻿using OsuParsers.Database;
using OsuParsers.Database.Objects;
using OsuParsers.Enums;
using OsuParsers.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsuParsers.Decoders
{
    internal class DatabaseDecoder
    {
        public OsuDatabase DecodeOsu(Stream s)
        {
            OsuDatabase db = new OsuDatabase();
            SerializationReader r = new SerializationReader(s);

            db.OsuVersion = r.ReadInt32();
            db.FolderCount = r.ReadInt32();
            db.AccountUnlocked = r.ReadBoolean();
            db.UnlockDate = r.ReadDateTime();
            db.PlayerName = r.ReadString();
            int beatmapCount = r.ReadInt32();
            db.BeatmapCount = beatmapCount;

            for (int i = 0; i < beatmapCount; i++)
            {
                DbBeatmap beatmap = new DbBeatmap();

                beatmap.BytesOfBeatmapEntry = r.ReadInt32();
                beatmap.Artist = r.ReadString();
                beatmap.ArtistUnicode = r.ReadString();
                beatmap.Title = r.ReadString();
                beatmap.TitleUnicode = r.ReadString();
                beatmap.Creator = r.ReadString();
                beatmap.Difficulty = r.ReadString();
                beatmap.AudioFileName = r.ReadString();
                beatmap.MD5Hash = r.ReadString();
                beatmap.FileName = r.ReadString();
                beatmap.RankedStatus = (RankedStatus)r.ReadByte();
                beatmap.CirclesCount = r.ReadUInt16();
                beatmap.SlidersCount = r.ReadUInt16();
                beatmap.SpinnersCount = r.ReadUInt16();
                beatmap.LastModifiedTime = r.ReadDateTime();
                beatmap.ApproachRate = db.OsuVersion >= 20140609 ? r.ReadSingle() : r.ReadByte();
                beatmap.CircleSize = db.OsuVersion >= 20140609 ? r.ReadSingle() : r.ReadByte();
                beatmap.HPDrain = db.OsuVersion >= 20140609 ? r.ReadSingle() : r.ReadByte();
                beatmap.OverallDifficulty = db.OsuVersion >= 20140609 ? r.ReadSingle() : r.ReadByte();
                beatmap.SliderVelocity = r.ReadDouble();

                if (db.OsuVersion >= 20140609)
                {
                    beatmap.StandardStarRating = r.ReadDictionary<Mods, double>();
                    beatmap.TaikoStarRating = r.ReadDictionary<Mods, double>();
                    beatmap.CatchStarRating = r.ReadDictionary<Mods, double>();
                    beatmap.ManiaStarRating = r.ReadDictionary<Mods, double>();
                }

                beatmap.DrainTime = r.ReadInt32();
                beatmap.TotalTime = r.ReadInt32();
                beatmap.AudioPreviewTime = r.ReadInt32();

                int timingPointsCount = r.ReadInt32();
                for (int j = 0; j < timingPointsCount; j++)
                {
                    DbTimingPoint timingPoint = new DbTimingPoint();
                    timingPoint.BPM = r.ReadDouble();
                    timingPoint.Offset = r.ReadDouble();
                    timingPoint.Inherited = r.ReadBoolean();
                    beatmap.TimingPoints.Add(timingPoint);
                }

                beatmap.BeatmapId = r.ReadInt32();
                beatmap.BeatmapSetId = r.ReadInt32();
                beatmap.ThreadId = r.ReadInt32();
                beatmap.StandardGrade = (Grade)r.ReadByte();
                beatmap.TaikoGrade = (Grade)r.ReadByte();
                beatmap.CatchGrade = (Grade)r.ReadByte();
                beatmap.ManiaGrade = (Grade)r.ReadByte();
                beatmap.LocalOffset = r.ReadInt16();
                beatmap.StackLeniency = r.ReadSingle();
                beatmap.Ruleset = (Ruleset)r.ReadByte();
                beatmap.Source = r.ReadString();
                beatmap.Tags = r.ReadString();
                beatmap.OnlineOffset = r.ReadInt16();
                beatmap.TitleFont = r.ReadString();
                beatmap.IsUnplayed = r.ReadBoolean();
                beatmap.LastPlayed = r.ReadDateTime();
                beatmap.IsOsz2 = r.ReadBoolean();
                beatmap.FolderName = r.ReadString();
                beatmap.LastCheckedAgainstOsuRepo = r.ReadDateTime();
                beatmap.IgnoreBeatmapSound = r.ReadBoolean();
                beatmap.IgnoreBeatmapSkin = r.ReadBoolean();
                beatmap.DisableStoryboard = r.ReadBoolean();
                beatmap.DisableVideo = r.ReadBoolean();
                beatmap.VisualOverride = r.ReadBoolean();
                if (db.OsuVersion < 20140609)
                    r.BaseStream.Seek(sizeof(short), SeekOrigin.Current); //let's skip this unknown variable
                r.BaseStream.Seek(sizeof(int), SeekOrigin.Current); //and this one
                beatmap.ManiaScrollSpeed = r.ReadByte();

                db.Beatmaps.Add(beatmap);
            }

            db.Permissions = (Permissions)r.ReadInt32();

            return db;
        }

        public CollectionDatabase DecodeCollection(Stream s)
        {
            CollectionDatabase db = new CollectionDatabase();
            SerializationReader r = new SerializationReader(s);

            db.OsuVersion = r.ReadInt32();
            int collectionsCount = r.ReadInt32();
            db.CollectionCount = collectionsCount;

            for (int i = 0; i < collectionsCount; i++)
            {
                Collection collection = new Collection();

                collection.Name = r.ReadString();
                int count = r.ReadInt32();
                collection.Count = count;

                for (int j = 0; j < count; j++)
                    collection.MD5Hashes.Add(r.ReadString());

                db.Collections.Add(collection);
            }

            return db;
        }

        public ScoresDatabase DecodeScores(Stream s)
        {
            ScoresDatabase db = new ScoresDatabase();
            SerializationReader r = new SerializationReader(s);

            db.OsuVersion = r.ReadInt32();
            int beatmapCount = r.ReadInt32();
            
            for (int i = 0; i < beatmapCount; i++)
            {
                string md5Hash = r.ReadString();
                int scoresCount = r.ReadInt32();
                List<Score> scores = new List<Score>();

                for (int j = 0; j < scoresCount; j++)
                {
                    Score score = new Score();
                    score.Ruleset = (Ruleset)r.ReadByte();
                    score.OsuVersion = r.ReadInt32();
                    score.BeatmapMD5Hash = r.ReadString();
                    score.PlayerName = r.ReadString();
                    score.ReplayMD5Hash = r.ReadString();
                    score.Count300 = r.ReadUInt16();
                    score.Count100 = r.ReadUInt16();
                    score.Count50 = r.ReadUInt16();
                    score.CountGeki = r.ReadUInt16();
                    score.CountKatu = r.ReadUInt16();
                    score.CountMiss = r.ReadUInt16();
                    score.ReplayScore = r.ReadInt32();
                    score.Combo = r.ReadUInt16();
                    score.PerfectCombo = r.ReadBoolean();
                    score.Mods = (Mods)r.ReadInt32();
                    string lifeBarGraphData = r.ReadString();
                    score.ScoreTimestamp = r.ReadDateTime();
                    r.BaseStream.Seek(sizeof(int), SeekOrigin.Current);
                    score.ScoreId = r.ReadInt64();
                    scores.Add(score);
                }

                db.Scores.Add(new Tuple<string, List<Score>>(md5Hash, scores));
            }

            return db;
        }

        public PresenceDatabase DecodePresence(Stream s)
        {
            PresenceDatabase db = new PresenceDatabase();
            SerializationReader r = new SerializationReader(s);

            db.OsuVersion = r.ReadInt32();
            int playersCount = r.ReadInt32();

            for (int i = 0; i < playersCount; i++)
            {
                Player player = new Player();
                player.UserId = r.ReadInt32();
                player.Username = r.ReadString();
                player.Timezone = r.ReadByte() - 24;
                player.CountryCode = r.ReadByte();
                byte b = r.ReadByte();
                player.Permissions = (Permissions)(b & ~0xe0);
                player.Ruleset = (Ruleset)Math.Max(0, Math.Min(3, ((b & 0xe0) >> 5)));
                player.Longitude = r.ReadSingle();
                player.Latitude = r.ReadSingle();
                player.Rank = r.ReadInt32();
                player.LastUpdateTime = r.ReadDateTime();
                db.Players.Add(player);
            }

            return db;
        }
    }
}
