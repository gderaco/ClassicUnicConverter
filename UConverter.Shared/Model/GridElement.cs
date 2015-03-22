using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UConverter.Model
{
    class GridElement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public String AssemblyPropertyName { get; set; }

        private double value;

        public String Name { get; set; }

        public double Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (value == this.value) return;
                this.value = value;
                NotifyPropertyChanged();
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string caller = "")
        {
            var eventHandler = PropertyChanged;

            if (eventHandler != null)
                eventHandler(this, new PropertyChangedEventArgs(caller));
        }
    }
}
