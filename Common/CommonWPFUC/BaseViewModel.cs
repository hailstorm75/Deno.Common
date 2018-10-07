using System.ComponentModel;
using PropertyChanged;

namespace CommonWPFUC
{
  [AddINotifyPropertyChangedInterface]
  internal class BaseViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
  }
}
