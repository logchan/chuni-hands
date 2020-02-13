using Newtonsoft.Json;

namespace chuni_hands {
    public sealed class Config : Helpers.PropertyChangedInvoker {
        static Config() {
            Helpers.PatchNotifyPropertyChanged<Config>();
        }

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int Exposure { get; set; } = -6;
        public int SensorSize { get; set; } = 21;
        public double Threshold { get; set; } = 10;
        public int Distance { get; set; } = 40;

        public int BootstrapSeconds { get; set; } = 2;
        public int CaptureWidth { get; set; } = 640;
        public int CaptureHeight { get; set; } = 480;
        public int Fps { get; set; } = 60;
        public bool LogDiff { get; set; }
        public string SendKeyMode { get; set; } = "be";
        public string EndPoint { get; set; } = "http://10.233.3.22:4420/update_air";
        public bool ShowVideo { get; set; } = true;

        [JsonIgnore]
        public bool FreezeVideo { get; set; }
    }
}
