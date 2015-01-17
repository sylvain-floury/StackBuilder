﻿#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using TreeDim.StackBuilder.Basics;
using TreeDim.StackBuilder.Graphics;
using TreeDim.StackBuilder.Desktop.Properties;

using log4net;

using Sharp3D.Math.Core;
#endregion

namespace TreeDim.StackBuilder.Desktop
{
    public partial class FormNewPalletCorners : FormNewBase
    {
        #region Data members
        static readonly ILog _log = LogManager.GetLogger(typeof(FormNewPalletCorners));
        #endregion

        #region Constructor
        public FormNewPalletCorners(Document doc, PalletCornerProperties palletCorners)
            : base(doc, palletCorners)
        {
            InitializeComponent();

            if (null != palletCorners)
            {
                CornerLength = palletCorners.Length;
                CornerWidth = palletCorners.Width;
                CornerThickness = palletCorners.Thickness;
                CornerWeight = palletCorners.Weight;
                CornerColor = palletCorners.Color;
            }
            else
            { 
                CornerLength = UnitsManager.ConvertLengthFrom(1200.0, UnitsManager.UnitSystem.UNIT_METRIC1);;
                CornerWidth = UnitsManager.ConvertLengthFrom(40.0, UnitsManager.UnitSystem.UNIT_METRIC1);;
                CornerThickness = UnitsManager.ConvertLengthFrom(5.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                CornerWeight = UnitsManager.ConvertMassFrom(0.050, UnitsManager.UnitSystem.UNIT_METRIC1);
                CornerColor = Color.Khaki;
            }
            UpdateStatus(string.Empty);
            // units
            UnitsManager.AdaptUnitLabels(this);
        }
        #endregion

        #region FormNewBase overrides
        public override string ItemDefaultName
        {
            get { return Resources.ID_PALLETCORNERS; }
        }
        #endregion

        #region Public properties
        public double CornerLength
        {
            get { return (double)nudLength.Value; }
            set { nudLength.Value = (decimal)value; }
        }
        public double CornerWidth
        {
            get { return (double)nudWidth.Value; }
            set { nudWidth.Value = (decimal)value;}
        }
        public double CornerThickness
        {
            get { return (double)nudThickness.Value; }
            set { nudThickness.Value = (decimal)value; }
        }
        public double CornerWeight
        {
            get { return (double)nudWeight.Value; }
            set { nudWeight.Value = (decimal)value; }
        }
        public Color CornerColor
        {
            get { return cbColorCorners.Color; }
            set { cbColorCorners.Color = value; }
        }
        #endregion

        #region Handlers
        private void onValueChanged(object sender, EventArgs e)
        {
            UpdateStatus(string.Empty);
            DrawCorner();
        }
        #endregion

        public override void UpdateStatus(string message)
        {
            base.UpdateStatus(message);

            if (CornerThickness >= CornerWidth)
                message = Properties.Resources.ID_INVALIDTHICKNESSWIDTHPAIR;
        }

        #region Draw corner
        private void DrawCorner()
        {
            try
            {
                double angle = 45;
                // instantiate graphics
                Graphics3DImage graphics = new Graphics3DImage(pictureBox.Size);
                graphics.CameraPosition = new Vector3D(
                    Math.Cos(angle * Math.PI / 180.0) * Math.Sqrt(2.0) * 10000.0
                    , Math.Sin(angle * Math.PI / 180.0) * Math.Sqrt(2.0) * 10000.0
                    , 10000.0);
                graphics.Target = Vector3D.Zero;
                graphics.SetViewport(-500.0f, -500.0f, 500.0f, 500.0f);

                if (CornerLength > 0 && CornerWidth > 0 && CornerThickness > 0
                    && CornerThickness < CornerWidth)
                {
                    // draw
                    PalletCornerProperties palletCornerProperties = new PalletCornerProperties(
                        null, ItemName, ItemDescription, CornerLength, CornerWidth, CornerThickness,
                        CornerWeight, CornerColor);

                    Corner palletCap = new Corner(0, palletCornerProperties);
                    palletCap.Draw(graphics);
                    graphics.AddDimensions(new DimensionCube(CornerWidth, CornerWidth, CornerLength));
                }
                graphics.Flush();
                pictureBox.Image = graphics.Bitmap;
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }        
        }
        #endregion

    }
}
