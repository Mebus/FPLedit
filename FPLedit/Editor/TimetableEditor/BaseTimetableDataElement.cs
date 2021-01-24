﻿using Eto.Forms;
using FPLedit.Shared;
using System.Collections.Generic;
using System.Linq;

namespace FPLedit.Editor.TimetableEditor
{
    internal abstract class BaseTimetableDataElement
    {
        public ITrain Train { get; set; }

        public Dictionary<Station, ArrDep> ArrDeps { get; set; }

        //private Dictionary<Station, TagEntry> TmpTags { get; } = new();

        public bool IsSelectedArrival { get; set; }

        public TextBox SelectedTextBox { get; set; }
        
        public DropDown SelectedDropDown { get; set; }

        public void SetTime(Station sta, bool arrival, string time)
        {
            var a = ArrDeps[sta];
            if (arrival)
                a.Arrival = TimeEntry.Parse(time);
            else
                a.Departure = TimeEntry.Parse(time);
            ArrDeps[sta] = a;
        }
        
        /*public void SetTmpTag(Station sta, bool arrival, string time)
        {
            var a = TmpTags.ContainsKey(sta) ? TmpTags[sta] : new TagEntry { Station = sta };
            if (arrival)
                a.ArrivalText = time;
            else
                a.DepartureText = time;
            TmpTags[sta] = a;
        }
        
        public string GetTmpTag(Station sta, bool arrival)
        {
            if (!TmpTags.ContainsKey(sta))
                return null;
            var a = TmpTags[sta];
            return arrival ? a.ArrivalText : a.DepartureText;
        }*/

        public void SetZlm(Station sta, string zlm)
        {
            var a = ArrDeps[sta];
            a.Zuglaufmeldung = zlm;
            ArrDeps[sta] = a;
        }

        public void SetTrapez(Station sta, bool trapez)
        {
            var a = ArrDeps[sta];
            a.TrapeztafelHalt = trapez;
            ArrDeps[sta] = a;
        }

        #region Errors
        private readonly List<ErrorEntry> errors = new List<ErrorEntry>();

        public bool HasError(Station sta, bool arrival)
        {
            var err = errors.FirstOrDefault(e => e.Station == sta && e.Arrival == arrival);
            return err != null && !string.IsNullOrEmpty(err.Text);
        }

        public bool HasAnyError => errors.Any(e => !string.IsNullOrEmpty(e.Text));

        public void SetError(Station sta, bool arrival, string text)
        {
            var err = errors.FirstOrDefault(e => e.Station == sta && e.Arrival == arrival);
            if (string.IsNullOrEmpty(text))
            {
                if (err != null)
                    errors.Remove(err);
                return;
            }
            if (err == null)
            {
                err = new ErrorEntry(sta, arrival, null);
                errors.Add(err);
            }
            err.Arrival = arrival;
            err.Text = text;
        }

        internal string GetErrorText(Station sta, bool arrival)
            => errors.FirstOrDefault(e => e.Station == sta && e.Arrival == arrival)?.Text;
        #endregion

        public bool IsLast(Station sta) => Train.GetPath().Last() == sta;

        public bool IsFirst(Station sta) => Train.GetPath().First() == sta;

        public abstract Station GetStation();

        private class ErrorEntry
        {
            public readonly Station Station;
            public bool Arrival;
            public string Text;

            public ErrorEntry(Station station, bool arrival, string text)
            {
                Station = station;
                Arrival = arrival;
                Text = text;
            }
        }

        /*private class TagEntry
        {
            public Station Station { get; init; }
            public string ArrivalText = null;
            public string DepartureText = null;
        }*/
    }
    
    internal class CCCO // CustomCellControlObject
    {
        public bool InhibitEvents { get; set; } = true;
        public BaseTimetableDataElement Data { get; set; }
    }
}
