using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PronamaChanIde2019
{
    public partial class BackgroundShell : UserControl
    {

        private enum Face
        {
            Blink1,
            Blink2,
            Blink3,
            Cry,
            Fine,
            Proud,
            Surprise,
            Wink
        }

        private DispatcherTimer _faceTimer;
        private Random _random = new Random();
        private readonly Dictionary<Face, BitmapImage> _images = new Dictionary<Face, BitmapImage>();

        public BackgroundShell()
        {
            InitializeComponent();

            _faceTimer = new DispatcherTimer();
            _faceTimer.Tick += _faceTimer_Tick;

            _images.Add(Face.Blink1, new BitmapImage(new Uri("Images/blink1.png", UriKind.Relative)));
            _images.Add(Face.Blink2, new BitmapImage(new Uri("Images/blink2.png", UriKind.Relative)));
            _images.Add(Face.Blink3, new BitmapImage(new Uri("Images/blink3.png", UriKind.Relative)));

            _images.Add(Face.Cry, new BitmapImage(new Uri("Images/cry.png", UriKind.Relative)));
            _images.Add(Face.Fine, new BitmapImage(new Uri("Images/fine.png", UriKind.Relative)));
            _images.Add(Face.Proud, new BitmapImage(new Uri("Images/proud.png", UriKind.Relative)));
            _images.Add(Face.Surprise, new BitmapImage(new Uri("Images/surprise.png", UriKind.Relative)));
            _images.Add(Face.Wink, new BitmapImage(new Uri("Images/wink.png", UriKind.Relative)));

            RestartTimer();
        }

        private void RestartTimer()
        {
            _faceTimer.Interval = TimeSpan.FromSeconds(_random.Next(2, 30));
            _faceTimer.Start();
        }

        private async void _faceTimer_Tick(object sender, EventArgs e)
        {
            _faceTimer.Stop();
            if (_random.Next(0, 10) < 8)
            {
                await Blink();
            }
            else
            {
                FaceImage.Source = _images[Face.Proud];
            }
            RestartTimer();
        }

        public async Task Fine()
        {
            FaceImage.Source = _images[Face.Fine];
            await Task.Factory.StartNew(() => Thread.Sleep(TimeSpan.FromSeconds(3)));
        }

        public async Task Blink()
        {
            FaceImage.Source = _images[Face.Blink2];
            await Task.Factory.StartNew(() =>
            {
                Thread.Sleep(new TimeSpan(800000));
            }).ContinueWith((x) =>
            {
                FaceImage.Source = _images[Face.Blink3];
            }, TaskScheduler.FromCurrentSynchronizationContext()).ContinueWith((x) =>
            {
                Thread.Sleep(new TimeSpan(800000));
            }).ContinueWith((x) =>
            {
                FaceImage.Source = _images[Face.Blink1];
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void ChangeFace(bool buildSuccess)
        {
            if (!_faceTimer.IsEnabled) // blinking
                return;

            _faceTimer.Stop();
            if (buildSuccess)
            {
                if (_random.Next(0, 10) < 5)
                    FaceImage.Source = _images[Face.Fine];
                else
                    FaceImage.Source = _images[Face.Wink];
            }
            else if (_random.Next(0, 10) < 7)
                FaceImage.Source = _images[Face.Surprise];
            else
                FaceImage.Source = _images[Face.Cry];
            RestartTimer();
        }

    }
}
