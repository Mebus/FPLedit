﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPLedit.Shared
{
    public struct FileState
    {
        public bool Opened;

        public bool Saved;

        public bool LineCreated;

        public bool TrainsCreated;

        public bool CanGoBack;

        public string FileName;

        public override bool Equals(object obj)
        {
            if (!(obj is FileState))
                return false;

            FileState s = (FileState)obj;

            return (s.Opened == Opened) && (s.Saved == Saved)
                && (s.LineCreated == LineCreated) && (s.TrainsCreated == TrainsCreated)
                && (s.CanGoBack == CanGoBack);
        }
    }
}
