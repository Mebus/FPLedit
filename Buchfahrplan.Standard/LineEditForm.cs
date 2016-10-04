﻿using Buchfahrplan.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Buchfahrplan.Standard
{
    public partial class LineEditForm : Form
    {
        private IInfo info;
        private Timetable tt;        

        public LineEditForm()
        {
            InitializeComponent();

            listView.Columns.Add("Bahnhof");
            listView.Columns.Add("Position");
        }

        public void Init(IInfo info)
        {
            this.info = info;
            tt = info.Timetable;
            info.BackupTimetable();

            UpdateStations();
        }

        private void UpdateStations()
        {            
            listView.Items.Clear();

            foreach (var station in tt.Stations.OrderBy(s => s.Kilometre))
            {
                listView.Items.Add(new ListViewItem(new[] { 
                    station.Name,                     
                    station.Kilometre.ToString() })
                    { Tag = station });
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            info.ClearBackup();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            info.RestoreTimetable();

            Close();
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Zuerst muss eine Station ausgewählt werden!", "Station bearbeiten");
                return;
            }

            if (listView.SelectedItems.Count > 0)
            {
                ListViewItem item = listView.Items[listView.SelectedIndices[0]];
                Station oldStation = tt.Stations[tt.Stations.IndexOf((Station)item.Tag)];

                NewStationForm nsf = new NewStationForm();
                nsf.Initialize(oldStation);
                if (nsf.ShowDialog() == DialogResult.OK)
                    UpdateStations();
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Zuerst muss eine Station ausgewählt werden!", "Station löschen");
                return;
            }

            if (listView.SelectedItems.Count > 0)
            {
                ListViewItem item = listView.Items[listView.SelectedIndices[0]];
                tt.Stations.Remove((Station)item.Tag);

                UpdateStations();
            }
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            NewStationForm nsf = new NewStationForm();
            if (nsf.ShowDialog() == DialogResult.OK)
            {
                Station sta = nsf.NewStation;

                tt.Stations.Add(sta);

                foreach (var t in tt.Trains)
                {
                    t.Arrivals.Add(sta, new TimeSpan());
                    t.Departures.Add(sta, new TimeSpan());
                }

                UpdateStations();
            }
        }

        private void editButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                if (listView.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Zuerst muss eine Station ausgewählt werden!", "Stations-Metadaten bearbeiten");
                    return;
                }

                if (listView.SelectedItems.Count > 0)
                {
                    ListViewItem item = listView.Items[listView.SelectedIndices[0]];
                    Station station = tt.Stations[tt.Stations.IndexOf((Station)item.Tag)];

                    MetaEdit mef = new MetaEdit();
                    mef.Initialize(station);
                    if (mef.ShowDialog() == DialogResult.OK)
                        UpdateStations();
                }
            }
        }
    }
}
