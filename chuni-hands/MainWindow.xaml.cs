using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Emgu.CV;

namespace chuni_hands {
    public partial class MainWindow : Window {
        private const string ConfigFile = "chuni-hands.json";

        private VideoCapture _capture;
        private Config _config = new Config();
        private volatile bool _closing = false;
        private readonly Mat _mat = new Mat();
        private byte[] _matData = new byte[0];
        private readonly List<Sensor> _sensors = new List<Sensor>(5);
        private Task _captureTask;
        private bool _hasPendingReset = false;
        private HttpClient _http = new HttpClient();

        public Config Config => _config;

        public MainWindow() {
            if (File.Exists(ConfigFile)) {
                _config = Helpers.Deserialize<Config>(ConfigFile);
            }

            InitializeComponent();
            Title += " version " + Helpers.GetVersion();

            TheCanvas.Sensors = _sensors;

            Logger.LogAdded += log => {
                LogBox.AppendText(log);
                LogBox.AppendText(Environment.NewLine);
                LogBox.ScrollToEnd();
            };
        }

        private void FrameUpdate() {

            // compute

            foreach (var sensor in _sensors) {
                sensor.Update(_mat, _hasPendingReset);
            }
            _hasPendingReset = false;

            foreach (var sensor in _sensors) {
                sensor.Draw(_mat);
            }

            // send key

            SendKey();

            // update display

            var length = _mat.Rows * _mat.Cols * _mat.NumberOfChannels;
            if (_matData.Length < length) {
                _matData = new byte[length];
            }

            _mat.CopyTo(_matData);

            var bm = BitmapSource.Create(_mat.Cols, _mat.Rows, 96, 96, PixelFormats.Bgr24, null, _matData, _mat.Cols * _mat.NumberOfChannels);
            TheCanvas.Image = bm;
            TheCanvas.InvalidateVisual();
        }

        private void SendKey() {
            if (IsActive) {
                return;
            }

            if (_sensors.All(s => !s.StateChanged)) {
                return;
            }

            switch (_config.SendKeyMode) {
                case "be": {
                        var airKeys = String.Concat(from sensor in _sensors select sensor.Active ? "1" : "0");
                        _http.GetAsync(_config.EndPoint + "?k=" + airKeys);
                    }
                    break;
                default:
                    throw new Exception("unknown SendKeyMode");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            var cap = new VideoCapture();
            cap.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, _config.CaptureWidth);
            cap.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, _config.CaptureHeight);
            cap.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Autofocus, 0);
            cap.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Exposure, _config.Exposure);

            _capture = cap;
            _capture.Read(_mat);
            _config.CaptureWidth = _mat.Cols;
            _config.CaptureHeight = _mat.Rows;

            for (var i = 0; i < 6; ++i) {
                _sensors.Add(new Sensor(i, _config));
            }

            _captureTask = Task.Run(CaptureLoop);
        }

        private void CaptureLoop() {
            // give camera some time to auto adjust, so user don't need to press reset right after start
            var bootstrapFrames = _config.BootstrapSeconds * _config.Fps;

            while (!_closing) {
                _capture.Read(_mat);
                if (bootstrapFrames > 0) {
                    --bootstrapFrames;
                }
                else {
                    Dispatcher?.BeginInvoke(new Action(FrameUpdate));
                }

                // what the...?? well, it works
                Thread.Sleep(1000 / _config.Fps);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            _closing = true;
            _captureTask.Wait();

            _capture.Stop();
            _capture.Dispose();

            Helpers.Serialize(_config, ConfigFile);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e) {
            _hasPendingReset = true;
        }

        private void SetThresholdButton_Click(object sender, RoutedEventArgs e) {
            if (Double.TryParse(ThresholdBox.Text, out var v)) {
                _config.Threshold = v;
            }
        }

        private void CenterButton_Click(object sender, RoutedEventArgs e) {
            _config.OffsetX = 0;
            _config.OffsetY = 0;
        }
    }
}
