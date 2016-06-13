﻿namespace AbilityLastHitMarker
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage.Common.Menu;

    internal class MenuManager
    {
        #region Fields

        private readonly string heroName;

        private readonly Menu menu;

        #endregion

        #region Constructors and Destructors

        public MenuManager(List<string> abilties, string name)
        {
            heroName = name;
            abilties.Reverse(); // correct order

            menu = new Menu("Ability Last Hit", "abilityLastHit", true);
            menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
            menu.AddItem(new MenuItem("sum", "Sum damage").SetValue(false))
                .SetTooltip("Damage will be summed from all selected abilities");
            menu.AddItem(
                new MenuItem("enabledAbilitiesAbilityLastHit", "Enabled abilities", true).SetValue(
                    new AbilityToggler(abilties.ToDictionary(x => x, x => true))));
            var configMenu = new Menu("Config", "abilityLastHitConfig");
            configMenu.AddItem(new MenuItem("iconSize", "Icon size").SetValue(new Slider(30, 10, 50)));
            configMenu.AddItem(new MenuItem("yPosition", "Y position").SetValue(new Slider(0, -100)));

            menu.AddSubMenu(configMenu);
            menu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public bool IsEnabled => menu.Item("enabled").IsActive();

        public float Size => menu.Item("iconSize").GetValue<Slider>().Value;

        public bool SumEnabled => menu.Item("sum").IsActive();

        public float Yposition => menu.Item("yPosition").GetValue<Slider>().Value;

        #endregion

        #region Public Methods and Operators

        public bool AbilityActive(string abilityName)
        {
            return
                menu.Item(heroName + "enabledAbilitiesAbilityLastHit").GetValue<AbilityToggler>().IsEnabled(abilityName);
        }

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
        }

        #endregion
    }
}