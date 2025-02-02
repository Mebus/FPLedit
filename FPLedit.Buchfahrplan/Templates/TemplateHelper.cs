﻿using FPLedit.Buchfahrplan.Model;
using FPLedit.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FPLedit.Shared.Analyzers;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace FPLedit.Buchfahrplan.Templates
{
    public sealed class TemplateHelper
    {
        private readonly BfplAttrs attrs;
        private readonly Timetable tt;
        private readonly IFilterRuleContainer filterable;
        private readonly IntersectionAnalyzer analyzer;

        public TemplateHelper(Timetable tt)
        {
            filterable = Plugin.FilterRuleContainer;
            this.tt = tt;
            attrs = BfplAttrs.GetAttrs(tt);
            analyzer = new IntersectionAnalyzer(tt);
        }

        private static string SafeHtml(string s) => Shared.Templating.TemplateOutput.SafeHtml(s);

        public string GetDaysHtml(ITrain tra, bool showDays)
        {
            var days = tra.Days.DaysToString(true);
            if (!showDays || days == "")
                return "";

            days = Regex.Replace(days, @"\[(\w*)\]", (m) => " <span class=\"days\">" + SafeHtml(m.Groups[1].Value) + "</span>");

            return "&nbsp;&nbsp;" + days;
        }

        public IStation[] GetStations(ITrain train)
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
                        throw new Exception(T._("Zwei benachbarte Stationen sollten nicht mehr als eine/keine Route gemeinsam haben! Zusammengefallene Routen sind vorhanden und werden nicht unterstützt."));
                    route = routes[0];
                }

                var maxPos = Math.Max(sta0.Positions.GetPosition(route).Value, sta1.Positions.GetPosition(route).Value);
                var minPos = Math.Min(sta0.Positions.GetPosition(route).Value, sta1.Positions.GetPosition(route).Value);

                var p1 = tt.Type == TimetableType.Network ? p.Where(po => po.Routes.Contains(route)) : p;
                var pointsBetween = p1.Where(po => po.Positions.GetPosition(route) > minPos && po.Positions.GetPosition(route) < maxPos).ToArray();
                points.InsertRange(points.IndexOf(sta0) + 1, pointsBetween);
                i += pointsBetween.Length;
            }
            return points.ToArray();
        }

        public ITrain[] GetTrains()
        {
            return tt.Trains.Where(t => filterable.LoadTrainRules(tt).All(r => !r.Matches(t))).ToArray();
        }

        public string OptAttr(string caption, string value)
        {
            if (!string.IsNullOrEmpty(value))
                return SafeHtml(caption + " " + value);
            return "";
        }

        public string Kreuzt(ITrain ot, Station s)
        {
            return SafeHtml(string.Join(", ", analyzer.CrossingAtStation(ot, s)
                .Select(tr => tr.TName + " " + IntersectDaysSt(ot, tr))));
        }

        public string Ueberholt(ITrain ot, Station s)
        {
            return SafeHtml(string.Join(", ", analyzer.OvertakeAtStation(ot, s)
                .Select(tr => tr.TName + " " + IntersectDaysSt(ot, tr))));
        }

        public string TrapezHalt(ITrain probeTrain, Station s)
        {
            var trapez = analyzer.TrapezAtStation(probeTrain, s);

            if (trapez.IsStopping)
                return "<span class=\"trapez-tt\">" + SafeHtml(probeTrain.TName) + "</span> " + SafeHtml(DaysToStringNotEqual(probeTrain, trapez.StopDays));
            if (trapez.IntersectingTrainsStopping.Any())
                return SafeHtml(string.Join(", ", trapez.IntersectingTrainsStopping.Select(t => t.TName)) + " " + DaysToStringNotEqual(probeTrain, trapez.StopDays));

            return "";
        }
        
        private string IntersectDaysSt(ITrain ot, ITrain t) 
            => SafeHtml(DaysToStringNotEqual(ot, ot.Days.IntersectingDays(t.Days)));

        private static string DaysToStringNotEqual(ITrain ot, Days days)
            => SafeHtml(days == ot.Days ? "" : days.DaysToString(true));
    }
}