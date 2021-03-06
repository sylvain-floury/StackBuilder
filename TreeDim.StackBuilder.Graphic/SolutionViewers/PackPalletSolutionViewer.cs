﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using treeDiM.StackBuilder.Basics;
using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public class PackPalletSolutionViewer
    {
        #region Data members
        private PackPalletSolution _solution;
        private PackPalletAnalysis _analysis;
        private bool _showDimensions = true;
        #endregion

        #region Constructor
        public PackPalletSolutionViewer(PackPalletSolution solution)
        {
            _analysis = null != solution ? solution.Analysis : null;
            _solution = solution;
        }
        #endregion

        #region Public methods
        public void Draw(Graphics2D graphics)
        {
            if (null == _solution || _solution.LayerCount == 0)
                return;

            BoxLayer blayer = _solution.Layer;
            if (blayer != null)
            {
                // initialize Graphics2D object
                graphics.NumberOfViews = 1;
                BBox3D bbox = blayer.BoundingBox(_analysis.PackProperties);
                graphics.SetViewport(0.0f, 0.0f, (float)bbox.Length, (float)bbox.Width);
                graphics.SetCurrentView(0);
                graphics.DrawRectangle(Vector2D.Zero, new Vector2D(_analysis.PalletProperties.Length, _analysis.PalletProperties.Width), Color.Black);
                uint pickId = 0;
                foreach (BoxPosition bPosition in blayer)
                    graphics.DrawBox(new Box(pickId++, _analysis.PackProperties, bPosition));
            }
        }
        public void Draw(Graphics3D graphics)
        {
            // sanity check
            if (null == _solution) return;
            InterlayerProperties interlayerProperties = _analysis.InterlayerProperties;
            // draw pallet
            Pallet pallet = new Pallet(_analysis.PalletProperties);
            pallet.Draw(graphics, Transform3D.Identity);
            // draw solution
            uint pickid = 0;
            for (int iLayerIndex = 0; iLayerIndex < _solution.LayerCount; ++iLayerIndex)
            {
                bool hasInterlayer = false;
                double zInterlayer = 0.0;
                BoxLayer blayer = _solution.GetBoxLayer(iLayerIndex, ref hasInterlayer, ref zInterlayer);

                if (hasInterlayer && (null != interlayerProperties))
                {
                    // instantiate box
                    Box box = new Box(pickid++, interlayerProperties);
                    // set position
                    box.Position = new Vector3D(
                        0.5 * (_analysis.PalletProperties.Length - interlayerProperties.Length)
                        , 0.5 * (_analysis.PalletProperties.Width - interlayerProperties.Width)
                        , zInterlayer);
                    // draw
                    graphics.AddBox(box);
                }
                foreach (BoxPosition bPosition in blayer)
                    graphics.AddBox(new Pack(pickid++, _analysis.PackProperties, bPosition));

                if (_showDimensions)
                {
                    graphics.AddDimensions(
                        new DimensionCube(BoundingBoxDim(Properties.Settings.Default.DimCasePalletSol1)
                        , Color.Black, false));
                    graphics.AddDimensions(
                        new DimensionCube(BoundingBoxDim(Properties.Settings.Default.DimCasePalletSol2)
                        , Color.Red, true));
                }
            }
        }

        public void DrawLayer(Graphics3D graphics, int layerIndex)
        {
            // sanity check
            if (null == _solution) return;

            if (_showDimensions)
            {
                BoxLayer layer = layerIndex %2 == 0 ? _solution.Layer : _solution.LayerSwapped;
                uint pickId = 0;
                foreach (BoxPosition bPosition in layer)
                    graphics.AddBox(new Pack(pickId++, _solution.Analysis.PackProperties, bPosition));
                graphics.AddDimensions(
                    new DimensionCube(layer.BoundingBox(_solution.Analysis.PackProperties)
                        , Color.Black, false));
            }
        }

        private BBox3D BoundingBoxDim(int index)
        {
            switch (index)
            {
                case 0: return _solution.BoundingBox;//_solution.BoundingBox;
                case 1: return _solution.LoadBoundingBox;//_solution.LoadBoundingBox;
                case 2: return _analysis.PalletProperties.BoundingBox;
                case 3: return new BBox3D(0.0, 0.0, 0.0, _analysis.PalletProperties.Length, _analysis.PalletProperties.Width, 0.0);
                default: return _solution.BoundingBox;
            }
        }
        #endregion

        #region Public properties
        public bool ShowDimensions
        {
            get { return _showDimensions; }
            set { _showDimensions = value; }
        }
        #endregion

    }
}
