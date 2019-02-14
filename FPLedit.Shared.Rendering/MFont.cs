﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPLedit.Shared.Rendering
{
    /// <summary>
    /// MetaFont to internally represent fonts.
    /// </summary>
    public class MFont
    {
        public MFont(string family, int size)
        {
            Family = family;
            Size = size;
        }

        public MFont(string family, int size, MFontStyle style) : this(family, size)
        {
            Style = style;
        }

        public string Family { get; set; }

        public int Size { get; set; }

        public MFontStyle Style { get; set; }

        public static explicit operator Eto.Drawing.Font(MFont m)
        {
            var family = FontCollection.Families.Contains(m.Family) ? m.Family : FontCollection.GenericSans;
            return new Eto.Drawing.Font(family, m.Size, (Eto.Drawing.FontStyle)m.Style);
        }

        #region Conversion
        public static MFont Parse(string def)
        {
            if (def == null || !def.StartsWith("font(") || !def.EndsWith(")"))
                return new MFont(Eto.Drawing.FontFamilies.SansFamilyName, 9); // Keine valide Font-Definition

            var parts = def.Substring(5, def.Length - 6).Split(';');
            var family = GetFontFamily(parts[0]);
            var style = (MFontStyle)int.Parse(parts[1]);
            var size = int.Parse(parts[2]);

            return new MFont(family, size, style);
        }

        private static string GetFontFamily(string name)
        {
            switch (name.ToLower())
            {
                case "sansserif":
                case "dialog":
                case "dialoginput":
                    return Eto.Drawing.FontFamilies.SansFamilyName;
                case "monospaced":
                    return Eto.Drawing.FontFamilies.MonospaceFamilyName;
                case "serif":
                    return Eto.Drawing.FontFamilies.SerifFamilyName;
            }
            return name;
        }

        public string FontToString()
        {
            var family = GetFontName(Family);
            var style = (int)Style;
            return "font(" + family + ";" + style + ";" + Size + ")";
        }

        private string GetFontName(string family)
        {
            if (family == Eto.Drawing.FontFamilies.SansFamilyName)
                return "SansSerif";
            if (family == Eto.Drawing.FontFamilies.SerifFamilyName)
                return "Serif";
            if (family == Eto.Drawing.FontFamilies.MonospaceFamilyName)
                return "Monospaced";
            return family;
        }
        #endregion
    }

    [Flags]
    public enum MFontStyle
    {
        Regular = 0x0,
        Bold = 0x1,
        Italic = 0x2,
        Underline = 0x4,
        Strikeout = 0x8
    }
}