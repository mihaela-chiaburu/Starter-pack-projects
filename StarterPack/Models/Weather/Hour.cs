﻿namespace StarterPack.Models.Weather
{
    public class Hour
    {
        public string Time { get; set; }
        public float Temp_C { get; set; }
        public Condition Condition { get; set; }
    }
}
