﻿using System;
using System.Windows;
using System.Threading;
using OpenDDSharp.org.omg.dds.demo;

namespace OpenDDSharp.ShapesDemo.Model
{
    public class ShapeDynamic : IDisposable
    {
        #region Constants
        private const double OPENDDS_FACTOR_X = 1d;
        private const double OPENDDS_FACTOR_Y = 1d;
        private const double RTI_FACTOR_X = 205d / 291d;
        private const double RTI_FACTOR_Y = 235d / 331d;
        #endregion

        #region Fields
        private bool _disposed;
        private ShapeTypeDataWriter _dataWriter;
        private Shape _shape;
        private Rect _constraint;
        private Timer _timer;        
        private double _angle;
        private double _alpha;
        private double _speed;
        private Random _random;
        private Rect _shapeBounds;
        private InteroperatibilityProvider _provider;
        private readonly double _factorX;
        private readonly double _factorY;
        #endregion

        #region Constructors
        public ShapeDynamic(ShapeTypeDataWriter dataWriter, Shape shape, Rect constraint, int speed, InteroperatibilityProvider provider)
        {
            _dataWriter = dataWriter;
            _shape = shape;
            _constraint = constraint;          
            _speed = speed;
            _provider = provider;

            _angle = Math.PI / 6;
            _alpha = Math.PI / 6;            
            _shapeBounds = new Rect(0, 0, _shape.Size, _shape.Size);
            _timer = new Timer(Simulate, null, 0, 25);            
            _random = new Random();

            switch(_provider)
            {
                case InteroperatibilityProvider.Rti:
                    _factorX = RTI_FACTOR_X;
                    _factorY = RTI_FACTOR_Y;
                    break;
                default:
                    _factorX = OPENDDS_FACTOR_X;
                    _factorY = OPENDDS_FACTOR_Y;
                    break;
            }
        }
        #endregion

        #region Methods
        private void Simulate(object state)
        {
            if (!_disposed)
            {
                int x = (int)Math.Round(_shape.X + _speed * Math.Cos(_angle));
                int y = (int)Math.Round(_shape.Y + _speed * Math.Sin(_angle));

                if (x <= 0)
                {
                    _angle = Flip() ? -_alpha : _alpha;
                    x = 0;
                }
                else if (x >= _constraint.Width - _shape.Size)
                {
                    _angle = Flip() ? (Math.PI + _alpha) : (Math.PI - _alpha);
                    x = (int)(_constraint.Width - _shape.Size);
                }
                else if (y <= 0)
                {
                    _angle = Flip() ? _alpha : Math.PI - _alpha;
                    y = 0;
                }
                else if (y >= _constraint.Height - _shape.Size)
                {
                    _angle = Flip() ? (Math.PI + _alpha) : -_alpha;
                    y = (int)(_constraint.Height - _shape.Size);
                }

                _dataWriter.Write(new ShapeType
                {
                    color = _shape.Color,
                    x = Convert.ToInt32((x * _factorX) + (_shape.Size / 2d)),
                    y = Convert.ToInt32((y * _factorY) + (_shape.Size / 2d)),
                    shapesize = _shape.Size
                });

                _shape.X = x;
                _shape.Y = y;
            }
        }

        private bool Flip()
        {
            bool doflip = false;
            if (_random.NextDouble() <= 1.0 / 2)
                doflip = true;

            return doflip;
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            if (!_disposed)
            {
                _timer.Dispose();
                _timer = null;

                _dataWriter = null;

                _disposed = true;
            }
        }
        #endregion
    }
}
