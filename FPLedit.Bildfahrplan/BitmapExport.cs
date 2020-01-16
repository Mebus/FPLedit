﻿using FPLedit.Bildfahrplan.Render;
using FPLedit.Shared;
using System;
using System.Collections.Generic;
using Eto.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLedit.Bildfahrplan
{
    public class BitmapExport : IExport
    {
        public string Filter => "Bildfahrplan als PNG (*.png)|*.png";

        public bool Export(Timetable tt, string filename, IPluginInterface pluginInterface, string[] flags = null)
        {
            try
            {
                Renderer renderer = new Renderer(tt, Timetable.LINEAR_ROUTE_ID);
                using (var bmp = new Bitmap(1000, renderer.GetHeight(true), PixelFormat.Format32bppRgba))
                using (var g = new Graphics(bmp))
                {
                    renderer.Draw(g, true);
                    bmp.Save(filename, ImageFormat.Png);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
