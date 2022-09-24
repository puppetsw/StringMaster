using System;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Windows;
using StringMaster.Services.Interfaces;

namespace StringMaster.Services.Implementation;

public class AcadLineweightDialogService : IAcadLineweightDialogService
{
    public string ShowDialog(string lineweight = "")
    {
        LineWeightDialog lwd = new LineWeightDialog();

        if (!string.IsNullOrEmpty(lineweight))
        {
            // convert string to lineweight
            var lw = LineweightStringtoLineweight(lineweight);
            lwd.LineWeight = lw;
        }

        var dr = lwd.ShowDialog();

        if (dr == DialogResult.OK)
            return LineweightToString(lwd.LineWeight);

        if (string.IsNullOrEmpty(lineweight))
            return LineweightToString(LineWeight.ByLineWeightDefault);

        return lineweight;
    }

    private string LineweightToString(LineWeight lineweight)
    {
        switch (lineweight)
        {
            case LineWeight.LineWeight000:
                return "0.00 mm";
            case LineWeight.LineWeight005:
                return "0.05 mm";
            case LineWeight.LineWeight009:
                return "0.09 mm";
            case LineWeight.LineWeight013:
                return "0.13 mm";
            case LineWeight.LineWeight015:
                return "0.15 mm";
            case LineWeight.LineWeight018:
                return "0.18 mm";
            case LineWeight.LineWeight020:
                return "0.20 mm";
            case LineWeight.LineWeight025:
                return "0.25 mm";
            case LineWeight.LineWeight030:
                return "0.30 mm";
            case LineWeight.LineWeight035:
                return "0.35 mm";
            case LineWeight.LineWeight040:
                return "0.40 mm";
            case LineWeight.LineWeight050:
                return "0.50 mm";
            case LineWeight.LineWeight053:
                return "0.53 mm";
            case LineWeight.LineWeight060:
                return "0.60 mm";
            case LineWeight.LineWeight070:
                return "0.70 mm";
            case LineWeight.LineWeight080:
                return "0.80 mm";
            case LineWeight.LineWeight090:
                return "0.90 mm";
            case LineWeight.LineWeight100:
                return "1.00 mm";
            case LineWeight.LineWeight106:
                return "1.06 mm";
            case LineWeight.LineWeight120:
                return "1.20 mm";
            case LineWeight.LineWeight140:
                return "1.40 mm";
            case LineWeight.LineWeight158:
                return "1.58 mm";
            case LineWeight.LineWeight200:
                return "2.00 mm";
            case LineWeight.LineWeight211:
                return "2.11 mm";
            case LineWeight.ByLayer:
                return "ByLayer";
            case LineWeight.ByBlock:
                return "ByBlock";
            case LineWeight.ByLineWeightDefault:
                return "Default";
            case LineWeight.ByDIPs:
                return LineWeight.ByDIPs.ToString();
            default:
                throw new ArgumentOutOfRangeException(nameof(lineweight), lineweight, null);
        }
    }

    private LineWeight LineweightStringtoLineweight(string lineweight)
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
