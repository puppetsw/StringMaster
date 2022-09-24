using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace StringMaster.Helpers;

public static class LineweightHelpers
{
    public static LineWeight LineweightStringtoLineweight(string lineweight)
    {
        switch (lineweight)
        {
            case "0.00 mm":
                return LineWeight.LineWeight000;
            case "0.05 mm":
                return LineWeight.LineWeight005;
            case "0.09 mm":
                return LineWeight.LineWeight009;
            case "0.13 mm":
                return LineWeight.LineWeight013;
            case "0.15 mm":
                return LineWeight.LineWeight015;
            case "0.18 mm":
                return LineWeight.LineWeight018;
            case "0.20 mm":
                return LineWeight.LineWeight020;
            case "0.25 mm":
                return LineWeight.LineWeight025;
            case "0.30 mm":
                return LineWeight.LineWeight030;
            case "0.35 mm":
                return LineWeight.LineWeight035;
            case "0.40 mm":
                return LineWeight.LineWeight040;
            case "0.50 mm":
                return LineWeight.LineWeight050;
            case "0.53 mm":
                return LineWeight.LineWeight053;
            case "0.60 mm":
                return LineWeight.LineWeight060;
            case "0.70 mm":
                return LineWeight.LineWeight070;
            case "0.80 mm":
                return LineWeight.LineWeight080;
            case "0.90 mm":
                return LineWeight.LineWeight090;
            case "1.00 mm":
                return LineWeight.LineWeight100;
            case "1.06 mm":
                return LineWeight.LineWeight106;
            case "1.20 mm":
                return LineWeight.LineWeight120;
            case "1.40 mm":
                return LineWeight.LineWeight140;
            case "1.58 mm":
                return LineWeight.LineWeight158;
            case "2.00 mm":
                return LineWeight.LineWeight200;
            case "2.11 mm":
                return LineWeight.LineWeight211;
            case "ByLayer":
                return LineWeight.ByLayer;
            case "ByBlock":
                return LineWeight.ByBlock;
            case "Default":
                return LineWeight.ByLineWeightDefault;
            case "ByDIPs":
                return LineWeight.ByDIPs;
            default:
                throw new ArgumentOutOfRangeException(nameof(lineweight), lineweight, null);
        }
    }
}