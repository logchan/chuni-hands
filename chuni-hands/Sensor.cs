using System;
using Emgu.CV;
using Emgu.CV.Structure;

namespace chuni_hands {
    internal sealed class Sensor {
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool Active { get; set; }
        public bool StateChanged { get; set; }

        private Mat _startValue;
        private Config _config;
        private int _id;

        public Sensor(int id, Config config) {
            _id = id;
            _config = config;
        }

        private Mat GetPartial(Mat frame) {
            var sz = _config.SensorSize;
            return new Mat(frame, new Range(Y - sz / 2, Y + sz / 2 + 1), new Range(X - sz / 2, X + sz / 2 + 1));
        }

        public void Update(Mat frame, bool forceInit) {

            // reposition

            X = _config.CaptureWidth / 2 + _config.OffsetX;
            X = Math.Max(Math.Min(X, _config.CaptureWidth - _config.SensorSize), _config.SensorSize);

            Y = _config.CaptureHeight / 2 + _config.OffsetY + (_id - 3) * _config.Distance;
            Y = Math.Max(Math.Min(Y, _config.CaptureHeight - _config.SensorSize), _config.SensorSize);

            // check area

            var pixels = GetPartial(frame);

            if (_startValue == null || forceInit) {
                _startValue = new Mat(pixels.Size, Emgu.CV.CvEnum.DepthType.Cv64F, pixels.NumberOfChannels);
                pixels.ConvertTo(_startValue, _startValue.Depth);
                _startValue /= 255;

                Active = false;
                StateChanged = false;
                return;
            }

            var pixelsD = new Mat(pixels.Size, Emgu.CV.CvEnum.DepthType.Cv64F, pixels.NumberOfChannels);
            pixels.ConvertTo(pixelsD, pixelsD.Depth);
            pixelsD /= 255;

            var matDiff = pixelsD - _startValue;
            var diff = matDiff.Dot(matDiff);
            if (_id == 0 && _config.LogDiff) {
                Logger.Info($"diff: {diff}");
            }

            var active = diff > _config.Threshold;
            StateChanged = active != Active;
            Active = active;
        }

        public void Draw(Mat frame) {
            var pixels = GetPartial(frame);
            var color = new byte[] { 0, 0, 0 };
            if (Active) {
                color[0] = 255;
            }
            else {
                color[2] = 255;
            }

            for (var y = 0; y < pixels.Rows; ++y) {
                for (var x = 0; x < pixels.Cols; ++x) {
                    pixels.Row(y).Col(x).SetTo(color);
                }
            }
        }
    }
}
