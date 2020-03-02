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
    /// NineteenShell.xaml の相互作用ロジック
    /// </summary>
    public partial class NineteenShell : UserControl, ICharacterShell
    {
        private enum Images
        {
            Blink1, // 通常表情（口閉じ）
            Blink2,
            Blink3,
            FaceCried,
            FaceEyesClosed, // ><
            FaceHappy,
            FaceSad,
            FaceShocked,
            FaceSmug,
            FaceFine, // EyesClosed の口（口開け）+ 通常の目（Blink1 の目）

            Body1,
            Body2,

            HandLeft1,
            HandLeft2,
            HandLeft3, // 👍

            HandRight1,
            HandRight2,
        }

        private readonly DispatcherTimer _emotionTimer;
        private readonly Random _random = new Random();
        private readonly Dictionary<Images, BitmapImage> _images = new Dictionary<Images, BitmapImage>();

        public double DefaultWidth { get; } = 293.88;
        public double DefaultHeight { get; } = 500;

        public int EmotionIntervalMin { get; set; } = 2000;
        public int EmotionIntervalMax { get; set; } = 30000;
        public int BlinkPercentage { get; set; } = 80;
        public int FaceFinePercentage { get; set; } = 50;

        public bool EmotionTimerEnabled { get; set; } = true;

        public NineteenShell()
        {
            InitializeComponent();

            _emotionTimer = new DispatcherTimer();
            _emotionTimer.Tick += EmotionTimer_Tick;

            foreach (Images value in Enum.GetValues(typeof(Images)))
            {
                _images.Add(value, new BitmapImage(new Uri($"Images/Nineteen/{value}.png", UriKind.Relative)));
            }

            // Read environment variable
            // Emotion interval
            if (int.TryParse(Environment.GetEnvironmentVariable("PRONAMA-CHAN_IDE_NINETEEN_EMOTION_INTERVAL", EnvironmentVariableTarget.User), out int emotionInterval))
            {
                EmotionIntervalMax = EmotionIntervalMin = emotionInterval;
            }
            else if (int.TryParse(Environment.GetEnvironmentVariable("PRONAMA-CHAN_IDE_NINETEEN_EMOTION_INTERVAL_MIN", EnvironmentVariableTarget.User), out int emotionIntervalMin))
            {
                EmotionIntervalMin = emotionIntervalMin;

                // Max is need min
                if (int.TryParse(Environment.GetEnvironmentVariable("PRONAMA-CHAN_IDE_NINETEEN_EMOTION_INTERVAL_MAX", EnvironmentVariableTarget.User), out int emotionIntervalMax))
                    EmotionIntervalMax = emotionIntervalMax >= emotionIntervalMin ? emotionIntervalMax : emotionIntervalMin;
                else if (EmotionIntervalMin > EmotionIntervalMax)
                    EmotionIntervalMax = EmotionIntervalMin;

            }

            // Blink percentage
            if (int.TryParse(Environment.GetEnvironmentVariable("PRONAMA-CHAN_IDE_NINETEEN_BLINK_PERCENTAGE", EnvironmentVariableTarget.User), out int blinkPercentage))
                BlinkPercentage = blinkPercentage;

            // Face fine percentage
            if (int.TryParse(Environment.GetEnvironmentVariable("PRONAMA-CHAN_IDE_NINETEEN_FACEFINE_PERCENTAGE", EnvironmentVariableTarget.User), out int faceFinePercentage))
                FaceFinePercentage = faceFinePercentage;

            RestartTimer();
        }


        private void RestartTimer()
        {
            if (!EmotionTimerEnabled) return;

            _emotionTimer.Interval = TimeSpan.FromMilliseconds(_random.Next(EmotionIntervalMin, EmotionIntervalMax));
            _emotionTimer.Start();
        }
        private void StopTimer() => _emotionTimer.Stop();

        private async void EmotionTimer_Tick(object sender, EventArgs e)
        {
            await ExpressEmotionAsync();
        }

        public async Task BlinkAsync()
        {
            FaceImage.Source = _images[Images.Blink2];
            await Task.Factory.StartNew(() =>
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(80));
            }).ContinueWith(x =>
            {
                FaceImage.Source = _images[Images.Blink3];
            }, TaskScheduler.FromCurrentSynchronizationContext()).ContinueWith(x =>
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(80));
            }).ContinueWith(x =>
            {
                FaceImage.Source = _images[Images.Blink1];
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private Action ExpressEmotionAfterBlinking;
        public void ExpressEmotion(bool buildSucceeded)
        {
            // While blinking, execute again after blinking is completed
            if (!_emotionTimer.IsEnabled) // blinking
            {                
                ExpressEmotionAfterBlinking = () => ExpressEmotion(buildSucceeded);
                return;
            }

            ExpressEmotionAfterBlinking = null;

            StopTimer();

            if (buildSucceeded)
                ExpressSucceededEmotion();
            else
                ExpressFailedEmotion();

            RestartTimer();
        }

        private void ExpressSucceededEmotion()
        {
            var patterns = new[] {
                (Images.Body1, Images.HandLeft1, Images.HandRight1),
                (Images.Body2, Images.HandLeft2, Images.HandRight2),
                (Images.Body1, Images.HandLeft3, Images.HandRight1),
                (Images.Body2, Images.HandLeft3, Images.HandRight2),
            };

            var i = _random.Next(0, patterns.Length);
            BodyImage.Source = _images[patterns[i].Item1];
            HandLeftImage.Source = _images[patterns[i].Item2];
            HandRightImage.Source = _images[patterns[i].Item3];

            var faces = new[] {
                //Images.FaceEyesClosed,
                Images.FaceHappy,
                Images.FaceSmug,
            };

            FaceImage.Source = _images[faces[_random.Next(0, faces.Length)]];
        }

        private void ExpressFailedEmotion()
        {
            var patterns = new[] {
                (Images.Body1, Images.HandLeft1, Images.HandRight1),
                (Images.Body2, Images.HandLeft2, Images.HandRight2),
            };

            var i = _random.Next(0, patterns.Length);
            BodyImage.Source = _images[patterns[i].Item1];
            HandLeftImage.Source = _images[patterns[i].Item2];
            HandRightImage.Source = _images[patterns[i].Item3];

            var faces = new[] {
                //Images.FaceCried,
                Images.FaceSad,
                Images.FaceShocked,
            };

            FaceImage.Source = _images[faces[_random.Next(0, faces.Length)]];

        }

        public async Task ExpressEmotionAsync()
        {
            StopTimer();
            var blinked = false;

            if (_random.Next(0, 100) < BlinkPercentage)
            {
                // Blink
                blinked = true;
                await BlinkAsync();
            }
            else if (_random.Next(0, 100) < FaceFinePercentage)
            {
                // Change face
                FaceImage.Source = _images[Images.FaceFine];
            }
            else
            {
                // Change pause

                var patterns = new[] {
                    (Images.Body1, Images.HandLeft1, Images.HandRight1),
                    (Images.Body2, Images.HandLeft2, Images.HandRight2),
                };

                var i = _random.Next(0, patterns.Length);
                BodyImage.Source = _images[patterns[i].Item1];
                HandLeftImage.Source = _images[patterns[i].Item2];
                HandRightImage.Source = _images[patterns[i].Item3];
            }

            RestartTimer();

            if (blinked)
            {
                ExpressEmotionAfterBlinking?.Invoke();
            }
        }
    }
}
