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

        [TestCase(1, 5)]
        [TestCase(2, 10)]
        [TestCase(3, 15)]
        [TestCase(4, 20)]
        [TestCase(5, 25)]
        public void PlayerRolls_TheSameValueForEveryRow_CalculatesCorrectScore(int rollValue, int expectedScore)
        {
            var fakeDice = new FakeDice(setAllRollsTo: rollValue);
            var player = new Player(fakeDice);

            var yahtzee = new YahtzeeGame();

            var rolls = player.Roll();

            Assert.That(yahtzee.PlayerScore(rolls), Is.EqualTo(expectedScore));
        }
    }

    public class FakeDice
    {
        public FakeDice(int setAllRollsTo)
        {
            Value = setAllRollsTo;
        }

        public int Value { get; private set; }
    }

    public class Player
    {
        private readonly FakeDice fakeDice;

        public Player(FakeDice fakeDice)
        {
            this.fakeDice = fakeDice;
        }

        public int[] Roll()
        {
            return new[] { fakeDice.Value, fakeDice.Value, fakeDice.Value, fakeDice.Value, fakeDice.Value };
        }
    }

    public class YahtzeeGame
    {
        public int PlayerScore(int[] rolls)
        {
            return rolls.Sum();
        }
    }
}
