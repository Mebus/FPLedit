﻿using Eto.Forms;
using FPLedit.Shared;
using FPLedit.Shared.Ui;
using FPLedit.Shared.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FPLedit.Editor
{
    internal class RenderSettingsForm : FDialog<DialogResult>
    {
#pragma warning disable CS0649
        private readonly TabControl tabControl;
        private readonly CheckBox expertCheckBox;
#pragma warning restore CS0649

        private readonly IInfo info;
        private readonly List<ISaveHandler> saveHandlers;
        private readonly List<IExpertHandler> expertHandlers;

        private RenderSettingsForm()
        {
            Eto.Serialization.Xaml.XamlReader.Load(this);

            saveHandlers = new List<ISaveHandler>();
            expertHandlers = new List<IExpertHandler>();

            this.AddSizeStateHandler();
        }

        public RenderSettingsForm(IInfo info) : this()
        {
            this.info = info;

            var designables = info.GetRegistered<IDesignableUiProxy>();

            tabControl.SuspendLayout();
            tabControl.Pages.Clear();

            foreach (var d in designables)
            {
                var c = d.GetControl(info);
                var tp = new TabPage(c)
                {
                    Text = d.DisplayName
                };
                c.BackgroundColor = tp.BackgroundColor;
                tabControl.Pages.Add(tp);

                if (c is ISaveHandler sh)
                    saveHandlers.Add(sh);
                if (c is IExpertHandler eh)
                    expertHandlers.Add(eh);
            }

            tabControl.ResumeLayout();

            expertCheckBox.Checked = info.Settings.Get<bool>("std.expert");
            expertCheckBox.CheckedChanged += ExpertCheckBox_CheckedChanged;
            ExpertCheckBox_CheckedChanged(this, null);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            info.Settings.Set("std.expert", expertCheckBox.Checked.Value);
            saveHandlers.ForEach(sh => sh.Save());
            Close(DialogResult.Ok);
        }

        private void CancelButton_Click(object sender, EventArgs e)
            => Close(DialogResult.Cancel);

        private void ExpertCheckBox_CheckedChanged(object sender, EventArgs e)
            => expertHandlers.ForEach(eh => eh.SetExpertMode(expertCheckBox.Checked.Value));
    }
}