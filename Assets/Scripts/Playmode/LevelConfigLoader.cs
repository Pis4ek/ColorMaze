using Configs;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Playmode
{
    public class LevelConfigLoader
    {
        private const string CONFIG_NAME_BASE = "LevelConfig";
        private const string CONFIGS_COUNT_NAME_BASE = "LevelsCountConfig";

        public LevelConfig LoadConfigForLevel(int level)
        {
            if (level <= 0) throw new Exception("Invalid level number. Level number can not be below zero.");

            var fileName = CONFIG_NAME_BASE + level;

            var text = Resources.Load<TextAsset>(fileName);
            return JsonConvert.DeserializeObject<LevelConfig>(text.text);
        }

        public LevelsCountConfig LoadLevelsCountConfig()
        {
            var text = Resources.Load<TextAsset>(CONFIGS_COUNT_NAME_BASE);
            return JsonConvert.DeserializeObject<LevelsCountConfig>(text.text);
        }

    }
}