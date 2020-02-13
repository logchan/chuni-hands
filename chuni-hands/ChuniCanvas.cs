﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace chuni_hands {
    internal sealed class ChuniCanvas : Canvas {

        public IEnumerable<Sensor> Sensors { get; set; }

        public ImageSource Image {
            get => (ImageSource)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(ChuniCanvas), new PropertyMetadata(null));

        public bool DrawImage {
            get { return (bool)GetValue(DrawImageProperty); }
            set { SetValue(DrawImageProperty, value); }
        }

        public static readonly DependencyProperty DrawImageProperty =
            DependencyProperty.Register("DrawImage", typeof(bool), typeof(ChuniCanvas), new PropertyMetadata(false));

        protected override void OnRender(DrawingContext dc) {
            base.OnRender(dc);

            var image = Image;
            if (image == null) {
                return;
            }

            var factor = ActualWidth / image.Width;
            factor = Math.Min(factor, ActualHeight / image.Height);
            var paddingX = (ActualWidth - image.Width * factor) / 2;
            var paddingY = (ActualHeight - image.Height * factor) / 2;
            var imageRect = new Rect(paddingX, paddingY, image.Width * factor, image.Height * factor);

            if (DrawImage) {
                dc.DrawImage(image, imageRect);
            }
            else {
                dc.DrawRectangle(Brushes.LightGray, null, imageRect);
            }

            DrawSensors(dc, factor, paddingX, paddingY);
        }

        private void DrawSensors(DrawingContext dc, double factor, double paddingX, double paddingY) {
            if (Sensors == null) {
                return;
            }

            foreach (var sensor in Sensors) {
                var brush = sensor.Active ? Brushes.Green : Brushes.Red;
                var sz = sensor.Size;
                var x = paddingX + (sensor.X - sz / 2) * factor;
                var y = paddingY + (sensor.Y - sz / 2) * factor;

                dc.DrawRectangle(brush, null, new Rect(x, y, sz * factor, sz * factor));

                /*
                // draw sensor id for debug
                var type = new Typeface("Microsoft YaHei");
                dc.DrawText(new FormattedText(sensor.Id.ToString(), 
                    CultureInfo.CurrentCulture, FlowDirection.LeftToRight, type, 12, Brushes.White, 96), 
                    new Point(x, y));
                    */
            }
        }
    }
}
