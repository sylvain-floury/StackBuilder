﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Text;

using treeDiM.StackBuilder.Basics;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Engine
{
    class HCylinderLoadPatternStaggered : HCylinderLoadPattern
    {
        #region Implementation of HCylinderLoadPattern abstract properties
        public override string Name
        {
            get { return "Staggered"; }
        }
        public override bool CanBeSwapped
        {
            get { return true; }
        }
        public override void Generate(CylLoad load, int maxCount, double actualLength, double actualWidth, double maxHeight)
        {
            load.Clear();

            double palletLength = GetPalletLength(load);
            double palletWidth = GetPalletWidth(load);
            double radius = load.CylinderRadius;
            double diameter = 2.0 * load.CylinderRadius;
            double length = load.CylinderLength;

            int sizeX = Convert.ToInt32( Math.Floor(GetPalletLength(load) / load.CylinderLength) );
            int sizeY = Convert.ToInt32(Math.Floor(GetPalletWidth(load) / (2.0 * load.CylinderRadius)));

            double offsetX = 0.5 * (palletLength - actualLength);
            double offsetY = 0.5 * (palletWidth - actualWidth);
            double offsetZ = load.PalletHeight + radius;

            int iLayer = 0;
            while (true)
            {
                if ((iLayer + 1) * diameter >= maxHeight)
                {
                    load.LimitReached = Limit.LIMIT_MAXHEIGHTREACHED;
                    return;
                }

                for (int j = 0; j < sizeY - (iLayer % 2); ++j)
                {
                    for (int i = 0; i < sizeX; ++i)
                    {
                        AddPosition(
                            load
                            , new CylPosition(
                                new Vector3D(
                                    offsetX + i * length
                                    , offsetY  + radius + (iLayer % 2) * radius + j * diameter
                                    , offsetZ + iLayer * radius * Math.Sqrt(3.0))
                                , HalfAxis.HAxis.AXIS_X_P)
                            );
                        if (maxCount > 0 && load.Count >= maxCount)
                        {
                            load.LimitReached = Limit.LIMIT_MAXNUMBERREACHED;
                            return;
                        }
                    }
                }
                ++iLayer;
            };
        }
        #endregion
    }
}
