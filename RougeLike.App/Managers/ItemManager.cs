﻿using RougeLike.App.Abstract;
using RougeLike.App.Common;
using RougeLike.Domain.Common;
using RougeLike.Domain.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RougeLike.App.Managers
{
    public class ItemManager
    {
        private readonly IItemService _itemService;

        public ItemManager(IItemService itemService)
        {
            _itemService = itemService;
        }

        public Item GetRandomItem(int level, CharacterClass classType)
        {
            Random random = new Random();
            Item item = null;
            bool isGoodItem = false;
            int id = 0;
            while (!isGoodItem)
            {
                id = random.Next(0, _itemService.GetCount());
                item = _itemService.GetItem(id);
                if (item.MinLevel <= level && item.CompatibiltyClass == classType)
                {
                    isGoodItem = true;
                }
            }

            return item;
        }

        public void Initialize()
        {
            List<Item> items = new List<Item>();

            if (File.Exists(ItemsInitializer.itemsBase))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Item>));

                StreamReader reader = new StreamReader(ItemsInitializer.itemsBase);

                items = (List<Item>)serializer.Deserialize(reader);
                reader.Close();
            }
            else
            {
                items = ItemsInitializer.Initialize();
                Save();
            }

            _itemService.SetList(items);
        }

        private void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Item>));

            FileStream writer = File.Create(ItemsInitializer.itemsBase);
            serializer.Serialize(writer, _itemService.GetAll());
            writer.Close();
        }

        public List<Item> GetShopStuff(int level, CharacterClass classType)
        {
            List<Item> items = _itemService.GetAll();
            List<Item> selected = new List<Item>();

            Random random = new Random();

            for (int i = 0; i < 6; i++)
            {
                bool isGoodItem = false;
                while (!isGoodItem)
                {
                    Item selectedItem = items[random.Next(0, items.Count)];

                    if (selectedItem.MinLevel <= level && selectedItem.CompatibiltyClass == classType)
                    {
                        isGoodItem = true;
                        selected.Add(selectedItem);
                    }
                }
            }

            return selected;
        }
    }
}