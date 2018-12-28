﻿namespace VeChainCore.Models
{
    public class CallResult
    {
        public string data { get; set; }
        public Event[] events { get; set; }
        public Transfer[] transfers { get; set; }
        public bool reverted { get; set; }
        public string vmError { get; set; }
    }
}