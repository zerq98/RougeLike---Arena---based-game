﻿using FluentAssertions;
using Moq;
using RougeLike.App.Abstract;
using RougeLike.App.Concrete;
using RougeLike.App.Managers;
using RougeLike.Domain.Common;
using RougeLike.Domain.Entity;
using System.Collections.Generic;
using Xunit;

namespace RougeLike.Tests
{
    public class CharacterManagerTests
    {
        private Character character = new Character()
        {
            Name = "Test",
            Class = CharacterClass.warrior,
            Health = 1,
            MaxHealth = 1,
            Level = 1,
            Money = 0,
            Experience = 0,
            NeededExperience = 1,
            Armor = 1,
            AttackDamage = 1,
            AttackSpeed = 1,
            Inventory = new Inventory() { Capacity = 1, Items = new List<Item>() }
        };

        [Fact]
        public void Should_Update_Hero()
        {
            var mock = new Mock<ICharacterService>();
            mock.Setup(s => s.UpdateCharacter(character));
            mock.Setup(s => s.GetCharacter()).Returns(character);

            var manager = new CharacterManager(new MenuActionService(), mock.Object);
            manager.UpdateCharacterAfterFight(1, 1);
            var levelAfterUpdate = manager.GetHeroLevel();

            levelAfterUpdate.Should().Be(2);
        }

        [Fact]
        public void Should_Add_Item_To_Inventory()
        {
            var mock = new Mock<ICharacterService>();
            mock.Setup(s => s.GetCharacter()).Returns(character);

            var manager = new CharacterManager(new MenuActionService(), mock.Object);

            Item item = new Item()
            {
                Id = 1,
                Name = "Test item",
                CompatibiltyClass = CharacterClass.warrior,
                Cost = 1,
                HeroStatToChange = "Attack Damage",
                IsUsable = false,
                MinLevel = 0,
                Value = 1
            };

            var itemsCount = manager.AddItemToInventory(item);
            var isStatUpdated = manager.GetHiddenStats()[0];

            itemsCount.Should().BeOfType(typeof(int));
            itemsCount.Should().Be(1);
            isStatUpdated.Should().BeOfType(typeof(int));
            isStatUpdated.Should().Be(2);
        }

        [Fact]
        public void Should_Sell_Item_From_Inventory()
        {
            Item item = new Item()
            {
                Id = 0,
                Name = "Test item",
                CompatibiltyClass = CharacterClass.warrior,
                Cost = 2,
                HeroStatToChange = "Attack Damage",
                IsUsable = false,
                MinLevel = 0,
                Value = 1
            };
            character.Inventory.Items.Add(item);
            character.AttackDamage = 2;

            var mock = new Mock<ICharacterService>();
            mock.Setup(s => s.UpdateCharacter(character));
            mock.Setup(s => s.GetCharacter()).Returns(character);

            var manager = new CharacterManager(new MenuActionService(), mock.Object);

            manager.SellItem(0);
            var checkMoney = manager.GetMoneyCount();
            var isStatUpdated = manager.GetHiddenStats()[0];

            checkMoney.Should().BeOfType(typeof(int));
            checkMoney.Should().Be(1);
            isStatUpdated.Should().BeOfType(typeof(int));
            isStatUpdated.Should().Be(1);
        }

        [Fact]
        public void Should_Buy_Item()
        {
            Item item = new Item()
            {
                Id = 0,
                Name = "Test item",
                CompatibiltyClass = CharacterClass.warrior,
                Cost = 2,
                HeroStatToChange = "Attack Damage",
                IsUsable = false,
                MinLevel = 0,
                Value = 1
            };
            character.Money = 2;

            var mock = new Mock<ICharacterService>();
            mock.Setup(s => s.UpdateCharacter(character));
            mock.Setup(s => s.GetCharacter()).Returns(character);

            var manager = new CharacterManager(new MenuActionService(), mock.Object);

            manager.BuyItem(item);
            var checkMoney = manager.GetMoneyCount();
            var isStatUpdated = manager.GetHiddenStats()[0];

            checkMoney.Should().BeOfType(typeof(int));
            checkMoney.Should().Be(0);
            isStatUpdated.Should().BeOfType(typeof(int));
            isStatUpdated.Should().Be(2);
        }
    }
}