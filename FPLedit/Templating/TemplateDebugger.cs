using System;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using FPLedit.Shared.UI;

namespace FPLedit.Templating
{
    internal sealed class TemplateDebugger : ITemplateDebugger
    {
        private ITemplateDebugger child;

        private void Invoke(Action action)
        {
            if (Application.Instance != null) Application.Instance.Invoke(action);
            else action();
        }

        public void SetContext(JavascriptTemplate template)
            => Invoke(() => child?.SetContext(template));

        public void Navigate(int line, int column) // We have an error.
            => Invoke(() => child?.Navigate(line, column));
        
        public void OpenDebugger() // We have an error, so show this form.
            => Invoke(() => child?.OpenDebugger());

        public void AttachDebugger(ITemplateDebugger debugger)
            => child = debugger;

        private TemplateDebugger()
        {
        }

        private static TemplateDebugger instance;

        public static TemplateDebugger GetInstance()
        {
            instance ??= new TemplateDebugger();
            return instance;
        }
    }

#pragma warning disable CA1001 // Disposable fields are not disposed as this is a windows kept in the background.
    internal sealed class GuiTemplateDebugger : ITemplateDebugger
    {
        private FForm form;
        private TextArea generatedCode;
        private bool opened;

        public void SetContext(JavascriptTemplate template)
        {
            EnsureForm();
            form.Visible = false; // Hide until we have an error.
            
            form.Title = "Generated code: " + template.Identifier;
            
            var line = 1;
            generatedCode.Text = string.Join(Environment.NewLine,template.CompiledCode
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                .Select(l => $"/* {line++,4} */  {l}"));
            generatedCode.Selection = new Range<int>(0, 0);
        }

        public void Navigate(int line, int column) // We have an error.
        {
            EnsureForm();
            var lines = generatedCode.Text.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            var start = string.Join(Environment.NewLine, lines.Take(line - 1)).Length + Environment.NewLine.Length;
            var end = string.Join(Environment.NewLine, lines.Take(line)).Length;
            generatedCode.Selection = new Range<int>(start, end);

            OpenDebugger();
        }
        
        public void OpenDebugger() // We have an error, so show this form.
        {
            EnsureForm();
            form.Show();
            form.Focus();
        }

        private void EnsureForm()
        {
            if (!opened)
            {
                generatedCode = new TextArea
                {
                    ReadOnly = true,
                    Wrap = false,
                    Font = Fonts.Monospace(10),
                };
                form = new FForm
                {
                    Content = generatedCode, 
                    Size = new Size(800, 800)
                };
                form.Closed += (s, e) => opened = false;
                opened = true;
            }
        }
    }
#pragma warning restore CA1001

    internal interface ITemplateDebugger
    {
        void SetContext(JavascriptTemplate template);
        void Navigate(int line, int column);
        void OpenDebugger();
    }
}