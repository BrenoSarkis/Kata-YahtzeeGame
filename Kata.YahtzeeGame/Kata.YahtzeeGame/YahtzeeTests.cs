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
        private Player player;
        private YahtzeeGame yahtzee;
        private FakeDice diceThatRollsOne; 
        private FakeDice diceThatRollsTwo;
        private FakeDice diceThatRollsThree; 
        private FakeDice diceThatRollsFour;
        private FakeDice diceThatRollsFive;

        [SetUp]
        public void SetUp()
        {
            player = new Player();
            yahtzee = new YahtzeeGame();

            diceThatRollsOne = new FakeDice(willReturnOnRoll: 1);
            diceThatRollsTwo = new FakeDice(willReturnOnRoll: 2);
            diceThatRollsThree = new FakeDice(willReturnOnRoll: 3);
            diceThatRollsFour = new FakeDice(willReturnOnRoll: 4);
            diceThatRollsFive = new FakeDice(willReturnOnRoll: 5);
        }

        [Test]
        public void WhenGameStarted_PlayerScoreIsZero()
        {
            var yahtzee = this.yahtzee;

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
            var yahtzee = this.yahtzee;

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

        [TestCase(Category.Ones, 4)]
        [TestCase(Category.Twos, 8)]
        [TestCase(Category.Threes, 12)]
        [TestCase(Category.Fours, 16)]
        [TestCase(Category.Fives, 20)]
        [TestCase(Category.Sixes, 24)]
        public void PlayerRolls_OnlyTheDesiredValueIsConsideredOnScore(Category category, int expectedScore)
        {
            var diceValue = new FakeDice(willReturnOnRoll: (int)category);
            var skippedDice = new FakeDice(willReturnOnRoll: (int)category -1);

            var firstRoll = player.Roll(diceValue);
            var secondRoll = player.Roll(diceValue);
            var thirdRoll = player.Roll(diceValue);
            var fourthRoll = player.Roll(diceValue);
            var finalRoll = player.Roll(skippedDice);

            Assert.That(yahtzee.CalculateScore(category, firstRoll, secondRoll, thirdRoll, fourthRoll, finalRoll), Is.EqualTo(expectedScore));
        }

        [Test]
        public void PlayerRolls_Pairs()
        {
            var firstRoll = player.Roll(diceThatRollsOne);
            var secondRoll = player.Roll(diceThatRollsTwo);
            var thirdRoll = player.Roll(diceThatRollsTwo);
            var fourthRoll = player.Roll(diceThatRollsTwo);
            var finalRoll = player.Roll(diceThatRollsOne);

            Assert.That(yahtzee.CalculateScore(Category.Pairs, firstRoll, secondRoll, thirdRoll, fourthRoll, finalRoll), Is.EqualTo(4));
        }

        [Test]
        public void PlayerRolls_Pairs_WhenThereAreNoPairsScoreIsZero()
        {
            var firstRoll = player.Roll(diceThatRollsOne);
            var secondRoll = player.Roll(diceThatRollsTwo);
            var thirdRoll = player.Roll(diceThatRollsThree);
            var fourthRoll = player.Roll(diceThatRollsFour);
            var finalRoll = player.Roll(diceThatRollsFive);

            Assert.That(yahtzee.CalculateScore(Category.Pairs, firstRoll, secondRoll, thirdRoll, fourthRoll, finalRoll), Is.EqualTo(0));
        }

        [Test]
        public void PlayerRolls_TwoPairs()
        {
            var firstRoll = player.Roll(diceThatRollsOne);
            var secondRoll = player.Roll(diceThatRollsOne);
            var thirdRoll = player.Roll(diceThatRollsTwo);
            var fourthRoll = player.Roll(diceThatRollsThree);
            var finalRoll = player.Roll(diceThatRollsThree);

            Assert.That(yahtzee.CalculateScore(Category.TwoPairs, firstRoll, secondRoll, thirdRoll, fourthRoll, finalRoll), Is.EqualTo(8));
        }
    }

    public enum Category
    {
        Chance,
        Ones,
        Twos,
        Threes,
        Fours,
        Fives,
        Sixes,
        Pairs,
        TwoPairs
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
                case Category.Twos:
                    return rolls.Where(r => r == (int)Category.Twos).Sum();
                case Category.Threes:
                    return rolls.Where(r => r == (int)Category.Threes).Sum();
                case Category.Fours:
                    return rolls.Where(r => r == (int)Category.Fours).Sum();
                case Category.Fives:
                    return rolls.Where(r => r == (int)Category.Fives).Sum();
                case Category.Sixes:
                    return rolls.Where(r => r == (int)Category.Sixes).Sum();
                case Category.Pairs:
                {
                    var pairs = rolls.GroupBy(r => r).Where(r => r.Count() > 1).Select(r => r.Key).Distinct();
                    return pairs.Any() ? pairs.Max(r => r) * 2 : 0;
                }
                case Category.TwoPairs:
                {
                    var pairs = rolls.GroupBy(r => r).Where(r => r.Count() > 1).Select(r => r.Key).Distinct();
                    var twoPairs = pairs.OrderByDescending(p => p).Take(2);
                    return twoPairs.Sum(p => p * 2);
                }
            }

            return rolls.Sum();
        }
    }
}
