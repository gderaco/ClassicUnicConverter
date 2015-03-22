using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UConverter.Model;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace UConverter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Object _unit;

        private readonly List<TypeInfo> _classes = Assembly.Load(new AssemblyName("UnitsNet")).DefinedTypes.
            Where(t => t.BaseType.Name == "ValueType" && t.DeclaredProperties.Count() > 2 && t.Name != "Length2d")
            .ToList();

        private Type _currentType;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            Pivot.ItemsSource = _classes.Select(menu =>
                                                new MenuElement
                                                {
                                                    Name = Util.Util.AddSpacesToSentence(menu.Name, true),
                                                    Elements = Type.GetType(menu.AssemblyQualifiedName).GetRuntimeProperties()
                                                                                        .Select(p => new GridElement() { Name = Util.Util.AddSpacesToSentence(p.Name, true), AssemblyPropertyName = p.Name })
                                                                                        .Where(p => p.Name != "Zero")
                                                                                        .ToList()
                                                }).ToList();
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            ComputeAndUpdateGridView(sender as TextBox);
        }

        private void UIElement_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                ComputeAndUpdateGridView(sender as TextBox);
        }

        private void ComputeAndUpdateGridView(TextBox senderTextBox)
        {
            try
            {
                _unit = Activator.CreateInstance(_currentType);

                double insertedValue = 0;
                if (Double.TryParse(senderTextBox.Text, out insertedValue))
                {
                    _unit =
                        _unit.GetType()
                            .GetRuntimeMethod("From" + senderTextBox.Tag, new Type[] { typeof(double) })
                            .Invoke(null, new object[] { insertedValue });

                    (Pivot.ItemsSource as List<UConverter.Model.MenuElement>)[Pivot.SelectedIndex].Elements.All(e =>
                    {
                        e.Value = (double)_currentType.GetRuntimeProperty(e.Name.Replace(" ", String.Empty)).GetValue(_unit, null);
                        return true;
                    });
                }
            }
            catch (Exception e)
            {
            }
        }

        private void Pivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Pivot.SelectedIndex >= 0)
            {
                _currentType = Type.GetType(_classes.ElementAt(Pivot.SelectedIndex).AssemblyQualifiedName);
            }
        }
    }
}
