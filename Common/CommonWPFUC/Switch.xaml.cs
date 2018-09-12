using PropertyChanged;
using CommonWPFUC.Annotations;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Runtime.CompilerServices;

namespace CommonWPFUC
{
  /// <summary>
  /// Interaction logic for Switch.xaml
  /// </summary>
  [AddINotifyPropertyChangedInterface]
  public partial class Switch : UserControl, INotifyPropertyChanged
  {
    #region Properties

    public double Size { get; private set; } = 13;
    private double CornerRadius => Size / 2;
    private double MainWidth => Size * 2;
    private double MainHeight => Size;

    #endregion

    public Switch()
    {
      DataContext = this;

      InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
