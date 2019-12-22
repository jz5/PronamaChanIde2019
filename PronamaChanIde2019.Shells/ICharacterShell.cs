using System.Threading.Tasks;

namespace PronamaChanIde2019.Shells
{
    public interface ICharacterShell
    {
        void ExpressEmotion(bool buildSucceeded);

        #region For Debug

        Task BlinkAsync();
        Task ExpressEmotionAsync();
        bool EmotionTimerEnabled { get; set; }

        #endregion
    }
}
