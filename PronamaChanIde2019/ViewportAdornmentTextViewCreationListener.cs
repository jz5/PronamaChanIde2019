using System.ComponentModel.Composition;
using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace PronamaChanIde2019
{
    /// <summary>
    /// Establishes an <see cref="IAdornmentLayer"/> to place the adornment on and exports the <see cref="IWpfTextViewCreationListener"/>
    /// that instantiates the adornment on the event of a <see cref="IWpfTextView"/>'s creation
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class ViewportAdornmentTextViewCreationListener : IWpfTextViewCreationListener
    {
        [Import]
        public SVsServiceProvider ServiceProvider { get; set; }

        // Disable "Field is never assigned to..." and "Field is never used" compiler's warnings. Justification: the field is used by MEF.
#pragma warning disable 649, 169

        /// <summary>
        /// Defines the adornment layer for the scarlet adornment. This layer is ordered
        /// after the selection layer in the Z-order
        /// </summary>
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("Pronama_chanIDE2019")]
        [Order(Before = PredefinedAdornmentLayers.Caret)]
        private AdornmentLayerDefinition _editorAdornmentLayer;

#pragma warning restore 649, 169

        private ViewportAdornment _adornment;
        private BuildEvents _buildEvents;
        /// <summary>
        /// Instantiates a ViewportAdornment manager when a textView is created.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> upon which the adornment should be placed</param>
        public void TextViewCreated(IWpfTextView textView)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _adornment = new ViewportAdornment(textView);

            if (ServiceProvider.GetService(typeof(DTE)) is DTE dte)
            {
                _buildEvents = dte.Events.BuildEvents;
                dte.Events.BuildEvents.OnBuildProjConfigDone += BuildEvents_OnBuildProjConfigDone;
            }
        }

        private void BuildEvents_OnBuildProjConfigDone(string Project, string ProjectConfig, string Platform, string SolutionConfig, bool Success)
        {
            if (_adornment != null)
                _adornment.Shell.ExpressEmotion(Success);
        }
    }
}
