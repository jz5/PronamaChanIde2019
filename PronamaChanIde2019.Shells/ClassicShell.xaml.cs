using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PronamaChanIde2019.Shells
{
    /// <summary>
    /// ClassicShell.xaml の相互作用ロジック
    /// </summary>
    public partial class ClassicShell : UserControl, ICharacterShell
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

        private readonly DispatcherTimer _faceTimer;
        private readonly Random _random = new Random();
        private readonly Dictionary<Face, BitmapImage> _images = new Dictionary<Face, BitmapImage>();

        public bool EmotionTimerEnabled { get; set; } = true;

        public ClassicShell()
        {
            InitializeComponent();

            _faceTimer = new DispatcherTimer();
            _faceTimer.Tick += FaceTimer_Tick;

            _images.Add(Face.Blink1, new BitmapImage(new Uri("Images/Classic/blink1.png", UriKind.Relative)));
            _images.Add(Face.Blink2, new BitmapImage(new Uri("Images/Classic/blink2.png", UriKind.Relative)));
            _images.Add(Face.Blink3, new BitmapImage(new Uri("Images/Classic/blink3.png", UriKind.Relative)));

            _images.Add(Face.Cry, new BitmapImage(new Uri("Images/Classic/cry.png", UriKind.Relative)));
            _images.Add(Face.Fine, new BitmapImage(new Uri("Images/Classic/fine.png", UriKind.Relative)));
            _images.Add(Face.Proud, new BitmapImage(new Uri("Images/Classic/proud.png", UriKind.Relative)));
            _images.Add(Face.Surprise, new BitmapImage(new Uri("Images/Classic/surprise.png", UriKind.Relative)));
            _images.Add(Face.Wink, new BitmapImage(new Uri("Images/Classic/wink.png", UriKind.Relative)));

            RestartTimer();
        }

        private void RestartTimer()
        {
            if (!EmotionTimerEnabled) return;

            _faceTimer.Interval = TimeSpan.FromSeconds(_random.Next(2, 30));
            _faceTimer.Start();
        }

        private async void FaceTimer_Tick(object sender, EventArgs e)
        {
            await ExpressEmotionAsync();
        }

        public async Task Fine()
        {
            FaceImage.Source = _images[Face.Fine];
            await Task.Factory.StartNew(() => Thread.Sleep(TimeSpan.FromSeconds(3)));
        }

        public async Task BlinkAsync()
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

        public void ExpressEmotion(bool buildSucceeded)
        {
            if (!_faceTimer.IsEnabled) // blinking
                return;

            _faceTimer.Stop();
            if (buildSucceeded)
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

        public async Task ExpressEmotionAsync()
        {
            _faceTimer.Stop();
            if (_random.Next(0, 10) < 8)
            {
                await BlinkAsync();
            }
            else
            {
                FaceImage.Source = _images[Face.Proud];
            }
            RestartTimer();
        }
    }
}
