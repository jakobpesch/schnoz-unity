
using UnityEngine;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Utils {
  public class Observable : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    // This method is called by the Set accessor of each property.
    // The CallerMemberName attribute that is applied to the optional propertyName
    // parameter causes the property name of the caller to be substituted as an argument.
    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {

      Debug.Log($"{propertyName} changed in {this}.");
      if (this.PropertyChanged != null) {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      } else {
        Debug.Log($"Noone listens to changes in {propertyName}.");
      }
    }
  }
}
