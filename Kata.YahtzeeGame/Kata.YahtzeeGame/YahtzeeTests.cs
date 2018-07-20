using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Kata.YahtzeeGame
{
    [TestFixture]
    public class YahtzeeTests
    {
        [Test]
        public void WhenGameStarted_PlayerScoreIsZero()
        {
            var yahtzee = new YahtzeeGame();

            Assert.That(yahtzee.CalculateScore(Category.Chance, 0), Is.EqualTo(0));
        }

        [TestCase(1, 5)]
        [TestCase(2, 10)]
        [TestCase(3, 15)]
        [TestCase(4, 20)]
        [TestCase(5, 25)]
        [TestCase(6, 30)]
        public void PlayerRolls_TheSameValueForEveryRow_CalculatesCorrectScore(int rollValue, int expectedScore)
        {
            var dice = new FakeDice(willReturnOnRoll: rollValue);
            var player = new Player();
            var yahtzee = new YahtzeeGame();

            var rolls = new[]
            {
                player.Roll(dice),
                player.Roll(dice),
                player.Roll(dice),
                player.Roll(dice),
                player.Roll(dice),
            };

            Assert.That(yahtzee.CalculateScore(Category.Chance, rolls), Is.EqualTo(expectedScore));
        }

        [Test]
        public void PlayerRolls_OnlyTheDesiredValueIsConsideredOnScore()
        {
            int expectedScore = 4;
            var diceValueOne = new FakeDice(willReturnOnRoll: 1);
            var diceValueTwo = new FakeDice(willReturnOnRoll: 2);
            var player = new Player();

            var yahtzee = new YahtzeeGame();

            var firstRoll = player.Roll(diceValueOne);
            var secondRoll = player.Roll(diceValueTwo);
            var thirdRoll = player.Roll(diceValueOne);
            var fourthRoll = player.Roll(diceValueOne);
            var finalRoll = player.Roll(diceValueOne);

            Assert.That(yahtzee.CalculateScore(Category.Ones, firstRoll, secondRoll, thirdRoll, fourthRoll, finalRoll), Is.EqualTo(expectedScore));
        }
    }

    public enum Category
    {
        Chance,
        Ones
    }

    public class FakeDice
    {
        public FakeDice(int willReturnOnRoll)
        {
            Value = willReturnOnRoll;
        }

        public int Value { get; set; }
    }

    public class Player
    {
        public int Roll(FakeDice fakeDice)
        {
            return  fakeDice.Value;
        }
    }

    public class YahtzeeGame
    {
        public int CalculateScore(Category category, params int[] rolls)
        {
            switch (category)
            {
                case Category.Ones:
                    return rolls.Where(r => r == (int)Category.Ones).Sum();
            }

            return rolls.Sum();
        }
    }
}
