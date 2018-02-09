﻿using FPLedit.Buchfahrplan.Model;
using FPLedit.Shared;
using FPLedit.Shared.Helpers;
using FPLedit.Shared.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPLedit.Buchfahrplan.Templates
{
    public class TemplateHelper
    {
        private BfplAttrs attrs;
        private Timetable tt;
        private IFilterableUi filterable;

        public TemplateHelper(Timetable tt)
        {
            filterable = new Forms.FilterableHandler();
            this.tt = tt;
            attrs = BfplAttrs.GetAttrs(tt);
        }

        public string HtmlName(string name, string prefix)
        {
            return prefix + name.Replace("#", "")
                .Replace(" ", "-")
                .Replace(".", "-")
                .Replace(":", "-")
                .ToLower();
        }

        public List<IStation> GetStations(Train train)
        {
            List<IStation> points = new List<IStation>();
            var fstations = train.GetPath().Where(s => filterable.LoadStationRules(tt).All(r => !r.Matches(s))); // Filter
            points.AddRange(fstations);

            var p = attrs?.Points ?? new List<BfplPoint>();
            for (int i = 0; i < points.Count; i++)
            {
                var sta0 = points[i];
                if (sta0 == points.Last())
                    break; // Hier ist die Strecke zuende
                var sta1 = points[i + 1];

                int route = Timetable.LINEAR_ROUTE_ID;
                if (tt.Type == TimetableType.Network)
                {
                    var routes = sta0.Routes.Where(r => sta1.Routes.Contains(r)).ToArray();
                    if (routes.Length > 1 || routes.Length == 0)
                        throw new Exception("Zwei Stationen können nicht mehr als eine/keine Route gemeinsam haben!");
                    route = routes[0];
                }

                var maxPos = Math.Max(sta0.Positions.GetPosition(route).Value, sta1.Positions.GetPosition(route).Value);
                var minPos = Math.Min(sta0.Positions.GetPosition(route).Value, sta1.Positions.GetPosition(route).Value);

                var p1 = tt.Type == TimetableType.Network ? p.Where(po => po.Routes.Contains(route)) : p;
                var pointsBetween = p1.Where(po => po.Positions.GetPosition(route) > minPos && po.Positions.GetPosition(route) < maxPos);
                points.InsertRange(points.IndexOf(sta0) + 1, pointsBetween);
                i += pointsBetween.Count();
            }
            return points;
        }

        public Train[] GetTrains()
        {
            return tt.Trains.Where(t => filterable.LoadTrainRules(tt).All(r => !r.Matches(t)))
                .ToArray();
        }

        public string OptAttr(string caption, string value)
        {
            if (value != null && value != "")
                return caption + " " + value;
            return "";
        }

        public string Kreuzt(Train ot, Station s)
        {
            var t = IntersectTrains(ot, s, true);
            if (t == null)
                return "";
            return t.TName + " " + IntersectDaysSt(ot, t);
        }

        public string Ueberholt(Train ot, Station s)
        {
            var t = IntersectTrains(ot, s, false);
            if (t == null)
                return "";
            return t.TName + " " + IntersectDaysSt(ot, t);
        }

        public string TrapezHalt(Train ot, Station s)
        {
            var it = IntersectTrains(ot, s, true);
            if (it == null)
                return "";

            var oth = ot.GetArrDep(s).TrapeztafelHalt;
            var ith = it.GetArrDep(s).TrapeztafelHalt;

            if (oth && !ith)
                return "<span class=\"trapez-tt\">" + ot.TName + "</span> " + IntersectDaysSt(ot, it);
            if (ith && !oth)
                return it.TName + " " + IntersectDaysSt(ot, it);
            if (ith && oth)
                return "<span class=\"trapez-tt\">" + ot.TName + "</span> " + IntersectDaysSt(ot, it);
            return "";
        }

        private string IntersectDaysSt(Train ot, Train t)
            => DaysHelper.DaysToString(DaysHelper.IntersectingDays(ot.Days, t.Days), true);

        private Train IntersectTrains(Train ot, Station s, bool kreuzung)
        {
            TimeSpan start = ot.GetArrDep(s).Arrival;
            TimeSpan end = ot.GetArrDep(s).Departure;

            if (start == TimeSpan.Zero || end == TimeSpan.Zero)
                return null;

            Func<Train, bool> pred = (t => t.Direction == ot.Direction); // Überholung
            if (kreuzung)
                pred = (t => t.Direction != ot.Direction); // Kreuzung

            foreach (var train in tt.Trains.Where(pred))
            {
                if (train == ot)
                    continue;

                TimeSpan start2 = train.GetArrDep(s).Arrival;
                TimeSpan end2 = train.GetArrDep(s).Departure;

                if (start2 == TimeSpan.Zero || end2 == TimeSpan.Zero)
                    continue;

                var st = start < start2 ? start2 : start;
                var en = end < end2 ? end : end2;
                var crossing = st < en ? true : false;

                if (crossing && DaysHelper.IntersectDays(ot.Days, train.Days))
                    return train;
            }

            return null;
        }
    }
}
