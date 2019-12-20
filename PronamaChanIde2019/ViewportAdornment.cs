using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PronamaChanIde2019
{
    public class ViewportAdornment
    {
        private readonly IWpfTextView _view;
        private readonly IAdornmentLayer _adornmentLayer;
        private readonly double _shellWidth = 200;
        private readonly double _shellHeight = 484.26;

        public BackgroundShell Shell { get; }

        public ViewportAdornment(IWpfTextView view)
        {
            _view = view;
            _view.ViewportHeightChanged += OnSizeChanged; ;
            _view.ViewportWidthChanged += OnSizeChanged;

            Shell = new BackgroundShell
            {
                Opacity = 0.35
            };

            if (double.TryParse(Environment.GetEnvironmentVariable("PRONAMA-CHAN_IDE_OPACITY", EnvironmentVariableTarget.User), out double opactiy))
            {
                if (0 < opactiy && opactiy <= 1)
                    Shell.Opacity = opactiy;
            }

            if (double.TryParse(Environment.GetEnvironmentVariable("PRONAMA-CHAN_IDE_HEIGHT", EnvironmentVariableTarget.User), out double height))
            {
                if (height > 0)
                {
                    _shellWidth *= height / _shellHeight;
                    _shellHeight = height;

                    Shell.Width = _shellWidth;
                    Shell.Height = _shellHeight;
                }
            }

            // Grab a reference to the adornment layer that this adornment should be added to
            _adornmentLayer = view.GetAdornmentLayer("Pronama_chanIDE2019");
            _view.LayoutChanged += OnSizeChanged;

            //OnSizeChanged(this, EventArgs.Empty);
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {

            // clear the adornment layer of previous adornments
            _adornmentLayer.RemoveAllAdornments();

            // Place the image in the bottom right hand corner of the Viewport

            var w = Shell.ActualWidth;
            var h = Shell.ActualHeight;

            if (Shell.ActualWidth == 0)
            {
                w = _shellWidth * (_view.ZoomLevel / 100);
                h = _shellHeight * (_view.ZoomLevel / 100);
            }

            Canvas.SetLeft(Shell, (_view.ViewportRight - w - 10));
            Canvas.SetTop(Shell, (_view.ViewportBottom - h - 10));

            // add the image to the adornment layer and make it relative to the viewport
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, Shell, null);
        }

    }
}
