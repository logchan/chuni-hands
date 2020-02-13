using System;
using System.Collections.Generic;
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

            dc.DrawImage(image, new Rect(paddingX, paddingY, image.Width * factor, image.Height * factor));
        }
    }
}
