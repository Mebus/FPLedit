﻿using FPLedit.Shared;
using FPLedit.Shared.Filetypes;
using FPLedit.Shared.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FPLedit.NonDefaultFiletypes
{
    internal class NetworkExport : BaseConverterFileType, IExport
    {
        public string Filter => "Fahrplan Dateien (*.fpl)|*.fpl";

        public bool Export(Timetable tt, string filename, IInfo info)
        {
            if (tt.Type == TimetableType.Network)
                throw new Exception("Der Fahrplan ist bereits ein Netzwerk-Fahrplan");

            var clone = tt.Clone();
            var old_version = clone.GetAttribute("version", "");

            var trainPaths = new Dictionary<Train, TrainPathData>();
            foreach (var orig in clone.Trains)
                trainPaths[orig] = new TrainPathData(clone, orig);

            var rt = Timetable.LINEAR_ROUTE_ID.ToString();
            var id = 0;
            var y = 0;
            foreach (var sta in clone.Stations)
            {
                ConvertStationLinToNet(sta);

                sta.SetAttribute("fpl-rt", rt);
                sta.SetAttribute("fpl-pos", (y += 40).ToString() + ";0");
                sta.SetAttribute("fpl-id", id++.ToString());
            }

            var actions = info.GetRegistered<ITimetableTypeChangeAction>();
            foreach (var action in actions)
                action.ToNetwork(clone);

            clone.SetAttribute("version", TimetableVersion.Extended_FPL.ToNumberString());

            foreach (var train in clone.Trains)
            {
                var data = trainPaths[train];

                train.Children.Clear();
                train.AddAllArrDeps(data.GetRawPath());
                train.XMLEntity.XName = "tr";

                foreach (var sta in data.PathEntries)
                {
                    if (sta.ArrDep != null)
                        train.GetArrDep(sta.Station).ApplyCopy(sta.ArrDep);
                }
            }

            ColorTimetableConverter.ConvertAll(clone);

            return new XMLExport().Export(clone, filename, info);
        }
    }
}
