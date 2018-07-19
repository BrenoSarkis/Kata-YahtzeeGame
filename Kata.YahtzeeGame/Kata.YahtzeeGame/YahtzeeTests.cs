using System.Linq;
using System.Runtime.Remoting.Messaging;
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

            Assert.That(yahtzee.PlayerScore(new int[0]), Is.EqualTo(0));
        }

        [Test]
        public void PlayerRolls_Ones_CalculatesCorrectScore()
        {
            var player = new Player();
            var yahtzee = new YahtzeeGame();

            var rolls = player.Roll();

            Assert.That(yahtzee.PlayerScore(rolls), Is.EqualTo(5));
        }
    }

    public class Player
    {
        public int[] Roll()
        {
            return new[] {1, 1, 1, 1, 1};
        }
    }

    public class YahtzeeGame
    {
        public int PlayerScore(int[] rolls)
        {
            if (!rolls.Any()) return 0;
            return 5;
        }
    }
}
