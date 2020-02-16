﻿using Eto.Forms;
using FPLedit.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FPLedit.Shared.Filetypes;

namespace FPLedit.NonDefaultFiletypes
{
    internal sealed class CleanedXmlExport : IExport
    {
        public string Filter => "Bereinigte Fahrplan Dateien (*.fpl)|*.fpl";

        private readonly Dictionary<string, List<string>> nodeNames;
        private readonly Dictionary<string, List<string>> attrsNames;
        private bool namesCreated;

        public CleanedXmlExport()
        {
            nodeNames = new Dictionary<string, List<string>>
            {
                [""] = new List<string>() // Initialize store for elememnts without parents
            };
            attrsNames = new Dictionary<string, List<string>>();
        }

        private void LoadRemovableXmlNames()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.Contains("FPLedit"));
            foreach (var assembly in types)
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (!type.IsClass || !type.IsPublic || type.IsAbstract)
                            continue;

                        var elm = type.GetCustomAttribute<XElmNameAttribute>(false);
                        if (elm == null)
                            continue; // This class represents no model element
                        
                        if (elm.IsFpleditElement)
                        {
                            if (elm.ParentElements != null && elm.ParentElements.Any())
                            {
                                foreach (var parent in elm.ParentElements)
                                {
                                    if (!nodeNames.ContainsKey(parent))
                                        nodeNames[parent] = new List<string>();
                                    nodeNames[parent].AddRange(elm.Names);
                                }
                            }
                            else
                                nodeNames[""].AddRange(elm.Names);
                        }

                        
                        var attrsToRemove = type
                            .GetProperties()
                            .Select(p => p.GetCustomAttribute<XAttrNameAttribute>())
                            .Where(p => p != null && p.IsFpleditElement)
                            .Select(p => p.Name).ToArray();

                        if (attrsToRemove.Length == 0)
                            continue;

                        foreach (var name in elm.Names)
                        {
                            if (!attrsNames.ContainsKey(name))
                                attrsNames[name] = new List<string>();
                            attrsNames[name].AddRange(attrsToRemove);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        private void ProcessEntity(XMLEntity node)
        {
            if (attrsNames.TryGetValue(node.XName, out var localAttrsNames))
                foreach (var attr in localAttrsNames)
                    node.RemoveAttribute(attr);

            IEnumerable<string> removeNodeNames = nodeNames[""];
            if (nodeNames.TryGetValue(node.XName, out var localNodeNames))
                removeNodeNames = removeNodeNames.Concat(localNodeNames);
            node.Children.RemoveAll(x => removeNodeNames.Contains(x.XName));

            foreach (var ch in node.Children)
                ProcessEntity(ch);
        }

        public bool Export(Timetable tt, Stream stream, IReducedPluginInterface pluginInterface, string[] flags = null)
        {
            if (!namesCreated)
                LoadRemovableXmlNames();
            namesCreated = true;
            
            if (tt.Type == TimetableType.Network)
            {
                MessageBox.Show("Der aktuelle Fahrplan ist ein Netzwerk-Fahrplan. Aus diesem erweiterten Fahrplanformat können aus technischen Gründen keine von FPLedit angelegten Daten gelöscht werden.");
                return false;
            }

            var res = MessageBox.Show("Hiermit werden alle in FPLedit zusätzlich eingebenen Werte (z.B. Lokomotiven, Lasten, Mindestbremshundertstel, Geschwindigkeiten, Wellenlinien, Trapeztafelhalte und Zuglaufmeldungen) und Buchfahrplaneinstellungen aus dem gespeicherten Fahrplan gelöscht! Fortfahren?",
                "FPLedit", MessageBoxButtons.YesNo, MessageBoxType.Warning);

            if (res == DialogResult.No)
                return false;

            var clone = tt.Clone(); // Klon zum anschließenden Verwerfen!
            ProcessEntity(clone.XMLEntity);

            return new XMLExport().Export(clone, stream, pluginInterface, flags);
        }
    }
}
