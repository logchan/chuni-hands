using System;
using Emgu.CV;
using Emgu.CV.Structure;

namespace chuni_hands {
    internal sealed class Sensor {
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool Active { get; private set; }
        public bool StateChanged { get; private set; }
        public int Size => _config.SensorSize;
        public int Id => _id;

        private Mat _startValue;
        private readonly Config _config;
        private readonly int _id;

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

            Y = _config.CaptureHeight - (_config.CaptureHeight / 2 + _config.OffsetY + _id * _config.Distance);
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
    }
}
