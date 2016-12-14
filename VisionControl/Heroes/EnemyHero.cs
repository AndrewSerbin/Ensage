﻿namespace VisionControl.Heroes
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;

    using SharpDX;

    using Units.Wards;

    internal class EnemyHero
    {
        #region Constants

        private const string DispenserName = "item_ward_dispenser";

        private const string ObserverName = "item_ward_observer";

        private const string SentryName = "item_ward_sentry";

        #endregion

        #region Fields

        private readonly Hero hero;

        #endregion

        #region Constructors and Destructors

        public EnemyHero(Hero enemy)
        {
            hero = enemy;
            Handle = enemy.Handle;
            ObserversCount = CountObservers();
            SentryCount = CountSentries();
        }

        #endregion

        #region Public Properties

        public uint Handle { get; }

        public bool IsAlive => hero.IsAlive;

        public bool IsValid => hero.IsValid;

        public bool IsVisible => IsValid && hero.IsVisible;

        public uint ObserversCount { get; set; }

        public Vector3 Position => hero.Position;

        public uint SentryCount { get; set; }

        #endregion

        #region Public Methods and Operators

        public double Angle(Ward ward)
        {
            return hero.FindRelativeAngle(ward.Position);
        }

        public uint CountObservers()
        {
            return (hero.FindItem(ObserverName)?.CurrentCharges ?? 0)
                   + (hero.FindItem(DispenserName)?.CurrentCharges ?? 0) + BackpackCount(ObserverName);
        }

        public uint CountSentries()
        {
            return (hero.FindItem(SentryName)?.CurrentCharges ?? 0)
                   + (hero.FindItem(DispenserName)?.SecondaryCharges ?? 0) + BackpackCount(SentryName);
        }

        public uint CountWards(ClassID id)
        {
            return id == ClassID.CDOTA_Item_ObserverWard ? CountObservers() : CountSentries();
        }

        public float Distance(EnemyHero enemy)
        {
            return hero.Distance2D(enemy.Position);
        }

        public bool DroppedWard(ClassID id)
        {
            return ObjectManager.GetEntities<PhysicalItem>().Any(x => x.Item.ClassID == id && x.Distance2D(hero) < 100);
        }

        public uint GetWardsCount(ClassID id)
        {
            return id == ClassID.CDOTA_Item_ObserverWard ? ObserversCount : SentryCount;
        }

        public void SetWardsCount(ClassID id, uint count)
        {
            if (id == ClassID.CDOTA_Item_ObserverWard)
            {
                ObserversCount = count;
            }
            else
            {
                SentryCount = count;
            }
        }

        public Vector3 WardPosition()
        {
            return hero.InFront(350);
        }

        #endregion

        #region Methods

        private uint BackpackCount(string name)
        {
            var count = 0u;
            for (var i = 6; i < 9; i++)
            {
                var currentSlot = (ItemSlot)i;

                var currentItem = hero.Inventory.GetItem(currentSlot);
                if (currentItem == null)
                {
                    continue;
                }

                var currentItemName = currentItem.StoredName();
                if (currentItemName != name && currentItemName != DispenserName)
                {
                    continue;
                }

                if (currentItemName == name)
                {
                    count += currentItem.CurrentCharges;
                }
                else
                {
                    if (name == ObserverName)
                    {
                        count += currentItem.CurrentCharges;
                    }
                    else
                    {
                        count += currentItem.SecondaryCharges;
                    }
                }
            }

            return count;
        }

        #endregion
    }
}