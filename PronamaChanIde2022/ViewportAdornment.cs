using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using PronamaChanIde2019.Shells;

namespace PronamaChanIde2022
{
    public class ViewportAdornment
    {
        private readonly IWpfTextView _view;
        private readonly IAdornmentLayer _adornmentLayer;
        private readonly double _shellWidth;
        private readonly double _shellHeight;

        public ICharacterShell Shell { get; }
        public UserControl ShellControl => (UserControl)Shell;


        public ViewportAdornment(IWpfTextView view)
        {
            _view = view;
            _view.ViewportHeightChanged += OnSizeChanged; ;
            _view.ViewportWidthChanged += OnSizeChanged;

            var mode = Environment.GetEnvironmentVariable("PRONAMA-CHAN_IDE_MODE", EnvironmentVariableTarget.User);
            if (!string.IsNullOrEmpty(mode))
            {
                if (mode == "CLASSIC")
                    Shell = new ClassicShell();
            }

            if (Shell == null)
                Shell = new NineteenShell();

            // Opacity
            ShellControl.Opacity = 0.4;

            if (double.TryParse(Environment.GetEnvironmentVariable("PRONAMA-CHAN_IDE_OPACITY", EnvironmentVariableTarget.User), out double opactiy))
            {
                if (0 < opactiy && opactiy <= 1)
                    (ShellControl).Opacity = opactiy;
            }

            // Width, Height
            var sizeInitialized = false;
            if (double.TryParse(Environment.GetEnvironmentVariable("PRONAMA-CHAN_IDE_HEIGHT", EnvironmentVariableTarget.User), out double height) &&
                height > 0)
            {
                sizeInitialized = true;

                _shellWidth *= height / _shellHeight;
                _shellHeight = height;
            }
            if (!sizeInitialized)
            {
                _shellWidth = Shell.DefaultWidth;
                _shellHeight = Shell.DefaultHeight;
            }
            (ShellControl).Width = _shellWidth;
            (ShellControl).Height = _shellHeight;

            // Grab a reference to the adornment layer that this adornment should be added to
            _adornmentLayer = view.GetAdornmentLayer("Pronama_chanIDE2022");
            _view.LayoutChanged += OnSizeChanged;

            //OnSizeChanged(this, EventArgs.Empty);
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            // clear the adornment layer of previous adornments
            _adornmentLayer.RemoveAllAdornments();

            // Place the image in the bottom right hand corner of the Viewport

            var w = (ShellControl).ActualWidth;
            var h = (ShellControl).ActualHeight;

            if ((ShellControl).ActualWidth == 0)
            {
                w = _shellWidth * (_view.ZoomLevel / 100);
                h = _shellHeight * (_view.ZoomLevel / 100);
            }

            Canvas.SetLeft((ShellControl), (_view.ViewportRight - w - 10));
            Canvas.SetTop((ShellControl), (_view.ViewportBottom - h - 10));

            // add the image to the adornment layer and make it relative to the viewport
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, (ShellControl), null);
        }

    }
}
