using System;

namespace StringMaster.Models;

public sealed class RadiusPoint
{
    public double Radius { get; set; }

    public Point Point { get; set; }

    public double MidOrd { get; set; }

    public bool IsArc() => Math.Abs(Radius) > 0.0;
}