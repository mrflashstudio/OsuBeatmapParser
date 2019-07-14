﻿using OsuParsers.Database.Objects;
using OsuParsers.Writers;
using System;
using System.Collections.Generic;

namespace OsuParsers.Database
{
    public class ScoresDatabase
    {
        public int OsuVersion { get; set; }
        public List<Tuple<string, List<Score>>> Scores { get; private set; } = new List<Tuple<string, List<Score>>>();

        public void Write(string path)
        {
            DatabaseWriter.WriteScoresDatabase(path, this);
        }
    }
}
