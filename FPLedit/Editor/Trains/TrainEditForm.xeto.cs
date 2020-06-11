﻿using Eto.Forms;
using FPLedit.Editor.TimetableEditor;
using FPLedit.Shared;
using FPLedit.Shared.UI;
using FPLedit.Shared.UI.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;

namespace FPLedit.Editor.Trains
{
    internal sealed class TrainEditForm : FDialog<DialogResult>
    {
#pragma warning disable CS0649
        private readonly TextBox nameTextBox, commentTextBox;
        private readonly ComboBox locomotiveComboBox, mbrComboBox, lastComboBox;
        private readonly Button fillButton, resetTransitionButton;
        private readonly SingleTimetableEditControl editor;
        private readonly DropDown transitionDropDown;
        private readonly DaysControlNarrow daysControl;
        private readonly GridView linkGridView;
#pragma warning restore CS0649
        private readonly NotEmptyValidator nameValidator;

        public Train Train { get; }
        
        public List<TransitionEntry> NextTrains { get; private set;  }

        private readonly Timetable tt;
        private readonly TrainEditHelper th;

        private Dictionary<Station, ArrDep> arrDepBackup;

        private TrainEditForm(Timetable tt)
        {
            Eto.Serialization.Xaml.XamlReader.Load(this);

            this.tt = tt;

            th = new TrainEditHelper();

            nameValidator = new NotEmptyValidator(nameTextBox, errorMessage: "Bitte einen Zugnamen eingeben!");

            locomotiveComboBox.Items.AddRange(GetAllItems(tt, t => t.Locomotive));
            lastComboBox.Items.AddRange(GetAllItems(tt, t => t.Last));
            mbrComboBox.Items.AddRange(GetAllItems(tt, t => t.Mbr));

            KeyDown += (s, e) => daysControl.HandleKeypress(e);

            resetTransitionButton.TextColor = Colors.Red;

            linkGridView.AddColumn<TrainLink>(tl => tl.TrainCount.ToString(), "Anzahl");
            linkGridView.AddColumn<TrainLink>(tl => new TimeEntry(0, tl.TimeOffset).ToShortTimeString(), "Erster Abstand");
            linkGridView.AddColumn<TrainLink>(tl => new TimeEntry(0, tl.TimeDifference).ToShortTimeString(), "Zeitdifferenz");
        }

        public TrainEditForm(Train train) : this(train.ParentTimetable)
        {
            Train = train;
            nameTextBox.Text = train.TName;
            locomotiveComboBox.Text = train.Locomotive;
            mbrComboBox.Text = train.Mbr;
            lastComboBox.Text = train.Last;
            commentTextBox.Text = train.Comment;
            daysControl.SelectedDays = train.Days;

            Title = "Zug bearbeiten";

            InitializeTrain();
        }

        /// <summary>
        /// Use <see cref="NextTrains"/> to wire up transitions.
        /// This form will NOT wire up transitions itself!
        /// </summary>
        /// <param name="tt"></param>
        /// <param name="direction"></param>
        /// <param name="path"></param>
        public TrainEditForm(Timetable tt, TrainDirection direction, List<Station> path = null) : this(tt)
        {
            Train = new Train(direction, tt);

            if (path != null)
                Train.AddAllArrDeps(path);
            if (tt.Type == TimetableType.Linear)
                Train.AddLinearArrDeps();

            InitializeTrain();
        }

        private void InitializeTrain()
        {
            editor.Initialize(Train.ParentTimetable, Train);

            transitionDropDown.ItemTextBinding = Binding.Property<Train, string>(t => t.TName);
            transitionDropDown.DataStore = tt.Trains.Where(t => t != Train).OrderBy(t => t.TName);
            
            var transitions = tt.GetEditableTransitions(Train);
            if (transitions.Count == 1)
                transitionDropDown.SelectedValue = transitions.Single().NextTrain;
            else
                transitionDropDown.Enabled = false;

            fillButton.Visible = tt.Type == TimetableType.Linear && th.FillCandidates(Train).Any();

            arrDepBackup = Train.GetArrDepsUnsorted()
                .Select(kvp => new KeyValuePair<Station, ArrDep>(kvp.Key, kvp.Value.Copy()))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            linkGridView.DataStore = Train.TrainLinks;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            if (!nameValidator.Valid)
            {
                MessageBox.Show("Bitte erst alle Fehler beheben:" + Environment.NewLine + nameValidator.ErrorMessage);
                return;
            }

            var nameExists = Train.ParentTimetable.Trains.Select(t => t.TName).Contains(nameTextBox.Text);

            if (nameExists)
            {
                if (MessageBox.Show("Ein Zug mit dem Namen \"" + nameTextBox.Text + "\" ist bereits vorhanden. Wirklich fortfahren?", "FPLedit",
                    MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }

            Train.TName = nameTextBox.Text;
            Train.Locomotive = locomotiveComboBox.Text;
            Train.Mbr = mbrComboBox.Text;
            Train.Last = lastComboBox.Text;
            Train.Comment = commentTextBox.Text;
            Train.Days = daysControl.SelectedDays;

            if (!editor.ApplyChanges())
                return;

            NextTrains = new List<TransitionEntry>()
            {
                new TransitionEntry((ITrain) transitionDropDown.SelectedValue, Days.All, null),
            };
            if (Train.Id > 0)
                tt.SetTransitions(Train, NextTrains);

            Close(DialogResult.Ok);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            foreach (var kvp in arrDepBackup)
                Train.GetArrDep(kvp.Key).ApplyCopy(kvp.Value);
            Close(DialogResult.Cancel);
        }

        private void FillButton_Click(object sender, EventArgs e)
        {
            using (var tfd = new TrainFillDialog(Train))
            {
                if (tfd.ShowModal() == DialogResult.Ok)
                {
                    th.FillTrain(tfd.ReferenceTrain, Train, tfd.Offset);

                    editor.Initialize(Train.ParentTimetable, Train);
                }
            }
        }

        private void ResetTransitionButton_Click(object sender, EventArgs e)
        {
            transitionDropDown.SelectedIndex = -1;
        }

        private void DeleteLinkButton_Click(object sender, EventArgs e)
        {
            if (linkGridView.SelectedItem != null)
            {
                tt.RemoveLink((TrainLink) linkGridView.SelectedItem);
                linkGridView.DataStore = Train.TrainLinks;
            }
            else
                MessageBox.Show("Erst muss eine Verknüpfung zum Löschen ausgewählt werden!");
        }

        private IEnumerable<ListItem> GetAllItems(Timetable tt, Func<ITrain, string> func)
            => tt.Trains.Select(func).Distinct().Where(s => s != "").OrderBy(s => s).Select(s => new ListItem { Text = s });
    }
}
