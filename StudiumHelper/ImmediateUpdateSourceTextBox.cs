using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StudiumHelper
{
    public class ImmediateUpdateSourceTextBox : TextBox
    {
        public static readonly new DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
            typeof(ImmediateUpdateSourceTextBox),
            new PropertyMetadata(default(string), OnTextChanged));

        private static void OnTextChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var txt = d as TextBox;
            txt.Text = (string)e.NewValue;
        }

        public new string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public ImmediateUpdateSourceTextBox()
        {
            base.TextChanged += (s, e) =>
            {
                Text = base.Text;
            };
        }
    }
}
