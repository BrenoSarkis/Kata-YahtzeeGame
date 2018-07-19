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
            var fakeDice = new FakeDice(setAllRollsTo: 1);
            var player = new Player(fakeDice);

            var yahtzee = new YahtzeeGame();

            var rolls = player.Roll();

            Assert.That(yahtzee.PlayerScore(rolls), Is.EqualTo(5));
        }

        [Test]
        public void PlayerRolls_Twos_CalculatesCorrectScore()
        {
            var fakeDice = new FakeDice(setAllRollsTo: 2);
            var player = new Player(fakeDice);

            var yahtzee = new YahtzeeGame();

            var rolls = player.Roll();

            Assert.That(yahtzee.PlayerScore(rolls), Is.EqualTo(10));
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
