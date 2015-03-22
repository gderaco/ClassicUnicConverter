using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using UConverter.Model;
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
using UConverter.Util;
using UnitsNet;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

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

            MenuList.ItemsSource = _classes.Select(menu => new MenuElement { Name = Util.Util.AddSpacesToSentence(menu.Name, true) }).ToList();
        }

        private void MenuList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuList.SelectedIndex >= 0)
            {
                _currentType = Type.GetType(_classes.ElementAt(MenuList.SelectedIndex).AssemblyQualifiedName);

                GridView.ItemsSource = _currentType.GetRuntimeProperties()
                                                .Select(p => new GridElement() { Name = Util.Util.AddSpacesToSentence(p.Name, true), AssemblyPropertyName = p.Name })
                                                .Where(p => p.Name != "Zero")
                                                .ToList();
            }
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

                    foreach (var item in GridView.Items)
                    {
                        GridElement gridElement = (item as GridElement);
                        gridElement.Value = (double)_currentType.GetRuntimeProperty(gridElement.AssemblyPropertyName).GetValue(_unit, null);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
