﻿namespace FPLedit.Aushangfahrplan.Forms
{
    partial class SettingsControl
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.hwexampleLabel = new System.Windows.Forms.Label();
            this.hwfontComboBox = new System.Windows.Forms.ComboBox();
            this.templateLabel = new System.Windows.Forms.Label();
            this.templateComboBox = new System.Windows.Forms.ComboBox();
            this.consoleCheckBox = new System.Windows.Forms.CheckBox();
            this.cssHelpLinkLabel = new System.Windows.Forms.LinkLabel();
            this.exampleLabel = new System.Windows.Forms.Label();
            this.cssLabel = new System.Windows.Forms.Label();
            this.fontLabel = new System.Windows.Forms.Label();
            this.cssTextBox = new System.Windows.Forms.TextBox();
            this.fontComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "Schriftart in Tabelle";
            // 
            // hwexampleLabel
            // 
            this.hwexampleLabel.AutoSize = true;
            this.hwexampleLabel.Location = new System.Drawing.Point(470, 76);
            this.hwexampleLabel.Name = "hwexampleLabel";
            this.hwexampleLabel.Size = new System.Drawing.Size(43, 13);
            this.hwexampleLabel.TabIndex = 26;
            this.hwexampleLabel.Text = "Beispiel";
            // 
            // hwfontComboBox
            // 
            this.hwfontComboBox.FormattingEnabled = true;
            this.hwfontComboBox.Location = new System.Drawing.Point(154, 73);
            this.hwfontComboBox.Name = "hwfontComboBox";
            this.hwfontComboBox.Size = new System.Drawing.Size(310, 21);
            this.hwfontComboBox.TabIndex = 25;
            this.hwfontComboBox.TextChanged += new System.EventHandler(this.hwfontComboBox_TextChanged);
            // 
            // templateLabel
            // 
            this.templateLabel.AutoSize = true;
            this.templateLabel.Location = new System.Drawing.Point(20, 19);
            this.templateLabel.Name = "templateLabel";
            this.templateLabel.Size = new System.Drawing.Size(126, 13);
            this.templateLabel.TabIndex = 14;
            this.templateLabel.Text = "Aushangfahrplan-Vorlage";
            // 
            // templateComboBox
            // 
            this.templateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.templateComboBox.FormattingEnabled = true;
            this.templateComboBox.Location = new System.Drawing.Point(154, 16);
            this.templateComboBox.Name = "templateComboBox";
            this.templateComboBox.Size = new System.Drawing.Size(310, 21);
            this.templateComboBox.TabIndex = 15;
            // 
            // consoleCheckBox
            // 
            this.consoleCheckBox.AutoSize = true;
            this.consoleCheckBox.Location = new System.Drawing.Point(154, 285);
            this.consoleCheckBox.Name = "consoleCheckBox";
            this.consoleCheckBox.Size = new System.Drawing.Size(334, 17);
            this.consoleCheckBox.TabIndex = 22;
            this.consoleCheckBox.Text = "CSS-Test-Konsole bei Vorschau aktivieren (Gilt für alle Fahrpläne)";
            this.consoleCheckBox.UseVisualStyleBackColor = true;
            // 
            // cssHelpLinkLabel
            // 
            this.cssHelpLinkLabel.AutoSize = true;
            this.cssHelpLinkLabel.Location = new System.Drawing.Point(20, 141);
            this.cssHelpLinkLabel.Name = "cssHelpLinkLabel";
            this.cssHelpLinkLabel.Size = new System.Drawing.Size(66, 13);
            this.cssHelpLinkLabel.TabIndex = 20;
            this.cssHelpLinkLabel.TabStop = true;
            this.cssHelpLinkLabel.Text = "Hilfe zu CSS";
            this.cssHelpLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.cssHelpLinkLabel_LinkClicked);
            // 
            // exampleLabel
            // 
            this.exampleLabel.AutoSize = true;
            this.exampleLabel.Location = new System.Drawing.Point(470, 46);
            this.exampleLabel.Name = "exampleLabel";
            this.exampleLabel.Size = new System.Drawing.Size(43, 13);
            this.exampleLabel.TabIndex = 18;
            this.exampleLabel.Text = "Beispiel";
            // 
            // cssLabel
            // 
            this.cssLabel.Location = new System.Drawing.Point(20, 103);
            this.cssLabel.Name = "cssLabel";
            this.cssLabel.Size = new System.Drawing.Size(128, 54);
            this.cssLabel.TabIndex = 19;
            this.cssLabel.Text = "Eigene CSS-Styles";
            // 
            // fontLabel
            // 
            this.fontLabel.AutoSize = true;
            this.fontLabel.Location = new System.Drawing.Point(20, 46);
            this.fontLabel.Name = "fontLabel";
            this.fontLabel.Size = new System.Drawing.Size(121, 13);
            this.fontLabel.TabIndex = 16;
            this.fontLabel.Text = "Schriftart im Aushangfpl.";
            // 
            // cssTextBox
            // 
            this.cssTextBox.AcceptsReturn = true;
            this.cssTextBox.AcceptsTab = true;
            this.cssTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cssTextBox.Location = new System.Drawing.Point(154, 100);
            this.cssTextBox.Multiline = true;
            this.cssTextBox.Name = "cssTextBox";
            this.cssTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.cssTextBox.Size = new System.Drawing.Size(310, 179);
            this.cssTextBox.TabIndex = 21;
            this.cssTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cssTextBox_KeyDown);
            // 
            // fontComboBox
            // 
            this.fontComboBox.FormattingEnabled = true;
            this.fontComboBox.Location = new System.Drawing.Point(154, 43);
            this.fontComboBox.Name = "fontComboBox";
            this.fontComboBox.Size = new System.Drawing.Size(310, 21);
            this.fontComboBox.TabIndex = 17;
            this.fontComboBox.TextChanged += new System.EventHandler(this.fontComboBox_TextChanged);
            // 
            // SettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.hwexampleLabel);
            this.Controls.Add(this.hwfontComboBox);
            this.Controls.Add(this.templateLabel);
            this.Controls.Add(this.templateComboBox);
            this.Controls.Add(this.consoleCheckBox);
            this.Controls.Add(this.cssHelpLinkLabel);
            this.Controls.Add(this.exampleLabel);
            this.Controls.Add(this.cssLabel);
            this.Controls.Add(this.fontLabel);
            this.Controls.Add(this.cssTextBox);
            this.Controls.Add(this.fontComboBox);
            this.Name = "SettingsControl";
            this.Size = new System.Drawing.Size(516, 305);
            this.Load += new System.EventHandler(this.SettingsControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label hwexampleLabel;
        private System.Windows.Forms.ComboBox hwfontComboBox;
        private System.Windows.Forms.Label templateLabel;
        private System.Windows.Forms.ComboBox templateComboBox;
        private System.Windows.Forms.CheckBox consoleCheckBox;
        private System.Windows.Forms.LinkLabel cssHelpLinkLabel;
        private System.Windows.Forms.Label exampleLabel;
        private System.Windows.Forms.Label cssLabel;
        private System.Windows.Forms.Label fontLabel;
        private System.Windows.Forms.TextBox cssTextBox;
        private System.Windows.Forms.ComboBox fontComboBox;
    }
}