using System.Threading.Tasks;

namespace PronamaChanIde2019.Shells
{
    public interface ICharacterShell
    {
        double DefaultWidth { get; }
        double DefaultHeight { get; }

        void ExpressEmotion(bool buildSucceeded);

        #region For Debug

        Task BlinkAsync();
        Task ExpressEmotionAsync();
        bool EmotionTimerEnabled { get; set; }

        #endregion
    }
}
