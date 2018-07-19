using System.Threading;
using NUnit.Framework;

namespace Kata.YahtzeeGame
{
    [TestFixture]
    public class YahtzeeTests
    {
        [Test]
        public void WhenGameStarted_PlayerScoreIsZero()
        {
            var yahtzee = new YahtzeeGame();

            Assert.That(yahtzee.PlayerScore, Is.EqualTo(0));
        }
    }

    public class YahtzeeGame
    {
        public int PlayerScore { get; } = 0;
    }
}
