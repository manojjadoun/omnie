﻿using System;

namespace VimeoDotNet.Models
{
    [Serializable]
    public class VideoStats
    {
        public int plays { get; set; }
        public int likes { get; set; }
        public int comments { get; set; }
    }
}