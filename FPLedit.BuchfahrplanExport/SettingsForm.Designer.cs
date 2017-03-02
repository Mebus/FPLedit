﻿namespace FPLedit.BuchfahrplanExport
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.fontComboBox = new System.Windows.Forms.ComboBox();
            this.cssTextBox = new System.Windows.Forms.TextBox();
            this.fontLabel = new System.Windows.Forms.Label();
            this.cssLabel = new System.Windows.Forms.Label();
            this.exampleLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.cssHelpLinkLabel = new System.Windows.Forms.LinkLabel();
            this.consoleCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // fontComboBox
            // 
            this.fontComboBox.FormattingEnabled = true;
            this.fontComboBox.Location = new System.Drawing.Point(146, 12);
            this.fontComboBox.Name = "fontComboBox";
            this.fontComboBox.Size = new System.Drawing.Size(310, 21);
            this.fontComboBox.TabIndex = 1;
            this.fontComboBox.TextChanged += new System.EventHandler(this.fontComboBox_TextChanged);
            // 
            // cssTextBox
            // 
            this.cssTextBox.AcceptsReturn = true;
            this.cssTextBox.AcceptsTab = true;
            this.cssTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cssTextBox.Location = new System.Drawing.Point(146, 39);
            this.cssTextBox.Multiline = true;
            this.cssTextBox.Name = "cssTextBox";
            this.cssTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.cssTextBox.Size = new System.Drawing.Size(310, 209);
            this.cssTextBox.TabIndex = 5;
            this.cssTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cssTextBox_KeyDown);
            // 
            // fontLabel
            // 
            this.fontLabel.AutoSize = true;
            this.fontLabel.Location = new System.Drawing.Point(12, 14);
            this.fontLabel.Name = "fontLabel";
            this.fontLabel.Size = new System.Drawing.Size(128, 13);
            this.fontLabel.TabIndex = 0;
            this.fontLabel.Text = "Schriftart im Buchfahrplan";
            // 
            // cssLabel
            // 
            this.cssLabel.Location = new System.Drawing.Point(12, 42);
            this.cssLabel.Name = "cssLabel";
            this.cssLabel.Size = new System.Drawing.Size(128, 54);
            this.cssLabel.TabIndex = 3;
            this.cssLabel.Text = "Eigene CSS-Styles für den Buchfahrplan";
            // 
            // exampleLabel
            // 
            this.exampleLabel.AutoSize = true;
            this.exampleLabel.Location = new System.Drawing.Point(462, 15);
            this.exampleLabel.Name = "exampleLabel";
            this.exampleLabel.Size = new System.Drawing.Size(43, 13);
            this.exampleLabel.TabIndex = 2;
            this.exampleLabel.Text = "Beispiel";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(388, 277);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Abbrechen";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.closeButton.Location = new System.Drawing.Point(469, 277);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 7;
            this.closeButton.Text = "Schließen";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // cssHelpLinkLabel
            // 
            this.cssHelpLinkLabel.AutoSize = true;
            this.cssHelpLinkLabel.Location = new System.Drawing.Point(12, 83);
            this.cssHelpLinkLabel.Name = "cssHelpLinkLabel";
            this.cssHelpLinkLabel.Size = new System.Drawing.Size(66, 13);
            this.cssHelpLinkLabel.TabIndex = 4;
            this.cssHelpLinkLabel.TabStop = true;
            this.cssHelpLinkLabel.Text = "Hilfe zu CSS";
            this.cssHelpLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.cssHelpLinkLabel_LinkClicked);
            // 
            // consoleCheckBox
            // 
            this.consoleCheckBox.AutoSize = true;
            this.consoleCheckBox.Location = new System.Drawing.Point(146, 254);
            this.consoleCheckBox.Name = "consoleCheckBox";
            this.consoleCheckBox.Size = new System.Drawing.Size(334, 17);
            this.consoleCheckBox.TabIndex = 6;
            this.consoleCheckBox.Text = "CSS-Test-Konsole bei Vorschau aktivieren (Gilt für alle Fahrpläne)";
            this.consoleCheckBox.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(556, 312);
            this.Controls.Add(this.consoleCheckBox);
            this.Controls.Add(this.cssHelpLinkLabel);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.exampleLabel);
            this.Controls.Add(this.cssLabel);
            this.Controls.Add(this.fontLabel);
            this.Controls.Add(this.cssTextBox);
            this.Controls.Add(this.fontComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Buchfahrplaneinstellungen";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox fontComboBox;
        private System.Windows.Forms.TextBox cssTextBox;
        private System.Windows.Forms.Label fontLabel;
        private System.Windows.Forms.Label cssLabel;
        private System.Windows.Forms.Label exampleLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.LinkLabel cssHelpLinkLabel;
        private System.Windows.Forms.CheckBox consoleCheckBox;
    }
}