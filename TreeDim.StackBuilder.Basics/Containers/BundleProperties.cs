﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    public class BundleProperties : BProperties
    {
        #region Data members
        private double _unitThickness = 0.0, _unitWeight = 0.0;
        private int _noFlats;
        private Color _color;
        #endregion

        #region Constructor
        public BundleProperties(Document document, string name, string description,
            double length, double width
            , double unitThickness
            , double unitWeight
            , int noFlats
            , Color color)
            : base(document, name, description)
        {
            _length = length;
            _width = width;
            _unitThickness = unitThickness;
            _unitWeight = unitWeight;
            _noFlats = noFlats;
            _color = color;
        }
        #endregion

        #region Override packable
        public override double Height
        { get { return _unitThickness * _noFlats; } }
        public override double Weight
        { get { return _unitWeight * _noFlats; } }
        public override OptDouble NetWeight
        { get { return new OptDouble(false, 0.0); } }
        public override bool OrientationAllowed(HalfAxis.HAxis axis)
        {   return (axis == HalfAxis.HAxis.AXIS_Z_N) || (axis == HalfAxis.HAxis.AXIS_Z_P); }
        public override bool IsCase { get { return false; } }
        #endregion

        #region Public properties
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
        public override Color[] Colors
        {
            get
            {
                Color[] colors = new Color[6];
                for (int i = 0; i < 6; ++i)
                    colors[i] = _color;
                return colors;
            }
        }
        public override Color GetColor(HalfAxis.HAxis axis)
        {
            return _color;
        }
        public override void SetColor(Color color)
        {
            _color = color;
        }

        public double UnitThickness
        {
            get { return _unitThickness; }
            set { _unitThickness = value; Modify(); }
        }
        public double UnitWeight
        {
            get { return _unitWeight; }
            set { _unitWeight = value; Modify(); }
        }
        public int NoFlats
        {
            get { return _noFlats; }
            set { _noFlats = value; Modify(); }
        }
        public override bool IsBundle { get { return true; } }
        #endregion
    }
}
